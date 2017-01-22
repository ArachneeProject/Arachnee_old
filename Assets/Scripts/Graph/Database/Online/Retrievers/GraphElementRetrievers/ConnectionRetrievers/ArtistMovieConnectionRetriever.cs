using System.Collections.Generic;
using SimpleJSON;

public abstract class ArtistMovieConnectionRetriever : GraphElementRetriever
{
    protected override abstract string GetQuery(string entryId);

    protected override object BuildResult(JSONNode node)
    {
        var connectedIds = new Dictionary<string, List<ConnectionType>>();

        // actors
        for (int i = 0; i < node["cast"].Count; i++)
        {
            string artistId = node["cast"][i]["id"].Value;
            if (!connectedIds.ContainsKey(artistId))
            {
                connectedIds[artistId] = new List<ConnectionType>();
            }
            connectedIds[artistId].Add(ConnectionType.Actor);
        }

        // directors
        for (int i = 0; i < node["crew"].Count; i++)
        {
            if (node["crew"][i]["job"].Value == "Director")
            {
                var artistId = node["crew"][i]["id"].Value;
                if (!connectedIds.ContainsKey(artistId))
                {
                    connectedIds[artistId] = new List<ConnectionType>();
                }
                connectedIds[artistId].Add(ConnectionType.Director);
            }
        }

        return connectedIds;
    }
}
