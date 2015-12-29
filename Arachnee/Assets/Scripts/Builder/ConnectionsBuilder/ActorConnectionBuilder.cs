using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ActorConnectionBuilder : ConnectionGameObjectBuilder
{
    public ActorConnectionBuilder(UnityEngine.LineRenderer lineRenderer, GraphBuilder graphBuilder)
    {
        this.lineRenderPrefab = lineRenderer;
        this.GraphBuilder = graphBuilder;
    }

    /// <summary>
    /// Add the ActorConnection component
    /// </summary>
    /// <param name="go"></param>
    protected override Connection addConcreteComponent(UnityEngine.GameObject go)
    {
        return go.AddComponent<ActorConnection>();
    }
}
