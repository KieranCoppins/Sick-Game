using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_CanUseItem : F_Condition
{
    [SerializeField] InventoryItem item;
    public override bool Invoke() => mob.inventory.Has(item);
}
