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
    /// highlights the movie and its connexions
    /// </summary>
    void OnMouseEnter()
    {
        this.particles.startSize *= 3;
        Graph.highlightEdges(this.Id);
        CameraGUI.Principal = this.Title;
        CameraGUI.Date = ""+this.Date;

        this.label.setHighlight(true);

        AudioManager.manager.playOnceAt(0, this.transform.position);
    }

    /// <summary>
    ///  stop highlighting and mask connexions
    /// </summary>
    void OnMouseExit()
    {
        CameraGUI.Principal = "";
        CameraGUI.Date = "";
        this.particles.startSize /= 3;

        this.label.setHighlight(false);

        Graph.maskAll();
    }

    
   



}
