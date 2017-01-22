using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The default retriever. Just associate an empty object to each identifier.
/// </summary>
public class EmptyRetriever : OnlineRetriever
{
    protected override IEnumerator RetrieveDataImpl(HashSet<string> identifiers)
    {
        foreach (var identifier in identifiers)
        {
            RetrievedData.Add(identifier, new object());
        }
        yield break;
    }
}
