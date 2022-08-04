using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour
{

    public Transform player;

    public float maxDistance;

    Vector3 mousePos;

    Vector3 playerPos;

    Vector2 direction;

    void Update()
    {

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        playerPos = player.transform.position;

        direction = (mousePos - playerPos).normalized;

        Physics2D.Raycast(playerPos, direction, maxDistance);
        
        Debug.DrawRay(playerPos, direction);
    }
}
