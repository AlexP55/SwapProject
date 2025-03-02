using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image Img;
    public Sprite OnSprite;
    public Sprite OffSprite;
    public MainController Main;

    public Vector3 SavePos;
    public Vector3 CV;
    public float smoothtime = 0.05f;
    public bool moving;

    public bool IsSelect
    {
        get
        {
            return Main.Selected == this;
        }
    }

    public void Select()
    {
        Img.sprite = OnSprite;
        transform.localScale = 1.1f * Vector3.one;
        Main.Selected = this;
    }

    public void UnSelect()
    {
        Img.sprite = OffSprite;
        transform.localScale = Vector3.one;
        if (IsSelect) Main.Selected = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Main.Select(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsSelect) Img.sprite = OnSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsSelect) Img.sprite = OffSprite;
    }

    private void Update()
    {
        if (moving)
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, SavePos, ref CV, smoothtime);
            if (Vector3.Distance(transform.localPosition, SavePos) < 1)
            {
                Stop();
                transform.localPosition = SavePos;
            }
        }
    }

    public void Move(Vector3 Pos)
    {
        SavePos = Pos;
        moving = true;
    }

    public void Stop()
    {
        moving = false;
        CV = Vector3.zero;
    }
}
