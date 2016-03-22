using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
public class Browser : MonoBehaviour 
{
    public Transform grid;
    public GameObject directoryButtonPrefab;

	// Use this for initialization
	void Start () 
    {
        string path = PlayerPrefs.GetString(Constants.PP_FOLDER_PATH);
        if (path == "")
        {
            Debug.LogWarning("Folder not defined");
            Application.LoadLevel(Constants.SC_FOLDER_SETTING);
            return;
        }
        if (!Directory.Exists(path))
        {
            Debug.LogWarning(path + " doesn't exist");
            Application.LoadLevel(Constants.SC_FOLDER_SETTING);
            return;
        }

        DirectoryInfo[] di = new DirectoryInfo(path).GetDirectories();

        foreach (DirectoryInfo dir in di)
        {
            GameObject go = Instantiate(this.directoryButtonPrefab) as GameObject;
            Text txt = go.GetComponentInChildren<Text>();
            txt.text = dir.Name;
            go.transform.SetParent(grid);
            go.GetComponent<Button>().onClick.AddListener(() => foo(txt.text));            
        }

	
	}
	
    public void foo(string identifier)
    {
        Debug.Log("clicked on " + identifier);
    }
}
