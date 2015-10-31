using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MovieSearchResult : MonoBehaviour 
{
    public Image Image;
    public Text Title;
    public Text overview;

    public int Id
    {
        get;
        set;
    }


    public string Overview
    {
        get;
        set;
    }


    public void SelectThis()
    {
        this.overview.text = this.Overview;
        PlayerPrefs.SetInt("MovieID", this.Id);
    }
}
