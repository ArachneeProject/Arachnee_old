using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour 
{
    public Text message;
    public Button browse;
    public Button settings;

	void Start () 
    {
        string dbPath = Application.dataPath + "/Database/couch.db";
        if (!File.Exists(dbPath))
        {
            message.text = "Database not found at " + dbPath + ")";
            message.fontStyle = FontStyle.Bold;
            message.color = Color.red;
            browse.interactable = false;
            settings.interactable = false;
        }
        else
        {
            this.message.text = getRandomPunchline();
        }
	}

    private string getRandomPunchline()
    {
        string punchPath = Application.dataPath + "/Punchlines/punch.txt";
        if (File.Exists(punchPath))
        {
            var punchlines = File.ReadAllLines(punchPath);
            int rand = Random.Range(0, punchlines.Length);
            return punchlines[rand].Replace('-', '\n');
        }
        return "welcome!";
    }
}
