using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ContainerInterface : MonoBehaviour
{
    //ui슬롯 프리팹
    public Slot_UI slot_UI_Prefab;
    //아이템을 담고있는 클래스
    public ContainerObject containerObject;
    //슬롯과 ui를 엮어줌
    public Dictionary<ContainerSlot, Slot_UI> slotsInterface = new Dictionary<ContainerSlot, Slot_UI>();

    //마우스 다운한 ui
    protected Slot_UI downUI;
    //마우스 업한 ui
    protected Slot_UI upUI;
    //예비 ui
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

    //만들어야 하는것은 원하는 종류의 아이템만 보여주는 함수를 만들어야함 (소모품만 보여준다던가 장비만 보여준다던가)


    //itemObjcet는 원본 아이템의 대한 정보를 갖고
    //container는 생성된 아이템의 정보를 갖는다
    public void UpdateSlotUI(ContainerSlot _slot)
    {
        // itemObject(sprite 소유) -> item(id 소유)
        // containerObject(database소유) -> container -> containerSlot(item소유) 
        if (_slot.Amount == 0 || _slot.GetItem.id < 0)
        {
            //빈슬롯
            if(slotsInterface[_slot] != null)
                slotsInterface[_slot].Setting(null, "", new Color(1,1,1,0));
        }
        else
        {
            if (slotsInterface[_slot] != null)
                slotsInterface[_slot].Setting(containerObject.database.GetItemObjectWithId(_slot.GetItem.id).sprite, _slot.Amount == 1 ? "" : _slot.Amount.ToString(), Color.white);
        }
    }

    //slot_ui가 호출해줌
    public void MouseDown(PointerEventData eventData)
    {
        //slot_ui만이 이 함수를 호출하기에 오류가 없음
        downUI = eventData.pointerCurrentRaycast.gameObject.transform.GetComponent<Slot_UI>();
        if(downUI.Slot.Amount > 0)
        {
            //ui 만들기
            moveUI = Instantiate(slot_UI_Prefab, Vector3.zero, Quaternion.identity, transform.parent);
            moveUI.GetComponent<Image>().raycastTarget = false;
            moveUI.Setting(downUI.transform.GetChild(0).GetComponent<Image>().sprite, "", Color.white);
            moveUI.transform.position = Input.mousePosition;
            //ui 움직이기 시작 (moveUI가 null이 아닌게 움직이는것)
        }
    }

    //slot_ui가 호출해줌
    public void MouseUp(PointerEventData eventData) 
    {
        if(eventData.pointerEnter.GetComponent<Slot_UI>() != null)
        {
            upUI = eventData.pointerEnter.GetComponent<Slot_UI>();
            //swap
            containerObject.SwapContainerSlot(downUI.Slot, upUI.Slot);
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
