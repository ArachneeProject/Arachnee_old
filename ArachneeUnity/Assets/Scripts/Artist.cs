using UnityEngine;


public class Artist : Entry
{

    // name
    public string Name
    {
        get;
        set;
    }

    // surname
    public string Surname
    {
        get;
        set;
    }

    // date of birth
    public int Birthdate
    {
        get;
        set;
    }


    public override void goOnGUI()
    {
        CameraGUI.Principal = this.Surname + " " + this.Name;
        CameraGUI.Date = ""+this.Birthdate;
    }
}

