using UnityEngine;
using System.Collections;
using System.IO;

public class MFCheck : MonoBehaviour 
{
    private DatabaseDialoger dlg;

	void Start () 
    {
        this.dlg = new DatabaseDialoger();

        var mainFolder = PlayerPrefs.GetString(Constants.PP_MainFolder);        
        if(mainFolder == string.Empty || !Directory.Exists(mainFolder))
        {
            Application.LoadLevel(Navigation.MFChange);
            return;
        }
        
        DirectoryInfo mf = new DirectoryInfo(mainFolder);
        
        foreach (DirectoryInfo d in mf.GetDirectories())
        {
            var hash = MiniMath.GetHash(d.Name);

            if(!this.dlg.FolderIsRegistered(hash))
            {
                PlayerPrefs.SetString(Constants.PP_NewFolderName, d.Name);
                PlayerPrefs.SetInt(Constants.PP_PreviousLevel, Application.loadedLevel);
                Application.LoadLevel(Navigation.AddNewMovie);
                return;
            }

        }

        PlayerPrefs.SetInt(Constants.PP_PreviousLevel, Application.loadedLevel);
        Application.LoadLevel(Navigation.Browser);
	
	}
	
}
