using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class JoliBrowser : MonoBehaviour 
{
    private List<PrettyFolder> folders = new List<PrettyFolder>();
    private DatabaseDialoger dlg;

    private Dictionary<string, List<PrettyFolder>> arrangement = new Dictionary<string, List<PrettyFolder>>();
    private List<string> genres = new List<string>();

    public PrettyFolder prettyFolderPrefab;
    public Text sectionPrefab;
    public Toggle movieTog;
    public Toggle serieTog;
    public Toggle seenTog;
    private bool mvTogWasOn = false;

    void Start()
    {
        this.mvTogWasOn = movieTog.isOn;

        this.dlg = new DatabaseDialoger();
        this.getFolders();
        this.getAllGenres();
        this.SortByGenres();
    }


	
    // sort by genres
    public void SortByGenres()
    {
        this.arrangement.Clear();

        foreach (string g in this.genres)
        {
            List<PrettyFolder> l = new List<PrettyFolder>();
            foreach (PrettyFolder p in folders)
            {                
                if (p.Genres.Contains(g))
                {
                    l.Add(p);
                }
            }
            if (l.Count > 0)
            {
                this.arrangement.Add(g, l);
            }            
        }
        
        this.Show();
    }

    // sort by year, from most recent to oldest
    public void SortByYear()
    {
        this.arrangement.Clear();

        this.folders = this.folders.OrderBy(f => f.Year).ToList();

        for (int i = folders.Count - 1; i >= 0; i--)
        {
            string date = ((folders[i].Year / 10) * 10).ToString();

            if(!this.arrangement.ContainsKey(date))
            {
                this.arrangement.Add(date, new List<PrettyFolder>());
            }
            this.arrangement[date].Add(this.folders[i]);
        }
        
        this.Show();
    }

    // sort alphabeticaly by title
    public void SortByTitle()
    {
        this.arrangement.Clear();
        
        List<PrettyFolder> others = new List<PrettyFolder>();

        this.folders = this.folders.OrderBy(f => f.Title).ToList();
        foreach (PrettyFolder p in this.folders)
        {
            string c = p.Title.ToLower()[0].ToString();

            if (this.arrangement.ContainsKey(c))
            {
                this.arrangement[c].Add(p);
            }
            else if (System.Char.IsLetter(c, 0) || System.Char.IsNumber(c, 0))
            {
                this.arrangement.Add(c, new List<PrettyFolder>());
                this.arrangement[c].Add(p);
            }
            else
            {
                others.Add(p);
            }
        }

        if (others.Count > 0)
        {
            this.arrangement.Add("Others", others);
        }
    }


    private void getFolders()
    {
        Debug.LogError("not implemented yet");
        // this.dlg.getFolders();
        // StartCoroutine(fancy());
    }


    private void getAllGenres()
    {
        Debug.LogError("not implemented yet");

        HashSet<string> s = this.dlg.GetAllGenres();

        this.genres = new List<string>(s);
        this.genres.Sort();
    }

    // display the folders on screen
    public void Show()
    {
        // tricky MacTrickFace
        if (!movieTog.isOn && !serieTog.isOn)
        {
            if (mvTogWasOn)
            {
                serieTog.isOn = true;
            }
            else
            {
                movieTog.isOn = true;
            }
            return;
        }
        mvTogWasOn = movieTog.isOn;

        Debug.Log("sort");
        Debug.LogError("not implemented yet");

        // pas de fancy ici
    }
}
