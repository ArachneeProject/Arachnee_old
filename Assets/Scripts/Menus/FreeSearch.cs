using UnityEngine;
using System.Collections;

public class FreeSearch : MonoBehaviour 
{
    public InternetSearch internetSearch;
    public Navigation nav;

    public void GoButton()
    {
        DatabaseDialoger dlg = new DatabaseDialoger();

        var selected = this.internetSearch.movieSearch.results[internetSearch.SelectedResult];

        PlayerPrefs.SetInt(Constants.PP_MovieID, selected.Id);

        if (!dlg.MovieIsRegistered(selected.Id))
        {
            dlg.InsertMovie(selected.Id, selected.Title, selected.Year, selected.PosterPath, false);
            string toUpdate = PlayerPrefs.GetString(Constants.PP_MoviesToUpdate);
            if (toUpdate == string.Empty)
            {
                toUpdate += selected.Id;
            }
            else
            {
                toUpdate += "," + selected.Id;
            }
            PlayerPrefs.SetString(Constants.PP_MoviesToUpdate, toUpdate);
        }

        nav.GoToDatabaseUpdate();        
    }

}
