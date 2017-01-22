using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class MFChange : MonoBehaviour 
{
    public Text message;
    public InputField pathFIeld;

    public void ChangePath()
    {
        var path = pathFIeld.text;
        if (path == string.Empty || !Directory.Exists(path))
        {
            message.text = "Sorry, this path is not valid.\nPlease enter a valid path to your main folder.";
            return;
        }

        PlayerPrefs.SetString(Constants.PP_MainFolder, path);
        int p = PlayerPrefs.GetInt(Constants.PP_PreviousLevel);
        PlayerPrefs.SetInt(Constants.PP_PreviousLevel, Application.loadedLevel);
        Application.LoadLevel(p);
    }
}
