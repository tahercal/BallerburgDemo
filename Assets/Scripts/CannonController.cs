using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("Parameters")]
    [Range(25,90)]
    public float angle;
    public float force;

    [Header("References")]
    [SerializeField]
    private Transform cannon_Body;
    [SerializeField]
    private Transform ball_SpawnPoint;

    [Header("Prefabs")]
    public GameObject cannonBall;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed");

            GameObject ballSpawned = Instantiate(cannonBall, ball_SpawnPoint);
            ballSpawned.GetComponent<BallMovement>().force = this.force;
            ballSpawned.GetComponent<BallMovement>().angle = this.angle;
        }
    }


    void FixedUpdate()
    {
        if (cannon_Body != null)
        {
            cannon_Body.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
