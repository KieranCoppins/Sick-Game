using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private float _playerDistance;
    [Tooltip("Camera speed = player speed * speed modifier")]
    [Range(1.1f, 2f)]
    [SerializeField] private float _speedModifier;

    private bool _idle = true;

    private Rigidbody2D _playerRb;
    private CharacterMovement _characterMovement;
    private Transform _player;
    private Camera _Camera;

    private Vector3 CAMERAOFFSET = Vector3.zero;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        CAMERAOFFSET.z =  transform.position.z;
        _playerRb = _player.GetComponent<Rigidbody2D>();
        _characterMovement = _player.GetComponent<CharacterMovement>();
        _Camera = GetComponent<Camera>();
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, _player.transform.position) > _playerDistance && _idle)
        {
            _idle = false;
            StartCoroutine(MoveToPlayer());
        }
    }

    IEnumerator MoveToPlayer()
    {
        while (Vector2.Distance(transform.position, _player.transform.position) > 0.01)
        {
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position + CAMERAOFFSET, _characterMovement.MovementSpeed * _speedModifier * Time.deltaTime);
            yield return null;
        }

        StartCoroutine(SnapToPlayer());

        _idle = true;
        yield break;
    }

    IEnumerator SnapToPlayer()
    {
        while (_playerRb.velocity.magnitude != 0)
        {
            transform.position = _player.transform.position + CAMERAOFFSET;
            yield return null;
        }
        yield break;
    }


    void MakePixelPerfect()
    {
        transform.position = new Vector3(RoundToNearestPixel(transform.position.x, _Camera), RoundToNearestPixel(transform.position.y, _Camera)) + CAMERAOFFSET;
    }

    public static float RoundToNearestPixel(float unityUnits, Camera viewingCamera)
    {
        float valueInPixels = (Screen.height / (viewingCamera.orthographicSize * 2)) * unityUnits;
        valueInPixels = Mathf.Round(valueInPixels);
        float adjustedUnityUnits = valueInPixels / (Screen.height / (viewingCamera.orthographicSize * 2));
        return adjustedUnityUnits;
    }
}
