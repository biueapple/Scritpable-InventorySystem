using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot_UI : MonoBehaviour , IPointerUpHandler , IPointerDownHandler , IDragHandler , IPointerClickHandler
{
    private ContainerInterface containerInterface;
    public ContainerInterface ContainerInterface { get { return containerInterface; } set { containerInterface = value; } }
    private ContainerSlot slot;
    public ContainerSlot Slot { get { return slot; } set { slot = value; } }
    public bool input;
    public bool output;

    public void OnDrag(PointerEventData eventData)
    {
        if (output)
        {
            //containerInterface.MouseDrag(eventData);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(output)
        {
            //containerInterface.MouseDown(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(output)
        {
            //containerInterface.MouseUp(eventData);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        containerInterface.SlotClick(this);
    }

    public void Setting(Sprite sprite, string str, Color color)
    {
        transform.GetChild(0).GetComponent<Image>().sprite = sprite;
        transform.GetChild(0).GetComponent<Image>().color = color;
        transform.GetChild(1).GetComponent<Text>().text = str;
    }

   
}
