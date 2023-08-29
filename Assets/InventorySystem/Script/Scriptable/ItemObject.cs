using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//�������� ����
public enum ITEM_TYPE
{
    DEFAULT,
    CONSUM,
    HELMET,
    TOP,
    BOTTOM,
    BOOTS,
    WEAPON,
    SHIELD,
}

//             2  1  1  1  2  1
//������ ���� (hp,mp,de,re,ad,ap)
public enum ATTRIBUTES
{
    AGILITY,        //��ø re,ad
    INTELLECT,      //���� mp,ap
    STAMINA,        //���¹̳� hp,de
    STRNGTH         //�� hp,ad
}

//�������� ���
public enum RATING
{
    NONE,
    COMMON,
    RARE,
    UNIQUE,
    LEGEND
}

//�������� ��ũ��Ʈ�� ������Ʈ�� ������ �ϳ����� ��������� ��ũ��Ʈ �󿡼� ���簡 �ɰ���
//�������� ��޿� ���� �������ִ� ������ ������ �޶�������
//�� ��ũ��Ʈ�������Ʈ�� ScriptableObjectMenu( [MenuItem("InventorySystem/Items/itemCreate")] )���� ���� �� �ֵ��� ������� ����
public class ItemObject : ScriptableObject
{
    public Item data;
    //�������� �̹���
    public Sprite sprite;
    //�����ۿ� ���� �� �ִ� ������ ����
    public Buff[] buffs;
    //�����ۿ� ���� �� �ִ� ���
    public RATING[] ratings;

    //���ο� �������� ����� ���� ��������
    public Item CreateItem(ItemObject _itemObject)
    {
        Item newitem = new Item(data, _itemObject);
        return newitem;
    }
}

//�� �������� ���鶩 �ݵ�� CreateItem�̰��� �̿��ؾ���
[System.Serializable]
public class Item
{
    public string name;
    public int id;

    //�� �������� Ȯ�������� ���� ���
    public RATING rating;
    //�� �������� Ȯ�������� ���� ����
    public Buff[] buffs;
    public bool stackable;
    public ITEM_TYPE type;
    [TextArea(15, 20)]
    public string description;

    //������ ���鶧 ��
    public Item()
    {
        name = "";
        id = -1;
    }
    //�̰��� ������ ���� ���ο� �������� ���鶧 ����
    public Item(Item data, ItemObject _itemObject)
    {
        name = data.name;
        id = data.id;
        type = data.type;

        if(_itemObject.ratings.Length > 0)
        {
            rating = _itemObject.ratings[Random.Range(1, _itemObject.ratings.Length)];
        }
       
        switch (rating)
        {
            case RATING.COMMON:
                buffs = new Buff[1];
                break;
            case RATING.RARE:
                buffs = new Buff[2];
                break;
            case RATING.UNIQUE:
                buffs = new Buff[3];
                break;
            case RATING.LEGEND:
                buffs = new Buff[4];
                break;
            default:
                buffs = new Buff[0];
                break;
        }
        int index;
        for (int i = 0; i < buffs.Length; i++)
        {
            index = Random.Range(0, _itemObject.buffs.Length);
            buffs[i] = new Buff(_itemObject.buffs[index].attribute, _itemObject.buffs[index].min, _itemObject.buffs[index].max);
        }
    }
}

//�������� ������ �ִ� ���� == ����
[System.Serializable]
public class Buff
{
    public ATTRIBUTES attribute;
    public int value;
    public int min;
    public int max;

    public Buff(ATTRIBUTES _attribute, int _min, int _max)
    {
        attribute = _attribute;
        min = _min;
        max = _max;
        SetValue();
    }
    //�������� �÷��ִ� ������ ��ġ
    public void SetValue()
    {
        value = Random.Range(min, max);
    }
}