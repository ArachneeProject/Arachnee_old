using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ActorConnection : Connection
{
    internal override void InitEntries(List<Entry> valueCollection)
    {
        foreach (Entry e in valueCollection)
        {
            // left is artist
            if (e is Artist && e.DatabaseId == this.InitLeftId)
            {
                this.Left = e;
                continue;
            }
            // right is movie
            else if (e is Movie && e.DatabaseId == this.InitRightId)
            {
                this.Right = e;
                continue;
            }
        }

        // code to put in common in Connection
        if (this.Left == null)
        {
            Logger.Trace("Artist with id " + this.InitLeftId + " was not found", LogLevel.Error);
        }
        else
        {
            this.Left.ConnectedEdges.Add(this);
        }
        if (this.Right == null)
        {
            Logger.Trace("Movie with id " + this.InitRightId + " was not found", LogLevel.Error);
        }
        else
        {
            this.Right.ConnectedEdges.Add(this);
        }
        
        //Debug.Log("Actor " + Left.DatabaseId + " Movie " + Right.DatabaseId);
    }
}
