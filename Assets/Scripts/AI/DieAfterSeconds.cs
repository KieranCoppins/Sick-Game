using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieAfterSeconds : MonoBehaviour
{
    [Tooltip("The number of seconds before this gameobject is destroyed")]
    [SerializeField] float seconds;

    public void StartSelfDestruct()
    {
        StartCoroutine(CountDown());
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
