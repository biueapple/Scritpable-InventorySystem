using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//�� ��ũ��Ʈ�������Ʈ�� ScriptableObjectMenu( [MenuItem("InventorySystem/Database/DatabaseCreate")] )���� ���� �� �ֵ��� ������� ����
//�� �����ͺ��̽��� ��������ִ� ���� �����۵��� ������ �ִ� ������Ʈ��
//�����ͺ��̽��� �ϳ��� �����ؾ���
public class ItemDatabaseObject : ScriptableObject
{
    public List<ItemObject> ItemObjects = new List<ItemObject>();

    public void ItemObjectAdd(ItemObject _itemObject)
    {
        if(ItemObjects == null)
            ItemObjects = new List<ItemObject>();
        ItemObjects.Add(_itemObject);
        _itemObject.data.id = ItemObjects.Count - 1;
        Debug.Log($"id : {ItemObjects.Count - 1}");
    }

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
