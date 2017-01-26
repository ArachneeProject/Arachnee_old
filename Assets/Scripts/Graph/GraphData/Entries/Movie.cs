using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Database("Movies","id_movie")]
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

    public string Overview { get; set; }
    
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

    public Movie()
    {
        this.DatabaseId = DefaultId;
    }

    public Movie(long databaseId)
    {
        this.DatabaseId = databaseId;
    }

    public override string ToString()
    {
        return this.Title;
    }

    public override Dictionary<string, object> Serialize()
    {
        return new Dictionary<string, object>
        {
            {"id_movie", this.DatabaseId},
            {"title", this.Title},
            {"year", this.Year},
            {"poster_path", this.PosterPath}
        };
    }

    public override void Deserialize(Dictionary<string, object> data)
    {
        this.DatabaseId = (long) data["id_movie"];
        this.Title = (string) data["title"];
        this.Year = (int) (long) data["year"];
        this.PosterPath = (string) data["poster_path"];
    }

    public override IEnumerable<Entry> GetOppositeEntries(ConnectionType connectionType, HashSet<long> oppositeIds, DatabaseDialoger database)
    {
        switch (connectionType)
        {
            case ConnectionType.Actor:
            case ConnectionType.Director:
                return database.GetEntries<Artist>(oppositeIds).Select(a => (Entry) a);
        }
        Debug.LogWarning("Not able to build entries opposed to Movie by " + connectionType + " connection");
        return Enumerable.Empty<Entry>();
    }

    public override Type GetOppositeOf(ConnectionType connectionType)
    {
        switch (connectionType)
        {
            case ConnectionType.Actor:
            case ConnectionType.Director:
                return typeof(Artist);
        }
        Debug.LogWarning("Movie type is not linked to any other type by " + connectionType + " connection");
        return typeof (object);
    }

    public override OnlineRetriever GetConnectionRetriever(ConnectionType connectionType)
    {
        switch (connectionType)
        {
            case ConnectionType.Actor:
                return new ActorRetriever(Constants.MovieUrl, Constants.CastQuery);
            case ConnectionType.Director:
                return new DirectorRetriever(Constants.MovieUrl, Constants.CastQuery);
        }
        Debug.LogWarning("Movie type is not linked to any other type by " + connectionType + " connection");
        return new EmptyRetriever();
    }

    public override OnlineRetriever GetOppositeEntryRetriever(ConnectionType connectionType)
    {
        switch (connectionType)
        {
            case ConnectionType.Actor:
            case ConnectionType.Director:
                return new ArtistRetriever();
        }
        Debug.LogWarning("Movie type is not linked to any other type by " + connectionType + " connection");
        return new EmptyRetriever();
    }
}
