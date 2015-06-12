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

    public override void goOnGUI()
    {
        CameraGUI.Principal = this.Title;
        CameraGUI.Date = "" + this.Date;
    }
    


    

    







}
