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

    private float[] d = new float[20];

    //список посещенных вершин
    private List<int> passed = new List<int>();
    //список оставшихся вершин
    private List<int> path = new List<int>();

    private int[] p = new int[20];

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

            for (int i = 0; i < GraphPoint.count; i++)
            {
                d[i] = 99999f;
            }

            //Начало пути
            int c = pointA.Id;
            //Конец пути
            int e = pointB.Id;

            for (int i = 0; i < GraphPoint.count; i++)
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
                        d[i] = 99999f;
                }
                p[i] = 0;
            }

            float min = 99999f;
            int minC = c;
            passed.Add(c);
            path.Clear();

            Debug.LogError("C=" + c);

            string str = "";
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = c;
            }

            while (passed.Count < GraphPoint.count)
            {
                str = "";
                for (int i = 1; i <= GraphPoint.count; i++)
                {
                    str += d[i].ToString() + "   ";
                }
                Debug.Log(str);
                min = 99999f;
                for (int i = 1; i <= GraphPoint.count; i++)
                {
                    if (!passed.Contains(i))
                    {
                        if (d[i] < min)
                        {
                            min = d[i];
                            minC = i;
                        }
                    }
                }
                if (passed.Count == 1)
                {
                    p[minC] = c;
                }
                c = minC;
                passed.Add(c);
                for (int i = 1; i <= GraphPoint.count; i++)
                {
                    if (!passed.Contains(i) && sizeMatrix[i, c] != 0f)
                    {
                        float length = d[c] + sizeMatrix[i, c];
                        if (d[i] > length)
                        {
                            d[i] = length;
                            p[i] = c;
                        }
                    }
                }
                Debug.Log("Вершина и путь до нее" + c + " " + d[c]);
            }
            str = "";
            for (int i = 1; i <= GraphPoint.count; i++)
            {
                str += d[i].ToString() + "   ";
            }
            Debug.Log(str);

            c = pointA.Id;
            int k = pointB.Id;
            int h = 0;
            graphPoints.AddRange(FindObjectsOfType<GraphPoint>());
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
            while (k != c && h < GraphPoint.count)
            {
                Debug.LogError("Path: " + k);
                foreach (var item in graphPoints)
                {
                    if (item.Id == k)
                    {
                        lineRenderer.positionCount++;
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, item.Position);
                    }
                }

                k = p[k];


                h++;
            }
            Debug.LogError("Path: " + k);
            foreach (var item in graphPoints)
            {
                if (item.Id == k)
                {
                    lineRenderer.positionCount++;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, item.Position);
                }
            }
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

        for (int i = 0; i < 50; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                sizeMatrix[i, j] = 99999f;
            }
        }

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
