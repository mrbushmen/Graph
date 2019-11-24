using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GraphData 
{
    public List<Edge> edges;
    public float[,] sizeMatrix;
    public Edge.Point[] points;

    public GraphData(Graph graph)
    {
        edges = graph.edges;
        sizeMatrix = graph.sizeMatrix;
        points = graph.points;
    }
}