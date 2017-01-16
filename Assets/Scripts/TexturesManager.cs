using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class TexturesManager
{
    public Dictionary<string, Texture2D> RetrievedTextures { get; private set; }
    private MonoBehaviour _caller;

    public TexturesManager(MonoBehaviour caller)
    {
        RetrievedTextures = new Dictionary<string, Texture2D>();
        _caller = caller;
    }
    
    public IEnumerator RetrieveTexture(HashSet<string> posterPathsSet)
    {
        RetrievedTextures.Clear();

        var posterPaths = posterPathsSet.Where(p => !string.IsNullOrEmpty(p)).ToList();

        // load texture from disk if possible
        foreach (var path in posterPaths.Where(TextureIsOnDisk))
        {
            Texture2D texture = new Texture2D(2,2);
            texture.LoadImage(File.ReadAllBytes(GetFilePathFromPoster(path)));
            RetrievedTextures.Add(path, texture);
        }

        // load texture from internet
        foreach (var path in posterPaths.Where(p => !RetrievedTextures.Keys.Contains(p)))
        {
            var onlineRetriever = new OnlineRetriever();
            yield return _caller.StartCoroutine(onlineRetriever.RetrievePoster(path));
            if (onlineRetriever.Succeeded)
            {
                // add to disk if possible
                File.WriteAllBytes(GetFilePathFromPoster(path), onlineRetriever.Texture.EncodeToJPG());
                RetrievedTextures.Add(path, onlineRetriever.Texture);
            }
            else
            {
                Debug.LogError("Error when retrieving poster " + path + ". Details: " + onlineRetriever.Error);
            }
        }

        foreach (var path in posterPaths.Where(p => !RetrievedTextures.Keys.Contains(p)))
        {
            RetrievedTextures.Add(path, GetDefaultTexture());
        }
    }

    public static Texture2D GetDefaultTexture()
    {
        return Resources.Load(Constants.Res_DefaultImage) as Texture2D;
    }

    private bool TextureIsOnDisk(string posterPath)
    {
        if (string.IsNullOrEmpty(posterPath))
        {
            return false;
        }
        return File.Exists(GetFilePathFromPoster(posterPath));
    }

    private string GetFilePathFromPoster(string posterPath)
    {
        string filename;
        if (string.IsNullOrEmpty(posterPath))
        {
            filename = Path.GetRandomFileName();
        }
        else
        {
            filename = Path.GetInvalidFileNameChars().Aggregate(posterPath,
                (current, invalidFileNameChar) => current.Replace(invalidFileNameChar, '_'));
        }
        return Path.Combine(Path.Combine(Application.dataPath, "Posters"), filename).Replace('\\', '/');
    }
}

