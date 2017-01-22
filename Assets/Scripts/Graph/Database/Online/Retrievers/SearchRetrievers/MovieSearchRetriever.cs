using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;

public class MovieSearchRetriever : GraphElementRetriever
{
    protected override string GetQuery(string entryId)
    {
        return Constants.MovieSearchUrl + entryId + "&" + Constants.ApiKey;
    }

    protected override object BuildResult(JSONNode node)
    {
        long movieId;
        if (!long.TryParse(node["id"].Value, out movieId))
        {
            return Entry.DefaultEntry;
        }
        var movie = new Movie(movieId);

        if (node["poster_path"].Value == "null")
        {
            return Entry.DefaultEntry;
        }

        movie.PosterPath = node["poster_path"].Value;
        movie.Title = node["title"].Value;
        movie.Overview = node["overview"];

        return movie;
    }
}
