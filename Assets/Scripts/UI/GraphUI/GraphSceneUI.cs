using UnityEngine;
using UnityEngine.UI;

public class GraphSceneUI : MonoBehaviour
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
    
    public void SelectEntry(PhysicalVertex vertex)
    {
        this._selectedVertex = vertex;
        selectedEntryName.text = vertex.Entry.ToString();

        // active gui
        this.selectedEntryName.gameObject.SetActive(true);
        this.foldupButton.gameObject.SetActive(true);
        this.unfoldButton.gameObject.SetActive(true);
        this.maskButton.gameObject.SetActive(true);
    }
    
    
    /*
    /// <summary>
    /// Fold entries having only one active connection to the given entry
    /// </summary>
    private void foldSingleConnected(Entry e)
    {
        foreach (Edge c in e.ConnectedEdges)
        {
            if (this.activeEdges.Contains(c))
            {
                Entry oppositeEntry = c.OppositeEntry(e);
                if (numberOfActiveEdges(oppositeEntry) == 1)
                {
                    this._activeVertices.Remove(oppositeEntry);
                    oppositeEntry.gameObject.SetActive(false);
                    this.activeEdges.Remove(c);
                    c.gameObject.SetActive(false);
                }
            }
        }
    }
    
    /// <summary>
    /// Return the number of edges that are active for the provided entry
    /// </summary>
    private int numberOfActiveEdges(Entry e)
    {
        int counter = 0;
        foreach (Edge c in e.ConnectedEdges)
        {
            if (this.activeEdges.Contains(c))
            {
                counter++;
            }
        }
        return counter;
    }

    public void Activate(Vertex v)
    {
        v.gameObject.SetActive(true);
        this._activeVertices.Add(v);
        foreach (Edge c in v.ConnectedEdges)
        {
            Entry opposite = c.OppositeEntry(v);
            if (this._activeVertices.Contains(opposite))
            {
                this.activeEdges.Add(c);
                c.gameObject.SetActive(true);
            }
        }
    }

    public void MaskSelectedEntry()
    {
        if (selectedEntry == null)
        {
            return;
        }

        this.foldSingleConnected(selectedEntry);
        foreach (Edge c in selectedEntry.ConnectedEdges)
        {
            this.activeEdges.Remove(c);
            c.gameObject.SetActive(false);
        }
        this._activeVertices.Remove(selectedEntry);
        selectedEntry.gameObject.SetActive(false);

        selectedEntry = null;

        // mask gui
        this.selectedEntryName.gameObject.SetActive(false);
        this.foldupButton.gameObject.SetActive(false);
        this.unfoldButton.gameObject.SetActive(false);
        this.maskButton.gameObject.SetActive(false);
    }

    public void UnfoldSelectedEntry()
    {
        if (selectedEntry == null)
        {
            return;
        }

        this.selectedEntry.gameObject.SetActive(true);
        this._activeVertices.Add(selectedEntry);
        foreach (Edge c in selectedEntry.ConnectedEdges)
        {
            Entry opposite = c.OppositeEntry(selectedEntry);
            opposite.gameObject.SetActive(true);
            this._activeVertices.Add(opposite);

            this.activeEdges.Add(c);
            c.gameObject.SetActive(true);
        }
    }

    public void FoldUpSelectedEntry()
    {
        if (selectedEntry == null)
        {
            return;
        }

        this.foldSingleConnected(this.selectedEntry);
    }

    */
}
