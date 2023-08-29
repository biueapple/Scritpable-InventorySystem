using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot_UI : MonoBehaviour , IPointerUpHandler , IPointerDownHandler , IDragHandler
{
    public ContainerInterface interfaece;
    public ContainerSlot slot;


    public void OnDrag(PointerEventData eventData)
    {
        interfaece.MouseDrag(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        interfaece.MouseDown(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        interfaece.MouseUp(eventData);
    }

    public void Setting(Sprite sprite, string str, Color color)
    {
        transform.GetChild(0).GetComponent<Image>().sprite = sprite;
        transform.GetChild(0).GetComponent<Image>().color = color;
        transform.GetChild(1).GetComponent<Text>().text = str;
    }
}
