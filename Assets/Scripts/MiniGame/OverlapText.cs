using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class OverlapText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Color initialColor = Color.black;
    Color mouseOverColor = new Color(0.85f, 0.85f, 0.85f, 1f);

    TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = mouseOverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = initialColor;
    }

}
