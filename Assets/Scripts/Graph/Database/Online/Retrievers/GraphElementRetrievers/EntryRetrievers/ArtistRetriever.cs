using System;
using SimpleJSON;
using UnityEngine;

public class ArtistRetriever : GraphElementRetriever
{
    protected override string GetQuery(string entryId)
    {
        return Constants.personUrl + entryId + "?" + Constants.apiKey;
    }

    protected override object BuildResult(JSONNode node)
    {
        long artistId;
        if (!long.TryParse(node["id"].Value, out artistId))
        {
            Debug.LogError("Unable to parse artist id");
            return Entry.DefaultEntry;
        }
        var artist = new Artist(artistId);

        if (node["profile_path"].Value == "null")
        {
            return Entry.DefaultEntry;
        }
        artist.PosterPath = node["profile_path"].Value;

        var name = node["name"].Value;
        int idx = name.LastIndexOf(' ');
        if (idx < 0)
        {
            artist.LastName = name;
            // also known as...
        }
        else
        {
            artist.FirstName = name.Substring(0, idx);
            artist.LastName = name.Substring(idx + 1);
        }
        
        return artist;
    }
}

