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
    RUNE,

}

//�������� ���
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
/// �������� ������ �ִ� ����
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
    //�������� �÷��ִ� ������ ��ġ
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

//�������� ��ũ��Ʈ�� ������Ʈ�� ������ �ϳ����� ��������� ��ũ��Ʈ �󿡼� ���簡 �ɰ���
//�������� ��޿� ���� �������ִ� ������ ������ �޶������� (���߿� ��޿� ���� ������ �޷��ִ°� �ƴ϶� ���� �ڴ´ٴ��� ���⿡�� ���ݽ����� ���� �޷��ִٴ���) 
//�� ��ũ��Ʈ�������Ʈ�� ScriptableObjectMenu( [MenuItem("InventorySystem/Items/itemCreate")] )���� ���� �� �ֵ��� ������� ����
//[System.Serializable]�� �ϴ� ������ ���� ������ �̸��� �����Ѵٴ��� �ϸ� ���� ������ �ʱ�ȭ �Ǵ� ������ ����⿡ �� �ؾ���
[System.Serializable]
public class ItemObject : ScriptableObject
{
    public Item data;
    //�������� �̹���
    public Sprite sprite;
    [Header("�� �������� ���������� ���� ������ �����̳� ���")]
    //�����ۿ� ���� �� �ִ� ������ ����
    public AttributeValue[] buffs;
    //�����ۿ� ���� �� �ִ� ���
    public RATING[] ratings;

    //���ο� �������� ����� ���� ��������
    public Item CreateItem()
    {
        Item newitem = new Item(this);
        return newitem;
    }
}

//�� �������� ���鶩 �ݵ�� CreateItem�̰��� �̿��ؾ���
[System.Serializable]
public class Item
{
    public string name;
    public int id;

    [SerializeField]
    //�� �������� Ȯ�������� ���� ���
    protected RATING rating;
    public RATING Rating { get { return rating; } }
    //�� �������� Ȯ�������� ���� ����
    [SerializeField]
    protected AttributeValue[] attributeValues;
    public AttributeValue[] AttributeValues { get { return attributeValues; } } 
    //ȿ��
    protected ItemEffect[] itemEffects;
    public ItemEffect[] ItemEffects { get { return itemEffects; } }
    //��
    [SerializeField]
    protected Rune[] rune;
    public Rune[] GetRune { get { return rune; } }
    //�� ���Կ� �������� �� �� �ִ���
    public bool stackable;
    public ITEM_TYPE type;
    //����
    [TextArea(5, 10)]
    public string description;



    //������ ���鶧 �� (ScriptableObject)
    public Item()
    {
        name = "";
        id = -1;
    }

    //�̰��� ������ ���� ���ο� �������� ���鶧 ����
    //�������� ȹ�� ������ �⺻������ ���� ������
    public Item(ItemObject _itemObject)
    {
        //�������� �⺻���� �޴���
        name = _itemObject.data.name;
        id = _itemObject.data.id;
        type = _itemObject.data.type;
        //�������� ��� ���ϴ���
        RatingDecision(_itemObject);

        //�������� ���������̰ų� ���ϰ�� �߰����� �۾�
        if (type == ITEM_TYPE.RUNE)
        {
            RuneAttributeDecision(_itemObject);
        }
        else
        {
            EquipAttributeDecision();
            //�������� ���� �ο���
            StatDecision(_itemObject);
            //�������� ȿ�� �ο���
            ItemEffectDecision();
        }
    }

