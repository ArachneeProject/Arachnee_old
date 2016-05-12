using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MovieSearchResult : MonoBehaviour 
{
    public Button button;
    public Text overview;

    public Texture2D Texture
    {
        get;
        set;
    }

    public int Id
    {
        get;
        set;
    }

    public string Title
    {
        get;
        set;
    }

    private string _year;
    public string Year
    {
        get { return _year; }
        set 
        {
            if (value.Length > 4)
            {
                _year = value.Substring(0, 4);
            }
            else
            {
                _year = value;
            }            
        }
    }
    
    public string Overview
    {
        get;
        set;
    }

    public string PosterPath
    {
        get;
        set;
    }
}
