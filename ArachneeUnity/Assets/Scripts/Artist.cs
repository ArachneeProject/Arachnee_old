﻿using UnityEngine;


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

    void OnMouseEnter()
    {
        renderer.material.color = Color.blue;
        Debug.Log(this.Surname + " " + this.Name + " (born in " + this.Birthdate + ")");
    }

    void OnMouseExit()
    {
        renderer.material.color = Color.white;
    }

    void OnMouseOver()
    {
        ConnexionBank.highlightConnexions<Artist>(this.Id);
    }
}

