using System.Collections;
using System;
using SimpleJSON;
using UnityEngine;

public class OnlineRetriever 
{    
    // serach = "https://api.themoviedb.org/3/search/movie?query=cube&api_key=The Skeleton Key";
    // poster "http://image.tmdb.org/t/p/w500/u50r6chJGO2iqxVAvtQ07obiCkB.jpg";
    // example of movie https://api.themoviedb.org/3/movie/137?api_key=The Skeleton Key

    private string urlMovieSearch = "https://api.themoviedb.org/3/search/movie?query=";
    private string urlTvSearch = "https://api.themoviedb.org/3/search/tv?query=";
    private string posterUrl = "http://image.tmdb.org/t/p/w500";
    private string movieUrl = "https://api.themoviedb.org/3/movie/";
    private string serieUrl = "https://api.themoviedb.org/3/tv/";
    private string movieGenresUrl = "http://api.themoviedb.org/3/genre/movie/list";
    private string serieGenresUrl = "http://api.themoviedb.org/3/genre/tv/list";
    private string personUrl = "https://api.themoviedb.org/3/person/";
    private string castQuery = "/credits?";
    private string creditsQuery = "/movie_credits?";
    private string apiKey = "api_key=The Skeleton Key";


    private JSONNode jNode = new JSONNode();
    public JSONNode NodeRetrieved
    {
        get 
        {
            return jNode;
        }
        private set
        {
            jNode = value;
        }
    }

    public Texture2D Texture
    {
        get;
        private set;
    }

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
        string url = this.urlMovieSearch + input + "&" + this.apiKey;
        if (!isMovieSearch)
        {
            url = this.urlTvSearch + input + "&" + this.apiKey;
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



    public IEnumerator RetrievePoster(string posterPath)
    {
        WWW www = new WWW(this.posterUrl + posterPath);
        yield return www;

        if (www.isDone)
        {
            this.Texture = www.texture; 
        }
    }


    /// <summary>
    /// Retrive the cast of the movie
    /// </summary>
    /// <param name="movieId"></param>
    /// <returns></returns>
    public IEnumerator RetrieveCast(long movieId)
    {
        WWW www = new WWW(this.movieUrl + movieId + this.castQuery + this.apiKey);
        yield return www;

        JSONNode node = JSON.Parse(www.text);
        if (node == null)
        {
            Debug.LogWarning("Query returned no result: " + this.movieUrl + movieId + this.castQuery + "(apiKey)");
            this.NodeRetrieved = null;
            yield break;
        }
        this.NodeRetrieved = node;
    }

    /// <summary>
    /// Retrieve the movies of the artist
    /// </summary>
    /// <param name="movieId"></param>
    /// <returns></returns>
    public IEnumerator RetrieveCredits(long artistId)
    {
        WWW www = new WWW(this.personUrl + artistId + this.creditsQuery + this.apiKey);
        yield return www;

        JSONNode node = JSON.Parse(www.text);
        if (node == null)
        {
            Debug.LogWarning("Query returned no result: " + this.personUrl + artistId + this.creditsQuery + "(apiKey)");
            this.NodeRetrieved = null;
            yield break;
        }
        this.NodeRetrieved = node;
    }

    /// <summary>
    /// Get infos about the movie with specified id
    /// </summary>
    /// <param name="movieId"></param>
    /// <returns></returns>
    public IEnumerator RetrieveMovie(long movieId)
    {
        WWW www = new WWW(this.movieUrl + movieId + "?" + this.apiKey);
        yield return www;

        JSONNode node = JSON.Parse(www.text);
        this.NodeRetrieved = node;
        if (node == null)
        {
            Debug.LogWarning("Query returned no result: " + this.movieUrl + movieId + "?" + "(apiKey)");
        }
    }

    public IEnumerator RetrieveSerie(long serieId)
    {
        WWW www = new WWW(this.serieUrl + serieId + "?" + this.apiKey);
        yield return www;

        JSONNode node = JSON.Parse(www.text);
        this.NodeRetrieved = node;
        if (node == null)
        {
            Debug.LogWarning("Query returned no result: " + this.serieUrl + serieId + "?" + "(apiKey)");
        }
    }

    /// <summary>
    /// Get infos about the artist with the specified id
    /// </summary>
    /// <param name="artistId"></param>
    /// <returns></returns>
    public IEnumerator RetrieveArtist(long artistId)
    {
        WWW www = new WWW(this.personUrl + artistId + "?" + this.apiKey);
        yield return www;

        JSONNode node = JSON.Parse(www.text);
        this.NodeRetrieved = node;
        if (node == null)
        {
            Debug.LogWarning("Query returned no result: " + this.personUrl + artistId + "?" + "(apiKey)");
        }
    }

    public IEnumerator RetrieveMovieGenres()
    {
        WWW www = new WWW(this.movieGenresUrl + "?" + this.apiKey);
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
        WWW www = new WWW(this.serieGenresUrl + "?" + this.apiKey);
        yield return www;

        JSONNode node = JSON.Parse(www.text);
        this.NodeRetrieved = node;
        if (node == null)
        {
            Debug.LogError("Nothing retrieved!");
        }
    }
}
