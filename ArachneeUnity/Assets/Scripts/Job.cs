using UnityEngine;
using System.Collections;


public class Job : MonoBehaviour
{
    // store the number of Jobs
    public static int jobCounter = 0;

    // id
    public int Id
    {
        get;
        private set;
    }

    // title of the job
    public string Title
    {
        get;
        private set;
    }

    // small description
    public string Description
    {
        get;
        private set;
    }

    /// <summary>
    /// default constructor
    /// </summary>
    public Job ()
    {
        this.Id = ++jobCounter;
        this.Title = "Human";
        this.Description = "No description available.";
    }
}

