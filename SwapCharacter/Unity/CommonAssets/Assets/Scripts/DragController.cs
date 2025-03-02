using UnityEngine;
using UnityEngine.EventSystems;


public class DragController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform currentTransform;
    private GameObject mainContent;
    private Vector3 currentPossition;
    private int totalChild;

    public void OnPointerDown(PointerEventData eventData)
    {
        currentPossition = currentTransform.position;
        mainContent = currentTransform.parent.gameObject;
        totalChild = mainContent.transform.childCount;
        currentTransform.localScale = 1.1f * Vector3.one;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 pointerPos = Camera.current.ScreenToWorldPoint(eventData.position);
        currentTransform.position =
            new Vector3(pointerPos.x, currentTransform.position.y, currentTransform.position.z);

        for (int i = 0; i < totalChild; i++)
        {
            if (i != currentTransform.GetSiblingIndex())
            {
                Transform otherTransform = mainContent.transform.GetChild(i);
                int distance = (int)Vector3.Distance(currentTransform.localPosition,
                    otherTransform.localPosition);
                if (distance <= 10)
                {
                    Vector3 newPos = new Vector3(otherTransform.position.x, currentTransform.position.y,
                        currentTransform.position.z);
                    otherTransform.position = new Vector3(currentPossition.x, otherTransform.position.y,
                        otherTransform.position.z);
                    currentTransform.position = newPos;
                    currentTransform.SetSiblingIndex(otherTransform.GetSiblingIndex());
                    currentPossition = currentTransform.position;
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        currentTransform.position = currentPossition;
        currentTransform.localScale = Vector3.one;
    }
}