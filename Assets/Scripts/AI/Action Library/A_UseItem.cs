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

    public override string GetTitle()
    {
        try
        {
            return $"Use {item.name}";
        }
        catch
        {
            return "Use Item";
        }
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        try
        {
            nodeView.error = "";
            return $"The mob will use {item.name} if they have it in their inventory.";
        }
        catch (System.Exception e)
        {
            nodeView.error=e.Message;
            return "There was an issue with this node";
        }
    }
}
