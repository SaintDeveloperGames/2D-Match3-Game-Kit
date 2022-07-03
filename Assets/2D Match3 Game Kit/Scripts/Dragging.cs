using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dragging : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private bool isDragging;
    private Gameplay _gameplay;

    private void Start()
    {
        _gameplay = GetComponent<Gameplay>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDragging || _gameplay.IsUpdateBoard)
            return;
        isDragging = true;
        if (eventData.pointerCurrentRaycast.gameObject == null || !eventData.pointerCurrentRaycast.gameObject.activeInHierarchy)
            return;
        var point = eventData.pointerCurrentRaycast.gameObject.GetComponent<Point>();
        if (point == null)
            return;
        var x = point.X;
        var y = point.Y;
        point.gameObject.transform.SetAsLastSibling();
        _gameplay.SwipeControl(eventData.delta.x, eventData.delta.y, x, y);
        StartCoroutine(Delay());
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.5f);
        isDragging = false;
    }
}
