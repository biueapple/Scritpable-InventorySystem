using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;



//�κ��丮�� �����̳ʷ� ��ü �κ��丮�� �ƴ϶� ���â�̳� ���ڶ��� �̰��� ���⶧��
public class ContainerObject : ScriptableObject
{
    //�������� ����� ���
    public string savePath;
    public ItemDatabaseObject database;
    public Container storage;

    //�÷��̾ �������� ȹ���ϸ� ȣ���� �Լ�
    public void Acquired(Item _item, int _amount)
    {
        ContainerSlot slot;
        //�������� ��ĥ �� ����
        if (_item.stackable)
        {
            //���� �������� �ִ��� Ȯ��
            slot = storage.GetSlot(_item);
            if(slot != null )
            {
                //������ ������ �߰�
                slot.AddAmount(_amount);
                return;
            }
            //���� �������� ���ٸ� ���� ä���ֱ�
            slot = storage.GetEmptySlot();
            if( slot != null )
            {
                slot.UpdateSlot(_item, _amount);
            }
        }
        //�������� ��ĥ �� ����
        else
        {
            //������� �ϳ��� �ֱ�
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

//���� �������� �����ϴ� ����
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

//�ϳ��� �������� �����ϴ� ����
[Serializable]
public class ContainerSlot
{
    //�� ���Կ� ���� �� �ִ� ����
    public ITEM_TYPE[] AllowedItems;
    //���� ������
    public Item item;
    //���� �������� ����
    public int amount;
    //�����ΰ� �ٲ�� ���� ȣ��
    [NonSerialized]
    public Action<ContainerSlot> afterCallback;
    //�����ΰ� �ٲ�� ���� ȣ��
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
        
        //���� ������ ���� �����̰ų� ��밡 �� �������̶�� ����
        if (AllowedItems.Length == 0 || _item.id < 0)
        {
            return true;
        }
            

        //���� ������ �ִµ� ��� �������� ���ǿ� �����ϸ� ����
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