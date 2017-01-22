using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Serie : Entry
{
    public string Title { get; set; }
    public int StartYear { get; set; }

    public Serie(long databaseId)
    {
        this.DatabaseId = databaseId;
    }

    public override string ToString()
    {
        return this.Title;
    }
}

