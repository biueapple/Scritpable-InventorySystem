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
    RUNE,

}

//아이템의 등급
public enum RATING
{
    NONE,
    COMMON,
    UNCOMMON,
    RARE,
    UNIQUE,
    LEGEND
}

/// <summary>
/// 아이템이 가지고 있는 스텟
/// </summary>
[System.Serializable]
public class AttributeValue
{
    public ATTRIBUTES attribute;
    [SerializeField]
    private int value;
    public int Value { get { return value; } }
    public int min;
    public int max;

    public AttributeValue(ATTRIBUTES _attribute, int _min, int _max)
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

    public void AddStat(Stat stat)
    {
        switch (attribute)
        {
            case ATTRIBUTES.HP:
                stat.MAXHP = value;
                break;
            case ATTRIBUTES.MP:
                stat.MAXMP = value;
                break;
            case ATTRIBUTES.DEFENCE:
                stat.DEFENCE = value;
                break;
            case ATTRIBUTES.RESISTANCE:
                stat.RESISTANCE = value;
                break;
            case ATTRIBUTES.AD:
                stat.AD = value;
                break;
            case ATTRIBUTES.AP:
                stat.AP = value;
                break;
            case ATTRIBUTES.SPEED:
                stat.SPEED = value;
                break;
            case ATTRIBUTES.ATTACKSPEED:
                stat.AttackSpeed = value;
                break;
            case ATTRIBUTES.CRITICAL:
                stat.Critical = value;
                break;
            case ATTRIBUTES.CRITICALMULTIPLIER:
                stat.CriticalMultiplier = value;
                break;
        }
    }
    public void TakeStat(Stat stat)
    {
        switch (attribute)
        {
            case ATTRIBUTES.HP:
                stat.MAXHP = -value;
                break;
            case ATTRIBUTES.MP:
                stat.MAXMP = -value;
                break;
            case ATTRIBUTES.DEFENCE:
                stat.DEFENCE = -value;
                break;
            case ATTRIBUTES.RESISTANCE:
                stat.RESISTANCE = -value;
                break;
            case ATTRIBUTES.AD:
                stat.AD = -value;
                break;
            case ATTRIBUTES.AP:
                stat.AP = -value;
                break;
            case ATTRIBUTES.SPEED:
                stat.SPEED = -value;
                break;
            case ATTRIBUTES.ATTACKSPEED:
                stat.AttackSpeed = -value;
                break;
            case ATTRIBUTES.CRITICAL:
                stat.Critical = -value;
                break;
            case ATTRIBUTES.CRITICALMULTIPLIER:
                stat.CriticalMultiplier = -value;
                break;
        }
    }
}

//아이템은 스크립트블 오브젝트로 원본이 하나씩만 만들어지고 스크립트 상에서 복사가 될것임
//아이템은 등급에 따라 가지고있는 버프의 갯수가 달라질거임 (나중엔 등급에 따라 버프가 달려있는게 아니라 룬을 박는다던가 무기에는 공격스탯이 따로 달려있다던가) 
//이 스크립트블오브젝트는 ScriptableObjectMenu( [MenuItem("InventorySystem/Items/itemCreate")] )에서 만들 수 있도록 만들어져 있음
//[System.Serializable]를 하는 이유는 만든 파일의 이름을 수정한다던가 하면 안의 내용이 초기화 되는 문제가 생기기에 꼭 해야함
[System.Serializable]
public class ItemObject : ScriptableObject
{
    public Item data;
    //아이템의 이미지
    public Sprite sprite;
    [Header("이 아이템을 생성했을때 보유 가능한 스탯이나 등급")]
    //아이템에 붙을 수 있는 스탯의 종류
    public AttributeValue[] buffs;
    //아이템에 붙을 수 있는 등급
    public RATING[] ratings;

    //새로운 아이템을 만들고 나서 리턴해줌
    public Item CreateItem()
    {
        Item newitem = new Item(this);
        return newitem;
    }
}

//새 아이템을 만들땐 반드시 CreateItem이것을 이용해야함
[System.Serializable]
public class Item
{
    public string name;
    public int id;

    [SerializeField]
    //이 아이템의 확정적으로 붙은 등급
    protected RATING rating;
    public RATING Rating { get { return rating; } }
    //이 아이템의 확정적으로 붙은 스탯
    [SerializeField]
    protected AttributeValue[] attributeValues;
    public AttributeValue[] AttributeValues { get { return attributeValues; } } 
    //효과
    protected ItemEffect[] itemEffects;
    public ItemEffect[] ItemEffects { get { return itemEffects; } }
    //룬
    [SerializeField]
    protected Rune[] rune;
    public Rune[] GetRune { get { return rune; } }
    //한 슬롯에 여러개가 들어갈 수 있는지
    public bool stackable;
    public ITEM_TYPE type;
    //설명
    [TextArea(5, 10)]
    public string description;



