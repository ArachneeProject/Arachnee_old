using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GraphUI : MonoBehaviour
{
    public Text selectedEntryName;
    public Button foldupButton;
    public Button maskButton;
    public Button unfoldButton;

    private PhysicalVertex _selectedVertex;
    
    void Start()
    {
        /*
        this.selectedEntryName.gameObject.SetActive(false);
        this.foldupButton.gameObject.SetActive(false);
        this.unfoldButton.gameObject.SetActive(false);
        this.maskButton.gameObject.SetActive(false);
        */
    }
    
    void SelectEntry(PhysicalVertex vertex)
    {
        this._selectedVertex = vertex;
        selectedEntryName.text = vertex.Entry.ToString();

        // active gui
        this.selectedEntryName.gameObject.SetActive(true);
        this.foldupButton.gameObject.SetActive(true);
        this.unfoldButton.gameObject.SetActive(true);
        this.maskButton.gameObject.SetActive(true);
    }

    public void AddPhysicalVertices(IEnumerable<PhysicalVertex> physicalVertices)
    {
        // init event
        foreach (PhysicalVertex vertex in physicalVertices)
        {
            vertex.EntryClickedEvent += SelectEntry;
        }

        // get posters
        StartCoroutine(PutPosters(physicalVertices));
    }

    public void AddPhysicalEdges(IEnumerable<PhysicalEdge> physicalEdges)
    {
        
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
