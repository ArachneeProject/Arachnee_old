using System;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
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

    public Artist(JSONNode node)
    {
        this.DatabaseId = long.Parse(node["id"].Value);

        var name = node["name"].Value;
        int idx = name.LastIndexOf(' ');
        if (idx < 0)
        {
            this.LastName = name;
            // also known as...
        }
        else
        {
            this.FirstName = name.Substring(0, idx);
            this.LastName = name.Substring(idx + 1);
        }

        if (node["profile_path"].Value == "null")
        {
            Debug.LogError("profile path is null");
            this.PosterPath = null;
        }
        else
        {
            this.PosterPath = node["profile_path"].Value;
        }
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
            {"last_name", this.LastName},
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

    public override OnlineRetriever GetConnectionRetriever(ConnectionType connectionType)
    {
        switch (connectionType)
        {
            case ConnectionType.Actor:
                return new ActorRetriever(Constants.ArtistUrl, Constants.CreditsQuery);
            case ConnectionType.Director:
                return new DirectorRetriever(Constants.ArtistUrl, Constants.CreditsQuery);
        }
        Debug.LogWarning("Artist type is not linked to any other type by " + connectionType + " connection");
        return new EmptyRetriever();
    }

    public override OnlineRetriever GetOppositeEntryRetriever(ConnectionType connectionType)
    {
        switch (connectionType)
        {
            case ConnectionType.Actor:
            case ConnectionType.Director:
                return new MovieRetriever();
        }
        Debug.LogWarning("Artist type is not linked to any other type by " + connectionType + " connection");
        return new EmptyRetriever();
    }
}
