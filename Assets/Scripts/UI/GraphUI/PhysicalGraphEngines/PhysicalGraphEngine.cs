using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PhysicalGraphEngine : MonoBehaviour
{
    public float coulombRepulsion = 150;
    public float hookeAttraction = 1;
    public float coulombRepulsionSquaredMaxDistance = 10000;
    public int maxActiveVertices = 300;

    protected readonly List<PhysicalVertex> ActiveVertices = new List<PhysicalVertex>();
    protected readonly List<PhysicalEdge> ActiveEdges = new List<PhysicalEdge>();

    public abstract void SetUpFrom(PhysicalUndirectedGraph graph);

    void FixedUpdate()
    {
        FixedUpdateImpl();
    }

    protected abstract void FixedUpdateImpl();
}