    //원본을 만들때 씀 (ScriptableObject)
    public Item()
    {
        name = "";
        id = -1;
    }

    //이것은 원본을 통해 새로운 아이템을 만들때 쓰임
    //아이템을 획득 했을때 기본적으로 사용될 생성자
    public Item(ItemObject _itemObject)
    {
        //아이템의 기본정보 받는중
        name = _itemObject.data.name;
        id = _itemObject.data.id;
        type = _itemObject.data.type;
        //아이템의 등급 정하는중
        RatingDecision(_itemObject);

        //아이템이 장비아이템이거나 룬일경우 추가적인 작업
        if (type == ITEM_TYPE.RUNE)
        {
            RuneAttributeDecision(_itemObject);
        }
        else
        {
            EquipAttributeDecision();
            //아이템의 스탯 부여중
            StatDecision(_itemObject);
            //아이템의 효과 부여중
            ItemEffectDecision();
        }
    }

    //이것은 아이템의 등급을 정해놓고 획득하도록 만든 생성자
    public Item(ItemObject _itemObject, RATING r)
    {
        //아이템의 기본정보 받는중
        name = _itemObject.data.name;
        id = _itemObject.data.id;
        type = _itemObject.data.type;
        //아이템의 등급 정하는중
        rating = r;

        //아이템이 장비아이템이거나 룬일경우 추가적인 작업
        if (type == ITEM_TYPE.RUNE)
        {
            //룬인경우 룬 하나를 만들고 정보를 만듬
            RuneAttributeDecision(_itemObject);
        }
        else
        {
            //장비인경우 빈 룬칸을 만들어야 할 수 있음 (등급에 따라 룬을 넣을 수 있기에)
            EquipAttributeDecision();
            //아이템의 스탯 부여중
            StatDecision(_itemObject);
            //아이템의 효과 부여중
            ItemEffectDecision();
        }
    }


    public void RatingDecision(ItemObject _itemObject)
    {
        if (type == ITEM_TYPE.HELMET || type == ITEM_TYPE.TOP || type == ITEM_TYPE.BOTTOM ||
            type == ITEM_TYPE.BOOTS || type == ITEM_TYPE.WEAPON || type == ITEM_TYPE.SHIELD ||
            type == ITEM_TYPE.RUNE)
        {
            //장비에 ratings.Length == 0 인건 실수이니까 그냥 오류가 나도록 함 (여기서 오류가 나면 ratings.Length == 0이라서 나는 거니까 수정해야함)
            //if (_itemObject.ratings.Length > 0)
            //{
            rating = _itemObject.ratings[Random.Range(0, _itemObject.ratings.Length)];
            //}
        }
    }

    public void EquipAttributeDecision()
    {
        //attributeValues는 장비와 룬이 소유
        //itemEffects rune은 장비만 소유
        //생각해보니 룬이 능력이 없어야 하나?
        //등급이 없다 = 장비아이템이 아니다 = 스탯이 붙어있지 않다
        switch (rating)
        {
            case RATING.COMMON:
                attributeValues = new AttributeValue[2];
                itemEffects = new ItemEffect[1];
                rune = new Rune[0];
                break;
            case RATING.UNCOMMON:
                attributeValues = new AttributeValue[3];
                itemEffects = new ItemEffect[1];
                rune = new Rune[0];
                break;
            case RATING.RARE:
                attributeValues = new AttributeValue[2];
                itemEffects = new ItemEffect[0];
                rune = new Rune[Random.Range(3, 6)];
                break;
            case RATING.UNIQUE:
                attributeValues = new AttributeValue[1];
                itemEffects = new ItemEffect[2];
                rune = new Rune[Random.Range(3, 6)];
                break;
            case RATING.LEGEND:
                attributeValues = new AttributeValue[4];
                //고유효과
                itemEffects = new ItemEffect[0];
                rune = new Rune[0];
                break;
            default:
                attributeValues = new AttributeValue[0];
                itemEffects = new ItemEffect[0];
                rune = new Rune[0];
                break;
        }
        //아이템의 룬 공간 마련중 (나중에 껴서 넣는 방식이라 공간만 마련해놓자)
        for (int i = 0; i < rune.Length; i++)
        {
            rune[i] = new Rune();
        }
    }
    
