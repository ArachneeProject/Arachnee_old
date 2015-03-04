using UnityEngine;
using System.Collections;


public class Artist : MonoBehaviour
{

    // id
    public int Id
    {
        get;
        set;
    }

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

    // job
    public Job Job
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

    void OnMouseEnter()
    {
        renderer.material.color = Color.blue;
        Debug.Log(this.Surname + " " + this.Name + " (born in " + this.Birthdate + ")");
    }

    void OnMouseExit()
    {
        renderer.material.color = Color.white;
    }
}

