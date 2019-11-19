using UnityEngine;

/// <summary>
/// Вершина графа
/// </summary>
[ExecuteInEditMode]
public class GraphPoint : MonoBehaviour
{
    private static int count = 0;

    [SerializeField]
    private Color32 color = new Color32(0, 255, 150, 255);

    /// <summary>
    /// Координаты вершины графа
    /// </summary>
    public Vector3 Position
    {
        get => transform.position;
    }

    //TODO: после отладки заменить поле свойством
    //public int Id { get; private set; } = 0;
    public int Id = 0;

    private void OnEnable()
    {
        count++;
        Id = count;
    }

    private void OnDisable()
    {
        count--;
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
