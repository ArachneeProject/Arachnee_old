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
    private OnlineRetriever onlRet = new OnlineRetriever();

    void Start()
    {
        int seedId = PlayerPrefs.GetInt("MovieID");

        Logger.Trace("ID of movie: " + seedId, LogLevel.Debug);
        this.dlg.Initialize("URI=file:" + Application.dataPath + "/Database/arachneeDatabase.db");
        StartCoroutine(this.mainCoroutine(seedId));        
    }

    private IEnumerator mainCoroutine(int seedId)
    {
        // if movie is not present in database
        if (!this.dlg.CheckIfEntryExistsInTable(seedId, "movies"))
        {
            this.message.text = "Movie is not present in the database, getting info from the internet...";
            yield return StartCoroutine(addMovieToDatabase(seedId));
        }

        // if movie successfully added, get casting
        if (this.dlg.CheckIfEntryExistsInTable(seedId, "movies"))
        {
            yield return StartCoroutine(this.getCast(seedId));
        }
        

        // launch scene
        this.message.text = "Launching scene...";
        Application.LoadLevel(2);
    }


    /// <summary>
    /// Add the movie and all infos about it in database (only if it has a poster)
    /// </summary>
    /// <param name="movieId"></param>
    /// <returns></returns>
    private IEnumerator addMovieToDatabase(int movieId)
    {
        yield return StartCoroutine(onlRet.RetrieveMovie(movieId));
        JSONNode node = onlRet.NodeRetrieved;

        string posterPath = node["poster_path"].Value;
        if (posterPath == null)
        {
            yield break;
        }
        string title = node["original_title"].Value;
        Int64 date = 1;
        string d = node["release_date"].Value;
        if (d.Length > 4)
        {
            date = Convert.ToInt64(d.Substring(0, 4));
        }
        
        this.dlg.InsertMovie(movieId, title, date, posterPath);
    }


    private IEnumerator addArtistToDatabase(int artistId)
    {
        yield return StartCoroutine(this.onlRet.RetrieveArtist(artistId));
        JSONNode node = this.onlRet.NodeRetrieved;

        string posterPath = node["profile_path"].Value;
        if (posterPath == null)
        {
            yield break;
        }
        
        string name = node["name"].Value;
        int idx = name.LastIndexOf(' ');
        string firstName = name.Substring(0, idx); 
        string lastName = name.Substring(idx + 1);

        this.dlg.InsertArtist(artistId, firstName, lastName);
    }

    /// <summary>
    /// Get the cast of the movie, add each new artist and each new connections in 'actors' table
    /// </summary>
    /// <param name="movieId"></param>
    /// <returns></returns>
    private IEnumerator getCast(int movieId)
    {
        this.message.text = "Getting casting...";

        yield return StartCoroutine(this.onlRet.RetrieveCast(movieId));
        JSONNode node = this.onlRet.NodeRetrieved;

        List<int> actors = new List<int>();
        for (int i = 0; i < node.Count; i++)
        {
            int artistId = Convert.ToInt32(node[i]["id"].Value);
            actors.Add(artistId);
        }

        List<int> actorsAdded = new List<int>();
        foreach (int a in actors)
        {
            // if artist doesn't exist in db
            if (!this.dlg.CheckIfEntryExistsInTable(a, "artists"))
            {
                yield return StartCoroutine(addArtistToDatabase(a));
                if (this.dlg.CheckIfEntryExistsInTable(a, "artists"))
                {
                    actorsAdded.Add(a);
                }
            }
        }

        foreach (int a in actorsAdded)
        {
            this.dlg.InsertConnection(a, movieId, "actors");
        }
    }

}
