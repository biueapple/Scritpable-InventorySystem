using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ContainerInterface : MonoBehaviour
{
    public Slot_UI slot_UI_Prefab;
    public ContainerObject containerObject;
    public Dictionary<ContainerSlot, Slot_UI> slotsInterface = new Dictionary<ContainerSlot, Slot_UI>();

    protected Slot_UI downUI;
    protected Slot_UI upUI;
    protected Slot_UI moveUI;

    private void Start()
    {
        //���� UI �����
        CreateSlotsUI();
        //�ݹ� �ֱ�
        for (int i = 0; i < containerObject.storage.slots.Length; i++)
        {
            containerObject.storage.slots[i].afterCallback += UpdateSlotUI;
            UpdateSlotUI(containerObject.storage.slots[i]);
        }
    }

    public abstract void CreateSlotsUI();

    //itemObjcet�� ���� �������� ���� ������ ����
    //container�� ������ �������� ������ ���´�
    public void UpdateSlotUI(ContainerSlot _slot)
    {
        // itemObject(sprite ����) -> item(id ����)
        // containerObject(database����) -> container -> containerSlot(item����) 
        if (_slot.amount == 0 || _slot.item.id < 0)
        {
            //�󽽷�
            if(slotsInterface[_slot] != null)
                slotsInterface[_slot].Setting(null, "", new Color(1,1,1,0));
        }
        else
        {
            if (slotsInterface[_slot] != null)
                slotsInterface[_slot].Setting(containerObject.database.GetItemObjectWithId(_slot.item.id).sprite, _slot.amount == 1 ? "" : _slot.amount.ToString(), Color.white);
        }
    }

    public void MouseDown(PointerEventData eventData)
    {
        downUI = eventData.pointerCurrentRaycast.gameObject.transform.GetComponent<Slot_UI>();
        if(downUI.slot.amount > 0)
        {
            //ui �����
            moveUI = Instantiate(slot_UI_Prefab, Vector3.zero, Quaternion.identity, transform.parent);
            moveUI.GetComponent<Image>().raycastTarget = false;
            moveUI.Setting(downUI.transform.GetChild(0).GetComponent<Image>().sprite, "", Color.white);
            moveUI.transform.position = Input.mousePosition;
            //ui �����̱� ���� (moveUI�� null�� �ƴѰ� �����̴°�)
        }
    }

    public void MouseUp(PointerEventData eventData) 
    {
        if(eventData.pointerEnter.GetComponent<Slot_UI>() != null)
        {
            upUI = eventData.pointerEnter.GetComponent<Slot_UI>();
            //swap
            containerObject.SwapContainerSlot(downUI.slot, upUI.slot);
        }

        //ui �����̱� ��
        //ui ����
        Destroy(moveUI.gameObject);
    }

    public void MouseDrag(PointerEventData eventData)
    {
        if (moveUI != null)
            moveUI.transform.position = Input.mousePosition;
    }
}
