using System;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public class ArtistSearchRetriever : GraphElementRetriever
{
    protected override string GetQuery(string entryId)
    {
        return Constants.ArtistSearchUrl + entryId + "&" + Credentials.ApiKey;
    }

    protected override object BuildResult(JSONNode node)
    {
        if (node["results"] == null)
        {
            Debug.LogError("Unable to get any result");
            return Enumerable.Empty<Artist>();
        }

        var results = new List<Artist>();
        foreach (var artistNode in node["results"].Childs)
        {
            if (artistNode["profile_path"].Value != "null")
            {
                results.Add(new Artist(artistNode));
            }
        }
        return results;
    }
}

