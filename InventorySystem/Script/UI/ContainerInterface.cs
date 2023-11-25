using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//�������� ����ִ� ContainerObject�� �����ؼ� ui�ν� �����ִ� Ŭ���� �� Ŭ������ ��ӹ޴� StaticInterface,DynamicInterface�� �̿��ؼ� ���
//������ corsorSlot�̶�°� ���� down�� ���԰� up�� ������ �����ϰ� swap�ϴ°� ���̿�����
//�������� �������� �����ٰų� �Ҷ� �ʿ��ؼ� corsorSlot�� �ϳ� ����� downSlot�� moveSlot�� swap upSlot�� moveSlot�� swap�ϴ� ������� �ٲ�
public abstract class ContainerInterface : MonoBehaviour
{
    //ui���� ������
    public Slot_UI slot_UI_Prefab;
    //�������� ����ִ� Ŭ����
    public ContainerObject containerObject;
    //���԰� ui�� ������
    public Dictionary<ContainerSlot, Slot_UI> slotsInterface = new Dictionary<ContainerSlot, Slot_UI>();

    public Slot_UI corsorUI;

    private Coroutine corsorCoroutine = null;

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
        if(corsorUI.Slot == null)
        {
            corsorUI.Slot = new ContainerSlot();
            corsorUI.Slot.afterCallback += UpdateCorsorSlot;
        }
    }

    public abstract void CreateSlotsUI();

    //������ �ϴ°��� ���ϴ� ������ �����۸� �����ִ� �Լ��� �������� (�Ҹ�ǰ�� �����شٴ��� ��� �����شٴ���)
    

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

    //itemObjcet�� ���� �������� ���� ������ ����
    //container�� ������ �������� ������ ���´�
    public void UpdateSlotUI(ContainerSlot _slot)
    {
        // itemObject(sprite ����) -> item(id ����)
        // containerObject(database����) -> container -> containerSlot(item����) 
        if (_slot.Amount == 0 || _slot.GetItem.id < 0)
        {
            //�󽽷�
            if(slotsInterface[_slot] != null)
                slotsInterface[_slot].Setting(null, "", new Color(1,1,1,0));
        }
        else
        {
            if (slotsInterface[_slot] != null)
                slotsInterface[_slot].Setting(containerObject.database.GetItemObjectWithId(_slot.GetItem.id).sprite, _slot.Amount == 1 ? "" : _slot.Amount.ToString(), Color.white);
        }
    }


    //��Ը� ������ �ʿ��ҵ�
    public void SlotClick(Slot_UI slot)
    {
        //�̹� �������� ���� ������
        if(corsorUI.Slot.Amount > 0)
        {
            //Ŭ���� ���� ���Ե� �������� �ִ°���
            if(slot.Slot.Amount > 0)
            {
                //�������� �ְ� ���°� ������
                if(slot.input && slot.output && slot.Slot.CanPlace(corsorUI.Slot.GetItem))
                {
                    containerObject.SwapContainerSlot(corsorUI.Slot, slot.Slot);
                    corsorUI.gameObject.SetActive(false);
                    if (corsorCoroutine != null)
                        StopCoroutine(corsorCoroutine);
                }
            }
            //�������� ���°���
            else
            {
                //�������� �ִ°� ������
                if (slot.input && slot.Slot.CanPlace(corsorUI.Slot.GetItem))
                {
                    containerObject.SwapContainerSlot(corsorUI.Slot, slot.Slot);
                    corsorUI.gameObject.SetActive(false);
                    if (corsorCoroutine != null)
                        StopCoroutine(corsorCoroutine);
                }
            }
        }
        //�������� ���� ���� ������
        else
        {
            //�������� �ִ� ĭ��
            if (slot.Slot.Amount > 0)
            {
                //Ŭ���� ������ ������ �������� �� �� �ִ� ĭ��
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

    //mouseDown �� mouseUp���� swap�� �ߴµ�
    //mouseDwon �� mouseUp�� ������ Ŭ�� �ٽ��ѹ� mouseDwon �� mouseUp�� ������ �������� �����ؾ� �ҵ���

    ////slot_ui�� ȣ������
    //public void MouseDown(PointerEventData eventData)
    //{
    //    downUI = eventData.pointerCurrentRaycast.gameObject.transform.GetComponent<Slot_UI>();
    //}

    ////slot_ui�� ȣ������
    //public void MouseUp(PointerEventData eventData) 
    //{
    //    if (eventData.pointerEnter != null && eventData.pointerEnter.GetComponent<Slot_UI>() != null)
    //    {
    //        upUI = eventData.pointerEnter.GetComponent<Slot_UI>();

    //        //�̹� Ŭ���� �������� ����
    //        if (moveSlot.Amount > 0)
    //        {
    //            //��Ŭ��
    //            if (eventData.button == PointerEventData.InputButton.Left)
    //            {
    //                //����� ��Ŭ���� ���δ� �ű�
    //                if (upUI.Slot.Amount <= 0)
    //                {
    //                    containerObject.SwapContainerSlot(upUI.Slot, moveUI.Slot);
    //                    moveUI.gameObject.SetActive(false);
    //                    StopCoroutine(moveUI.ContainerInterface.moveingUI);
    //                }
    //                //������� �������� ��Ŭ���� ���� �������̶�� ���δ� �ű�
    //                else if (upUI.Slot.Amount > 0 && upUI.Slot.GetItem.id == moveSlot.GetItem.id)
    //                {
    //                    upUI.Slot.AddAmount(moveSlot.Amount);
    //                    moveSlot.AddAmount(-moveSlot.Amount);
    //                    moveUI.gameObject.SetActive(false);
    //                    StopCoroutine(moveingUI);
    //                }
    //                //��������� �ʰ� ���� �����۵� �ƴ϶�� ������ ��ȯ
    //                else
    //                {
    //                    containerObject.SwapContainerSlot(moveUI.Slot, upUI.Slot);
    //                }
    //            }
    //            //��Ŭ��
    //            else if (eventData.button == PointerEventData.InputButton.Right)
    //            {
    //                //����� ��Ŭ���� �ϳ��� �ű��
    //                if (upUI.Slot.Amount <= 0)
    //                {
    //                    upUI.Slot.UpdateSlot(moveSlot.GetItem, 1);
    //                    moveSlot.AddAmount(-1);
    //                }
    //                //�̹� �������� �ִ°��� ��Ŭ���� ���� �������̶�� �ϳ��� �ű��
    //                else if (upUI.Slot.Amount > 0 && upUI.Slot.GetItem.id == moveSlot.GetItem.id)
    //                {
    //                    upUI.Slot.AddAmount(1);
    //                    moveSlot.AddAmount(-1);
    //                }
    //                //�� �ܿ��� �ƹ��ϵ� �Ͼ�� ���� (�̹� �������� �����鼭 ���� �����۵� �ƴ϶�� �ƹ��ϵ� ����)

    //                //���̻� ����ִ� �������� ���ٸ� ��Ȱ��ȭ
    //                if (moveSlot.Amount <= 0)
    //                {
    //                    moveUI.gameObject.SetActive(false);
    //                    StopCoroutine(moveingUI);
    //                }
    //            }
    //        }
    //        //�̹� Ŭ���� �������� ����
    //        else
    //        {
    //            //���� ����� �� Ŭ��
    //            if (downUI == upUI)
    //            {
    //                //�����Ѱ��� �������� �ִٸ�
    //                if (downUI.Slot.Amount > 0)
    //                {
    //                    //��Ŭ�� (����)
    //                    if (eventData.button == PointerEventData.InputButton.Left)
    //                    {
    //                        //ui Ȱ��ȭ
    //                        moveUI.gameObject.SetActive(true);
    //                        containerObject.SwapContainerSlot(downUI.Slot, moveUI.Slot);
    //                        moveingUI = StartCoroutine(MoveUIMoving());
    //                    }
    //                    //��Ŭ���� �ƹ��ϵ� �Ͼ�� ����
    //                }    
    //            }
    //        }
    //        downUI = null;
    //        upUI = null;
    //    }
    //}

    ////�������� ��� �巡���ϸ� �巡���� �κ��� ����Ͻ� 1/n�ϴ°ɷ�
    //public void MouseDrag(PointerEventData eventData)
    //{
    //    dragUI = eventData.pointerEnter.GetComponent<Slot_UI>();
    //    if (downUI != dragUI)
    //    {
    //        //moveui���� down�� drag ui���� 1/n�� ������ �ؾ���
    //    }
    //}

}
