using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PrettyFolder : MonoBehaviour 
{
    public Image image;
    public Text info;
    public GameObject seenUI;

    public int Id
    {
        get;
        set;
    }

    public string Hash
    {
        get;
        set;
    }

    public string Title
    {
        get;
        set;
    }

    public int Year
    {
        get;
        set;
    }

    public string Quality
    {
        get;
        set;
    }


    private bool _seen;
    public bool Seen
    {
        get { return _seen; }
        set 
        { 
            _seen = value;
            seenUI.SetActive(value);
        }
    }
    

    public string PosterPath
    {
        get;
        set;
    } 

    public bool IsSerie
    {
        get;
        set;
    }

    private List<string> _genres = new List<string>();

    public List<string> Genres
    {
        get { return _genres; }
        set { _genres = value; }
    }
    

    public void ClickOnThisFolder()
    {
        int id = this.Id;
        if (this.IsSerie)
        {
            id = -id;
        }

        Debug.LogError("Not rebuilt");
        //PlayerPrefs.SetInt(Constants.PP_MovieID, id);
        //PlayerPrefs.SetString(Constants.PP_FolderHash, Hash);
        Application.LoadLevel(Navigation.Details);
    }
}
