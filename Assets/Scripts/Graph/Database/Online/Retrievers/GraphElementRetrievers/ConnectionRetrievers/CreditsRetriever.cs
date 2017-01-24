public class CreditsRetriever : ArtistMovieConnectionRetriever
{
    protected override string GetQuery(string entryId)
    {
        return Constants.ArtistUrl + entryId + Constants.CreditsQuery + Credentials.ApiKey;
    }
}
