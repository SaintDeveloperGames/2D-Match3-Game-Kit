using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay : GameBoard
{
    internal bool IsUpdateBoard;

    [Tooltip("The speed of movement of objects when swiping.")]
    [SerializeField] private float _speedMovement;
    [Tooltip("Speed multiplier drop items when updating the board.")]
    [SerializeField] private float _multiplierUpdateBoard;

    private List<List<Point>> _identicalPoints = new List<List<Point>>();
    private List<int> _newValuesPoints = new List<int>();
    private List<bool> _matchesHorizontal = new List<bool>();

    private void SwapTwoItems(int x, int y, int i, int j, float speedMovement)
    {
        if (x + i > ArrayLayout.Height - 1 || x + i < 0 || y + j > ArrayLayout.Width - 1 || y + j < 0 || Points[x + i, y + j] == null ||
            (!Points[x + i, y + j].gameObject.activeSelf && Points[x, y].gameObject.activeSelf))
            return;
        StartCoroutine(WaitMoveItems(Points[x, y], Points[x + i, y + j], x, y, i, j, speedMovement));
    }

    private void DataExchangeTwoPoints(int x, int y, int i, int j)
    {
        var tempItem = Points[x, y];
        Points[x, y].IndexChange(i, j, true);
        Points[x + i, y + j].IndexChange(i, j, false);
        Points[x, y] = Points[x + i, y + j];
        Points[x + i, y + j] = tempItem;
    }

    private IEnumerator MoveItems(Point startPoint, Point nextPoint, Vector2 tempPosStart, Vector2 tempPosNext, float speedMovement)
    {
        while (Vector2.Distance(startPoint.RectTransform.anchoredPosition, tempPosNext) > 0 && Vector2.Distance(nextPoint.RectTransform.anchoredPosition, tempPosStart) > 0)
        {
            float speed = speedMovement / ArrayLayout.Width * Time.deltaTime;
            startPoint.RectTransform.anchoredPosition = Vector2.MoveTowards(startPoint.RectTransform.anchoredPosition, tempPosNext, speed);
            nextPoint.RectTransform.anchoredPosition = Vector2.MoveTowards(nextPoint.RectTransform.anchoredPosition, tempPosStart, speed);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator WaitMoveItems(Point startPoint, Point nextPoint, int x, int y, int i, int j, float speedMovement)
    {
        yield return MoveItems(startPoint, nextPoint, SlotPositions[x, y], SlotPositions[x + i, y + j], speedMovement);
        DataExchangeTwoPoints(x, y, i, j);
        if ((startPoint.gameObject.activeSelf && !AnyMatches(startPoint.X, startPoint.Y, true)) & (nextPoint.gameObject.activeSelf && !AnyMatches(nextPoint.X, nextPoint.Y, false)))
        {
            StartCoroutine(MoveItems(nextPoint, startPoint, SlotPositions[x, y], SlotPositions[x + i, y + j], speedMovement));
            DataExchangeTwoPoints(x + i, y + j, -i, -j);
        }
        else
            UpdateBoard();
    }

    internal void SwipeControl(float deltaX, float deltaY, int x, int y)
    {
        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
            DirectionSwipes(deltaX, x, y, 0, 1);
        else
            DirectionSwipes(deltaY, x, y, -1, 0);
    }

    private void DirectionSwipes(float dirCoordinate, int x, int y, int i, int j)
    {
        if (dirCoordinate > 0)
            SwapTwoItems(x, y, i, j, _speedMovement);
        else
            SwapTwoItems(x, y, -i, -j, _speedMovement);
    }

    private bool AnyMatches(int x, int y, bool isMove)
    {
        return GetListInactivePoints(FindMatches(false, x, y, x, ArrayLayout.Height), isMove, false).Count > 0 |
            GetListInactivePoints(FindMatches(true, x, y, y, ArrayLayout.Width), isMove, true).Count > 0;
    }

    private bool IsMatch(int x, int y, int floatIndex, List<Point> identicalPoints, bool isHorizontalMatch)
    {
        return isHorizontalMatch ? AddMatch(x, y, x, floatIndex, identicalPoints) : AddMatch(x, y, floatIndex, y, identicalPoints);
    }

    private bool AddMatch(int x, int y, int i, int j, List<Point> identicalPoints)
    {
        if (Points[i, j] != null && (Points[i, j].gameObject.activeSelf || !Points[x, y].gameObject.activeSelf) && Points[i, j].Value == Points[x, y].Value)
        {
            for (int k = i + 1; k < ArrayLayout.Height; k++)
            {
                if (Points[k, j] == null)
                    break;
                if (!Points[k, j].gameObject.activeSelf)
                    return false;
            }
            identicalPoints.Add(Points[i, j]);
            return true;
        }
        return false;
    }

    private List<Point> GetListInactivePoints(List<Point> identicalPoints, bool isMove, bool isHorizontal)
    {
        if (identicalPoints.Count > 2)
        {
            for (int i = 0; i < identicalPoints.Count; i++)
            {
                identicalPoints[i].gameObject.SetActive(false);
            }
            ScorePoints(identicalPoints);
            if (isMove)
                ScoreMoves--;
            _identicalPoints.Add(identicalPoints);
            _matchesHorizontal.Add(isHorizontal);
            return identicalPoints;
        }
        return new List<Point>();
    }

    private List<Point> FindMatches(bool isHorizontalMatch, int x, int y, int indexToChange, int length)
    {
        List<Point> identicalPoints = new List<Point>();
        for (int j = indexToChange; j < length; j++)
        {
            if (!IsMatch(x, y, j, identicalPoints, isHorizontalMatch))
                break;
        }
        for (int j = indexToChange - 1; j >= 0; j--)
        {
            if (!IsMatch(x, y, j, identicalPoints, isHorizontalMatch))
                break;
        }
        return identicalPoints;
    }

    #region UPDATE GAME BOARD

    private void UpdateBoard()
    {
        IsUpdateBoard = true;
        SortPoints();
        SearchSpawnPoints();
        if (_identicalPoints.Count == 0)
        {
            _newValuesPoints.Clear();
            IsUpdateBoard = false;
            return;
        }
        if (_matchesHorizontal[0] == true)
            SwapTwoItems(_identicalPoints[0][0].X, _identicalPoints[0][0].Y, -1, 0, _speedMovement * _multiplierUpdateBoard);
        else
            SwapTwoItems(_identicalPoints[0][_identicalPoints[0].Count - 1].X, _identicalPoints[0][_identicalPoints[0].Count - 1].Y, -_identicalPoints[0].Count, 0, _speedMovement * _multiplierUpdateBoard);
    }

    private void SearchSpawnPoints()
    {
        if (_identicalPoints[0][0].X != 0 && Points[_identicalPoints[0][0].X - 1, _identicalPoints[0][0].Y] != null)
            return;
        var random = GetRandomValue();
        _identicalPoints[0][0].gameObject.GetComponent<Image>().sprite = SpritesItems[random];
        _identicalPoints[0][0].Value = random + 1;
        _identicalPoints[0][0].gameObject.SetActive(true);
        AnyMatches(_identicalPoints[0][0].X, _identicalPoints[0][0].Y, false);
        _identicalPoints[0].Remove(_identicalPoints[0][0]);
        RemoveListPoints();
        if (_identicalPoints.Count > 0 && _identicalPoints[0].Count > 0)
            SearchSpawnPoints();
    }

    private void SortPoints()
    {
        _identicalPoints[0].Sort((point0, point1) => { return point0.CompareTo(point1); });
    }

    private void RemoveListPoints()
    {
        if (_identicalPoints[0].Count == 0)
        {
            _identicalPoints.Remove(_identicalPoints[0]);
            _matchesHorizontal.Remove(_matchesHorizontal[0]);
            if (_identicalPoints.Count > 0)
                SortPoints();
        }
    }

    private int GetRandomValue()
    {
        var random = Random.Range(0, SpritesItems.Count);
        if (_newValuesPoints.Count > 0)
        {
            while (random == _newValuesPoints[_newValuesPoints.Count - 1])
                random = Random.Range(0, SpritesItems.Count);
        }
        _newValuesPoints.Add(random);
        return random;
    }

    #endregion

    #region SCORE POINTS

    private void ScorePoints(List<Point> identicalPoints)
    {
        if (identicalPoints[0].Value == ValueNecessaryItem)
            ScoreItems -= identicalPoints.Count;
    }

    #endregion
}
