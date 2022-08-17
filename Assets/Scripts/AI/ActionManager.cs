using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public Queue<Action> actionQueue = new Queue<Action>();
    public List<IEnumerator> currentActions = new List<IEnumerator>();

    public delegate void OnFinishDelegate(IEnumerator coroutine);
    public event OnFinishDelegate onFinish;

    public bool executingActions { get; protected set; }

    private void Start()
    {
        executingActions = false;
        onFinish += delegate (IEnumerator coroutine)
        {
            // Remove this action from current actions
            currentActions.Remove(coroutine);

            // Check if we have any more actions in the current actions
            if (currentActions.Count == 0)
                executingActions = false;
        };
    }

    public void ScheduleAction(Action action)
    {
        Debug.Log(action);
        if (action != null)
            actionQueue.Enqueue(action);
    }

    public void Execute()
    {
        bool addActions = true;
        bool interruptorActive = false;
        bool currentActionsChanged = false;
        while (addActions && actionQueue.Count > 0)
        {
            // First we want to see if we have any interruptor actions
            foreach (Action a in actionQueue)
            {
                if (a.Interruptor)
                {
                    // If we have an interruptor clear all our actions and do this one
                    currentActions.Clear();
                    currentActions.Add(a.Execute());
                    actionQueue.Clear();
                    interruptorActive = true;
                    currentActionsChanged = true;
                    ExecuteActions();
                    break;
                }
            }

            if (interruptorActive)
                break;

            Action action = actionQueue.Dequeue();
            if (currentActions.Count > 0)
            {
                if (action.ASyncAction)
                {
                    currentActions.Add(action.Execute());
                    currentActionsChanged = true;
                }
                else
                {
                    addActions = false;
                }
            }
            else
            {
                currentActions.Add(action.Execute());
                currentActionsChanged = true;
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
