using UnityEngine;
using System.Collections;
using System;
using SimpleJSON;
using UnityEngine.UI;

public class OnlineRetriever : MonoBehaviour 
{
    //private string testurl = "https://api.themoviedb.org/3/movie/550?api_key=The Skeleton Key";
    //private string testurlSerach = "https://api.themoviedb.org/3/search/movie?query=cube&api_key=The Skeleton Key";
    //private string testposterUrl = "http://image.tmdb.org/t/p/w500/u50r6chJGO2iqxVAvtQ07obiCkB.jpg";

    public InputField inputField;
    public MovieSearchResult[] movieResults;
    public Text loading;
    public Text overviewBox;

    private string urlSearch = "https://api.themoviedb.org/3/search/movie?query=";
    private string posterUrl = "http://image.tmdb.org/t/p/w500";

    private string apiKey = " x x x x ";


    /// <summary>
    /// go get the movies online
    /// </summary>
    public void Retrieve()
    {
        this.resetFields();
        string processedInput = this.processInput(this.inputField.text);
        StartCoroutine(this.retrieveResultOfSerach(processedInput));
    }

    private void resetFields()
    {
        for (int i = 0; i<this.movieResults.Length ; i++)
        {
            this.movieResults[i].Image.sprite = null;
            this.movieResults[i].Title.text = "-";
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


    private IEnumerator retrieveResultOfSerach(string input)
    {
        this.loading.gameObject.SetActive(true);

        WWW www = new WWW(this.urlSearch + input + this.apiKey);
        yield return www;

        JSONNode node = JSON.Parse(www.text);
        if (node["results"] == null)
        {
            Logger.Trace("No result", LogLevel.Error);
            this.loading.gameObject.SetActive(false);
            yield break;
        }

        JSONNode resultsNode = node["results"];
        for (int i = 0; i < resultsNode.Count && i < this.movieResults.Length; i++)
        {
            this.movieResults[i].Title.text = resultsNode[i]["original_title"].Value;
            this.movieResults[i].Overview = resultsNode[i]["overview"].Value;

            string posterUrl = resultsNode[i]["poster_path"].Value;
            if (posterUrl != "null")
            {
                StartCoroutine(this.retrievePoster(posterUrl, this.movieResults[i].Image));
            }
            else
            {
                Logger.Trace("No poster found", LogLevel.Warning);
                this.loading.gameObject.SetActive(false);
            }
            
        }
    }


    /// <summary>
    /// Retrieve the poster at "posterPath" and apply it on the image "img"
    /// </summary>
    /// <param name="posterPath"></param>
    /// <param name="img"></param>
    /// <returns></returns>
    private IEnumerator retrievePoster(string posterPath, Image img)
    {
        WWW www = new WWW(this.posterUrl + posterPath);

        yield return www;

        if (www.texture != null && img != null)
        {
            img.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.zero);
        }

        this.loading.gameObject.SetActive(false);
    }


    public void SelectResult(string overview)
    {
        this.overviewBox.text = overview;
    }
}
