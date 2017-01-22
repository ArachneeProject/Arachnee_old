public class CastRetriever : ArtistMovieConnectionRetriever
{
    protected override string GetQuery(string entryId)
    {
        return Constants.movieUrl + entryId + Constants.castQuery + Constants.apiKey;
    }
}

