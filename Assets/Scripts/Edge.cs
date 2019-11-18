using UnityEngine;

/// <summary>
/// Ребро графа
/// </summary>
[SelectionBase]
public class Edge : MonoBehaviour
{
    public GraphPoint pointA;
    public GraphPoint pointB;

    public Edge(GraphPoint a, GraphPoint b)
    {
        pointA = a;
        pointB = b;
    }
}