using System.Collections.Generic;

/// <summary>
/// Ребро графа
/// </summary>
[System.Serializable]
public class Edge
{
    public List<int> points = new List<int>();

    public GraphPoint pointA;
    public GraphPoint pointB;

    public Edge(GraphPoint a, GraphPoint b)
    {
        pointA = a;
        pointB = b;
        points.Add(a.Id);
        points.Add(b.Id);
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