using System;
using UnityEngine;

public class Point : MonoBehaviour, IComparable
{
    public int X;
    public int Y;
    public int Value;
    public RectTransform RectTransform;

    private void Start()
    {
        RectTransform = GetComponent<RectTransform>();
    }

    public void IndexChange(int i, int j, bool isStartItem)
    {
        if (isStartItem)
        {
            X += i;
            Y += j;
        }
        else
        {
            X -= i;
            Y -= j;
        }
    }

    public int CompareTo(object obj)
    {
        var point = (Point)obj;
        var thisDistance = Math.Sqrt(X * X + Y * Y);
        var thatDistance = Math.Sqrt(point.X * point.X + point.Y * point.Y);
        return thisDistance.CompareTo(thatDistance);
    }
}
