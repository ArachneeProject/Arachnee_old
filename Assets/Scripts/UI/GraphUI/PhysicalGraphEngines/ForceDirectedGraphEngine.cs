using UnityEngine;

namespace Assets.Scripts.UI.GraphUI.PhysicalGraph
{
    public class ForceDirectedGraphEngine : PhysicalGraphEngine
    {
        public override void SetUpFrom(PhysicalUndirectedGraph graph)
        {
            foreach (var physicalVertex in graph.PhysicalVertices)
            {
                if (ActiveVertices.Count < maxActiveVertices)
                {
                    ActiveVertices.Add(physicalVertex);
                }
            }

            foreach (var physicalEdge in graph.PhysicalEdges)
            {
                if (ActiveVertices.Contains(physicalEdge.Left)
                && ActiveVertices.Contains(physicalEdge.Right))
                {
                    this.ActiveEdges.Add(physicalEdge);
                }
            }
        }

        protected override void FixedUpdateImpl()
        {
            foreach (var vertex in this.ActiveVertices)
            {
                // repulsion
                foreach (var otherVertex in this.ActiveVertices)
                {
                    if (vertex != otherVertex)
                    {
                        float squaredDistance = MiniMath.getSquaredDistance(vertex.transform.position, otherVertex.transform.position);
                        if (squaredDistance < coulombRepulsionSquaredMaxDistance)
                        {
                            Vector3 repulsion = this.coulombRepulsion * (vertex.transform.position - otherVertex.transform.position) * (1F / squaredDistance);
                            if (vertex.RigidBody != null)
                            {
                                vertex.RigidBody.AddForce(repulsion);
                            }
                        }
                    }
                }

                // attraction to center
                if (vertex.RigidBody != null)
                {
                    vertex.RigidBody.AddForce(-vertex.transform.position);
                }
            }

            // attraction
            foreach (var edge in this.ActiveEdges)
            {
                Vector3 attraction = this.hookeAttraction * (edge.Left.transform.position - edge.Right.transform.position);
                if (edge.Left.RigidBody != null && edge.Right.RigidBody != null)
                {
                    edge.Left.RigidBody.AddForce(-attraction);
                    edge.Right.RigidBody.AddForce(attraction);
                }
            }
        }
    }
}
