using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class FolderPathSet : MonoBehaviour 
{
    public InputField input;
    public Text message;

	
	public void CheckInput()
    {
        if (this.input.text == "")
        {
            this.message.text = "Please provide a valid path";
            return;
        }
        if (Directory.Exists(this.input.text))
        {
            PlayerPrefs.SetString(Constants.PP_FOLDER_PATH, this.input.text);
            Debug.Log(PlayerPrefs.GetString(Constants.PP_FOLDER_PATH));
            Application.LoadLevel(Constants.SC_BROWSER);
            this.message.text = "Ok!";
        }
        else
        {
            this.message.text = "The provided folder path doesn't exist";
            return;
        }
    }
}
