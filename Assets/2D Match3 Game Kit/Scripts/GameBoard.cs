using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    [Header("Level parameters")]
    [Tooltip("The required number of items to win.")]
    [SerializeField] internal int ScoreItems;
    [Tooltip("The number of the item (starting from 1) that we will collect.")]
    [SerializeField] internal int ValueNecessaryItem;
    [Tooltip("The allowed number of moves.")]
    [SerializeField] internal int ScoreMoves;
    [Tooltip("List of items (you can add and remove items if necessary).")]
    [SerializeField] internal List<Sprite> SpritesItems;

    private protected Point[,] Points = new Point[ArrayLayout.X, ArrayLayout.Y];
    private protected Vector2[,] SlotPositions = new Vector2[ArrayLayout.X, ArrayLayout.Y];
    private protected ArrayLayout ArrayLayout;

    [SerializeField] private GameObject _slot;
    [SerializeField] private GameObject _slotsBoard;
    [SerializeField] private Sprite _spriteSlot;

    [SerializeField] private GameObject _item;
    [SerializeField] private GameObject _itemsBoard;

    private void Start()
    {
        ArrayLayout = GetComponent<ArrayLayout>();
        InitialazeBoard();
    }

    private void InitialazeBoard()
    {
        for (int x = 0; x < ArrayLayout.Height; x++)
        {
            for (int y = 0; y < ArrayLayout.Width; y++)
            {
                if (ArrayLayout.Row[x].Column[y] == -1)
                    continue;
                GetElement(_slot, _slotsBoard, _spriteSlot, x, y);
                if (ArrayLayout.Row[x].Column[y] > 0)
                {
                    var item = GetElement(_item, _itemsBoard, SpritesItems[ArrayLayout.Row[x].Column[y] - 1], x, y);
                    DefiningPoint(item, x, y);
                }
            }
        }
    }

    private GameObject GetElement(GameObject element, GameObject parent, Sprite sprite, int x, int y)
    {
        var prefab = Instantiate(element, parent.transform);
        var rectPrefab = prefab.GetComponent<RectTransform>();
        rectPrefab.sizeDelta = GetSizeElement(parent);
        rectPrefab.anchoredPosition = new Vector2(rectPrefab.sizeDelta.x * y, rectPrefab.sizeDelta.x * -x);
        SlotPositions[x, y] = rectPrefab.anchoredPosition;
        prefab.GetComponent<Image>().sprite = sprite;
        return prefab;
    }

    private void DefiningPoint(GameObject gameObject, int x, int y)
    {
        gameObject.name = "Item [" + x + "," + y + "]";
        Point point = gameObject.GetComponent<Point>();
        Points[x, y] = point;
        point.X = x;
        point.Y = y;
        point.Value = ArrayLayout.Row[x].Column[y];
    }

    private Vector2 GetSizeElement(GameObject board)
    {
        var rectBoard = board.GetComponent<RectTransform>();
        float width;
        float height;
        width = rectBoard.sizeDelta.x / ArrayLayout.Width;
        height = rectBoard.sizeDelta.y / ArrayLayout.Height;
        var minSize = width >= height ? height : width;
        rectBoard.sizeDelta = new Vector2(minSize * ArrayLayout.Width, minSize * ArrayLayout.Height);
        return new Vector2(minSize, minSize);
    }
}
