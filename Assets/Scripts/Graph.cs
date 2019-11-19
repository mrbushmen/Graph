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
    
    private int edgeCount = 0;

    public List<Edge> connectedPoints = new List<Edge>();

    private int[,] matrix=new int[50,50];

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
            int A = pointA.Id;
            int B = pointB.Id;

            if (matrix[A, B] != 1)
            {
                Connect(pointA, pointB);
                Debug.Log(string.Format("Conected {0}, {1}", pointA.Id, pointB.Id));
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
            bool isConnected = matrix[pointA.Id,pointB.Id]==1;

            Debug.Log(string.Format("Deleting edge {0}, {1}...", pointA.Id, pointB.Id));

            if (isConnected)
            {
                Disсonnect(pointA, pointB);
            }
        }
    }
    private void Disсonnect(GraphPoint pointA, GraphPoint pointB)
    {
        for (int i = 0; i < connectedPoints.Count; i++)
        {
            if (connectedPoints[i].points.Contains(pointA.Id) && connectedPoints[i].points.Contains(pointB.Id))
            {
                connectedPoints.Remove(connectedPoints[i]);
                matrix[pointA.Id, pointB.Id] = 0;
                matrix[pointB.Id, pointA.Id] = 0;
                edgeCount--;
                Update();
                break;
            }
        }
    }

    private void Connect(GraphPoint pointA, GraphPoint pointB)
    {
        matrix[pointA.Id, pointB.Id] = 1;
        matrix[pointB.Id, pointA.Id] = 1;
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
        for (int i = 0; i < 50; i++)
        {
            for (int j = 0; i < 50; i++)
            {
                matrix[i, j] = 0;
            }
        }
    }
}
