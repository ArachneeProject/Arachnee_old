using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Serie : Entry
{
    public string Title { get; set; }
    public int StartYear { get; set; }

    public Serie(long databaseId)
    {
        this.DatabaseId = databaseId;
    }

    public override string ToString()
    {
        return this.Title;
    }

    public override Dictionary<string, object> Serialize()
    {
        throw new NotImplementedException();
    }

    public override void Deserialize(Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<Entry> GetOppositeEntries(ConnectionType connectionType, HashSet<long> oppositeIds, DatabaseDialoger database)
    {
        throw new NotImplementedException();
    }

    public override Type GetOppositeOf(ConnectionType connectionType)
    {
        throw new NotImplementedException();
    }

    public override OnlineRetriever GetConnectionRetriever(ConnectionType connectionType)
    {
        throw new NotImplementedException();
    }

    public override OnlineRetriever GetOppositeEntryRetriever(ConnectionType connectionType)
    {
        throw new NotImplementedException();
    }
}

