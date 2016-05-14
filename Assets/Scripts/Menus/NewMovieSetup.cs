using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class NewMovieSetup : MonoBehaviour 
{
    public Text foldername;
    public InputField input;
    public InternetSearch internetSearch;

    public Toggle seenToggle;
    public Toggle[] qualityToggles;

    private DatabaseDialoger dlg;
    private string folder;

	void Start () 
    {
        this.dlg = new DatabaseDialoger();

        this.folder = PlayerPrefs.GetString(Constants.PP_NewFolderName);
        foldername.text = this.folder + " is a new folder.";


        string toInput = PlayerPrefs.GetString(Constants.PP_NewFolderName);

        toInput = toInput.Replace(".", " ");
        toInput = toInput.Replace("_", " ");
        toInput = Regex.Replace(toInput, @".\d{4}.*", "");

        toInput = Regex.Replace(toInput, @"\[.*\]", "");
        toInput = Regex.Replace(toInput, @"\(.*", "");
        toInput = Regex.Replace(toInput, @"=.*", "");
        toInput = Regex.Replace(toInput, @"-.*", "");
        toInput = Regex.Replace(toInput, @"\{.*\}", "");

        toInput = Regex.Replace(toInput, @"720p.*", "");
        toInput = Regex.Replace(toInput, @"1080p.*", "");

        input.text = toInput;
        internetSearch.GoGetResults();
	}
	
	public void MarkAsIgnored()
    {
        var hash = Constants.GetHash(folder);
        dlg.InsertIgnoredFolder(hash);

        Application.LoadLevel(Navigation.MFCheck);
    }

    public void AddMovie()
    {
        var mv = this.internetSearch.movieSearch.results[internetSearch.SelectedResult];

        string quality = "DVD";
        if (qualityToggles[1].isOn)
        {
            quality = "BluRay 720p";
        }
        else if (qualityToggles[2].isOn)
        {
            quality = "BluRay 1080p";
        }
        
        
        this.dlg.InsertMovieFolder(Constants.GetHash(folder), mv.Id, quality);

        if (!this.dlg.MovieIsRegistered(mv.Id))
        {
            this.dlg.InsertMovie(mv.Id, mv.Title, mv.Year, mv.PosterPath, seenToggle.isOn);

            string toUpdate = PlayerPrefs.GetString(Constants.PP_MoviesToUpdate);
            if (toUpdate == string.Empty)
            {
                toUpdate += mv.Id;
            }
            else
            {
                toUpdate += "," + mv.Id;
            }
            PlayerPrefs.SetString(Constants.PP_MoviesToUpdate, toUpdate);
        }
        

        Application.LoadLevel(Navigation.MFCheck);
    }

    public void AddSerie()
    {
        var serie = this.internetSearch.movieSearch.results[internetSearch.SelectedResult];
        
        this.dlg.InsertSerie(serie.Id, serie.Title, serie.Year, serie.PosterPath, seenToggle.isOn);
        this.dlg.InsertSerieFolder(Constants.GetHash(folder), serie.Id);

        Application.LoadLevel(Navigation.MFCheck);
    }
}
