using UnityEngine;
using System.Collections;

public class Navigation : MonoBehaviour 
{
    public const int MainMenu = 0;
    public const int Settings = 1;
    public const int IgnoredFolders = 2;
    public const int MFChange = 3;
    public const int ClearDB = 4;
    public const int MFCheck = 5;
    public const int AddNewMovie = 6;
    public const int AddNewSerie = 7;
    public const int DatabaseUpdate = 8;
    public const int Browser = 9;
    public const int Details = 10;
    public const int Graph = 11;
    public const int FreeSearch = 12;

    public void GoToMainMenu()
    {
        PlayerPrefs.SetInt(Constants.PP_PreviousLevel, Application.loadedLevel);
        Application.LoadLevel(MainMenu);
    }

    public void GoToSettings()
    {
        PlayerPrefs.SetInt(Constants.PP_PreviousLevel, Application.loadedLevel);
        Application.LoadLevel(Settings);
    }

    public void GoToIgnoredFolder()
    {
        PlayerPrefs.SetInt(Constants.PP_PreviousLevel, Application.loadedLevel);
        Application.LoadLevel(IgnoredFolders);
    }

    public void GoToMFChange()
    {
        PlayerPrefs.SetInt(Constants.PP_PreviousLevel, Application.loadedLevel);
        Application.LoadLevel(MFChange);
    }

    public void GoToClearDB()
    {
        PlayerPrefs.SetInt(Constants.PP_PreviousLevel, Application.loadedLevel);
        Application.LoadLevel(ClearDB);
    }

    public void GoToMFCheck()
    {
        PlayerPrefs.SetInt(Constants.PP_PreviousLevel, Application.loadedLevel);
        Application.LoadLevel(MFCheck);
    }
    
    public void GoToAddNewMovie()
    {
        PlayerPrefs.SetInt(Constants.PP_PreviousLevel, Application.loadedLevel);
        Application.LoadLevel(AddNewMovie);
    }
    
    public void GoToAddNewSerie()
    {
        PlayerPrefs.SetInt(Constants.PP_PreviousLevel, Application.loadedLevel);
        Application.LoadLevel(AddNewSerie);
    }

    public void GoToDatabaseUpdate()
    {
        PlayerPrefs.SetInt(Constants.PP_PreviousLevel, Application.loadedLevel);
        Application.LoadLevel(DatabaseUpdate);
    }

    public void GoToBrowser()
    {
        PlayerPrefs.SetInt(Constants.PP_PreviousLevel, Application.loadedLevel);
        Application.LoadLevel(Browser);
    }

    public void GoToDetails()
    {
        PlayerPrefs.SetInt(Constants.PP_PreviousLevel, Application.loadedLevel);
        Application.LoadLevel(Details);
    }

    public void GoToGraph()
    {
        PlayerPrefs.SetInt(Constants.PP_PreviousLevel, Application.loadedLevel);
        Application.LoadLevel(Graph);
    }

    public void GoToFreeSearch()
    {
        PlayerPrefs.SetInt(Constants.PP_PreviousLevel, Application.loadedLevel);
        Application.LoadLevel(FreeSearch);
    }

    public void GoToPreviousLevel()
    {
        int p = PlayerPrefs.GetInt(Constants.PP_PreviousLevel);
        PlayerPrefs.SetInt(Constants.PP_PreviousLevel, Application.loadedLevel);
        Application.LoadLevel(p);
    }
}
