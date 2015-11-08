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

            uint leftId = (uint)((Int64) row[0]);
            uint rightId = (uint)((Int64) row[1]);
            connect.Left = this.getLeftEntry(leftId);
            connect.Right = this.getRightEntry(rightId);

            //connect.SetUpPositions();

            this.GraphBuilder.AddConnectionToGraph(connect);
        }
    }

    protected abstract Connection addConcreteComponent(GameObject go);

    protected abstract Entry getLeftEntry(uint idLeft);
    protected abstract Entry getRightEntry(uint idRight);

}
