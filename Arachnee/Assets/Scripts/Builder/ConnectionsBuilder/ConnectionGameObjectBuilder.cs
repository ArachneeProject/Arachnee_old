using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class ConnectionGameObjectBuilder : IGameObjectBuilder
{
    protected LineRenderer lineRenderPrefab; // prefab of the LineRenderer to build

    protected GraphBuilder GraphBuilder
    {
        get;
        set;
    }

    /// <summary>
    /// Instantiate the linerenders for each connection in the database
    /// </summary>
    /// <param name="set"></param>
    public void BuildGameObject(DataSet set)
    {
        foreach (DataRow row in set.Tables[0].Rows)
        {
            LineRenderer lr = (LineRenderer)MonoBehaviour.Instantiate(this.lineRenderPrefab, Vector3.zero, Quaternion.identity);
            Connection connect = this.addConcreteComponent(lr.gameObject);

            connect.LineRenderer = lr;

            connect.InitLeftId = (uint)((Int64) row[0]);
            connect.InitRightId = (uint)((Int64) row[1]);
            
            this.GraphBuilder.AddConnectionToGraph(connect);
        }
    }

    protected abstract Connection addConcreteComponent(GameObject go);

}
