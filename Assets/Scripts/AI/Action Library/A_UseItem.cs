using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_UseItem : Action
{
    [SerializeField] InventoryItem item;

    public A_UseItem() : base()
    {
        Flags |= ActionFlags.Interruptor;
    }

    public override IEnumerator Execute()
    {
        // TODO: use some kind of animation here
        mob.inventory.Use(item);
        yield return new WaitForSeconds(0.1f);
        yield return null;
    }
}
