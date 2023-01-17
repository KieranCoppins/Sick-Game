using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieAfterSeconds : MonoBehaviour
{
    [Tooltip("The number of seconds before this gameobject is destroyed")]
    [SerializeField] private float _seconds;

    public void StartSelfDestruct()
    {
        StartCoroutine(CountDown());
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(_seconds);
        Destroy(gameObject);
    }
}
