using System;

public class Artist : Entry
{
    public string FirstName
    {
        get;
        set;
    }

    public string LastName
    {
        get;
        set;
    }

    public int BirthYear
    {
        get;
        set;
    }

    public Artist(long databaseId)
    {
        this.DatabaseId = databaseId;
    }

    public override string ToString()
    {
        return this.FirstName + " " + this.LastName;
    }
}
