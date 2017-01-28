using SimpleJSON;
using UnityEngine;

public class MovieRetriever : GraphElementRetriever
{
    protected override string GetQuery(string entryId)
    {
        return Constants.MovieUrl + entryId + "?" + Credentials.ApiKey;
    }

    protected override object BuildResult(JSONNode node)
    {
        if (node["poster_path"].Value == "null")
        {
            return Entry.DefaultEntry;
        }
        return new Movie(node);
    }
}

