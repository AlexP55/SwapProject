using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image Img;
    public Sprite OnSprite;
    public Sprite OffSprite;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Img.sprite = OnSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Img.sprite = OffSprite;
    }
}
