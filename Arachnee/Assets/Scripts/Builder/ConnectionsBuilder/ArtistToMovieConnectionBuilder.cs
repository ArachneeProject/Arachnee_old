using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class ArtistToMovieConnectionBuilder : ConnectionGameObjectBuilder
{


    /// <summary>
    /// return the artist for the left entry
    /// </summary>
    /// <param name="idLeft"></param>
    /// <returns></returns>
    protected override Entry getLeftEntry(uint idLeft)
    {
        return this.GraphBuilder.GetEntryWithId(this.GraphBuilder.GetNewIdOfArtist(idLeft));
    }

    /// <summary>
    /// returns the movie for the right entry
    /// </summary>
    /// <param name="idRight"></param>
    /// <returns></returns>
    protected override Entry getRightEntry(uint idRight)
    {
        return this.GraphBuilder.GetEntryWithId(this.GraphBuilder.GetNewIdOfMovie(idRight));
    }
}
