using System.Collections;
using System;
using SimpleJSON;
using UnityEngine;

public class OnlineRetriever
{    
    // serach = "https://api.themoviedb.org/3/search/movie?query=cube&api_key=The Skeleton Key";
    // poster "http://image.tmdb.org/t/p/w500/u50r6chJGO2iqxVAvtQ07obiCkB.jpg";
    // example of movie https://api.themoviedb.org/3/movie/137?api_key=The Skeleton Key

    private const string urlMovieSearch = "https://api.themoviedb.org/3/search/movie?query=";
    private const string urlTvSearch = "https://api.themoviedb.org/3/search/tv?query=";
    private const string posterUrl = "http://image.tmdb.org/t/p/w500";
    private const string movieUrl = "https://api.themoviedb.org/3/movie/";
    private const string serieUrl = "https://api.themoviedb.org/3/tv/";
    private const string movieGenresUrl = "http://api.themoviedb.org/3/genre/movie/list";
    private const string serieGenresUrl = "http://api.themoviedb.org/3/genre/tv/list";
    private const string personUrl = "https://api.themoviedb.org/3/person/";
    private const string castQuery = "/credits?";
    private const string creditsQuery = "/movie_credits?";
    private const string apiKey = "api_key=The Skeleton Key";
    
    private JSONNode _jNode = new JSONNode();
    public JSONNode NodeRetrieved
    {
        get 
        {
            return _jNode;
        }
        private set
        {
            _jNode = value;
        }
    }

    public bool Succeeded = false;
    public string Error;

    #region textures
    public Texture2D Texture
    {
        get;
        private set;
    }
    
    public IEnumerator RetrievePoster(string posterPath)
    {
        Texture = null;
        using (var www = new WWW(posterUrl + posterPath))
        {
            yield return www;
            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                this.Texture = www.texture;
            }
            
            Succeeded = this.Texture != null;
            Error = www.error;
        }
    }
    #endregion textures

    #region userSearch
    /// <summary>
    /// modify the input field to be sent as a query
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private string processInput(string text)
    {
        return text.Replace(" ","%20");
    }

    /// <summary>
    /// Retrieve JSON info
    /// </summary>
    /// <param name="input"></param>
    /// <param name="node"></param>
    /// <returns></returns>
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
    #endregion userSearch

    #region entries
    /// <summary>
    /// Get infos about the movie with specified id
    /// </summary>
    public IEnumerator RetrieveMovie(long movieId)
    {
        WWW www = new WWW(movieUrl + movieId + "?" + apiKey);
        yield return www;

        JSONNode node = JSON.Parse(www.text);
        this.NodeRetrieved = node;
        if (node == null)
        {
            Debug.LogWarning("Query returned no result: " + movieUrl + movieId + "?" + "(apiKey)");
        }
    }

    public IEnumerator RetrieveSerie(long serieId)
    {
        WWW www = new WWW(serieUrl + serieId + "?" + apiKey);
        yield return www;

        JSONNode node = JSON.Parse(www.text);
        this.NodeRetrieved = node;
        if (node == null)
        {
            Debug.LogWarning("Query returned no result: " + serieUrl + serieId + "?" + "(apiKey)");
        }
    }

    /// <summary>
    /// Get infos about the artist with the specified id
    /// </summary>
    public IEnumerator RetrieveArtist(long artistId)
    {
        WWW www = new WWW(personUrl + artistId + "?" + apiKey);
        yield return www;

        JSONNode node = JSON.Parse(www.text);
        this.NodeRetrieved = node;
        if (node == null)
        {
            Debug.LogWarning("Query returned no result: " + personUrl + artistId + "?" + "(apiKey)");
        }
    }
    #endregion entries

    #region connections

    /// <summary>
    /// Retrive the cast of the movie
    /// </summary>
    /// <param name="movieId"></param>
    /// <returns></returns>
    public IEnumerator RetrieveCast(long movieId)
    {
        WWW www = new WWW(movieUrl + movieId + castQuery + apiKey);
        yield return www;

        JSONNode node = JSON.Parse(www.text);
        if (node == null)
        {
            Debug.LogWarning("Query returned no result: " + movieUrl + movieId + castQuery + "(apiKey)");
            this.NodeRetrieved = null;
            yield break;
        }
        this.NodeRetrieved = node;
    }

    /// <summary>
    /// Retrieve the movies of the artist
    /// </summary>
    public IEnumerator RetrieveCredits(long artistId)
    {
        WWW www = new WWW(personUrl + artistId + creditsQuery + apiKey);
        yield return www;

        JSONNode node = JSON.Parse(www.text);
        if (node == null)
        {
            Debug.LogWarning("Query returned no result: " + personUrl + artistId + creditsQuery + "(apiKey)");
            this.NodeRetrieved = null;
            yield break;
        }
        this.NodeRetrieved = node;
    }

    #endregion connections

    #region genres
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
    #endregion genres
}
