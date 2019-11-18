using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Обрабатывает и хранит вершины графа
/// </summary>
[ExecuteInEditMode]
public class Graph : MonoBehaviour
{
    private List<Transform> allObjects = new List<Transform>();
    private List<GraphPoint> points = new List<GraphPoint>();

    private List<GraphPoint> selectedPoints = new List<GraphPoint>();
    private LineRenderer lineRenderer;

    private Dictionary<GraphPoint, List<GraphPoint>> graph = new Dictionary<GraphPoint, List<GraphPoint>>();
    private int edgeCount = 0;

    private List<Edge> connectedPoints = new List<Edge>();

    private void Init()
    {
        points.AddRange(FindObjectsOfType<GraphPoint>());

        foreach (var item in points)
        {
            allObjects.Add(item.GetComponent<Transform>());
        }

        lineRenderer = GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {
        DeleteAllEdges();
    }

    private void Update()
    {
        for (int i = 0; i < connectedPoints.Count; i++)
        {
            Debug.DrawLine(connectedPoints[i].pointA.Position, connectedPoints[i].pointB.Position);
        }
    }

    /// <summary>
    /// Соединить две выбранные вершины
    /// </summary>
    public void DrawEdge()
    {
        selectedPoints.Clear();

        foreach (GameObject item in Selection.objects)
        {
            selectedPoints.Add(item.GetComponent<GraphPoint>());
        }

        if (selectedPoints.Count != 2)
        {
            Debug.LogError("Graph editor: Выбери две вершины!");
        }
        else
        {
            GraphPoint pointA = selectedPoints[0];
            GraphPoint pointB = selectedPoints[1];
            bool a = graph.ContainsKey(pointA);
            bool b = graph.ContainsKey(pointB);

            Debug.Log(a + " " + b);

            if (a && b)
            {
                if (graph[pointA].Contains(pointB))
                {
                    Debug.Log("Return");
                    return;
                }
                else
                {
                    Debug.Log("Conected1");
                    Connect(pointA, pointB);
                }
            }
            else
            {
                if (!a)
                {
                    graph.Add(pointA, new List<GraphPoint>());
                    Debug.Log("AddA");
                }
                if (!b)
                {
                    graph.Add(pointB, new List<GraphPoint>());
                    Debug.Log("AddB");
                }
                Connect(pointA, pointB);
                Debug.Log("Conected2");
            }
        }
    }

    /// <summary>
    /// Удалить ребро между выбранными вершинами
    /// </summary>
    public void DeleteEdge()
    {
        selectedPoints.Clear();

        foreach (GameObject item in Selection.objects)
        {
            selectedPoints.Add(item.GetComponent<GraphPoint>());
        }

        if (selectedPoints.Count != 2)
        {
            Debug.LogError("Graph editor: Выбери две вершины!");
        }
        else
        {
            GraphPoint pointA = selectedPoints[0];
            GraphPoint pointB = selectedPoints[1];
            bool a = graph.ContainsKey(pointA);
            bool b = graph.ContainsKey(pointB);

            Debug.Log(a + " " + b);

            if (a && b)
            {
                Disсonnect(pointA, pointB);
            }
        }
    }

    private void Disсonnect(GraphPoint pointA, GraphPoint pointB)
    {
        Edge r = new Edge(pointA, pointB);

        foreach (var item in connectedPoints)
        {
            if (item == r)
            {
                connectedPoints.Remove(item);
                graph[pointA].Remove(pointB);
                graph[pointB].Remove(pointA);
                edgeCount--;
                Update();
                break;
            }
        }

        
    }

    private void Connect(GraphPoint pointA, GraphPoint pointB)
    {
        graph[pointA].Add(pointB);
        graph[pointB].Add(pointA);
        edgeCount++;
        connectedPoints.Add(new Edge(pointA, pointB));
        Update();
    }

    /// <summary>
    /// Удалить все ребра
    /// </summary>
    public void DeleteAllEdges()
    {
        edgeCount = 0;
        connectedPoints.Clear();
        graph.Clear();
    }
}
