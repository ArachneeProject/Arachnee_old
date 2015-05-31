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
    /// highlights the movie and its connexions
    /// </summary>
    void OnMouseEnter()
    {
        this.particles.startSize *= 2.5F;
        Graph.highlightEdges(this.Id);
        CameraGUI.Principal = this.Surname + " " + this.Name;
        CameraGUI.Date = ""+this.Birthdate;

        this.label.setHighlight(true);

        AudioManager.manager.playOnceAt(0, this.transform.position);
    }
    
    /// <summary>
    ///  stop highlighting and mask connexions
    /// </summary>
    void OnMouseExit()
    {
        this.particles.startSize /= 2.5F;
        CameraGUI.Principal = "";
        CameraGUI.Date = "";

        this.label.setHighlight(false);

        Graph.maskAll();
    }
}

