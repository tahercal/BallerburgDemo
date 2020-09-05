using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer ballSprite;
    public Rigidbody2D rigidbody;

    [Header("Parameters")]
    public float force;
    public float angle;


    private Vector2 direction;

    private void Start()
    {
        direction = new Vector2(0f, angle);
    }

    private void FixedUpdate()
    {
        rigidbody.AddForce(direction * Time.deltaTime, ForceMode2D.Impulse);
    }
}
