using System;
using SimpleJSON;
using UnityEngine;

public class ArtistRetriever : GraphElementRetriever
{
    protected override string GetQuery(string entryId)
    {
        return Constants.ArtistUrl + entryId + "?" + Credentials.ApiKey;
    }

    protected override object BuildResult(JSONNode node)
    {
        if (node["profile_path"].Value == "null")
        {
            return Entry.DefaultEntry;
        }
        return new Artist(node);
    }
}

