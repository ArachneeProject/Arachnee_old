using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Graph.GraphData
{
    public class GraphBuilder
    {
        private readonly DatabaseDialoger _database = new DatabaseDialoger();

        public EntryGraph GetGraph(Entry seedEntry, int connectivity = 1, int allowedVertices = 1000)
        {
            if (Entry.IsNullOrDefault(seedEntry))
            {
                Debug.LogError("Graph not built because seed entry was the default entry");
                return new EntryGraph();
            }

            EntryGraph entryGraph = new EntryGraph();
            entryGraph.AddVertex(seedEntry);
            allowedVertices--;

            HashSet<Entry> entriesToConnect = new HashSet<Entry> { seedEntry };
            while (entriesToConnect.Any() && allowedVertices > 0 && connectivity > 0)
            {
                Debug.Log("Building graph... (" + entryGraph.Vertices.Count() + " vertices done)");

                HashSet<Entry> nextStep = new HashSet<Entry>();

                foreach (var entryToConnect in entriesToConnect)
                {
                    var connections = this._database.GetConnectedEntries(entryToConnect);

                    foreach (var connectedEntry in connections.Keys)
                    {
                        if (allowedVertices > 0)
                        {
                            if (entryGraph.AddVertex(connectedEntry))
                            {
                                allowedVertices--;
                                nextStep.Add(connectedEntry);
                            }
                        }
                        foreach (var connectionType in connections[connectedEntry])
                        {
                            entryGraph.AddConnection(entryToConnect, connectedEntry, connectionType);
                        }
                    }
                }

                entriesToConnect.Clear();
                foreach (var entry in nextStep)
                {
                    entriesToConnect.Add(entry);
                }
                connectivity--;
            }

            Debug.Log("Graph built, Vertices: " + entryGraph.Vertices.Count() + 
                      ", Edges: " + entryGraph.EdgesCount() + 
                      " (parameters were " + allowedVertices + " allowed vertices)");

            return entryGraph;
        }
    }
}
