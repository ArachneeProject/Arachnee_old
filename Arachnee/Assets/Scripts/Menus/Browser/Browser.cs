using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
public class Browser : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
    {
        string path = PlayerPrefs.GetString(Constants.PP_FOLDER_PATH);
        if (path == string.Empty || File.Exists(path))
        {
            Debug.Log("yolo");
        }
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
