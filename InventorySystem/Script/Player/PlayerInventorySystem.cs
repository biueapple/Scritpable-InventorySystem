using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventorySystem : MonoBehaviour
{
    public ContainerObject inventory;
    public ContainerObject equipment;
    public Unit character;

    // Start is called before the first frame update
    void Start()
    {
        //equipment�� ���â�̶� �������� ���� ���ö����� ������ �߰��ϰų� ������ �ϱ⿡
        if (equipment != null)
        {
            for (int i = 0; i < equipment.storage.slots.Length; i++)
            {
                equipment.storage.slots[i].afterCallback += EquipmentSlotAfter;
                //ó�� �����Ҷ� ��� �԰��ִٸ� ������ �÷�����ϴϱ�
                //EquipmentSlotBefore�� �ֱ� ���� �ϴ� ������ start�ϱ����� �̹� Container�� �������� �ִ� ���¶�
                //�̹� �ִ¸�ŭ ���� �ٽ� �־��ִ� ���̱⿡ EquipmentSlotBefore�� �ֱ� ���� �ϴ°�
                equipment.storage.slots[i].UpdateSlot();
                equipment.storage.slots[i].beforeCallback += EquipmentSlotBefore;
            }
        }
        for (int i = 0; i < inventory.storage.slots.Length; i++)
        {
            inventory.storage.slots[i].UpdateSlot();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void EquipmentSlotAfter(ContainerSlot _slot)
    {
        //���� �ø���
        // +=�� �ƴ϶� = x �� ������ stat�� getset�κп� �� �س��� ������
        if (_slot.GetItem.id < 0)
            return;
        //���� ��ĭ�� �پ��ִ� ������ �縸ŭ ������
        for (int i = 0; i < _slot.GetItem.AttributeValues.Length; i++)
        {
            _slot.GetItem.AttributeValues[i].AddStat(character.stat);
        }

        //���Կ� �鵵 �پ��ٸ� �鰹����ŭ �׸��� ��ȿ� �ִ� ������ ������ŭ ���������
        for(int i = 0; i < _slot.GetItem.GetRune.Length; i++)
        {
            for (int j = 0; j < _slot.GetItem.GetRune[i].AttributeValues.Length; j++)
            {
                _slot.GetItem.GetRune[i].AttributeValues[j].AddStat(character.stat);
            }
        }

        //�������� ȿ���� �ִµ� �����ۿ��� ȿ���� ���� ȿ���� ���� ������ �ֱ⶧���� ������ ȿ���� �����ͼ� ����
        for(int i = 0; i < _slot.GetItem.ItemEffects.Length; i++)
        {
            _slot.GetItem.ItemEffects[i] = ItemImpactManager.GetEffect(_slot.GetItem.ItemEffects[i]);
            _slot.GetItem.ItemEffects[i].Installation(character.stat);
        }
    }

    public void EquipmentSlotBefore(ContainerSlot _slot)
    {
        //���� ������
        // -= �� �ƴ϶� = -x �� ������ stat�� GetSet �κп� �� �س��� ������
        if (_slot.GetItem.id < 0)
            return;
        //�����ۿ� ���� ���� ������
        for (int i = 0; i < _slot.GetItem.AttributeValues.Length; i++)
        {
            _slot.GetItem.AttributeValues[i].TakeStat(character.stat);
        }
        //�����ۿ� ���� �鿡 ���� ���� ������
        for (int i = 0; i < _slot.GetItem.GetRune.Length; i++)
        {
            for (int j = 0; j < _slot.GetItem.GetRune[i].AttributeValues.Length; j++)
            {
                _slot.GetItem.GetRune[i].AttributeValues[j].TakeStat(character.stat);
            }
        }
        //�����ۿ� ���� ȿ�� ������
        for (int i = 0; i < _slot.GetItem.ItemEffects.Length; i++)
        {
            _slot.GetItem.ItemEffects[i].Uninstallation(character.stat);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<GroundItem>() != null)
        {
            if(other.GetComponent<GroundItem>().item != null)
            {
                inventory.Acquired(other.GetComponent<GroundItem>().item.CreateItem(), 1);
                Destroy(other.gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponent<GroundItem>() != null)
        {
            if (collision.transform.GetComponent<GroundItem>().item != null)
            {
                inventory.Acquired(collision.transform.GetComponent<GroundItem>().item.CreateItem(), 1);
                Destroy(collision.gameObject);
            }
        }
    }

    private void OnApplicationQuit()
    {
        //if (inventory != null)
        //    inventory.Clear();
        //if (equipment != null)
        //    equipment.Clear();
    }
}
