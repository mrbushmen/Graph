using UnityEngine;
using UnityEditor;

/// <summary>
/// Окно редактора графа
/// </summary>
public class GraphEditor : EditorWindow
{
    public static Color PointColor = Color.green;
    public static Color SelectedPointColor = Color.red;
    public static Color EdgeColor = Color.white;
    public static Color PathColor = Color.yellow;

    private static string message = string.Empty;

    private static GUIStyle messageStyle = new GUIStyle();

    private static Graph graph;

    private static Graph Graph
    {
        get
        {
            if (graph == null)
            {
                graph = FindObjectOfType<Graph>();
            }
            return graph;
        }
    }

    public static bool IsDebug { get; private set; } = true;

    [MenuItem("Window/Graph editor")]
    public static void ShowWindow()
    {
        GetWindow<GraphEditor>("Graph editor");
    }

    private void OnGUI()
    {
        var headStyle = GUI.skin.GetStyle("Label");
        headStyle.alignment = TextAnchor.UpperCenter;
        headStyle.fontStyle = FontStyle.Bold;

        GUILayout.Label("This is graph editor!", headStyle);

        if (GUILayout.Button("Создать ребро"))
        {
            Graph.DrawEdge();
        }

        if (GUILayout.Button("Удалить ребро"))
        {
            Graph.DeleteEdge();
        }

        if (GUILayout.Button("Удалить все ребра"))
        {
            Graph.DeleteAllEdges();
        }

        if (GUILayout.Button("Построить кратчайший путь"))
        {
            Graph.FindPath();
        }

        if (GUILayout.Button("Удалить путь"))
        {
            Graph.ClearPath();
        }

        if (GUILayout.Button("Сохранить граф"))
        {
            Graph.Save();
        }

        GUILayout.Label("Настройки", headStyle);
        PointColor = EditorGUILayout.ColorField("Цвет вершины", PointColor);
        SelectedPointColor = EditorGUILayout.ColorField("Цвет выбранной вершины", SelectedPointColor);
        EdgeColor = EditorGUILayout.ColorField("Цвет ребер", EdgeColor);
        PathColor = EditorGUILayout.ColorField("Цвет кратчайшего пути", PathColor);

        IsDebug=GUILayout.Toggle(IsDebug, "Включить вывод сообщений в консоль");

        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        GUILayout.Label("Здесь будут отображаться сообщения:", EditorStyles.boldLabel);
        GUILayout.Label(message, messageStyle);

        GUILayout.EndVertical();
    }

    public static void ShowMessage(string msg, Color msgColor)
    {
        message = msg;
        messageStyle.normal.textColor = msgColor;
        //TODO: вызвать отрисовку UI.
    }
}
