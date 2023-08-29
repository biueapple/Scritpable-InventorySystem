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
        //슬롯 UI 만들기
        CreateSlotsUI();
        //콜백 넣기
        for (int i = 0; i < containerObject.storage.slots.Length; i++)
        {
            containerObject.storage.slots[i].afterCallback += UpdateSlotUI;
            UpdateSlotUI(containerObject.storage.slots[i]);
        }
    }

    public abstract void CreateSlotsUI();

    //itemObjcet는 원본 아이템의 대한 정보를 갖고
    //container는 생성된 아이템의 정보를 갖는다
    public void UpdateSlotUI(ContainerSlot _slot)
    {
        // itemObject(sprite 소유) -> item(id 소유)
        // containerObject(database소유) -> container -> containerSlot(item소유) 
        if (_slot.amount == 0 || _slot.item.id < 0)
        {
            //빈슬롯
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
            //ui 만들기
            moveUI = Instantiate(slot_UI_Prefab, Vector3.zero, Quaternion.identity, transform.parent);
            moveUI.GetComponent<Image>().raycastTarget = false;
            moveUI.Setting(downUI.transform.GetChild(0).GetComponent<Image>().sprite, "", Color.white);
            moveUI.transform.position = Input.mousePosition;
            //ui 움직이기 시작 (moveUI가 null이 아닌게 움직이는것)
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

        //ui 움직이기 끝
        //ui 삭제
        Destroy(moveUI.gameObject);
    }

    public void MouseDrag(PointerEventData eventData)
    {
        if (moveUI != null)
            moveUI.transform.position = Input.mousePosition;
    }
}
