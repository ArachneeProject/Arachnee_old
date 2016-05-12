using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class DatabaseUpdate : MonoBehaviour
{
    public Text steps;
    public Text progress;

    public Text message;
    private DatabaseDialoger dlg;
    private OnlineRetriever onlRet = new OnlineRetriever();

    private HashSet<long> moviesToDo = new HashSet<long>();
    private HashSet<long> artistsToDo = new HashSet<long>();
    

    void Start()
    {
        this.dlg = new DatabaseDialoger();

        this.steps.text = "";
        this.progress.text = "";

        this.message.text = "Initialization...";

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
        this.steps.text = "Updating movies";
        this.message.text = "";
        int globalCount = 1;
        foreach (long idMovie in this.moviesToDo)
        {
            // progress
            this.progress.text = globalCount++ + "/" + moviesToDo.Count;

            // add the movie to the database
            //yield return StartCoroutine(addMovieToDatabase(idMovie));

            // if movie successfully added, add casting
            if (this.dlg.MovieIsRegistered(idMovie))
            {
                yield return StartCoroutine(this.addCasting(idMovie));
            }
        }
        this.moviesToDo.Clear();

        // foreach artist to do
        this.steps.text = "Updating artists";
        globalCount = 1;
        foreach (long idArt in this.artistsToDo)
        {
            // progress
            this.progress.text = globalCount++ + "/" + artistsToDo.Count;

            // add the artist to the database
            yield return StartCoroutine(addArtistToDatabase(idArt));

            // if artist successfully added, add credits
            if (this.dlg.ArtistIsRegistered(idArt))
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
        string movies = PlayerPrefs.GetString(Constants.PP_MoviesToUpdate);

        Debug.Log(movies);

        if (movies == string.Empty)
        {
            this.message.text = "Launching scene...";
            Application.LoadLevel(Navigation.Graph);
            return;
        }

        char[] separator = {','};
        string[] arrS = movies.Split(separator);
        long[] arrL = Array.ConvertAll(arrS, e => long.Parse(e));
        


        this.moviesToDo = new HashSet<long>(arrL);
    }


    /*
    // Insert the movie and all infos about it in database (only if it has a poster)
    private IEnumerator addMovieToDatabase(long movieId)
    {

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


        string date = node["release_date"].Value;
        if (date.Length >= 4)
        {
            date = date.Substring(0, 4);
        }
        else
        {
            Debug.LogError("date is wrong: " + node["release_date"].Value + " (on id " + movieId + ")");
            yield break;
        }

        this.dlg.InsertMovie(movieId, title, date, posterPath, false);
    }
    */

    // insert the connections between the given movie and the artists already existing in the database
    private IEnumerator addCasting(long movieId)
    {
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
            // id
            long artistId = Convert.ToInt64(node["cast"][i]["id"].Value);

            // if artist exists in db
            if (this.dlg.ArtistIsRegistered(artistId))
            {
                // add the connection
                this.dlg.InsertActorConnection(artistId, movieId);
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
                // id
                long artistId = Convert.ToInt64(node["crew"][i]["id"].Value);

                // if artist exists in db
                if (this.dlg.ArtistIsRegistered(artistId))
                {
                    // add the connection
                    this.dlg.InsertDirectorConnection(artistId, movieId);
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
            // id
            var mvId = Convert.ToInt64(node["cast"][i]["id"].Value);

            // if the movie exists
            if (this.dlg.MovieIsRegistered(mvId))
            {
                // insert the connection
                this.dlg.InsertActorConnection(artistI, mvId);
            }
            else
            {
                // else add to ToDo list
                this.moviesToDo.Add((int)mvId);
            }
        }

        // for each movie the artist directed
        for (int i = 0; i < node["crew"].Count; i++)
        {
            if (node["crew"][i]["job"].Value == "Director")
            {
                // id
                Int64 mvId = Convert.ToInt64(node["crew"][i]["id"].Value);

                // if the movie exists
                if (this.dlg.MovieIsRegistered(mvId))
                {
                    // add the connection
                    this.dlg.InsertDirectorConnection(artistI, mvId);
                }
                else
                {
                    // add to ToDoList
                    this.moviesToDo.Add((int)mvId);
                }
            }
        }


    }


    // save what needs to be done next time
    void saveToDo()
    {
        PlayerPrefs.SetString(Constants.PP_MoviesToUpdate, string.Empty);

        this.message.text = "Launching scene...";
        Application.LoadLevel(Navigation.Graph);
    }


    //  https://api.themoviedb.org/3/person/76489/movie_credits?api_key=The Skeleton Key
}
