using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//�� ��ũ��Ʈ�������Ʈ�� ScriptableObjectMenu( [MenuItem("InventorySystem/Database/DatabaseCreate")] )���� ���� �� �ֵ��� ������� ����
//�� �����ͺ��̽��� ��������ִ� ���� �����۵��� ������ �ִ� ������Ʈ��
//�����ͺ��̽��� �ϳ��� �����ؾ���
public class ItemDatabaseObject : ScriptableObject
{
    //���� �����ϴ� ��� �����ۿ����� ���� ������
    [SerializeField]
    private List<ItemObject> ItemObjects = new List<ItemObject>();

    //���ο� �������� ����� ������ ȣ�� ScriptableObjectMenu���� �����
    public void ItemObjectAdd(ItemObject _itemObject)
    {
        if(ItemObjects == null)
            ItemObjects = new List<ItemObject>();
        ItemObjects.Add(_itemObject);
        _itemObject.data.id = ItemObjects.Count - 1;
    }

    //�����ۿ� ���� ������ id�� ������ ��������
    public ItemObject GetItemObjectWithId(int _id)
    {
        if(_id > ItemObjects.Count)
            return null;

        if (ItemObjects[_id].data.id == _id)
            return ItemObjects[_id];
        for (int i = 0; i < ItemObjects.Count; i++)
        {
            if(ItemObjects[i].data.id == _id)
                return ItemObjects[i];
        }
        return null;
    }
}
