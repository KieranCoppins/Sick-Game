using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerHandler : MonoBehaviour
{
    [SerializeField] UnityEvent onEnter;
    [SerializeField] UnityEvent onStay;
    [SerializeField] UnityEvent onExit;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;
        onEnter.Invoke();
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;
        onExit.Invoke();
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;
        onStay.Invoke();
    }

}
