using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DirectorConnectionBuilder : ArtistToMovieConnectionBuilder
{
    public DirectorConnectionBuilder(UnityEngine.LineRenderer lineRenderer, GraphBuilder graphBuilder)
    {
        this.lineRenderPrefab = lineRenderer;
        this.GraphBuilder = graphBuilder;
    }

    protected override Connection addConcreteComponent(UnityEngine.GameObject go)
    {
        return go.AddComponent<DirectorConnection>();
    }
}
