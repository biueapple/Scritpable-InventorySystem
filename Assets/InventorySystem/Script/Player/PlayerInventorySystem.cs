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
        if(equipment != null)
        {
            for (int i = 0; i < equipment.storage.slots.Length; i++)
            {
                equipment.storage.slots[i].afterCallback += EquipmentSlotAfter;
                equipment.storage.slots[i].beforeCallback += EquipmentSlotBefore;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    AGILITY,        //민첩 re,ad
    INTELLECT,      //지능 mp,ap
    STAMINA,        //스태미나 hp,de
    STRNGTH         //힘 hp,ad
     */

    public void EquipmentSlotAfter(ContainerSlot _slot)
    {
        //스텟 올리기
        if(_slot.item.id < 0)
            return;
        for(int i = 0; i < _slot.item.buffs.Length; i++)
        {
            switch (_slot.item.buffs[i].attribute)
            {
                case ATTRIBUTES.AGILITY:
                    character.stat.RESISTANCE = _slot.item.buffs[i].value /*배율을 곱하자*/;
                    character.stat.AD = _slot.item.buffs[i].value /*배율을 곱하자*/;
                    break;
                case ATTRIBUTES.INTELLECT:
                    character.stat.MAXMP = _slot.item.buffs[i].value /*배율을 곱하자*/;
                    character.stat.AP = _slot.item.buffs[i].value /*배율을 곱하자*/;
                    break;
                case ATTRIBUTES.STAMINA:
                    character.stat.MAXHP = _slot.item.buffs[i].value /*배율을 곱하자*/;
                    character.stat.DEFENCE = _slot.item.buffs[i].value /*배율을 곱하자*/;
                    break;
                case ATTRIBUTES.STRNGTH:
                    character.stat.MAXHP = _slot.item.buffs[i].value /*배율을 곱하자*/;
                    character.stat.AD = _slot.item.buffs[i].value /*배율을 곱하자*/;
                    break;
                default:
                    break;
            }
        }
    }

    public void EquipmentSlotBefore(ContainerSlot _slot)
    {
        //스텟 내리기
        if (_slot.item.id < 0)
            return;
        for (int i = 0; i < _slot.item.buffs.Length; i++)
        {
            switch (_slot.item.buffs[i].attribute)
            {
                case ATTRIBUTES.AGILITY:
                    character.stat.RESISTANCE = -_slot.item.buffs[i].value /*배율을 곱하자*/;
                    character.stat.AD = -_slot.item.buffs[i].value /*배율을 곱하자*/;
                    break;
                case ATTRIBUTES.INTELLECT:
                    character.stat.MAXMP = -_slot.item.buffs[i].value /*배율을 곱하자*/;
                    character.stat.AP = -_slot.item.buffs[i].value /*배율을 곱하자*/;
                    break;
                case ATTRIBUTES.STAMINA:
                    character.stat.MAXHP = -_slot.item.buffs[i].value /*배율을 곱하자*/;
                    character.stat.DEFENCE = -_slot.item.buffs[i].value /*배율을 곱하자*/;
                    break;
                case ATTRIBUTES.STRNGTH:
                    character.stat.MAXHP = -_slot.item.buffs[i].value /*배율을 곱하자*/;
                    character.stat.AD = -_slot.item.buffs[i].value /*배율을 곱하자*/;
                    break;
                default:
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<GroundItem>() != null)
        {
            if(other.GetComponent<GroundItem>().item != null)
            {
                inventory.Acquired(other.GetComponent<GroundItem>().item.CreateItem(other.GetComponent<GroundItem>().item), 1);
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
                inventory.Acquired(collision.transform.GetComponent<GroundItem>().item.CreateItem(collision.transform.GetComponent<GroundItem>().item), 1);
                Destroy(collision.gameObject);
            }
        }
    }

    private void OnApplicationQuit()
    {
        inventory.Clear();
        if(equipment != null)
            equipment.Clear();
    }
}
