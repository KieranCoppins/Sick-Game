using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] float playerDistance;
    [Tooltip("Camera speed = player speed * speed modifier")]
    [Range(1.1f, 2f)]
    [SerializeField] float speedModifier;

    bool idle = true;

    Rigidbody2D playerRb;
    CharacterMovement characterMovement;
    Transform player;
    Camera camera;

    Vector3 CAMERAOFFSET = Vector3.zero;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        CAMERAOFFSET.z =  transform.position.z;
        playerRb = player.GetComponent<Rigidbody2D>();
        characterMovement = player.GetComponent<CharacterMovement>();
        camera = GetComponent<Camera>();
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
        while (Vector2.Distance(transform.position, player.transform.position) > 0.01)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position + CAMERAOFFSET, characterMovement.MovementSpeed * speedModifier * Time.deltaTime);
            yield return null;
        }

        StartCoroutine(SnapToPlayer());

        idle = true;
        yield break;
    }

    IEnumerator SnapToPlayer()
    {
        while (playerRb.velocity.magnitude != 0)
        {
            transform.position = player.transform.position + CAMERAOFFSET;
            yield return null;
        }
        yield break;
    }


    void MakePixelPerfect()
    {
        transform.position = new Vector3(RoundToNearestPixel(transform.position.x, camera), RoundToNearestPixel(transform.position.y, camera)) + CAMERAOFFSET;
    }

    public static float RoundToNearestPixel(float unityUnits, Camera viewingCamera)
    {
        float valueInPixels = (Screen.height / (viewingCamera.orthographicSize * 2)) * unityUnits;
        valueInPixels = Mathf.Round(valueInPixels);
        float adjustedUnityUnits = valueInPixels / (Screen.height / (viewingCamera.orthographicSize * 2));
        return adjustedUnityUnits;
    }
}
