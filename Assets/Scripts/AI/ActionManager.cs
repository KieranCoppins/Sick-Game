using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager
{
    public Queue<Action> actionQueue = new Queue<Action>();
    public List<IEnumerator> currentActions = new List<IEnumerator>();

    public void ScheduleAction(Action action)
    {
        actionQueue.Enqueue(action);
    }

    public void Execute()
    {
        bool addActions = true;
        while (addActions)
        {
            Action action = actionQueue.Dequeue();
            if (currentActions.Count > 0)
            {
                if (action.ASyncAction)
                {
                    currentActions.Add(action.Execute());
                }
                else
                {
                    addActions = false;
                }
            }
            else
            {
                currentActions.Add(action.Execute());
            }
        }
    }
}
