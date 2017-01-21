using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public abstract class EntryRetriever : OnlineRetriever
{
    protected override IEnumerator RetrieveDataImpl(HashSet<string> identifiers)
    {
        var entryIds = identifiers.Where(id => !string.IsNullOrEmpty(id));

        foreach (var entryId in entryIds)
        {
            var www = new WWW(GetEntryQuery(entryId));
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError(www.error);
                yield break;
            }

            var node = JSON.Parse(www.text);
            if (node == null)
            {
                Debug.LogWarning("No result found");
                yield break;
            }

            // build the entry
            string posterPath = node["poster_path"].Value;
            if (posterPath == "null")
            {
                _retrievedData.Add(entryId, Entry.DefaultEntry);
                yield break;
            }

            _retrievedData.Add(entryId, BuildEntry(node));
        }
    }

    protected abstract string GetEntryQuery(string entryId);
    protected abstract Entry BuildEntry(JSONNode node);
    
    #region userSearch
    /*
    
    /// <summary>
    /// modify the input field to be sent as a query
    /// </summary>
    private string processInput(string text)
    {
        return text.Replace(" ","%20");
    }

    /// <summary>
    /// Retrieve JSON info
    /// </summary>
    public IEnumerator RetrieveResultOfSerach(string input, bool isMovieSearch)
    {
        input = this.processInput(input);
        string url = urlMovieSearch + input + "&" + apiKey;
        if (!isMovieSearch)
        {
            url = urlTvSearch + input + "&" + apiKey;
        }
        WWW www = new WWW(url);
        yield return www;

        JSONNode node = JSON.Parse(www.text);
        if (node == null)
        {
            Debug.LogWarning("Query returned no result");
            this.NodeRetrieved = null;
            yield break;
        }
        this.NodeRetrieved = node["results"];
    }
    
    */
    #endregion userSearch

    #region entries

    /*
     * new WWW(serieUrl + serieId + "?" + apiKey);
       new WWW(personUrl + artistId + "?" + apiKey);
    WWW www = new WWW(movieUrl + movieId + castQuery + apiKey);
     * WWW www = new WWW(personUrl + artistId + creditsQuery + apiKey);
     * 
    */
    #endregion entries

    #region connections
    /*
    // insert the connections between the given movie and the artists already existing in the database
    private IEnumerator addCasting(long movieId)
    {
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
    */
    #endregion connections

    #region genres
    /*
    public IEnumerator RetrieveMovieGenres()
    {
        WWW www = new WWW(movieGenresUrl + "?" + apiKey);
        yield return www;

        JSONNode node = JSON.Parse(www.text);
        this.NodeRetrieved = node;
        if (node == null)
        {
            Debug.LogError("Nothing retrieved!");
        }
    }

    public IEnumerator RetrieveSerieGenres()
    {
        WWW www = new WWW(serieGenresUrl + "?" + apiKey);
        yield return www;

        JSONNode node = JSON.Parse(www.text);
        this.NodeRetrieved = node;
        if (node == null)
        {
            Debug.LogError("Nothing retrieved!");
        }
    }
     */
    #endregion genres
}
