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

    public static bool operator ==(Edge one, Edge two)
    {
        bool result = (one.pointA == two.pointA) && (one.pointB == two.pointB);
        result |= (one.pointA == two.pointB) && (one.pointB == two.pointA);
        return result;
    }

    public static bool operator !=(Edge one, Edge two)
    {
        bool result = (one.pointA == two.pointA) && (one.pointB == two.pointB);
        result |= (one.pointA == two.pointB) && (one.pointB == two.pointA);
        return !result;
    }
}