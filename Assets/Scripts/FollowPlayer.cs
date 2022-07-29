using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;

    public float playerDistance;

    public float speed;

    Vector3 CAMERA_OFFSET = new(0, 0, -10);

    bool idle = true;

    void Start()
    {
        transform.position = player.transform.position + CAMERA_OFFSET;
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, player.transform.position) > playerDistance && idle)
        {
            idle = false;
            StartCoroutine(MoveToPlayer());
        }
    }

    IEnumerator MoveToPlayer()
    {
        while (Vector2.Distance(transform.position, player.transform.position) > 0.000001)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position + CAMERA_OFFSET, speed * Time.deltaTime);
            yield return null;
        }

        yield return SnapToPlayer();

        idle = true;
        yield break;
    }

    IEnumerator SnapToPlayer()
    {
        while (player.GetComponent<Rigidbody2D>().velocity.magnitude != 0)
        {
            transform.position = player.transform.position + CAMERA_OFFSET;
            yield return null;
        }

        yield break;
    }
}
