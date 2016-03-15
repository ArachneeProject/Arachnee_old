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

    private List<long> moviesToDo = new List<long>();
    private List<long> artistsToDo = new List<long>();


    void Start()
    {
        this.message.text = "Initialization...";               
        this.dlg.Initialize("URI=file:" + Application.dataPath + "/Database/arachneeDatabase.db");

        StartCoroutine(this.mainCoroutine());        
    }



    // main routine
    private IEnumerator mainCoroutine()
    {
        // load what to do
        loadToDo();
        if (this.moviesToDo.Count == 0)
        {
            yield break;
        }

        // foreach movie to do
        foreach (long idMovie in this.moviesToDo)        
        {
            // add the movie to the database
            yield return StartCoroutine(addMovieToDatabase(idMovie));

            // if movie successfully added, add casting
            if (this.dlg.CheckIfEntryExistsInTable(idMovie, "movies"))
            {
                yield return StartCoroutine(this.addCasting(idMovie));
            }
        }
        this.moviesToDo.Clear();

        // foreach artist to do
        foreach (long idArt in this.artistsToDo)
        {
            // add the artist to the database
            yield return StartCoroutine(addArtistToDatabase(idArt));

            // if artist successfully added, add credits
            if (this.dlg.CheckIfEntryExistsInTable(idArt, "artists"))
            {
                yield return StartCoroutine(this.addCredits(idArt));
            }
        }
        this.artistsToDo.Clear();
        
        // save what to do next time
        saveToDo();
    }

    // load what needs to be done
    void loadToDo()
    {
        long seed = PlayerPrefs.GetInt("MovieID");

        // if the seed is present in database, load the scene
        if (this.dlg.CheckIfEntryExistsInTable(seed, "movies"))
        {
            this.message.text = "Launching scene...";
            Application.LoadLevel(2);
        }
        else
        {
            // else let the routine run
            this.moviesToDo.Add(PlayerPrefs.GetInt("MovieID"));
            Logger.Trace("ID of movie: " + PlayerPrefs.GetInt("MovieID"), LogLevel.Debug);
        }        
    }



    // Insert the movie and all infos about it in database (only if it has a poster)
    private IEnumerator addMovieToDatabase(long movieId)
    {
        this.message.text = "Inserting...";

        yield return StartCoroutine(onlRet.RetrieveMovie(movieId));
        JSONNode node = onlRet.NodeRetrieved;
        if (node == null)
        {
            Debug.LogError("No movie was found with the id " + movieId);
            yield break;
        }

        string posterPath = node["poster_path"].Value;
        if (posterPath == "null")
        {
            yield break;
        }
        string title = node["original_title"].Value;
        this.message.text = title;

        Int64 date = 1;
        string d = node["release_date"].Value;
        if (d.Length > 4)
        {
            date = Convert.ToInt64(d.Substring(0, 4));
        } 
        else
        {
            Debug.LogError("date is wrong: " + node["release_date"].Value + " (on id " + movieId + ")");
            yield break;
        }

        this.dlg.InsertMovie(movieId, title, date, posterPath);
    }


    // insert the connections between the given movie and the artists already existing in the database
    private IEnumerator addCasting(long movieId)
    {
        this.message.text += "\ninserting casting...";

        yield return StartCoroutine(this.onlRet.RetrieveCast(movieId));
        JSONNode node = this.onlRet.NodeRetrieved;
        if (node == null)
        {
            Debug.LogError("No casting was found about the movie with the id " + movieId);
            yield break;
        }

        // for each actor in the movie
        for (int i = 0; i < node["cast"].Count; i++)
        {
            long artistId = Convert.ToInt64(node["cast"][i]["id"].Value);

            // if artist exists in db
            if (this.dlg.CheckIfEntryExistsInTable(artistId, "artists"))
            {
                // add the connection
                this.dlg.InsertConnection(artistId, movieId, "actors");
            }
            else
            {
                // else add to ToDo list
                this.artistsToDo.Add(artistId);
            }
        }

        // for each director in the movie
        for (int i = 0; i < node["crew"].Count; i++)
        {
            if (node["crew"][i]["job"].Value == "Director")
            {
                long artistId = Convert.ToInt64(node["crew"][i]["id"].Value);

                // if artist exists in db
                if (this.dlg.CheckIfEntryExistsInTable(artistId, "artists"))
                {
                    // add the connection
                    this.dlg.InsertConnection(artistId, movieId, "directors");
                }
                // else add to ToDo list
                else
                {
                    this.artistsToDo.Add(artistId);
                }
            }            
        }
    }


    /// Insert the artist's infos in the database (only if he/she has a poster)
    private IEnumerator addArtistToDatabase(long artistId)
    {
        this.message.text = "Inserting...";

        yield return StartCoroutine(this.onlRet.RetrieveArtist(artistId));
        JSONNode node = this.onlRet.NodeRetrieved;
        if (node == null)
        {
            Debug.LogError("No artist was found with the id " + artistId);
            yield break;
        }

        string posterPath = node["profile_path"].Value;
        if (posterPath == "null")
        {
            yield break;
        }

        string name = node["name"].Value;
        this.message.text = name;
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


    /// Insert the connections between the given artist and the movies already existing in the database
    private IEnumerator addCredits(long artistI)
    {
        this.message.text += "\ninserting credits...";

        yield return StartCoroutine(this.onlRet.RetrieveCredits(artistI));
        JSONNode node = this.onlRet.NodeRetrieved;
        if (node == null)
        {
            Debug.LogError("No credits were found about the artist with the id " + artistI);
            yield break;
        }


        // for each movie the artist played in
        for (int i = 0; i < node["cast"].Count; i++)
        {
            var mvId = Convert.ToInt64(node["cast"][i]["id"].Value);

            // if the movie exists
            if (this.dlg.CheckIfEntryExistsInTable(mvId, "movies"))
            {
                // insert the connection
                this.dlg.InsertConnection(artistI, mvId, "actors");              
            }
            else
            {
                // else add to ToDo list
                this.moviesToDo.Add((int) mvId);
            }
        }

        // for each movie the artist directed
        for (int i = 0; i < node["crew"].Count; i++)
        {
            if (node["crew"][i]["job"].Value == "Director")
            {
                Int64 mvId = Convert.ToInt64(node["crew"][i]["id"].Value);

                // if the movie exists
                if (this.dlg.CheckIfEntryExistsInTable(mvId, "movies"))
                {
                    // add the connection
                    this.dlg.InsertConnection(artistI, mvId, "directors");                    
                }
                else
                {
                    // add to ToDoList
                    this.moviesToDo.Add((int) mvId);
                }
            }
        }

            
    }


    // save what needs to be done next time
    void saveToDo()
    {
        this.message.text = "Launching scene...";
        Application.LoadLevel(2);
    }


    //  https://api.themoviedb.org/3/person/76489/movie_credits?api_key=The Skeleton Key
}
