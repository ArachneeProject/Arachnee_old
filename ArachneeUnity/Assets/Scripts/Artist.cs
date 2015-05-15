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

    /// <summary>
    /// default constructor
    /// </summary>
    public Artist()
    {
        this.Name = "Allan";
        this.Surname = "Smithee";
        this.Birthdate = 1950;
    }

    /// <summary>
    /// highlights the movie and its connexions
    /// </summary>
    void OnMouseEnter()
    {
        this.GetComponent<ParticleSystem>().startSize *= 2.5F;
        Graph.highlightEdges(this.Id);
        CameraGUI.Principal = this.Surname + " " + this.Name;
        CameraGUI.Date = ""+this.Birthdate;
    }
    
    /// <summary>
    ///  stop highlighting and mask connexions
    /// </summary>
    void OnMouseExit()
    {
        this.GetComponent<ParticleSystem>().startSize /= 2.5F;
        CameraGUI.Principal = "";
        CameraGUI.Date = "";
        Graph.maskAll();
    }
}

