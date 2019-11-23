using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Обрабатывает и хранит вершины графа
/// </summary>
[ExecuteInEditMode]
public class Graph : MonoBehaviour
{
    private const float INF = 999999f;
    private const int MAX_POINTS_COUNT = 10;

    private List<Transform> allObjects = new List<Transform>();
    private List<GraphPoint> graphPoints = new List<GraphPoint>();

    private List<GraphPoint> selectedPoints = new List<GraphPoint>();
    private LineRenderer lineRenderer;

    private int edgeCount = 0;

    public List<Edge> edges = new List<Edge>();

    private float[,] sizeMatrix = new float[MAX_POINTS_COUNT, MAX_POINTS_COUNT];

    private float[] d = new float[MAX_POINTS_COUNT];

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

            for (int i = 0; i <= GraphPoint.count; i++)
            {
                d[i] = INF;
            }

            //Начало пути
            int curPoint = pointA.Id;
            //Конец пути
            int endPoint = pointB.Id;

            Debug.Log("C=" + curPoint+ "  E=" + endPoint);

            for (int i = 0; i < GraphPoint.count; i++)
            {
                //Если между вершинами есть ребро, заносим его длину в массив
                if (sizeMatrix[curPoint, i] != 0 && sizeMatrix[curPoint, i] != INF)
                {
                    d[i] = sizeMatrix[curPoint, i];
                }
                else
                {
                    if (curPoint == i)
                        d[i] = 0;
                    else
                        d[i] = INF;
                }
                p[i] = 0;
            }

            float min = INF;
            int minC = curPoint;
            passed.Add(curPoint);
            path.Clear();

            Debug.Log("C=" + curPoint);

            string str = "";
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = curPoint;
            }

            while (passed.Count < GraphPoint.count)
            {
                str = "";
                for (int i = 1; i <= GraphPoint.count; i++)
                {
                    str += d[i].ToString() + "   ";
                }
                Debug.Log(str);
                min = INF;

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
                    p[minC] = curPoint;
                }
                curPoint = minC;
                passed.Add(curPoint);
                for (int i = 1; i <= GraphPoint.count; i++)
                {
                    if (!passed.Contains(i) && sizeMatrix[i, curPoint] != 0f)
                    {
                        float length = d[curPoint] + sizeMatrix[i, curPoint];
                        if (d[i] > length)
                        {
                            d[i] = length;
                            p[i] = curPoint;
                        }
                    }
                }
                Debug.Log("Вершина и путь до нее" + curPoint + " " + d[curPoint]);
            }
            str = "";
            for (int i = 1; i <= GraphPoint.count; i++)
            {
                str += d[i].ToString() + "   ";
            }
            Debug.Log(str);

            curPoint = pointA.Id;
            int k = pointB.Id;
            int h = 0;
            graphPoints.AddRange(FindObjectsOfType<GraphPoint>());
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
            while (k != curPoint && h < GraphPoint.count)
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
        lineRenderer.positionCount = 0;

        for (int i = 0; i < MAX_POINTS_COUNT; i++)
        {
            for (int j = 0; j < MAX_POINTS_COUNT; j++)
            {
                sizeMatrix[i, j] = INF;
                sizeMatrix[j, i] = INF;
            }
        }

    }

    private void OnEnable()
    {
        Init();
        DeleteAllEdges();
    }

    private void Update()
    {
        for (int i = 0; i < edges.Count; i++)
        {
            Debug.DrawLine(edges[i].pointA.Position, edges[i].pointB.Position);
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

            if (sizeMatrix[A, B] == INF)
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
            bool isConnected = sizeMatrix[pointA.Id, pointB.Id] != INF;

            Debug.Log(string.Format("Deleting edge {0}, {1}...", pointA.Id, pointB.Id));

            if (isConnected)
            {
                Disсonnect(pointA, pointB);
            }
        }
    }
    private void Disсonnect(GraphPoint pointA, GraphPoint pointB)
    {
        for (int i = 0; i < edges.Count; i++)
        {
            if (edges[i].points.Contains(pointA.Id) && edges[i].points.Contains(pointB.Id))
            {
                edges.Remove(edges[i]);
                sizeMatrix[pointA.Id, pointB.Id] = INF;
                sizeMatrix[pointB.Id, pointA.Id] = INF;
                Update();
                break;
            }
        }
    }

    private void Connect(GraphPoint pointA, GraphPoint pointB)
    {
        var edge = new Edge(pointA, pointB);

        sizeMatrix[pointA.Id, pointB.Id] = edge.Size;
        sizeMatrix[pointB.Id, pointA.Id] = edge.Size;

        edges.Add(edge);

        Debug.Log("Size= " + edge.Size);
        Update();
        PrintMatrix();
    }


    private void PrintMatrix()
    {
        string str = "   ";
        for (int i = 0; i < MAX_POINTS_COUNT; i++)
        {
            str += i + "   ";
        }
        Debug.LogWarning(str);
        str = "";

        for (int i = 0; i < MAX_POINTS_COUNT; i++)
        {
            for (int j = 0; j < MAX_POINTS_COUNT; j++)
            {
                if (sizeMatrix[i, j] == INF)
                {
                    str += "∞";
                }
                else
                {
                    str += sizeMatrix[i, j];
                }                
                str += "  ";
            }
            Debug.LogWarning(i.ToString() +"| " + str);
            str = "";
        }
    }

    /// <summary>
    /// Удалить все ребра
    /// </summary>
    public void DeleteAllEdges()
    {
        edges.Clear();
        for (int i = 0; i < MAX_POINTS_COUNT; i++)
        {
            for (int j = 0; i < MAX_POINTS_COUNT; i++)
            {
                sizeMatrix[i, j] = INF;
                sizeMatrix[j, i] = INF;
            }
        }
    }
}
