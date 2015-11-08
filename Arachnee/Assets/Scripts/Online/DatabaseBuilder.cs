using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class DatabaseBuilder : MonoBehaviour 
{
    public Text message;
    private DatabaseDialoger dlg = new DatabaseDialoger();

    // entries
    private HashSet<int> artistsIds = new HashSet<int>();
    private HashSet<int> moviesIds = new HashSet<int>();

    // connections
    private HashSet<Tuple> actorConnections = new HashSet<Tuple>();


	// Use this for initialization
	void Start () 
    {
        Logger.Trace("ID of movie: " + PlayerPrefs.GetInt("MovieID"), LogLevel.Debug);
        this.dlg.Initialize("URI=file:" + Application.dataPath + "/Database/arachneeDatabase.db");
        StartCoroutine(this.buildDatabase(PlayerPrefs.GetInt("MovieID")));
	}
	
    IEnumerator buildDatabase(int startingPoint)
    {
        yield return StartCoroutine(this.getCast(startingPoint));
        foreach(int idArtist in this.artistsIds)
        {
            yield return StartCoroutine(this.getCredits(idArtist));
        }
        yield return StartCoroutine(this.insertConnections());
        yield return StartCoroutine(this.insertEntries());
        this.message.text = "Launching scene...";
        Application.LoadLevel(2);
    }

    IEnumerator getCast(int movieId)
    {
        this.message.text = "Getting casting...";

        OnlineRetriever onlineRetriever = new OnlineRetriever();

        yield return StartCoroutine(onlineRetriever.RetrieveCast(movieId));
        JSONNode node = onlineRetriever.NodeRetrieved;

        for (int i = 0; i < node.Count; i++)
        {
            int artistId = Convert.ToInt32(node[i]["id"].Value);
            this.artistsIds.Add(artistId);
            this.actorConnections.Add(new Tuple(artistId,movieId));
        }
    }

    IEnumerator getCredits(int artistId)
    {
        this.message.text = "Getting credits...";

        OnlineRetriever onlineRetriever = new OnlineRetriever();

        yield return StartCoroutine(onlineRetriever.RetrieveCredits(artistId));
        JSONNode node = onlineRetriever.NodeRetrieved;

        for (int i = 0; i < node.Count; i++)
        {
            int movieId = Convert.ToInt32(node[i]["id"].Value);
            this.moviesIds.Add(movieId);
            this.actorConnections.Add(new Tuple(artistId, movieId));
        }
    }

    /// <summary>
    /// Insert artist->movie connections in database
    /// </summary>
    IEnumerator insertConnections()
    {
        this.message.text = "Inserting new connections...";

        this.actorConnections = this.cleanList(this.actorConnections,"actors");        
        foreach (Tuple t in this.actorConnections)
        {   
            this.dlg.InsertConnection((Int64)t.Left, (Int64)t.Right, "actors");
            yield return null;
        }
    }

    IEnumerator insertEntries()
    {
        OnlineRetriever onlRet = new OnlineRetriever();
        // movies
        this.message.text = "Inserting new movies...";
        this.moviesIds = this.cleanList(this.moviesIds, "movies");
        foreach (int i in this.moviesIds)
        {
            yield return StartCoroutine(onlRet.RetrieveMovie(i));
            JSONNode node = onlRet.NodeRetrieved;
            string title = node["original_title"].Value;
            Int64 date = 1;
            string d = node["release_date"].Value;
            if (d.Length > 4)
            {
                Convert.ToInt64(d.Substring(0, 4));
            }
            string posterPath = node["poster_path"].Value;
                        
            this.dlg.InsertMovie(i,title,date,posterPath);
        }

        // artists
        this.message.text = "Inserting new artists...";
        this.artistsIds = this.cleanList(this.artistsIds, "artists");
        foreach (int i in this.artistsIds)
        {
            this.dlg.InsertArtist(i);
        }
    }

    /// <summary>
    /// Remove connexions already existing in database
    /// </summary>
    /// <param name="l"></param>
    /// <returns></returns>
    HashSet<Tuple> cleanList(HashSet<Tuple> l, string tableName)
    {
        HashSet<Tuple> cl = new HashSet<Tuple>();
        foreach (Tuple t in l)
        {
            if (!this.dlg.CheckIfAMConnectionExistsInTable(t.Left, t.Right, tableName))
            {
                cl.Add(t);
            }
        }
        return cl;
    }

    /// <summary>
    /// Remove entries already existing in database
    /// </summary>
    /// <param name="l"></param>
    /// <param name="tableName"></param>
    /// <returns></returns>
    HashSet<int> cleanList(HashSet<int> l, string tableName)
    {
        HashSet<int> cl = new HashSet<int>();
        foreach (int i in l)
        {
            if (!this.dlg.CheckIfEntryExistsInTable(i,tableName))
            {
                cl.Add(i);
            }
        }
        return cl;
    }
}
