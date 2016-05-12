using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
        foldername.text = folder + " is a new folder.";
        
        // regexp...
        input.text = folder.Replace(".", " ");
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
