public class CastRetriever : ArtistMovieConnectionRetriever
{
    protected override string GetQuery(string entryId)
    {
        return Constants.MovieUrl + entryId + Constants.CastQuery + Constants.ApiKey;
    }
}

