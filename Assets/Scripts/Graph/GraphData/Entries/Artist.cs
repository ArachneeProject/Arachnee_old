using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Database("Artists","id_artist")]
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

    public Artist()
    {
        this.DatabaseId = DefaultId;
    }

    public Artist(long databaseId)
    {
        this.DatabaseId = databaseId;
    }

    public override string ToString()
    {
        return this.FirstName + " " + this.LastName;
    }

    public override Dictionary<string, object> Serialize()
    {
        return new Dictionary<string, object>
        {
            {"id_artist", this.DatabaseId},
            {"first_name", this.FirstName},
            {"last_name", this.FirstName},
            {"poster_path", this.PosterPath}
        };
    }

    public override void Deserialize(Dictionary<string, object> data)
    {
        this.DatabaseId = (long) data["id_artist"];
        this.FirstName = (string)data["first_name"];
        this.LastName = (string)data["last_name"];
        this.PosterPath = (string)data["poster_path"];
    }

    public override IEnumerable<Entry> GetOppositeEntries(ConnectionType connectionType, HashSet<long> oppositeIds, DatabaseDialoger database)
    {
        switch (connectionType)
        {
                case ConnectionType.Actor:
                case ConnectionType.Director:
                return database.GetEntries<Movie>(oppositeIds).Select(m => (Entry) m);
        }
        Debug.LogWarning("Not able to build entries opposed to Artist by " + connectionType + " connection");
        return Enumerable.Empty<Entry>();
    }

    public override Type GetOppositeOf(ConnectionType connectionType)
    {
        switch (connectionType)
        {
            case ConnectionType.Actor:
            case ConnectionType.Director:
                return typeof(Movie);
        }
        Debug.LogWarning("Artist type is not linked to any other type by " + connectionType + " connection");
        return typeof(object);
    }
}
