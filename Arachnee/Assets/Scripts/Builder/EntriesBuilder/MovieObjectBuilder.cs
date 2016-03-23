using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEngine;

public class MovieObjectBuilder : EntryGameObjectBuilder
{
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="prefab"></param>
    public MovieObjectBuilder(GameObject prefab, GraphBuilder gBuild, int rangeOfBuild)
    {
        this.prefab = prefab;
        this.GraphBuilder = gBuild;
        this.rangeOfBuilding = rangeOfBuild;
    }


    protected override Entry addConcreteComponent(GameObject obj, DataRow row)
    {
        Movie mv = obj.AddComponent<Movie>();

        mv.DatabaseId = (Int64) row[0];
        mv.Title = (string)row[1];
        mv.Year = (int)((Int64) row[2]);
        mv.PosterPath = (string)row[3];

        //Logger.Trace("Building " + mv.Title + " " + mv.Year, LogLevel.Debug);

        return mv;
    }
    
}
