using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//아이템의 종류
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
//스텟의 종류 (hp,mp,de,re,ad,ap)
public enum ATTRIBUTES
{
    AGILITY,        //민첩 re,ad
    INTELLECT,      //지능 mp,ap
    STAMINA,        //스태미나 hp,de
    STRNGTH         //힘 hp,ad
}

//아이템의 등급
public enum RATING
{
    NONE,
    COMMON,
    RARE,
    UNIQUE,
    LEGEND
}

//아이템은 스크립트블 오브젝트로 원본이 하나씩만 만들어지고 스크립트 상에서 복사가 될것임
//아이템은 등급에 따라 가지고있는 버프의 갯수가 달라질거임
//이 스크립트블오브젝트는 ScriptableObjectMenu( [MenuItem("InventorySystem/Items/itemCreate")] )에서 만들 수 있도록 만들어져 있음
public class ItemObject : ScriptableObject
{
    public Item data;
    //아이템의 이미지
    public Sprite sprite;
    //아이템에 붙을 수 있는 스탯의 종류
    public Buff[] buffs;
    //아이템에 붙을 수 있는 등급
    public RATING[] ratings;

    //새로운 아이템을 만들고 나서 리턴해줌
    public Item CreateItem(ItemObject _itemObject)
    {
        Item newitem = new Item(data, _itemObject);
        return newitem;
    }
}

//새 아이템을 만들땐 반드시 CreateItem이것을 이용해야함
[System.Serializable]
public class Item
{
    public string name;
    public int id;

    //이 아이템의 확정적으로 붙은 등급
    public RATING rating;
    //이 아이템의 확정적으로 붙은 스탯
    public Buff[] buffs;
    public bool stackable;
    public ITEM_TYPE type;
    [TextArea(15, 20)]
    public string description;

    //원본을 만들때 씀
    public Item()
    {
        name = "";
        id = -1;
    }
    //이것은 원본을 통해 새로운 아이템을 만들때 쓰임
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

//아이템이 가지고 있는 스텟 == 버프
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
    //아이템이 올려주는 스텟의 수치
    public void SetValue()
    {
        value = Random.Range(min, max);
    }
}