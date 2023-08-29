using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicInterface : ContainerInterface
{
    public int X_START;
    public int Y_START;

    public int X_SPACE_BETWWEN_ITEM;
    public int NUMBER_OF_COLUMN;
    public int Y_SPACE_BETWWEN_ITEMS;

    public override void CreateSlotsUI()
    {
        for (int i = 0; i < containerObject.storage.slots.Length; i++)
        {
            //ui�����
            Slot_UI slot_ui = Instantiate(slot_UI_Prefab, Vector3.zero, Quaternion.identity, transform);
            slot_ui.interfaece = this;
            slot_ui.name = "slot_" + i;
            slot_ui.slot = containerObject.storage.slots[i];
            //��ġ���ϰ�
            slot_ui.GetComponent<RectTransform>().localPosition = GetPosition(i);
           
            //����
            slotsInterface.Add(containerObject.storage.slots[i], slot_ui);
        }
    }

    private Vector3 GetPosition(int i)
    {
        return new Vector3(X_START + (X_SPACE_BETWWEN_ITEM * (i % NUMBER_OF_COLUMN)), Y_START + (-Y_SPACE_BETWWEN_ITEMS * (i / NUMBER_OF_COLUMN)), 0);
    }
}
