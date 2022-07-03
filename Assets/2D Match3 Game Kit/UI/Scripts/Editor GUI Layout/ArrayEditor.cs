#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ArrayLayout))]
public class ArrayEditor : Editor
{
    private ArrayLayout _arrayLayout;

    void OnEnable()
    {
        _arrayLayout = target as ArrayLayout;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("The sequence numbers of items from the sprite list are entered into the matrix.");
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("0 - no item.");
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("-1 - empty space.");
        EditorGUILayout.Space();
        _arrayLayout.Height = EditorGUILayout.IntField("Number of rows, no more " + ArrayLayout.X, _arrayLayout.Height);
        _arrayLayout.Width = EditorGUILayout.IntField("Number of columns, no more " + ArrayLayout.Y, _arrayLayout.Width);
        _arrayLayout.NumberOfItems = EditorGUILayout.IntField("Number of items ", _arrayLayout.NumberOfItems);
        EditorGUILayout.Space();
        if (GUILayout.Button("Random"))
        {
            for (int x = 0; x < _arrayLayout.Height; x++)
            {
                for (int y = 0; y < _arrayLayout.Width; y++)
                {
                    _arrayLayout.Row[x].Column[y] = EditorGUILayout.IntField(Random.Range(1, _arrayLayout.NumberOfItems + 1));
                }
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        for (int x = 0; x < _arrayLayout.Height; x++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int y = 0; y < _arrayLayout.Width; y++)
            {
                _arrayLayout.Row[x].Column[y] = EditorGUILayout.IntField(_arrayLayout.Row[x].Column[y]);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        if (GUI.changed)
        {
            Undo.RecordObject(_arrayLayout, "Save");
            EditorUtility.SetDirty(_arrayLayout);
        }
    }
}
#endif