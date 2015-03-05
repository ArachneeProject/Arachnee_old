using UnityEngine;


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

    /// <summary>
    /// default constructor
    /// </summary>
    public Job ()
    {
        this.Title = "Human";
        this.Description = "No description available.";
    }
}

