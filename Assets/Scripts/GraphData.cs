using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphData : ScriptableObject
{
    public const int MAX_POINTS_COUNT = 10;
    public float[,] sizeMatrix = new float[MAX_POINTS_COUNT + 1, MAX_POINTS_COUNT + 1];

    public List<GraphPoint> graphPoints = new List<GraphPoint>();
    public List<Edge> edges = new List<Edge>();
}
