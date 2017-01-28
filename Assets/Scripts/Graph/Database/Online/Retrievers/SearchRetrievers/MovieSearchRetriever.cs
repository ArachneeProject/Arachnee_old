using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public class MovieSearchRetriever : GraphElementRetriever
{
    protected override string GetQuery(string entryId)
    {
        return Constants.MovieSearchUrl + entryId + "&" + Credentials.ApiKey;
    }

    protected override object BuildResult(JSONNode node)
    {
        if (node["results"] == null)
        {
            Debug.LogError("Unable to get any result");
            return Enumerable.Empty<Movie>();
        }

        var results = new List<Movie>();
        foreach (var movieNode in node["results"].Childs)
        {
            if (movieNode["poster_path"].Value != "null")
            {
                results.Add(new Movie(movieNode));
            }
        }
        return results;
    }
}
