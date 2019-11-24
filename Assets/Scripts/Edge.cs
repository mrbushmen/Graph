using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Ребро графа
/// </summary>
[System.Serializable]
public class Edge
{
    public List<int> points = new List<int>();

    public Point A;
    public Point B;

    private float size;
    public float Size
    {
        get => size;
    }

    public Edge(Vector3  pointA, Vector3 pointB, int aId, int bId)
    {
        Math.Round(Vector3.Distance(pointA, pointB), 1);
        A = new Point(pointA);
        B = new Point(pointB);

        points.Add(aId);
        points.Add(bId);
    }

   
    public bool HasPoint(int pointId)
    {
        return (points.Contains(pointId));
    }

    [System.Serializable]
    public struct Point
    {
        public float X;
        public float Y;
        public float Z;

        public Point(float x,float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point(Vector3 vector)
        {
            X = vector.x;
            Y = vector.y;
            Z = vector.z;
        }

        public Vector3 ToVector()
        {
            return new Vector3(X, Y, Z);
        }
    }

}