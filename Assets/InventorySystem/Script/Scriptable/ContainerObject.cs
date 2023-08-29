using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;



//인벤토리를 컨테이너로 대체 인벤토리만 아니라 장비창이나 상자또한 이것을 쓰기때문
public class ContainerObject : ScriptableObject
{
    //아이템이 저장될 경로
    public string savePath;
    public ItemDatabaseObject database;
    public Container storage;

    //플레이어가 아이템을 획득하면 호출할 함수
    public void Acquired(Item _item, int _amount)
    {
        ContainerSlot slot;
        //아이템이 겹칠 수 있음
        if (_item.stackable)
        {
            //같은 아이템이 있는지 확인
            slot = storage.GetSlot(_item);
            if(slot != null )
            {
                //있으면 갯수만 추가
                slot.AddAmount(_amount);
                return;
            }
            //같은 아이템이 없다면 새로 채워넣기
            slot = storage.GetEmptySlot();
            if( slot != null )
            {
                slot.UpdateSlot(_item, _amount);
            }
        }
        //아이템이 겹칠 수 없음
        else
        {
            //빈공간에 하나씩 넣기
            for(int i = 0; i < _amount; i++)
            {
                slot = storage.GetEmptySlot();
                if( slot != null )
                {
                    slot.UpdateSlot(_item, 1);
                    return;
                }
            }
        }
    }

    public void SwapContainerSlot(ContainerSlot _slot1, ContainerSlot _slot2)
    {
        if (_slot1.CanPlace(_slot2.item) && _slot2.CanPlace(_slot1.item))
        {
            ContainerSlot temp = new ContainerSlot(_slot2.item, _slot2.amount);
            _slot2.UpdateSlot(_slot1.item, _slot1.amount);
            _slot1.UpdateSlot(temp.item, temp.amount);
        }
    }




    [ContextMenu("Save")]
    public void Save()
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, storage);
        stream.Close();
    }

    [ContextMenu("Load")]
    public void Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            Container newContainer = (Container)formatter.Deserialize(stream);
            for (int i = 0; i < storage.slots.Length; i++)
            {
                storage.slots[i].UpdateSlot(newContainer.slots[i].item, newContainer.slots[i].amount);
            }
            stream.Close();
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        for (int i = 0; i < storage.slots.Length; i++)
        {
            storage.slots[i].afterCallback = null;
            storage.slots[i].beforeCallback = null;
            storage.slots[i].UpdateSlot(new Item(), 0);
        }
    }
}

//여러 아이템을 저장하는 공간
[Serializable]
public class Container
{
    public ContainerSlot[] slots;
    public ContainerSlot GetSlot(Item _item)
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item.id ==  _item.id)
                return slots[i];
        }
        return null;
    }
    public ContainerSlot GetEmptySlot()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].amount == 0 || slots[i].item.id == -1)
            {
                return slots[i];
            }
        }
        return null;
    }
}

//하나의 아이템을 저장하는 공간
[Serializable]
public class ContainerSlot
{
    //이 슬롯에 들어올 수 있는 종류
    public ITEM_TYPE[] AllowedItems;
    //현재 아이템
    public Item item;
    //현재 아이템의 갯수
    public int amount;
    //무엇인가 바뀌고 나서 호출
    [NonSerialized]
    public Action<ContainerSlot> afterCallback;
    //무엇인가 바뀌기 전에 호출
    [NonSerialized]
    public Action<ContainerSlot> beforeCallback;

    public ContainerSlot()
    {
        UpdateSlot(new Item(), 0);
    }

    public ContainerSlot(Item _item, int _amount)
    {
        UpdateSlot(_item, _amount);
    }

    public void UpdateSlot(Item _item, int _amount)
    {
        if (beforeCallback != null)
            beforeCallback(this);

        amount = _amount;
        if (amount > 0)
            item = _item;
        else
            item = new Item();

        if (afterCallback != null)
            afterCallback(this);
    }

    public void UpdateSlot()
    {
        UpdateSlot(item, amount);   
    }

    public void AddAmount(int value)
    {
        UpdateSlot(item, amount + value);
    }

    public bool CanPlace(Item _item)
    {
        
        //내가 조건이 없는 슬롯이거나 상대가 빈 아이템이라면 가능
        if (AllowedItems.Length == 0 || _item.id < 0)
        {
            return true;
        }
            

        //내가 조건이 있는데 상대 아이템이 조건에 부합하면 가능
        for(int i = 0; i < AllowedItems.Length; i++)
        {
            if (AllowedItems[i] == _item.type)
            {
                return true;
            }
        }

        return false;
    }
}