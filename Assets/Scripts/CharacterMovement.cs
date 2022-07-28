using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Character movement needs a rigidbody2D component
[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5f;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // We dont want gravity since technically down in unity is at the bottom of the screen
        rb.gravityScale = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Get our raw axis input so no gliding occures
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Create a vector from this and normalise it. Multiply it by the movementSpeed and use this as our velocity for the rigidbody
        rb.velocity = new Vector2(horizontal, vertical).normalized * movementSpeed;
    }
}
