using System.Collections;
using System;
using SimpleJSON;
using UnityEngine;

public class OnlineRetriever 
{
    //private string testurl = "https://api.themoviedb.org/3/movie/550?api_key=The Skeleton Key";
    //private string testurlSerach = "https://api.themoviedb.org/3/search/movie?query=cube&api_key= x x x";
    //private string testposterUrl = "http://image.tmdb.org/t/p/w500/u50r6chJGO2iqxVAvtQ07obiCkB.jpg";
    
    private string urlSearch = "https://api.themoviedb.org/3/search/movie?query=";
    private string posterUrl = "http://image.tmdb.org/t/p/w500";
    private string movieUrl = "https://api.themoviedb.org/3/movie/";
    private string personUrl = "https://api.themoviedb.org/3/person/";
    private string castQuery = "/credits?";
    private string creditsQuery = "/movie_credits?";
    private string apiKey = "api_key= x x x";


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
    public IEnumerator RetrieveResultOfSerach(string input)
    {
        input = this.processInput(input);

        WWW www = new WWW(this.urlSearch + input + "&" + this.apiKey);
        yield return www;

        JSONNode node = JSON.Parse(www.text);
        this.NodeRetrieved = node["results"];
    }


    /// <summary>
    /// Retrieve the poster at "posterPath" and apply it on the image "img"
    /// </summary>
    /// <param name="posterPath"></param>
    /// <param name="img"></param>
    /// <returns></returns>
    public IEnumerator RetrievePoster(string posterPath, UnityEngine.UI.Image img)
    {
        WWW www = new WWW(this.posterUrl + posterPath);
        yield return www;

        if (www.texture != null && img != null)
        {
            img.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.zero);
        }
    }

    /// <summary>
    /// Retrive the cast of the movie
    /// </summary>
    /// <param name="movieId"></param>
    /// <returns></returns>
    public IEnumerator RetrieveCast(int movieId)
    {
        WWW www = new WWW(this.movieUrl + movieId + this.castQuery + this.apiKey);
        yield return www;

        JSONNode node = JSON.Parse(www.text);
        this.NodeRetrieved = node["cast"];
    }

    /// <summary>
    /// Retrieve the movies of the artist
    /// </summary>
    /// <param name="movieId"></param>
    /// <returns></returns>
    public IEnumerator RetrieveCredits(int artistId)
    {
        WWW www = new WWW(this.personUrl + artistId + this.creditsQuery + this.apiKey);
        yield return www;

        JSONNode node = JSON.Parse(www.text);
        this.NodeRetrieved = node["cast"];
    }


    internal IEnumerator RetrieveMovie(int id)
    {
        WWW www = new WWW(this.movieUrl + id + "?" + this.apiKey);
        yield return www;

        JSONNode node = JSON.Parse(www.text);
        this.NodeRetrieved = node;
    }
}
