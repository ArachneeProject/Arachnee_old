using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

public class ArtistRetriever : EntryRetriever
{
    protected override string GetEntryQuery(string entryId)
    {
        throw new NotImplementedException();
    }

    protected override Entry BuildEntry(JSONNode node)
    {
        throw new NotImplementedException();
        /*
         var artist = new Artist(artistId);
            string posterPath = node["profile_path"].Value;
            if (posterPath == "null")
            {
                RetrievedEntries.Add(Entry.DefaultEntry);
                yield break;
            }

            string name = node["name"].Value;

            int idx = name.LastIndexOf(' ');
            if (idx < 0)
            {
                artist.LastName = name;
                // also known as...
            }
            else
            {
                artist.FirstName = name.Substring(0, idx);
                artist.FirstName = name.Substring(idx + 1);
            }

            RetrievedEntries.Add(artist);
         */
    }
}

