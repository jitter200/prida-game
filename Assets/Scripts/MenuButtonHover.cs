using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Hover Sprites")]
    [SerializeField] private GameObject hoverSprite1;
    [SerializeField] private GameObject hoverSprite2;

    [Header("Text")]
    [SerializeField] private Graphic buttonText;

    [Header("Text Colors")]
    [SerializeField] private Color normalTextColor = new Color(1f, 0.93f, 0.75f, 1f);
    [SerializeField] private Color hoverTextColor = Color.white;

    private void Start()
    {
        SetHover(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetHover(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        SetHover(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetHover(false);
    }

    private void SetHover(bool isHover)
    {
        if (hoverSprite1 != null)
            hoverSprite1.SetActive(isHover);

        if (hoverSprite2 != null)
            hoverSprite2.SetActive(isHover);

        if (buttonText != null)
            buttonText.color = isHover ? hoverTextColor : normalTextColor;
    }
}