using System.Collections.Generic;
using SimpleJSON;

public class DirectorRetriever : GraphElementRetriever
{
    private readonly string _entryQuery;
    private readonly string _targetQuery;

    public DirectorRetriever(string entryQuery, string targetQuery)
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
        for (int i = 0; i < node["crew"].Count; i++)
        {
            if (node["crew"][i]["job"].Value == "Director")
            {
                var artistId = node["crew"][i]["id"].Value;
                long id;
                if (long.TryParse(artistId, out id))
                {
                    ids.Add(id);
                }
            }
        }

        return ids;
    }
}
