using UnityEngine;

public class PhysicalEdge : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    public PhysicalVertex Left { get; set; }
    public PhysicalVertex Right { get; set; }

    public ConnectionType ConnectionType { get; set; }

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Left.RigidBody != null && Right.RigidBody != null)
        {
            _lineRenderer.SetPosition(0, Left.transform.position);
            _lineRenderer.SetPosition(1, Right.transform.position);
        }
    }
}