    //�̰��� �������� ����� ���س��� ȹ���ϵ��� ���� ������
    public Item(ItemObject _itemObject, RATING r)
    {
        //�������� �⺻���� �޴���
        name = _itemObject.data.name;
        id = _itemObject.data.id;
        type = _itemObject.data.type;
        //�������� ��� ���ϴ���
        rating = r;

        //�������� ���������̰ų� ���ϰ�� �߰����� �۾�
        if (type == ITEM_TYPE.RUNE)
        {
            //���ΰ�� �� �ϳ��� ����� ������ ����
            RuneAttributeDecision(_itemObject);
        }
        else
        {
            //����ΰ�� �� ��ĭ�� ������ �� �� ���� (��޿� ���� ���� ���� �� �ֱ⿡)
            EquipAttributeDecision();
            //�������� ���� �ο���
            StatDecision(_itemObject);
            //�������� ȿ�� �ο���
            ItemEffectDecision();
        }
    }


    public void RatingDecision(ItemObject _itemObject)
    {
        if (type == ITEM_TYPE.HELMET || type == ITEM_TYPE.TOP || type == ITEM_TYPE.BOTTOM ||
            type == ITEM_TYPE.BOOTS || type == ITEM_TYPE.WEAPON || type == ITEM_TYPE.SHIELD ||
            type == ITEM_TYPE.RUNE)
        {
            //��� ratings.Length == 0 �ΰ� �Ǽ��̴ϱ� �׳� ������ ������ �� (���⼭ ������ ���� ratings.Length == 0�̶� ���� �Ŵϱ� �����ؾ���)
            //if (_itemObject.ratings.Length > 0)
            //{
            rating = _itemObject.ratings[Random.Range(0, _itemObject.ratings.Length)];
            //}
        }
    }

    public void EquipAttributeDecision()
    {
        //attributeValues�� ���� ���� ����
        //itemEffects rune�� ��� ����
        //�����غ��� ���� �ɷ��� ����� �ϳ�?
        //����� ���� = ���������� �ƴϴ� = ������ �پ����� �ʴ�
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
                //����ȿ��
                itemEffects = new ItemEffect[0];
                rune = new Rune[0];
                break;
            default:
                attributeValues = new AttributeValue[0];
                itemEffects = new ItemEffect[0];
                rune = new Rune[0];
                break;
        }
        //�������� �� ���� ������ (���߿� ���� �ִ� ����̶� ������ �����س���)
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
                //����ȿ��
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
        //���⼭ ������ ���°��� buffs�� �������� �ʾұ� ������ �����ؾ��� (���ų� ���̸鼭 ������ �� �ִ� ������ ������ ���⶧���� ������ ���°�)
        int index;
        for (int i = 0; i < attributeValues.Length; i++)
        {
            index = Random.Range(0, _itemObject.buffs.Length);
            attributeValues[i] = new AttributeValue(_itemObject.buffs[index].attribute, _itemObject.buffs[index].min, _itemObject.buffs[index].max);
        }
    }

    //�����ʿ� (�����ϴ� ����Ʈ�߿� �ϳ��� �ƴ϶� �������� ���� ���� �ϳ��� ���Ϲ޾ƾ� ��)
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

///         ����  ȿ��  ��
///COMMON    2     1    x
///UNCOMMON  3     1    x 
///RARE      2     0    o     
///UNIQUE    1     2    o   
///LEGEND    4     1    x 
///
///���� 3~5���� �������� ���� ����
///�鵵 ����� �ֳ�
///���� ���ȸ� �÷��ִ� �����ΰ�
///�ܾ �ϼ��ϸ� �߰� ����
///�����غ��� �鵵 �������ε�

[System.Serializable]
//��ް� ���ڴ� �������� �ް� ������ ����� ������ �޴´�
//�鿡�� ȿ���� �־�����ϴ��� �����
public class Rune
{
    [SerializeField]
    protected AttributeValue[] attributeValues;
    public AttributeValue[] AttributeValues { get { return attributeValues; } }
    [SerializeField]
    private char word;
    public char Word { get { return word; } }

    //��� �������� �ڽ��� ������� ���鶧
    public Rune()
    {
        attributeValues = new AttributeValue[0];
    }
    //�� �������� �ڽ��� ���� ���� �����Ҷ�
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