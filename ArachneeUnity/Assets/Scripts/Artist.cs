using UnityEngine;
using System.Collections;


public class Artist : MonoBehaviour
{
    // store the number of Artists
    public static int artistCounter = 0;

    // id
    public int Id
    {
        get;
        private set;
    }

    // name
    public string Name
    {
        get;
        private set;
    }

    // surname
    public string Surname
    {
        get;
        private set;
    }

    // date of birth
    public int Birthdate
    {
        get;
        private set;
    }

    // job
    public Job Job
    {
        get;
        private set;
    }

    public Artist()
    {
        this.Id = ++artistCounter;
        this.Name = "Allan";
        this.Surname = "Smithee";
        this.Birthdate = 1950;
    }
}

