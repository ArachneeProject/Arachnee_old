using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ArtistObjectBuilder : GameObjectBuilder
{
    /// <summary>
    /// Constructor 
    /// </summary>
    /// <param name="prefab"></param>
    public ArtistObjectBuilder(GameObject prefab) : base(prefab) { }

    /// <summary>
    /// Build the Artist Game Object
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public override GameObject BuildGameObject(Vector3 position)
    {
        throw new NotImplementedException();
    }
}
