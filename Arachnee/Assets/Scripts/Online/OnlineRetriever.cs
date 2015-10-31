using System.Collections;
using System;
using SimpleJSON;
using UnityEngine;

public class OnlineRetriever 
{
    //private string testurl = "https://api.themoviedb.org/3/movie/550?api_key= x x x";
    //private string testurlSerach = "https://api.themoviedb.org/3/search/movie?query=cube&api_key= x x x";
    //private string testposterUrl = "http://image.tmdb.org/t/p/w500/u50r6chJGO2iqxVAvtQ07obiCkB.jpg";

    

    private string urlSearch = "https://api.themoviedb.org/3/search/movie?query=";
    private string posterUrl = "http://image.tmdb.org/t/p/w500";
    private string apiKey = " x x x ";

    public JSONNode LastJSONNodeRetrieved
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
    public IEnumerator RetrieveResultOfSerach(string input)
    {
        input = this.processInput(input);

        WWW www = new WWW(this.urlSearch + input + this.apiKey);
        Debug.Log(this.urlSearch + input + this.apiKey);
        yield return www;

        JSONNode node = JSON.Parse(www.text);
        if (this.LastJSONNodeRetrieved == null)
        {
            this.LastJSONNodeRetrieved = new JSONNode();
        }
        this.LastJSONNodeRetrieved = node["results"];
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


    
}
