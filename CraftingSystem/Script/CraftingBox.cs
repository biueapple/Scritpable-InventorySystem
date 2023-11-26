using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingBox : MonoBehaviour
{
    private ContainerInterface containerInterface;
    public Crafting craftingManager;
    public ItemDatabaseObject itemDatabaseObject;
    // Start is called before the first frame update
    void Start()
    {
        containerInterface = GetComponent<ContainerInterface>();
        for(int i = 0; i < 9; i++)
        {
            containerInterface.containerObject.storage.slots[i].afterCallback += CraftingCheck;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CraftingCheck(ContainerSlot slot)
    { 
        Table matrix = new Table3X3();
        int[,] ints = new int[3, 3]
        {
            { containerInterface.containerObject.storage.slots[0].GetItem.id ,containerInterface.containerObject.storage.slots[1].GetItem.id,containerInterface.containerObject.storage.slots[2].GetItem.id },
            { containerInterface.containerObject.storage.slots[3].GetItem.id,containerInterface.containerObject.storage.slots[4].GetItem.id,containerInterface.containerObject.storage.slots[5].GetItem.id },
            { containerInterface.containerObject.storage.slots[6].GetItem.id,containerInterface.containerObject.storage.slots[7].GetItem.id,containerInterface.containerObject.storage.slots[8].GetItem.id }
        };
        matrix.codes = ints;
        matrix.Slice(matrix, out matrix);
        int id;
        int count;
        craftingManager.Combination(matrix, out id, out count);

        if(itemDatabaseObject.GetItemObjectWithId(id) != null)
        {
            containerInterface.containerObject.storage.slots[9].UpdateSlot(itemDatabaseObject.GetItemObjectWithId(id).data, count);
            containerInterface.containerObject.storage.slots[9].beforeCallback += ResultOutput;
        }
        else if(itemDatabaseObject.GetItemObjectWithId(id) == null && containerInterface.containerObject.storage.slots[9].GetItem.id != -1)
        {
            containerInterface.containerObject.storage.slots[9].beforeCallback -= ResultOutput;
            containerInterface.containerObject.storage.slots[9].UpdateSlot(null, 0);
        }
    }

    public void ResultOutput(ContainerSlot slot)
    {
        Debug.Log("123");
        for (int i = 0; i < 9; i++)
        {
            if(containerInterface.containerObject.storage.slots[i].GetItem.id != -1)
                containerInterface.containerObject.storage.slots[i].AddAmount(-1);
        }
    }
}
