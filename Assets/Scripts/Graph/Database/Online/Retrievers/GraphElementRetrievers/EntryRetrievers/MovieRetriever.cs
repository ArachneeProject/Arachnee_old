using SimpleJSON;

public class MovieRetriever : GraphElementRetriever
{
    protected override string GetQuery(string entryId)
    {
        return Constants.movieUrl + entryId + "?" + Constants.apiKey;
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

