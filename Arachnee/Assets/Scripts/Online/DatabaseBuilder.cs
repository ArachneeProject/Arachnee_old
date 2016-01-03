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
        this.message.text = "Initialization...";
        int seedId = PlayerPrefs.GetInt("MovieID");

        //Logger.Trace("ID of movie: " + seedId, LogLevel.Debug);
        this.dlg.Initialize("URI=file:" + Application.dataPath + "/Database/arachneeDatabase.db");
        StartCoroutine(this.mainCoroutine(seedId));        
    }

    /// <summary>
    /// Main coroutine
    /// </summary>
    /// <param name="seedId"></param>
    /// <returns></returns>
    private IEnumerator mainCoroutine(int seedId)
    {
        // if movie is not present in database
        if (!this.dlg.CheckIfEntryExistsInTable(seedId, "movies"))
        {            
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
    /// Insert the movie and all infos about it in database (only if it has a poster)
    /// </summary>
    /// <param name="movieId"></param>
    /// <returns></returns>
    private IEnumerator addMovieToDatabase(int movieId)
    {
        this.message.text = "Inserting movie...";

        yield return StartCoroutine(onlRet.RetrieveMovie(movieId));
        JSONNode node = onlRet.NodeRetrieved;
        if (node == null)
        {
            Logger.Trace("No movie was found with the id " + movieId, LogLevel.Warning);
            yield break;
        }

        string posterPath = node["poster_path"].Value;
        if (posterPath == "null")
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



    /// <summary>
    /// Get the cast of the movie, insert each new artist and each new connections in 'actors' and 'directors' table
    /// </summary>
    /// <param name="movieId"></param>
    /// <returns></returns>
    private IEnumerator getCast(int movieId)
    {
        this.message.text = "Getting casting...";

        yield return StartCoroutine(this.onlRet.RetrieveCast(movieId));
        JSONNode node = this.onlRet.NodeRetrieved;
        if (node == null)
        {
            Logger.Trace("No casting was found about the movie with the id " + movieId, LogLevel.Warning);
            yield break;
        }

        // for each actor in the movie
        List<int> actors = new List<int>();
        for (int i = 0; i < node["cast"].Count; i++)
        {
            int artistId = Convert.ToInt32(node["cast"][i]["id"].Value);
            actors.Add(artistId);
        }

        // for each director in the movie
        List<int> directors = new List<int>();
        for (int i = 0; i < node["crew"].Count; i++)
        {
            if (node["crew"][i]["job"].Value == "Director")
            {
                int artistId = Convert.ToInt32(node["crew"][i]["id"].Value);
                directors.Add(artistId);
            }            
        }

        // insert all those artists
        this.message.text = "Inserting artists...";
        List<int> artistsNotAdded = new List<int>();
        foreach (int a in actors)
        {
            // if artist doesn't exist in db
            if (!this.dlg.CheckIfEntryExistsInTable(a, "artists"))
            {
                // add artist to database
                yield return StartCoroutine(addArtistToDatabase(a));
                if (!this.dlg.CheckIfEntryExistsInTable(a, "artists"))
                {
                    artistsNotAdded.Add(a);
                }
            }
        }
        foreach (int a in directors)
        {
            // if artist doesn't exist in db
            if (!this.dlg.CheckIfEntryExistsInTable(a, "artists"))
            {
                // add artist to database
                yield return StartCoroutine(addArtistToDatabase(a));
                if (!this.dlg.CheckIfEntryExistsInTable(a, "artists"))
                {
                    artistsNotAdded.Add(a);
                }
            }
        }
        
        // remove useless artists
        foreach (int a in artistsNotAdded)
        {
            actors.Remove(a);
            directors.Remove(a);
        }

        // insert connections
        this.message.text = "Inserting casting...";
        foreach (int a in actors)
        {
            if (!this.dlg.CheckIfAMConnectionExistsInTable(a,movieId, "actors"))
            {
                this.dlg.InsertConnection(a, movieId, "actors");
            }            
        }
        foreach (int a in directors)
        {
            if (!this.dlg.CheckIfAMConnectionExistsInTable(a, movieId, "directors"))
            {
                this.dlg.InsertConnection(a, movieId, "directors");
            }
        }

        yield return StartCoroutine(addCredits(actors));
        yield return StartCoroutine(addCredits(directors));
    }


    /// <summary>
    /// Insert the 'actors' and 'directors' connections for each artist in the list
    /// </summary>
    /// <param name="artistsAdded"></param>
    /// <returns></returns>
    private IEnumerator addCredits(List<int> artistsAdded)
    {
        this.message.text = "Getting credits...";

        foreach (int artId in artistsAdded)
        {

            yield return StartCoroutine(this.onlRet.RetrieveCredits(artId));
            JSONNode node = this.onlRet.NodeRetrieved;
            if (node == null)
            {
                Logger.Trace("No credits were found about the artist with the id " + artistsAdded, LogLevel.Warning);
                yield break;
            }

            // for each movie the artist played in
            for (int i = 0; i < node["cast"].Count; i++)
            {
                var mvId = Convert.ToInt64(node["cast"][i]["id"].Value);

                if (this.dlg.CheckIfEntryExistsInTable(mvId, "movies"))
                {
                    if (!this.dlg.CheckIfAMConnectionExistsInTable(artId,mvId,"actors"))
                    {
                        this.dlg.InsertConnection(artId, mvId, "actors");
                    }                    
                }
            }

            // for each movie the artist directed
            for (int i = 0; i < node["crew"].Count; i++)
            {
                if (node["crew"][i]["job"].Value == "Director")
                {
                    Int64 mvId = Convert.ToInt64(node["crew"][i]["id"].Value);
                    if (this.dlg.CheckIfEntryExistsInTable(mvId, "movies"))
                    {
                        if (!this.dlg.CheckIfAMConnectionExistsInTable(artId, mvId, "directors"))
                        {
                            this.dlg.InsertConnection(artId, mvId, "directors");
                        }
                    }
                }
            }

            // this is where you have to continue...  https://api.themoviedb.org/3/person/76489/movie_credits?api_key=The Skeleton Key
        }
    }



    /// <summary>
    /// Insert the artist's infos in the database (only if he/she has a poster)
    /// </summary>
    /// <param name="artistId"></param>
    /// <returns></returns>
    private IEnumerator addArtistToDatabase(int artistId)
    {
        yield return StartCoroutine(this.onlRet.RetrieveArtist(artistId));
        JSONNode node = this.onlRet.NodeRetrieved;
        if (node == null)
        {
            Logger.Trace("No artist was found with the id " + artistId, LogLevel.Warning);
            yield break;
        }

        string posterPath = node["profile_path"].Value;
        if (posterPath == "null")
        {
            yield break;
        }

        string name = node["name"].Value;
        string firstName = "";
        string lastName = "";

        int idx = name.LastIndexOf(' ');
        if (idx < 0)
        {
            lastName = name;
            // also known as...
        }
        else
        {
            firstName = name.Substring(0, idx);
            lastName = name.Substring(idx + 1);
        }

        this.dlg.InsertArtist(artistId, firstName, lastName, posterPath);
    }


}
