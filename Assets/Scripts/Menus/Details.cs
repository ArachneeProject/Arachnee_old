using UnityEngine;
using System.Collections;
using SimpleJSON;
using UnityEngine.UI;
using System.IO;

public class Details : MonoBehaviour 
{
    OnlineRetriever onlret = new OnlineRetriever();

    public Text original_title;
    public Text akatitle;
    public Text year; // release_date / first_air_date
    
    public Text genres;
    public Text runtime; // runtime / number_of_episodes + number_of_seasons
    public Text vote_average;

    public Image poster;
    public Text overview;

    public Button seeGraphButton;

	void Start () 
    {
        seeGraphButton.gameObject.SetActive(false);

        long id = PlayerPrefs.GetInt(Constants.PP_MovieID);
        if (id == 0)
        {
            Debug.LogWarning("nothing provided");
            Application.LoadLevel(Navigation.Browser);
            return;
        }

        original_title.text = "";
        akatitle.text = "Loading...";
        year.text = "";
        genres.text = "";
        runtime.text = "";
        vote_average.text = "";
        overview.text = "";

        if (id<0)
        {
            StartCoroutine(showDetailsSerie(-id));
        }
        else
        {
            this.seeGraphButton.gameObject.SetActive(true);
            StartCoroutine(showDetailsMovie(id));
        }
	}

    private IEnumerator showDetailsMovie(long id)
    {
        yield return StartCoroutine(this.onlret.RetrieveMovie(id));

        JSONNode node = onlret.NodeRetrieved;
        if (node == null)
        {
            Debug.LogError("No result");
            akatitle.text = "A problem occured. Check if your internet connection is working. You can also see the log file for more informations.";
            yield break;
        }

        string value = "";

        // original title
        original_title.text = node["original_title"].Value;

        // aka title
        value = node["title"].Value;
        if (original_title.text != value)
        {
            akatitle.text = "a.k.a. " + value;
        }
        else
        {
            akatitle.text = "";
        }

        // date
        value = node["release_date"].Value;
        if (value.Length > 4)
        {
            value = value.Substring(0, 4);
        }
        original_title.text += " (" + value + ")";

        // genre
        genres.text = "";
        for (int i = 0; i < node["genres"].Count; i++)
        {
            genres.text += node["genres"][i]["name"].Value + ", ";
        }
        genres.text = genres.text.Remove(genres.text.Length - 2);

        // runtime
        runtime.text = node["runtime"].Value + " min.";

        // vote
        vote_average.text = "Average note: " + node["vote_average"].Value + "/10";

        // overview
        overview.text = node["overview"];

        // poster
        string poster = node["poster_path"].Value;
        yield return StartCoroutine(fancy(poster));
    }

    private IEnumerator showDetailsSerie(long id)
    {
        yield return StartCoroutine(this.onlret.RetrieveSerie(id));

        JSONNode node = onlret.NodeRetrieved;
        if (node == null)
        {
            Debug.LogError("Sorry");
            akatitle.text = "A problem occured, check the log file for more informations.";
            yield break;
        }

        string value = "";

        // original title
        original_title.text = node["original_title"].Value;

        // aka title
        value = node["title"].Value;
        if (original_title.text != value)
        {
            akatitle.text = "a.k.a " + value;
        }
        else
        {
            akatitle.text = "";
        }

        // start date
        value = node["first_air_date"].Value;
        if (value.Length > 4)
        {
            value = value.Substring(0, 4);
        }
        original_title.text += " (" + value + " - ";

        // end date
        value = node["last_air_date"].Value;
        if (value.Length > 4)
        {
            value = value.Substring(0, 4);
        }
        original_title.text += value + ")";


        // genre
        genres.text = "";
        for (int i = 0; i < node["genres"].Count; i++)
        {
            genres.text += node["genres"][i]["name"].Value + ", ";
        }
        if (genres.text.Length > 2)
        {
            genres.text = genres.text.Remove(genres.text.Length - 2);
        }        

        // seasons/episodes
        runtime.text = node["number_of_episodes"].Value + " ep. ";
        runtime.text += " (" + node["number_of_seasons"].Value + " seas.)";


        // vote
        vote_average.text = "Average note: " + node["vote_average"].Value + "/10";

        // overview
        overview.text = node["overview"];

        // poster
        string poster = node["poster_path"].Value;
        yield return StartCoroutine(fancy(poster));
    }

	private IEnumerator fancy(string poster)
    {
        string imgPath = Application.dataPath + "/Posters/" + poster.Replace("/", "");
        Texture2D tex;

        if (!File.Exists(imgPath))
        {
            yield return StartCoroutine(this.onlret.RetrievePoster("/" + poster));
            if (onlret.Texture != null)
            {
                var otex = onlret.Texture;
                File.WriteAllBytes(imgPath, otex.EncodeToJPG());
            }
        }

        if (File.Exists(imgPath))
        {
            tex = new Texture2D(2, 2);
            tex.LoadImage(File.ReadAllBytes(imgPath));
        }
        else
        {
            tex = Resources.Load(Constants.Res_DefaultImage) as Texture2D;
        }

        this.poster.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        this.poster.color = Color.white;
    }

    public void OpenFolder()
    {
        var mainFolder = PlayerPrefs.GetString(Constants.PP_MainFolder);
        if (mainFolder == string.Empty || !Directory.Exists(mainFolder))
        {
            Application.LoadLevel(Navigation.MFChange);
            return;
        }
        
        
        string targetHash = PlayerPrefs.GetString(Constants.PP_FolderHash);
        string fullPath = "";

        DirectoryInfo mf = new DirectoryInfo(mainFolder);
        foreach (DirectoryInfo d in mf.GetDirectories())
        {
            var hash = Constants.GetHash(d.Name);
            if (hash == targetHash)
            {
                fullPath = d.FullName;
                break;
            }
        }

        if (fullPath != string.Empty)
        {
            System.Diagnostics.Process.Start(fullPath);
        }
        else
        {
            Debug.LogError("Folder not found! (hash: " + targetHash + ")");
        }
    }
}
