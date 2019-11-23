/*
 Неисправленные баги и проблемы:
 - ! При переносе точки, построенный путь не обновляется.
 - ! При попытке построить путь между вершинами, которые не связаны, путь будет построен "по воздуху".
 - Нет решения с размерностью массивов для хранения информации о графе.
 - Нет решения с максимальным количеством вершин.
 - ! Не реализовано сохранение информации о графе.
 - Не реализовано сохранение настроек редактора.
 - Не реализован вывод сообщений в окно редактора.
 */


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
    /// <summary>
    /// Максимальное количество вершин графа
    /// </summary>
    private const int MAX_POINTS_COUNT = 10;
    
    private List<GraphPoint> graphPoints = new List<GraphPoint>();
    private List<GraphPoint> selectedPoints = new List<GraphPoint>();

    /// <summary>
    /// Все ребра графа
    /// </summary>
    private List<Edge> edges = new List<Edge>();

    private float[,] sizeMatrix = new float[MAX_POINTS_COUNT+1, MAX_POINTS_COUNT + 1];

    //Длины кратчайших путей от текущей точки до всех остальных
    private float[] d = new float[MAX_POINTS_COUNT + 1];
    /// <summary>
    /// Список посещенных вершин
    /// </summary>
    private List<int> passed = new List<int>();
    //Координаты точек искомого пути
    private Vector3[] path = new Vector3[MAX_POINTS_COUNT + 1];
    //Кратчайший путь. В i-ом элементе содержится номер вершины, из которой пришли в вершину с номером i
    private int[] foundedPoints = new int[MAX_POINTS_COUNT + 1];

    private LineRenderer lineRenderer;

    [SerializeField]
    private GraphData graphData;

    public void FindPath()
    {
        if (CheckSelection())
        {
            GraphPoint pointA = selectedPoints[0];
            GraphPoint pointB = selectedPoints[1];

            passed.Clear();

            for (int i = 0; i <= MAX_POINTS_COUNT; i++)
            {
                d[i] = INF;
            }

            //Начало пути
            int curPoint = pointA.Id;
            //Конец пути
            int endPoint = pointB.Id;

            for (int i = 0; i <= MAX_POINTS_COUNT; i++)
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

            while (passed.Count < MAX_POINTS_COUNT)
            {
                min = INF;
                for (int i = 1; i <= MAX_POINTS_COUNT; i++)
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
                for (int i = 1; i <= MAX_POINTS_COUNT; i++)
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
            }

            curPoint = pointA.Id;
            int wayPoint = pointB.Id;
            int h = 0;
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
            lineRenderer.startColor = GraphEditor.PathColor;
            lineRenderer.endColor = GraphEditor.PathColor;
            bool isEnd = false;
            while (!isEnd && h < MAX_POINTS_COUNT)
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

    private void OnCreatePoint(GraphPoint point)
    {
        if (graphPoints.Contains(point)) return;
        graphPoints.Add(point);
        GDebug.Log("Точка " + point.Id + " создана");
        if (GraphPoint.Count == MAX_POINTS_COUNT - 1)
        {
            GDebug.LogWarning("количество точек на сцене достигло максимума!");
        }
        else
        {
            if (GraphPoint.Count == MAX_POINTS_COUNT)
            {
                GDebug.LogError("количество точек на сцене достигло максимума! Лишняя точка удалена.");
                DestroyImmediate(point.gameObject);
            }
        }
    }

    private void OnDestroyPoint(GraphPoint point)
    {
        graphPoints.Remove(point);

        for (int i = 0; i < MAX_POINTS_COUNT; i++)
        {
            sizeMatrix[i, point.Id] = INF;
            sizeMatrix[point.Id, i] = INF;
        }
        FindAndDeleteEdge(point);
        GDebug.Log("Точка "+ point.Id+" удалена");
    }

    private void FindAndDeleteEdge(GraphPoint point)
    {
        foreach (var edge in edges)
        {
            if (edge.points.Contains(point.Id))
            {
                edges.Remove(edge);
                FindAndDeleteEdge(point);
                break;
            }
        }
    }

    private void Init()
    {
        graphPoints.Clear();
        graphPoints.AddRange(FindObjectsOfType<GraphPoint>());

        GraphPoint.DestroyPointsHandler += OnDestroyPoint;
        GraphPoint.CreatePointsHandler += OnCreatePoint;

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
            if (edges[i].pointA != null && edges[i].pointB != null)
            {
                Debug.DrawLine(edges[i].pointA.Position, edges[i].pointB.Position,GraphEditor.EdgeColor);
            }
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
                GDebug.Log(string.Format("соединены {0}, {1}", pointA.Id, pointB.Id));
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

            GDebug.Log(string.Format("удаление ребра {0}, {1}...", pointA.Id, pointB.Id));

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
            GDebug.LogError("Выбери две вершины!");
        }

        return selectedPoints.Count == 2;
    }

    public void Save()
    {

    }
}
