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

    private float velocity;

    private void Start()
    {
        rigidbody.AddForce(transform.right * force * Time.deltaTime, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Ball triggered with an object, destroy ball");
        Destroy(gameObject);
    }
}
