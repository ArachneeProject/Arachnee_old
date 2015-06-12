﻿using UnityEngine;


public class Job : Entry
{

    // title of the job
    public string Title
    {
        get;
        set;
    }

    // small description
    public string Description
    {
        get;
        set;
    }

    public override void goOnGUI()
    {
        CameraGUI.Principal = this.Title;
    }

}

