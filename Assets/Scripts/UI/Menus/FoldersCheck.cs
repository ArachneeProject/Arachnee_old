using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;

public class FoldersCheck : MonoBehaviour 
{


    private UserFoldersDialoger _dlg;

	void Start () 
    {
        this._dlg = new UserFoldersDialoger();

        var mainFolder = PlayerPrefs.GetString(Constants.PP_MainFolder);        
        if(mainFolder == string.Empty || !Directory.Exists(mainFolder))
        {
            Application.LoadLevel(Navigation.MFChange);
            return;
        }

	    var unregisteredDirectories = Directory.GetDirectories(mainFolder).Where(folderPath => !this._dlg.FolderIsRegistered(folderPath)).ToList();
        if (unregisteredDirectories.Any())
	    {
            StartCoroutine(AddNewFolder(unregisteredDirectories.First()));
	        return;
	    }

        PlayerPrefs.SetInt(Constants.PP_PreviousLevel, Application.loadedLevel);
        Application.LoadLevel(Navigation.Browser);
	}

    private IEnumerator AddNewFolder(string folderPath)
    {
        yield break;
    }
}
