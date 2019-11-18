using UnityEngine;
using UnityEditor;

/// <summary>
/// Окно редактора графа
/// </summary>
public class GraphEditor : EditorWindow
{
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
            FindObjectOfType<Graph>().DrawEdge();
        }

        if (GUILayout.Button("Delete edge"))
        {            
            FindObjectOfType<Graph>().DeleteEdge();
        }

        if (GUILayout.Button("Delete all edges"))
        {
            FindObjectOfType<Graph>().DeleteAllEdges();
        }
    }

}
