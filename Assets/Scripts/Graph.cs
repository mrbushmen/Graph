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

    private List<GraphPoint> graphPoints = new List<GraphPoint>();

    private List<GraphPoint> selectedPoints = new List<GraphPoint>();
    private LineRenderer lineRenderer;

    //Все ребра графа
    public List<Edge> edges = new List<Edge>();

    private float[,] sizeMatrix = new float[MAX_POINTS_COUNT, MAX_POINTS_COUNT];
    //Длины кратчайших путей от текущей точки до всех остальных
    private float[] d = new float[MAX_POINTS_COUNT];
    /// <summary>
    /// Список посещенных вершин
    /// </summary>
    private List<int> passed = new List<int>();
    //Координаты точек искомого пути
    private Vector3[] path = new Vector3[MAX_POINTS_COUNT];
    //Кратчайший путь. В i-ом элементе содержится номер вершины, из которой пришли в текущую
    private int[] foundedPoints = new int[MAX_POINTS_COUNT];

    public void FindPath()
    {
        if (CheckSelection())
        {
            GraphPoint pointA = selectedPoints[0];
            GraphPoint pointB = selectedPoints[1];

            passed.Clear();

            for (int i = 0; i <= GraphPoint.count; i++)
            {
                d[i] = INF;
            }

            //Начало пути
            int curPoint = pointA.Id;
            //Конец пути
            int endPoint = pointB.Id;

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
                foundedPoints[i] = 0;
            }

            float min = INF;
            int minC = curPoint;
            passed.Add(curPoint);

            for (int i = 0; i < foundedPoints.Length; i++)
            {
                foundedPoints[i] = curPoint;
            }

            while (passed.Count < GraphPoint.count)
            {
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
                    foundedPoints[minC] = curPoint;
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
                            foundedPoints[i] = curPoint;
                        }
                    }
                }
                Debug.Log("GraphEditor: Вершина и путь до нее" + curPoint + " " + d[curPoint]);
            }

            curPoint = pointA.Id;
            int wayPoint = pointB.Id;
            int h = 0;
            graphPoints.AddRange(FindObjectsOfType<GraphPoint>());
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
            bool isEnd = false;
            while (!isEnd && h < GraphPoint.count)
            {
                foreach (var graphPoint in graphPoints)
                {
                    if (graphPoint.Id == wayPoint)
                    {
                        path[wayPoint] = graphPoint.Position;
                    }
                }
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, path[wayPoint]);
                if (wayPoint == curPoint)
                {
                    isEnd = true;
                }

                wayPoint = foundedPoints[wayPoint];
                h++;
            }
        }
    }

    private void Init()
    {
        graphPoints.AddRange(FindObjectsOfType<GraphPoint>());

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
        if (CheckSelection())
        {
            GraphPoint pointA = selectedPoints[0];
            GraphPoint pointB = selectedPoints[1];
            int A = pointA.Id;
            int B = pointB.Id;

            if (sizeMatrix[A, B] == INF)
            {
                Connect(pointA, pointB);
                Debug.Log(string.Format("GraphEditor: Conected {0}, {1}", pointA.Id, pointB.Id));
            }
        }
    }

    /// <summary>
    /// Удалить ребро между выбранными вершинами
    /// </summary>
    public void DeleteEdge()
    {
        if (CheckSelection())
        {
            GraphPoint pointA = selectedPoints[0];
            GraphPoint pointB = selectedPoints[1];
            bool isConnected = sizeMatrix[pointA.Id, pointB.Id] != INF;

            Debug.Log(string.Format("GraphEditor: Deleting edge {0}, {1}...", pointA.Id, pointB.Id));

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

        Update();
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

    public void ClearPath()
    {
        lineRenderer.positionCount = 0;
    }

    private bool CheckSelection()
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

        return selectedPoints.Count == 2;
    }
}
