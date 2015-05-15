using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour 
{
	public void loadScene(int sceneIndex)
    {
        Application.LoadLevel(sceneIndex);
    }

    public void clearGraph()
    {
        Graph.clearGraph();
    }
}
