using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour 
{
    public void LoadLevel(int levelIndex)
    {
        Application.LoadLevel(levelIndex);
    }
	
}
