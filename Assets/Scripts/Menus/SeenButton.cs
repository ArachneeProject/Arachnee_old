using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SeenButton : MonoBehaviour 
{
    public Image img;
    private DatabaseDialoger dlg;
    private long id = 0;
    private bool seen = false;
	void Start () 
    {
        this.dlg = new DatabaseDialoger();

        this.id = PlayerPrefs.GetInt(Constants.PP_MovieID);
        if (this.id > 0)
        {
            seen = this.dlg.MovieWasSeen(id);
        }
        else if (this.id < 0)
        {
            seen = this.dlg.SerieWasSeen(-id);
        }
        else
        {
            Debug.LogError("No id");
            return;
        }
                
        
        if (seen)
        {
            img.color = Color.yellow;
        }
        else
        {
            img.color = new Color(255,255,255,0.35F);
        }
	}
	
    public void ClickButton()
    {
        seen = !seen;

        if (this.id > 0)
        {
            this.dlg.UpdateMovieSeen(this.id, seen);
        }
        else if (this.id < 0)
        {
            this.dlg.UpdateSerieSeen(-this.id, seen);
        }
        else
        {
            Debug.LogError("No id");
            return;
        }

        if (seen)
        {
            img.color = Color.yellow;
        }
        else
        {
            img.color = new Color(255, 255, 255, 0.35F);
        }
    }
}