    public void RuneAttributeDecision(ItemObject _itemObject)
    {
        switch (rating)
        {
            case RATING.COMMON:
                rune = new Rune[1];
                rune[0] = new Rune(_itemObject, 2, rating);
                itemEffects = new ItemEffect[0];

                break;
            case RATING.UNCOMMON:
                rune = new Rune[1];
                rune[0] = new Rune(_itemObject, 3, rating);
                itemEffects = new ItemEffect[0];

                break;
            case RATING.RARE:
                rune = new Rune[1];
                rune[0] = new Rune(_itemObject, 2, rating);
                itemEffects = new ItemEffect[0];
              
                break;
            case RATING.UNIQUE:
                rune = new Rune[1];
                rune[0] = new Rune(_itemObject, 1, rating);
                itemEffects = new ItemEffect[0];
                break;
            case RATING.LEGEND:
                rune = new Rune[1];
                rune[0] = new Rune(_itemObject, 4, rating);
                //고유효과
                break;
            default:
                rune = new Rune[1];
                rune[0] = new Rune();
                itemEffects = new ItemEffect[0];
                break;
        }
    }

    public void StatDecision(ItemObject _itemObject)
    {
        //여기서 오류가 나는것은 buffs를 정해주지 않았기 때문에 수정해야함 (장비거나 룬이면서 소유할 수 있는 스탯의 종류가 없기때문에 오류가 나는것)
        int index;
        for (int i = 0; i < attributeValues.Length; i++)
        {
            index = Random.Range(0, _itemObject.buffs.Length);
            attributeValues[i] = new AttributeValue(_itemObject.buffs[index].attribute, _itemObject.buffs[index].min, _itemObject.buffs[index].max);
        }
    }

    //수정필요 (존재하는 이펙트중에 하나가 아니라 종류별로 나눈 다음 하나를 리턴받아야 함)
    public void ItemEffectDecision()
    {
        for (int i = 0; i < itemEffects.Length; i++)
        {
            itemEffects[i] = ItemImpactManager.GetEffect();
        }
    }

    public bool InsertRune(Item _item)
    {
        for(int i = 0; i <  rune.Length; i++)
        {
            if (rune[i].AttributeValues.Length == 0)
            {
                return rune[i].InsertRune(_item);
            }
        }
        return false;
    }
}

///         스탯  효과  룬
///COMMON    2     1    x
///UNCOMMON  3     1    x 
///RARE      2     0    o     
///UNIQUE    1     2    o   
///LEGEND    4     1    x 
///
///룬은 3~5까지 랜덤으로 보유 가능
///룬도 등급이 있네
///룬은 스탯만 올려주는 느낌인가
///단어를 완성하면 추가 스탯
///생각해보니 룬도 아이템인데

[System.Serializable]
//등급과 문자는 랜덤으로 받고 스탯은 등급의 영향을 받는다
//룬에도 효과를 넣어줘야하는지 고민중
public class Rune
{
    [SerializeField]
    protected AttributeValue[] attributeValues;
    public AttributeValue[] AttributeValues { get { return attributeValues; } }
    [SerializeField]
    private char word;
    public char Word { get { return word; } }

    //장비 아이템이 자신의 룬공간을 만들때
    public Rune()
    {
        attributeValues = new AttributeValue[0];
    }
    //룬 아이템이 자신이 가진 룬을 설정할때
    public Rune(ItemObject _itemObject, int count, RATING r)
    {
        attributeValues = new AttributeValue[count];
        int index;
        for (int i = 0; i < count; i++)
        {
            index = Random.Range(0, _itemObject.buffs.Length);
            attributeValues[i] = new AttributeValue(_itemObject.buffs[index].attribute, _itemObject.buffs[index].min + (int)r, _itemObject.buffs[index].max + (int)r);
        }
        SetRandomCharacter();
    }

    public bool InsertRune(Item data)
    {
        if(data.type == ITEM_TYPE.RUNE)
        {
            attributeValues = new AttributeValue[data.AttributeValues.Length];
            for (int i = 0; i < attributeValues.Length; i++)
            {
                attributeValues[i] = new AttributeValue(data.AttributeValues[i].attribute, data.AttributeValues[i].Value, data.AttributeValues[i].Value);
            }
            return true;
        }
        return false;
    }

    public void SetRandomCharacter()
    {
        word = (char)Random.Range('A', 'Z' + 1);
    }
}