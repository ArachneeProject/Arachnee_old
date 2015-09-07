using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEngine;

public class ArtistObjectBuilder : EntryGameObjectBuilder
{
    /// <summary>
    /// Constructor 
    /// </summary>
    /// <param name="prefab"></param>
    public ArtistObjectBuilder(GameObject prefab, GraphBuilder gBuilder)
    {
        this.prefab = prefab;
        this.GraphBuilder = gBuilder;
    }


    protected override Entry addConcreteComponent(GameObject obj, DataRow row)
    {
        Artist art = obj.AddComponent<Artist>();

        art.Id = (uint)((Int64)row[0]);
        art.FirstName = (string) row[1];
        art.LastName = (string) row[2];

        Logger.Trace("Building " + art.FirstName + " " + art.LastName, LogLevel.Debug);

        return art;
    }
}
