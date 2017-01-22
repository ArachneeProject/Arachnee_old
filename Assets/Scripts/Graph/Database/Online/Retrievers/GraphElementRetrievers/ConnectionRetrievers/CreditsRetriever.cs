public class CreditsRetriever : ArtistMovieConnectionRetriever
{
    protected override string GetQuery(string entryId)
    {
        return Constants.personUrl + entryId + Constants.creditsQuery + Constants.apiKey;
    }
}
