using SimpleJSON;

namespace Assets.Scripts.Online
{
    public class MovieRetriever : EntryRetriever
    {
        protected override string GetEntryQuery(string entryId)
        {
            return Constants.movieUrl + entryId + "?" + Constants.apiKey;
        }

        protected override Entry BuildEntry(JSONNode node)
        {
            long movieId;
            if (!long.TryParse(node["id"].Value, out movieId))
            {
                return Entry.DefaultEntry;
            }
            var movie = new Movie(movieId);
            
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
}
