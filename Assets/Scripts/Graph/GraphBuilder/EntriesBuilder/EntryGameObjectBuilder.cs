using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class EntryGameObjectBuilder : IGameObjectBuilder
{
    public GameObject prefab; // prefab of the GameObject to build
    protected float rangeOfBuilding = 20;

    protected GraphBuilder GraphBuilder
    {
        get;
        set;
    }


    /// <summary>
    /// Create the GameObject
    /// </summary>
    /// <returns></returns>
    public void BuildGameObject(DataSet set)
    {        
        foreach (DataRow row in set.Tables[0].Rows)
        {
            GameObject obj = (GameObject) MonoBehaviour.Instantiate(this.prefab, this.rangeOfBuilding * UnityEngine.Random.insideUnitSphere, Quaternion.identity);
            Entry e = this.addConcreteComponent(obj, row);

            this.GraphBuilder.AddEntryToGraph(e);
        }
    }


    /// <summary>
    /// Add the right component
    /// </summary>
    /// <param name="obj"></param>
    protected abstract Entry addConcreteComponent(GameObject obj, DataRow row);


}
