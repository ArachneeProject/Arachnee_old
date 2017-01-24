using System;
using SimpleJSON;

public class ArtistSearchRetriever : GraphElementRetriever
{
    protected override string GetQuery(string entryId)
    {
        return Constants.ArtistSearchUrl + entryId + "&" + Credentials.ApiKey;
    }

    protected override object BuildResult(JSONNode node)
    {
        var artist = new Artist(long.Parse(node["id"].Value));

        if (node["profile_path"].Value == "null")
        {
            
        }
        return artist;
    }
}

