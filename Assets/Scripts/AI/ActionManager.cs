using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public Queue<ActionPacket> actionQueue = new Queue<ActionPacket>();
    public List<IEnumerator> currentActions = new List<IEnumerator>();

    public delegate void OnFinishDelegate(IEnumerator coroutine);
    public event OnFinishDelegate onFinish;

    public bool executingActions { get; protected set; }

    bool waitForActions = false;

    private void Start()
    {
        executingActions = false;
        onFinish += delegate (IEnumerator coroutine)
        {
            // Remove this action from current actions
            currentActions.Remove(coroutine);

            // Check if we have any more actions in the current actions
            if (currentActions.Count == 0)
            {
                waitForActions = false;
                executingActions = false;
            }
        };
    }

    /// <summary>
    /// Schedule an action to be executed inside the action manager
    /// </summary>
    /// <param name="action"></param>
    public void ScheduleAction(Action action)
    {
        // We need to check if our action is already in our queue
        foreach (ActionPacket a in actionQueue)
        {
            if (a.action == action)
                return;
        }

        if (action != null)
            actionQueue.Enqueue(new ActionPacket(action));
    }

    public void Execute()
    {
        bool currentActionsChanged = false;
        bool acceptASyncActions = false;

        List<ActionPacket> tempList = new List<ActionPacket>(actionQueue);

        // Remove any expired actions
        foreach (ActionPacket a in actionQueue)
        {
            if (Time.time - a.time > 2.0f)
            {
                tempList.Remove(a);
            }
        }

        actionQueue = new Queue<ActionPacket>(tempList);

        if (waitForActions)
            return;

        // First we want to see if we have any interruptor actions
        foreach (ActionPacket a in actionQueue)
        {
            if (a.action.Interruptor)
            {
                tempList = new List<ActionPacket>(actionQueue);
                // If we have an interruptor clear all our actions and do this one
                currentActions.Clear();
                currentActions.Add(a.action.Execute());
                tempList.Remove(a);
                actionQueue = new Queue<ActionPacket>(tempList);
                currentActionsChanged = true;
                acceptASyncActions = a.action.ASyncAction;
                waitForActions = !a.action.Interruptable;
                break;
            }
        }

        while (actionQueue.Count > 0)
        {
            if (currentActions.Count > 0)
            {
                Action action = actionQueue.Peek().action;
                if (action.ASyncAction && acceptASyncActions)
                {
                    currentActions.Add(actionQueue.Dequeue().action.Execute());
                    currentActionsChanged = true;
                    waitForActions = !action.Interruptable;
                }
                else
                    break;
            }
            else
            {
                Action action = actionQueue.Dequeue().action;
                currentActions.Add(action.Execute());
                currentActionsChanged = true;
                acceptASyncActions = action.ASyncAction;
                waitForActions = !action.Interruptable;
            }
        }
        if (currentActionsChanged)
            ExecuteActions();
    }


    protected void ExecuteActions()
    {
        StopAllCoroutines();    // they should already be stopped unless there is an interruptor
        // Execute all actions in current actions
        foreach (IEnumerator action in currentActions)
        {
            executingActions = true;
            StartCoroutine(ActionWrapper(action));
        }
    }

    IEnumerator ActionWrapper(IEnumerator coroutine)
    {
        bool running = true;
        IEnumerator e = coroutine;
        while (running)
        {
            if (e != null && e.MoveNext())
                yield return e.Current;
            else
                running = false;
        }
        OnFinishDelegate handler = onFinish;
        if (handler != null)
            handler(coroutine);
    }
}

public struct ActionPacket
{
    public readonly Action action;
    public readonly float time;

    public ActionPacket(Action action)
    {
        this.action = action;
        time = Time.time;
    }
}
