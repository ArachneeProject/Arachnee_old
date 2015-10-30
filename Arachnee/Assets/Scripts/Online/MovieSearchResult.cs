using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MovieSearchResult : MonoBehaviour 
{
    public Image Image;
    public Text Title;
    public Text OverviewBox;

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


    public void SetOverviewBox()
    {
        this.OverviewBox.text = this.Overview;
    }
}
