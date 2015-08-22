using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class GameObjectBuilder : MonoBehaviour
{
    private GameObject prefab; // prefab of the GameObject to build

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="prefab"></param>
    public GameObjectBuilder(GameObject prefab)
    {
        this.prefab = prefab;
    }


    /// <summary>
    /// Create the GameObject
    /// </summary>
    /// <returns></returns>
    public abstract GameObject BuildGameObject(Vector3 position);


    /// <summary>
    /// Clone the prefab
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public GameObject InstantiatePrefab(Vector3 position)
    {
        GameObject go = (GameObject)Instantiate(prefab, position, Quaternion.identity);
        return go;
    }

}
