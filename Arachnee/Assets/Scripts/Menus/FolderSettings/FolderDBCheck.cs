using UnityEngine;
using System.Collections;
using System.IO;

public class FolderDBCheck : MonoBehaviour 
{
    DatabaseDialoger dlg = new DatabaseDialoger();

	// Use this for initialization
	void Start () 
    {
        this.dlg.Initialize();
        string folderPath = PlayerPrefs.GetString(Constants.PP_FOLDER_PATH);
        if (!Directory.Exists(folderPath))
        {
            // Application load level
            Debug.LogError("path doesn't exist");
            return;
        }

        DirectoryInfo di = new DirectoryInfo(folderPath);
        var directories = di.GetDirectories();
        foreach (DirectoryInfo d in directories)
        {
            if (this.dlg.CheckIfEntryExistsInTable("name",d.Name,"folders"))
            {
                Debug.Log(d.Name + " was found !");
            }
            else
            {

            }
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
