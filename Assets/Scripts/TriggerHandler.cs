using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerHandler : MonoBehaviour
{
    [SerializeField] private UnityEvent _onEnter;
    [SerializeField] private UnityEvent _onStay;
    [SerializeField] private UnityEvent _onExit;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;
        _onEnter.Invoke();
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;
        _onExit.Invoke();
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;
        _onStay.Invoke();
    }

}
