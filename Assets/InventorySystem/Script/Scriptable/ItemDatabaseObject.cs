using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//이 스크립트블오브젝트는 ScriptableObjectMenu( [MenuItem("InventorySystem/Database/DatabaseCreate")] )에서 만들 수 있도록 만들어져 있음
//이 데이터베이스는 만들어져있는 원본 아이템들을 가지고 있는 오브젝트임
//데이터베이스는 하나만 존재해야함
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
