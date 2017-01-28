using Assets.Scripts.Graph.GraphData;
using Assets.Scripts.UI.GraphUI.PhysicalGraph;
using UnityEngine;

namespace Assets.Scripts.Scenes
{
    public class GraphSceneSetup : MonoBehaviour
    {
        public ForceDirectedGraphEngine graphEngine;
        public GraphSceneUI graphUI;
        public PhysicalGraphBuilder physicalGraphBuilder;
        
        void Start()
        {
            // get seed entry identifier
            var seedIdentifier = PlayerPrefs.GetString(Constants.PP_SeedVertexIdentifier);
            if (string.IsNullOrEmpty(seedIdentifier))
            {
                Debug.LogError("PlayerPref \"" + Constants.PP_SeedVertexIdentifier + "\" gave an empty string.");
                return;
            }

            // get corresponding entry
            var seedEntry = new DatabaseDialoger().GetEntry(seedIdentifier);
            if (Entry.IsNullOrDefault(seedEntry))
            {
                Debug.LogError("The seed vertex " + seedIdentifier + " gave the default entry");
                return;
            }

            // build graph
            var graph = new GraphBuilder().GetGraph(seedEntry, connectivity:3);
            var physicalGraph = this.physicalGraphBuilder.BuildPhysicalGraph(graph, graphUI.SelectEntry);
            this.graphEngine.SetUpFrom(physicalGraph);
        }
    }
}

