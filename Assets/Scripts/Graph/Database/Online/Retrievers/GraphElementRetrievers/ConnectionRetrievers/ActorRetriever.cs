using System.Collections.Generic;
using SimpleJSON;

public class ActorRetriever : GraphElementRetriever
{
    private readonly string _entryQuery;
    private readonly string _targetQuery;

    public ActorRetriever(string entryQuery, string targetQuery)
    {
        _entryQuery = entryQuery;
        _targetQuery = targetQuery;
    }

    protected override string GetQuery(string entryId)
    {
        return _entryQuery + entryId + _targetQuery + Credentials.ApiKey;
    }

    protected override object BuildResult(JSONNode node)
    {
        var ids = new List<long>();

        for (int i = 0; i < node["cast"].Count; i++)
        {
            string artistId = node["cast"][i]["id"].Value;
            long id;
            if (long.TryParse(artistId, out id))
            {
                ids.Add(id);
            }
        }

        return ids;
    }
}

