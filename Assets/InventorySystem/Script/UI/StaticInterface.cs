using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticInterface : ContainerInterface
{
    public Slot_UI[] slots;

    public override void CreateSlotsUI()
    {
        for(int i = 0; i < containerObject.storage.slots.Length; i++)
        {
            if(i >= slots.Length)
            {
                slotsInterface.Add(containerObject.storage.slots[i], null);
            }
            else
            {
                slots[i].slot = containerObject.storage.slots[i];
                slots[i].interfaece = this;
                slots[i].name = "slot_" + i;

                slotsInterface.Add(containerObject.storage.slots[i], slots[i]);
            }
        }
    }
}
