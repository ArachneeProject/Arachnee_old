using UnityEngine;
using System.Collections;


public class Movie : MonoBehaviour
{
    public static int moviesCounter = 0;

    // id of the movie
    public int Id
    {
        get;
        set;
    }

    // title of the movie
    public string Title
    {
        get;
        set;
    }

    // date of release
    public int Date
    {
        get;
        set;
    }

    public Artist Director
    {
        get;
        set;
    }


    /// <summary>
    /// default constructor
    /// </summary>
    public Movie()
    {
        this.Id = -1;
        this.Title = "Default title";
        this.Date = 1899;
    }



    /// <summary>
    /// highlights the movie
    /// </summary>
    public void OnMouseEnter()
    {
        renderer.material.color = Color.red;
        Debug.Log(this.Title + ", " + this.Date);
    }

    /// <summary>
    /// stops highlighting the movie
    /// </summary>
    public void OnMouseExit()
    {
        renderer.material.color = Color.white;
    }



}
