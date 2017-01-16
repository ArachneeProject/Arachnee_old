using UnityEngine;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.Linq;
using System.IO;
using UnityEngine.UI;

public class OverkillBrowser : MonoBehaviour 
{
    private DatabaseDialoger dlg;
    private OnlineRetriever onlret = new OnlineRetriever();

    public PrettyFolder prefab;
    public Canvas c;
    public Vector2 dimensions;
    public Vector2 offset;
    public Vector2 spacing;
    public float scrollspeed = 200;
    
    public Toggle seenTog;

    private List<PrettyFolder> folders;

	void Start () 
    {
        this.dlg = new DatabaseDialoger();

        this.folders = buildFolders();

        this.SortByTitle();

        if (!Directory.Exists(Application.dataPath + "/Posters"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Posters");
        }
        StartCoroutine(this.fancy());
	}

    private IEnumerator fancy()
    {
        foreach (PrettyFolder pf in this.folders)
        {
            string imgPath = Application.dataPath + "/Posters/" + pf.PosterPath.Replace("/", "");
            Texture2D tex;

            if (!File.Exists(imgPath))
            {
                yield return StartCoroutine(this.onlret.RetrievePoster(pf.PosterPath));
                if (onlret.Texture != null)
                {
                    var otex = onlret.Texture;
                    File.WriteAllBytes(imgPath, otex.EncodeToJPG());
                }                
            }

            if (File.Exists(imgPath))
            {
                tex = new Texture2D(2,2);
                tex.LoadImage(File.ReadAllBytes(imgPath));
            }
            else
            {
                 tex = Resources.Load(Constants.Res_DefaultImage) as Texture2D;
            }
            
            pf.image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);

            yield return new WaitForEndOfFrame();
        }
    }


    private void showList()
    {
        int maxrow = (int)Mathf.Max(1, ((Screen.width - offset.x) / (dimensions.x + spacing.x) - 1));

        int i = 0;
        int j = 0;
        foreach (PrettyFolder pf in folders)
        {
            if (!seenTog.isOn && pf.Seen)
            {
                pf.gameObject.SetActive(false);
                continue;
            }
            pf.gameObject.SetActive(true);

            pf.transform.position = new Vector3(
                offset.x + i * (dimensions.x + spacing.x),
                Screen.height - (offset.y + j * (dimensions.y + spacing.y)),
                0);

            i++;
            if (i > maxrow)
            {
                i = 0;
                j++;
            }
        }
    }

    public void SortByTitle()
    {
        this.folders = this.folders.OrderBy(f => f.Title).ToList();
        this.showList();
    }

    public void SortByYear()
    {
        this.folders = this.folders.OrderBy(f => f.Year).ToList();
        this.showList();
    }

    private List<PrettyFolder> buildFolders()
    {
        Debug.LogError("Not implemented");
        List<PrettyFolder> list = new List<PrettyFolder>();
        return list;

        /*
        // movies
        var set = this.dlg.GetDataSet("SELECT title, year, poster_path, seen, hash, quality, id "
        + "FROM Movies INNER JOIN FoldersM ON Movies.id = FoldersM.idMovie").Tables[0].Rows;

        foreach(DataRow row in set)
        {
            PrettyFolder pf = Instantiate(this.prefab) as PrettyFolder;
            pf.transform.SetParent(c.transform);
            
            pf.Title = (string) row[0];
            pf.Year = (int)(long)row[1];
            pf.PosterPath = (string)row[2];
            pf.Seen = ((long)row[3] != 0);
            pf.Hash = (string)row[4];
            pf.Quality = (string) row[5];
            pf.Id = (int)(long)row[6];
            pf.IsSerie = false;

            pf.info.text = pf.Title + "\n" + pf.Year.ToString() + "\n" + pf.Quality;
            list.Add(pf);
        }

        // series
        set = this.dlg.GetDataSet("SELECT title, startYear, poster_path, seen, hash, id "
        + "FROM Series INNER JOIN FoldersS ON Series.id = FoldersS.idSerie").Tables[0].Rows;

        foreach (DataRow row in set)
        {
            PrettyFolder pf = Instantiate(this.prefab) as PrettyFolder;
            pf.transform.SetParent(c.transform);

            pf.Title = (string)row[0];
            pf.Year = (int)(long)row[1];
            pf.PosterPath = (string)row[2];
            pf.Seen = ((long)row[3] != 0);
            pf.Hash = (string)row[4];
            pf.Id = (int)(long)row[5];
            pf.IsSerie = true;

            pf.info.text = pf.Title + "\n" + pf.Year.ToString() + "\n" + pf.Quality;
            list.Add(pf);
        }
        
        return list;
        */
    }


    void Update()
    {        
        var d = - Input.GetAxis("Mouse ScrollWheel");
        if (this.folders[0].transform.position.y < Screen.height - offset.y && d < 0)
        {            
            return;
        }
        if (this.folders[folders.Count - 1].transform.position.y > 2 * offset.y && d > 0)
        {
            return;
        }

        foreach (PrettyFolder fp in this.folders)
        {
            fp.transform.Translate(scrollspeed * d * Vector3.up);
        }
    }
}
