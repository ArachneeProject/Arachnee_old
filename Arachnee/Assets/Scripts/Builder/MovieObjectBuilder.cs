using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MovieObjectBuilder : GameObjectBuilder
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="prefab"></param>
    public MovieObjectBuilder(GameObject prefab) : base(prefab) { }


    /// <summary>
    /// Build the Movie GameObject
    /// </summary>
    /// <returns></returns>
    public override GameObject BuildGameObject(Vector3 position)
    {
        GameObject obj = base.InstantiatePrefab(position);

        return obj;
    }
}
