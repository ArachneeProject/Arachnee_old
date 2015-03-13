using UnityEngine;
using System.Collections.Generic;


public class Movie : Entry
{
    
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

    

    /// <summary>
    /// constructor
    /// </summary>
    public Movie()
    {
        this.Id = -1;
        this.Title = "Default title";
        this.Date = 1899;
    }



    /// <summary>
    /// highlights the movie and its connexions
    /// </summary>
    void OnMouseEnter()
    {
        renderer.material.color = Color.red;
        ConnexionBank.highlightConnexions<Movie>(this.Id);
        Debug.Log(this.Title + ", " + this.Date);
    }

    /// <summary>
    ///  stop highlighting and mask connexions
    /// </summary>
    void OnMouseExit()
    {
        renderer.material.color = Color.white;
        ConnexionBank.maskAll();
    }

    
   



}
