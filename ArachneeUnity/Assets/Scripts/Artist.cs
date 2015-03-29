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
        this.particleSystem.startSize *= 2.5F;
        Graph.highlightConnexions<Artist>(this.Id);
        Debug.Log(this.Surname + " " + this.Name + " (born in " + this.Birthdate + ")");
    }
    
    /// <summary>
    ///  stop highlighting and mask connexions
    /// </summary>
    void OnMouseExit()
    {
        this.particleSystem.startSize /= 2.5F;
        Graph.maskAll();
    }
}

