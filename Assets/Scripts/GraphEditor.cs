using UnityEngine;
using UnityEditor;

/// <summary>
/// Окно редактора графа
/// </summary>
public class GraphEditor : EditorWindow
{
    private Graph graph;
    private Graph Graph
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

    [MenuItem("Window/Graph editor")]
    public static void ShowWindow()
    {
        GetWindow<GraphEditor>("Graph editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("This is graph editor!", EditorStyles.boldLabel);

        if (GUILayout.Button("Draw edge"))
        {
            Graph.DrawEdge();
        }

        if (GUILayout.Button("Delete edge"))
        {
            Graph.DeleteEdge();
        }

        if (GUILayout.Button("Delete all edges"))
        {
            Graph.DeleteAllEdges();
        }

        if (GUILayout.Button("Find path"))
        {
            Graph.FindPath();
        }

        if (GUILayout.Button("Clear path"))
        {
            Graph.ClearPath();
        }
    }

}
