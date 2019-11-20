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
    private List<GraphPoint> graphPoints = new List<GraphPoint>();

    private List<GraphPoint> selectedPoints = new List<GraphPoint>();
    private LineRenderer lineRenderer;

    private int edgeCount = 0;

    public List<Edge> connectedPoints = new List<Edge>();

    private int[,] matrix = new int[50, 50];
    private float[,] sizeMatrix = new float[50, 50];

    private float[] d = new float[50];

    //список посещенных вершин
    private List<int> passed = new List<int>();
    //список оставшихся вершин
    private List<int> points = new List<int>();

    public void FindPath()
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
            //TODO: поиск кратчайшего пути
            passed.Clear();

            for (int i = 0; i < 50; i++)
            {
                d[i] = float.MaxValue;
            }

            //Начало пути
            int c = pointA.Id;
            //Конец пути
            int e = pointB.Id;

            for (int i = 0; i < 50; i++)
            {
                //Если между вершинами есть ребро, заносим его длину в массив
                if (matrix[c, i] == 1)
                {
                    d[i] = sizeMatrix[c, i];
                }
                else
                {
                    if (c == i)
                        d[i] = 0;
                    else
                        d[i] = float.MaxValue;
                }
            }

            float min = float.MaxValue;
            int minC = c;
            while (passed.Count < GraphPoint.count && c!=e)
            {
                for (int i = 0; i < GraphPoint.count; i++)
                {
                    //если расстояние до вершины меньше, чем минимальное + расстояние до текущей
                    if (d[i] < min + d[c] && d[i] != 0 && !passed.Contains(i))
                    {
                        min = d[i];
                        minC = i;
                    }
                }
                passed.Add(c);
                c = minC;
                Debug.LogError(c);

                for (int i = 0; i < 50; i++)
                {
                    //Если между вершинами есть ребро, заносим его длину в массив
                    if (matrix[c, i] == 1 && !passed.Contains(i))
                    {
                        d[i] = sizeMatrix[c, i]+min;
                    }
                    else
                    {
                        if (c == i)
                            d[i] = min;
                        else
                            d[i] = float.MaxValue;
                    }
                }
            }


            //var c = pointA.Id;
            //passed.Add(c);
            //float min = float.MaxValue;
            //int minC = c;

            //for (int i = 1; i <= GraphPoint.count; i++)
            //{
            //    if (! passed.Contains(c))
            //    {
            //        if ( sizeMatrix[i, c]<min)
            //        {
            //            min = sizeMatrix[i, c];
            //            minC = c;
            //        }
            //    }
            //}
            //passed.Add(c);
        }
    }


    private void Init()
    {
        graphPoints.AddRange(FindObjectsOfType<GraphPoint>());
        foreach (var item in graphPoints)
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
            bool isConnected = matrix[pointA.Id, pointB.Id] == 1;

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

        var edge = new Edge(pointA, pointB);
        connectedPoints.Add(edge);

        sizeMatrix[pointA.Id, pointB.Id] = edge.Size;
        sizeMatrix[pointB.Id, pointA.Id] = edge.Size;

        Debug.Log("Size= " + edge.Size);

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
