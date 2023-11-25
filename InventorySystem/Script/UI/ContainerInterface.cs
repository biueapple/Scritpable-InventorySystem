using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//아이템을 담고있는 ContainerObject와 연동해서 ui로써 보여주는 클래스 이 클래스를 상속받는 StaticInterface,DynamicInterface를 이용해서 사용
//원래는 corsorSlot이라는게 없고 down한 슬롯과 up한 슬롯을 저장하고 swap하는게 끝이였지만
//아이템을 여러개로 나눈다거나 할때 필요해서 corsorSlot을 하나 만들고 downSlot과 moveSlot을 swap upSlot과 moveSlot을 swap하는 방법으로 바꿈
public abstract class ContainerInterface : MonoBehaviour
{
    //ui슬롯 프리팹
    public Slot_UI slot_UI_Prefab;
    //아이템을 담고있는 클래스
    public ContainerObject containerObject;
    //슬롯과 ui를 엮어줌
    public Dictionary<ContainerSlot, Slot_UI> slotsInterface = new Dictionary<ContainerSlot, Slot_UI>();

    public Slot_UI corsorUI;

    private Coroutine corsorCoroutine = null;

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
        if(corsorUI.Slot == null)
        {
            corsorUI.Slot = new ContainerSlot();
            corsorUI.Slot.afterCallback += UpdateCorsorSlot;
        }
    }

    public abstract void CreateSlotsUI();

    //만들어야 하는것은 원하는 종류의 아이템만 보여주는 함수를 만들어야함 (소모품만 보여준다던가 장비만 보여준다던가)
    

    public void UpdateCorsorSlot(ContainerSlot _slot)
    {
        if (_slot.Amount == 0 || _slot.GetItem.id < 0)
        {
            corsorUI.Setting(null, "", new Color(1, 1, 1, 0));
        }
        else
        {
            corsorUI.Setting(containerObject.database.GetItemObjectWithId(_slot.GetItem.id).sprite, _slot.Amount == 1 ? "" : _slot.Amount.ToString(), Color.white);
        }
    }

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


    //대규모 수정이 필요할듯
    public void SlotClick(Slot_UI slot)
    {
        //이미 아이템을 잡은 상태임
        if(corsorUI.Slot.Amount > 0)
        {
            //클릭한 곳의 슬롯도 아이템이 있는곳임
            if(slot.Slot.Amount > 0)
            {
                //아이템을 넣고 빼는게 가능함
                if(slot.input && slot.output && slot.Slot.CanPlace(corsorUI.Slot.GetItem))
                {
                    containerObject.SwapContainerSlot(corsorUI.Slot, slot.Slot);
                    corsorUI.gameObject.SetActive(false);
                    if (corsorCoroutine != null)
                        StopCoroutine(corsorCoroutine);
                }
            }
            //아이템이 없는곳임
            else
            {
                //아이템을 넣는게 가능함
                if (slot.input && slot.Slot.CanPlace(corsorUI.Slot.GetItem))
                {
                    containerObject.SwapContainerSlot(corsorUI.Slot, slot.Slot);
                    corsorUI.gameObject.SetActive(false);
                    if (corsorCoroutine != null)
                        StopCoroutine(corsorCoroutine);
                }
            }
        }
        //아이템을 잡지 않은 상태임
        else
        {
            //아이템이 있는 칸임
            if (slot.Slot.Amount > 0)
            {
                //클릭한 아이템 슬롯이 아이템을 뺄 수 있는 칸임
                if (slot.output)
                {
                    containerObject.SwapContainerSlot(corsorUI.Slot, slot.Slot);
                    corsorUI.gameObject.SetActive(true);
                    if (corsorCoroutine != null)
                        StopCoroutine(corsorCoroutine);
                    corsorCoroutine = StartCoroutine(CorsorMoving());
                }
            }
        }
    }


    private IEnumerator CorsorMoving()
    {
        while(true)
        {
            corsorUI.transform.position = Input.mousePosition;
            yield return null;
        }
    }

    //mouseDown 와 mouseUp으로 swap을 했는데
    //mouseDwon 와 mouseUp이 같으면 클릭 다시한번 mouseDwon 와 mouseUp이 같으면 스왑으로 변경해야 할듯함

    ////slot_ui가 호출해줌
    //public void MouseDown(PointerEventData eventData)
    //{
    //    downUI = eventData.pointerCurrentRaycast.gameObject.transform.GetComponent<Slot_UI>();
    //}

    ////slot_ui가 호출해줌
    //public void MouseUp(PointerEventData eventData) 
    //{
    //    if (eventData.pointerEnter != null && eventData.pointerEnter.GetComponent<Slot_UI>() != null)
    //    {
    //        upUI = eventData.pointerEnter.GetComponent<Slot_UI>();

    //        //이미 클릭한 아이템이 있음
    //        if (moveSlot.Amount > 0)
    //        {
    //            //좌클릭
    //            if (eventData.button == PointerEventData.InputButton.Left)
    //            {
    //                //빈곳에 좌클릭시 전부다 옮김
    //                if (upUI.Slot.Amount <= 0)
    //                {
    //                    containerObject.SwapContainerSlot(upUI.Slot, moveUI.Slot);
    //                    moveUI.gameObject.SetActive(false);
    //                    StopCoroutine(moveUI.ContainerInterface.moveingUI);
    //                }
    //                //비어있지 않은곳에 좌클릭시 같은 아이템이라면 전부다 옮김
    //                else if (upUI.Slot.Amount > 0 && upUI.Slot.GetItem.id == moveSlot.GetItem.id)
    //                {
    //                    upUI.Slot.AddAmount(moveSlot.Amount);
    //                    moveSlot.AddAmount(-moveSlot.Amount);
    //                    moveUI.gameObject.SetActive(false);
    //                    StopCoroutine(moveingUI);
    //                }
    //                //비어있지도 않고 같은 아이템도 아니라면 아이템 교환
    //                else
    //                {
    //                    containerObject.SwapContainerSlot(moveUI.Slot, upUI.Slot);
    //                }
    //            }
    //            //우클릭
    //            else if (eventData.button == PointerEventData.InputButton.Right)
    //            {
    //                //빈곳에 우클릭시 하나만 옮기기
    //                if (upUI.Slot.Amount <= 0)
    //                {
    //                    upUI.Slot.UpdateSlot(moveSlot.GetItem, 1);
    //                    moveSlot.AddAmount(-1);
    //                }
    //                //이미 아이템이 있는곳에 우클릭시 같은 아이템이라면 하나만 옮기기
    //                else if (upUI.Slot.Amount > 0 && upUI.Slot.GetItem.id == moveSlot.GetItem.id)
    //                {
    //                    upUI.Slot.AddAmount(1);
    //                    moveSlot.AddAmount(-1);
    //                }
    //                //그 외에는 아무일도 일어나지 않음 (이미 아이템이 있으면서 같은 아이템도 아니라면 아무일도 없음)

    //                //더이상 들고있는 아이템이 없다면 비활성화
    //                if (moveSlot.Amount <= 0)
    //                {
    //                    moveUI.gameObject.SetActive(false);
    //                    StopCoroutine(moveingUI);
    //                }
    //            }
    //        }
    //        //이미 클릭한 아이템이 없음
    //        else
    //        {
    //            //선택 제대로 된 클릭
    //            if (downUI == upUI)
    //            {
    //                //선택한곳에 아이템이 있다면
    //                if (downUI.Slot.Amount > 0)
    //                {
    //                    //좌클릭 (스왑)
    //                    if (eventData.button == PointerEventData.InputButton.Left)
    //                    {
    //                        //ui 활성화
    //                        moveUI.gameObject.SetActive(true);
    //                        containerObject.SwapContainerSlot(downUI.Slot, moveUI.Slot);
    //                        moveingUI = StartCoroutine(MoveUIMoving());
    //                    }
    //                    //우클릭은 아무일도 일어나지 않음
    //                }    
    //            }
    //        }
    //        downUI = null;
    //        upUI = null;
    //    }
    //}

    ////아이템을 잡고 드래그하면 드래그한 부분이 빈곳일시 1/n하는걸로
    //public void MouseDrag(PointerEventData eventData)
    //{
    //    dragUI = eventData.pointerEnter.GetComponent<Slot_UI>();
    //    if (downUI != dragUI)
    //    {
    //        //moveui에서 down과 drag ui에서 1/n로 나누기 해야함
    //    }
    //}

}
