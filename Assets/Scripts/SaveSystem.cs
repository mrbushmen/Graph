using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public static class SaveSystem
{
    public static void SaveGraph(Graph graph)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/graph.dat";
        FileStream stream = new FileStream(path, FileMode.Create);

        GraphData data = new GraphData(graph);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GraphData LoadGraph()
    {
        string path = Application.persistentDataPath + "/graph.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GraphData data = formatter.Deserialize(stream) as GraphData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("GraphEditor: файл не найден " + path);
            return null;
        }
    }
}
