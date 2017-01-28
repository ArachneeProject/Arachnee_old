using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PhysicalEntryBuilder : MonoBehaviour
{
    public PhysicalVertex moviePrefab;
    public PhysicalVertex artistPrefab;

    private readonly Dictionary<Type, PhysicalVertex> _vertexAssociation = new Dictionary<Type, PhysicalVertex>();

    void Start()
    {
        _vertexAssociation.Add(typeof(Movie), moviePrefab);
        _vertexAssociation.Add(typeof(Artist), artistPrefab);
    }

    /// <summary>
    /// Instantiate the gameobjects corresponding to the given entries. Return the instantiated gameobjects.
    /// </summary>
    public IEnumerable<PhysicalVertex> BuildEntries(IEnumerable<Entry> entries, PhysicalVertex.EntryClickHandler entryClickedHandler)
    {
        var builtEntries = new List<PhysicalVertex>();
        foreach (var entry in entries.Where(e => !Entry.IsNullOrDefault(e)))
        {
            PhysicalVertex prefab;
            if (!_vertexAssociation.TryGetValue(entry.GetType(), out prefab))
            {
                Debug.LogError("Unable to find prefab for " + entry.GetType().Name);
                continue;
            }
            var physicalVertex = Instantiate(prefab);
            physicalVertex.Entry = entry;
            physicalVertex.EntryClickedEvent += entryClickedHandler;
            builtEntries.Add(physicalVertex);
        }
        
        StartCoroutine(PutPosters(builtEntries));
        return builtEntries;
    }

    private IEnumerator PutPosters(IEnumerable<PhysicalVertex> vertices)
    {
        foreach (var vertexWithNoPoster in vertices.Where(v => string.IsNullOrEmpty(v.Entry.PosterPath)))
        {
            vertexWithNoPoster.gameObject.GetComponent<Renderer>().material.mainTexture = TexturesRetriever.GetDefaultTexture();
            Debug.LogError(vertexWithNoPoster + " doesn't have any poster");
        }

        vertices = vertices.Where(v => !string.IsNullOrEmpty(v.Entry.PosterPath)).ToList();

        Dictionary<int, IEnumerable<PhysicalVertex>> priorities = new Dictionary<int, IEnumerable<PhysicalVertex>>();
        priorities.Add(0, vertices.Where(v => MiniMath.CanSee(Camera.main.transform, v.transform)) ); // visible vertices
        priorities.Add(1, vertices.Where(v => v.isActiveAndEnabled && !priorities[0].Contains(v))); // active vertices
        priorities.Add(2, vertices.Where(v => !priorities[0].Contains(v) && !priorities[1].Contains(v))); // all others
        
        for (int priority = 0; priority < 3; priority++)
        {
            var textureRetriever = new TexturesRetriever();
            yield return StartCoroutine(textureRetriever.RetrieveData(new HashSet<string>(priorities[priority]
                                                                        .Select(v => v.Entry.PosterPath))));
            foreach (var physicalVertex in priorities[priority])
            {
                object textureObj;
                if (!textureRetriever.RetrievedData.TryGetValue(physicalVertex.Entry.PosterPath, out textureObj))
                {
                    Debug.LogError("No texture found for " + physicalVertex + " (posterpath is \"" + physicalVertex.Entry.PosterPath + "\")");
                    continue;
                }

                var texture = textureObj as Texture2D;
                if (texture == null)
                {
                    Debug.LogError("Texture object was not a Texture2D for " + physicalVertex + " (posterpath is \"" + physicalVertex.Entry.PosterPath + "\")");
                    continue;
                }

                physicalVertex.gameObject.GetComponent<Renderer>().material.mainTexture = texture;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
