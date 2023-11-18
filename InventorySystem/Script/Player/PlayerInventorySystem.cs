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
        //equipment는 장비창이라 아이템이 들어가고 나올때마다 스탯을 추가하거나 내려야 하기에
        if (equipment != null)
        {
            for (int i = 0; i < equipment.storage.slots.Length; i++)
            {
                equipment.storage.slots[i].afterCallback += EquipmentSlotAfter;
                //처음 시작할때 장비를 입고있다면 스탯을 올려줘야하니까
                //EquipmentSlotBefore를 넣기 전에 하는 이유는 start하기전에 이미 Container에 아이템이 있는 상태라
                //이미 있는만큼 빼고 다시 넣어주는 꼴이기에 EquipmentSlotBefore를 넣기 전에 하는것
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
        //스텟 올리기
        // +=이 아니라 = x 인 이유는 stat의 getset부분에 다 해놨기 때문에
        if (_slot.GetItem.id < 0)
            return;
        //슬롯 한칸에 붙어있는 스탯의 양만큼 더해줌
        for (int i = 0; i < _slot.GetItem.AttributeValues.Length; i++)
        {
            _slot.GetItem.AttributeValues[i].AddStat(character.stat);
        }

        //슬롯에 룬도 붙었다면 룬갯수만큼 그리고 룬안에 있는 스탯의 갯수만큼 더해줘야함
        for(int i = 0; i < _slot.GetItem.GetRune.Length; i++)
        {
            for (int j = 0; j < _slot.GetItem.GetRune[i].AttributeValues.Length; j++)
            {
                _slot.GetItem.GetRune[i].AttributeValues[j].AddStat(character.stat);
            }
        }

        //아이템의 효과를 넣는데 아이템에게 효과는 없고 효과에 대한 정보만 있기때문에 정보로 효과를 가져와서 넣음
        for(int i = 0; i < _slot.GetItem.ItemEffects.Length; i++)
        {
            _slot.GetItem.ItemEffects[i] = ItemImpactManager.GetEffect(_slot.GetItem.ItemEffects[i]);
            _slot.GetItem.ItemEffects[i].Installation(character.stat);
        }
    }

    public void EquipmentSlotBefore(ContainerSlot _slot)
    {
        //스텟 내리기
        // -= 이 아니라 = -x 인 이유는 stat의 GetSet 부분에 다 해놨기 때문에
        if (_slot.GetItem.id < 0)
            return;
        //아이템에 붙은 스탯 때는중
        for (int i = 0; i < _slot.GetItem.AttributeValues.Length; i++)
        {
            _slot.GetItem.AttributeValues[i].TakeStat(character.stat);
        }
        //아이템에 붙은 룬에 붙은 스탯 때는중
        for (int i = 0; i < _slot.GetItem.GetRune.Length; i++)
        {
            for (int j = 0; j < _slot.GetItem.GetRune[i].AttributeValues.Length; j++)
            {
                _slot.GetItem.GetRune[i].AttributeValues[j].TakeStat(character.stat);
            }
        }
        //아이템에 붙은 효과 때는중
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
