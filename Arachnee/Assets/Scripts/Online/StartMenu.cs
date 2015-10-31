using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleJSON;

public class StartMenu : MonoBehaviour 
{
    public InputField inputField;
    public MovieSearchResult[] movieResults;
    public Text loading;

    private OnlineRetriever onlineRetriever = new OnlineRetriever();

    
    /// <summary>
    /// public function for the button in menu
    /// </summary>
    public void GoGetResults()
    {
        StartCoroutine(this.goGetResultsOnline());
    }

    /// <summary>
    /// go get the movies online
    /// </summary>
    private IEnumerator goGetResultsOnline()
    {
        this.resetFields();

        yield return StartCoroutine(this.onlineRetriever.RetrieveResultOfSerach(this.inputField.text));

        JSONNode resultsNode = this.onlineRetriever.LastJSONNodeRetrieved;

        Debug.Log("Checking result...");
        if (resultsNode == null)
        {
            Logger.Trace("No result", LogLevel.Info);
            yield break;
        }

        for (int i = 0; i < resultsNode.Count && i < this.movieResults.Length; i++)
        {
            this.movieResults[i].Title.text = resultsNode[i]["original_title"].Value;
            this.movieResults[i].Overview = resultsNode[i]["overview"].Value;

            string posterUrl = resultsNode[i]["poster_path"].Value;
            if (posterUrl != "null")
            {
                yield return StartCoroutine(this.onlineRetriever.RetrievePoster(posterUrl, this.movieResults[i].Image));
            }
            else
            {
                Logger.Trace("No poster found", LogLevel.Warning);
                this.loading.gameObject.SetActive(false);
            }
        }
    }


    private void resetFields()
    {
        for (int i = 0; i < this.movieResults.Length; i++)
        {
            this.movieResults[i].Image.sprite = null;
            this.movieResults[i].Title.text = "-";
        }
    }

}
