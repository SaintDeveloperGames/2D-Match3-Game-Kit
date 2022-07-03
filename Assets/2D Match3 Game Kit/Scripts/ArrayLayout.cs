using UnityEngine;

public class ArrayLayout : MonoBehaviour
{
    public static int Y = 10;
    public static int X = 13;
    public int NumberOfItems;
    public int Height;
    public int Width;

    [System.Serializable]
    public class Rows
    {
        public int[] Column = new int[Y];
    }

    public Rows[] Row = new Rows[X];
}