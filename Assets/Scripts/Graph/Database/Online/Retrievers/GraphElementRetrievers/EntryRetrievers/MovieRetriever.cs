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
        var movie = new Movie(long.Parse(node["id"].Value));
        
        movie.PosterPath = node["poster_path"].Value;
        movie.Title = node["original_title"].Value;
        string date = node["release_date"].Value;
        if (date.Length >= 4)
        {
            int year;
            int.TryParse(date.Substring(0, 4), out year);
            movie.Year = year;
        }

        return movie;
    }
}

