using UnityEngine;
using System.Collections;
using SimpleJSON;

public class MovieSearch : MonoBehaviour 
{
    public MovieSearchResult[] results;
    private OnlineRetriever onlineRetriever = new OnlineRetriever();

    
    public IEnumerator GetResults(string input)
    {
        foreach (MovieSearchResult res in this.results)
        {
            res.Id = Constants.NULL_MOVIE_ID;
            res.Title = "";
            res.Year = "";
            res.Texture = null;
        }

        yield return StartCoroutine(this.onlineRetriever.RetrieveResultOfSerach(input));

        JSONNode resultsNode = this.onlineRetriever.NodeRetrieved;
        if (resultsNode == null)
        {
            yield break;
        }

        for (int i = 0; i < resultsNode.Count && i < this.results.Length; i++)
        {
            this.results[i].Id = int.Parse(resultsNode[i]["id"].Value);
            this.results[i].Title = resultsNode[i]["original_title"].Value;
            this.results[i].Year = resultsNode[i]["release_date"].Value;
            this.results[i].Overview = resultsNode[i]["overview"].Value;

            string posterUrl = resultsNode[i]["poster_path"].Value;
            if (posterUrl != "null")
            {
                yield return StartCoroutine(this.onlineRetriever.RetrievePoster(posterUrl));
                this.results[i].Texture = this.onlineRetriever.Texture;
            }
            if (this.results[i].Texture == null)
            {
                this.results[i].Texture = Resources.Load(Constants.RES_DEFAULT_IMAGE) as UnityEngine.Texture2D;
            }
        }
    }


}
