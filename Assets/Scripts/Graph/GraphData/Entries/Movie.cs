using System;

public class Movie : Entry
{
    public string Title { get; set; }

    public int Year { get; set; }

    public int Runtime
    {
        get
        {
            throw new System.NotImplementedException();
        }
        set
        {
        }
    }

    public int Plot
    {
        get
        {
            throw new System.NotImplementedException();
        }
        set
        {
        }
    }
    
    public int Rating
    {
        get
        {
            throw new System.NotImplementedException();
        }
        set
        {
        }
    }
    
    public Movie(long databaseId)
    {
        this.DatabaseId = databaseId;
    }

    public override string ToString()
    {
        return this.Title;
    }
}
