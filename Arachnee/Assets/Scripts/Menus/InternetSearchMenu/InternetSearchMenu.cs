using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleJSON;

public class InternetSearchMenu : MonoBehaviour 
{
    public InputField inputField;
    public MovieSearch movieSearch;
    public Text loading;
    public Text overview;
    public Button buildButton;

    public Image[] resultImages;
    public Text[] resultTitles;

    private int selectedResult = -1;
    private Sprite defaultSprite;

    void Start()
    {
        this.loading.gameObject.SetActive(false);
        foreach (MovieSearchResult msr in this.movieSearch.results)
        {
            msr.gameObject.SetActive(false);
        }
        buildButton.gameObject.SetActive(false);
        this.overview.text = "";

        Texture2D tex = Resources.Load(Constants.RES_DEFAULT_IMAGE) as Texture2D;
        this.defaultSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
    }

    /// <summary>
    /// public function for the button in menu
    /// </summary>
    public void GoGetResults()
    {
        foreach (MovieSearchResult msr in this.movieSearch.results)
        {
            msr.gameObject.SetActive(false);
        }
        this.buildButton.gameObject.SetActive(false);
        this.overview.text = "";

        if (this.inputField.text != "")
        {
            StartCoroutine(this.goGetResultsOnline());
        }       
    }

    /// <summary>
    /// go get the movies online
    /// </summary>
    private IEnumerator goGetResultsOnline()
    {
        this.loading.gameObject.SetActive(true);
        yield return StartCoroutine(this.movieSearch.GetResults(this.inputField.text));
        this.loading.gameObject.SetActive(false);

        for (int i = 0; i < this.resultImages.Length; i++)
        {
            var tex = this.movieSearch.results[i].Texture;
            if (tex == null)
            {
                this.resultImages[i].sprite = this.defaultSprite;
            }
            else
            {
                this.resultImages[i].sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            }
        }
        for (int i = 0; i < this.resultTitles.Length; i++)
        {
            this.resultTitles[i].text = this.movieSearch.results[i].Title + "(" + this.movieSearch.results[i].Year + ")";
        }

        foreach (MovieSearchResult res in this.movieSearch.results)
        {
            if (res.Id != Constants.NULL_MOVIE_ID)
            {
                res.gameObject.SetActive(true);
            }
        }
    }

    public void SelectResult(int index)
    {
        this.buildButton.gameObject.SetActive(true);
        this.overview.text = this.movieSearch.results[index].Overview;
        this.selectedResult = index;
    }

    public void LaunchBuild()
    {
        PlayerPrefs.SetInt(Constants.PP_MOVIE_ID, this.movieSearch.results[this.selectedResult].Id);
        Application.LoadLevel(Constants.SC_DATABASE_UPDATE);
    }
}
