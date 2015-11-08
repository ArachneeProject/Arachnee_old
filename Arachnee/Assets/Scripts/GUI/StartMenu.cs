using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleJSON;

public class StartMenu : MonoBehaviour 
{
    public InputField inputField;
    public MovieSearchResult[] movieResults;
    public Text loading;
    public Text overview;

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
        this.loading.gameObject.SetActive(true);

        yield return StartCoroutine(this.onlineRetriever.RetrieveResultOfSerach(this.inputField.text));

        JSONNode resultsNode = this.onlineRetriever.NodeRetrieved;

        if (resultsNode == null)
        {
            Logger.Trace("No result", LogLevel.Info);
            this.loading.gameObject.SetActive(false);
            yield break;
        }

        for (int i = 0; i < resultsNode.Count && i < this.movieResults.Length; i++)
        {
            this.movieResults[i].Id = int.Parse(resultsNode[i]["id"].Value);
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
        this.loading.gameObject.SetActive(false);
    }

    /// <summary>
    /// Reset the fields of the menu
    /// </summary>
    private void resetFields()
    {
        for (int i = 0; i < this.movieResults.Length; i++)
        {
            this.movieResults[i].Image.sprite = null;
            this.movieResults[i].Title.text = "-";
        }
        this.overview.text = "";
    }

}
