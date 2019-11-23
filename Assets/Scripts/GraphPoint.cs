using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Вершина графа
/// </summary>
[ExecuteInEditMode]
public class GraphPoint : MonoBehaviour
{
    public static Action<GraphPoint> DestroyPointsHandler;
    public static Action<GraphPoint> CreatePointsHandler;

    /// <summary>
    /// Количество точек на сцене.
    /// </summary>
    public static int Count { get; private set; } = 0;

    private static List<int> usedId = new List<int>();

    [SerializeField]
    private Color32 color = new Color32(0, 255, 150, 255);

    /// <summary>
    /// Координаты вершины графа
    /// </summary>
    public Vector3 Position
    {
        get => transform.position;
    }

    /// <summary>
    /// Идентификатор (номер, имя) точки. Присваивается при создании/включении объекта
    /// со скриптом GraphPoint.
    /// </summary>
    public int Id { get; private set; } = 0;

    private void OnEnable()
    {
        Count++;
        for (int i = 1; i <= Count; i++)
        {
            if (!usedId.Contains(i))
            {
                Id = i;
            }
        }
        usedId.Add(Id);
        CreatePointsHandler?.Invoke(this);
    }

    private void OnDisable()
    {
        Count--;
        usedId.Remove(Id);
        DestroyPointsHandler?.Invoke(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.7f);
    }
}
