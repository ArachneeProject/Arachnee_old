using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleJSON;

public class InternetSearch : MonoBehaviour 
{
    public bool isMovieSearch = true;
    public InputField inputField;
    public MovieSearch movieSearch;
    public Text loading;
    public Text overview;
    public Button okButton;

    public Image[] resultImages;
    public Text[] resultTitles;

    private int selectedResult = -1;
    private Sprite defaultSprite;

    public int SelectedResult
    {
        get { return this.selectedResult; }
        private set { }
    }

    void Start()
    {
        this.loading.gameObject.SetActive(false);
        foreach (MovieSearchResult msr in this.movieSearch.results)
        {
            msr.gameObject.SetActive(false);
        }
        okButton.gameObject.SetActive(false);
        this.overview.text = "";

        Texture2D tex = Resources.Load(Constants.Res_DefaultImage) as Texture2D;
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
        this.okButton.gameObject.SetActive(false);
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
        yield return new WaitForEndOfFrame();
        this.loading.gameObject.SetActive(true);
        yield return StartCoroutine(this.movieSearch.GetResults(this.inputField.text, isMovieSearch));
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
            Debug.LogError("caca here");

            /*
            if (res.Id != Constants.NULL_MOVIE_ID)
            {
                res.gameObject.SetActive(true);
            }
             */
        }
    }

    public void SelectResult(int index)
    {
        this.okButton.gameObject.SetActive(true);
        this.overview.text = this.movieSearch.results[index].Overview;
        this.selectedResult = index;
    }

}
