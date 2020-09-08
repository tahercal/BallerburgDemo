using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    //Accessibility Restriction
    [Header("Parameters")]
    [Range(25,90)]
    [SerializeField]
    private float angle;
    [SerializeField]
    private float force;

    [Header("References")]

    [SerializeField]
    private Transform cannon_Body;

    [SerializeField]
    private Transform ball_SpawnPoint;

    [Header("Prefabs")]
    [SerializeField]
    private BallMovement cannonBall = null;


    private void Update()
    {
        //Security Check
        if (!cannon_Body)
            return;

        cannon_Body.rotation = Quaternion.Euler(0f, 0f, angle);

        if (cannonBall && ball_SpawnPoint &&  Input.GetKeyDown(KeyCode.Space)) // Increase security
        {
            //Debug.Log("Space pressed");

            BallMovement cannonBallInstance = Instantiate<BallMovement>(cannonBall, ball_SpawnPoint);
            if (cannonBallInstance.Rigidbody2D != null)
                cannonBallInstance.Rigidbody2D.AddForce(cannon_Body.transform.right * force, ForceMode2D.Impulse);

            /*
            ballSpawned.GetComponent<BallMovement>().force = this.force;
            ballSpawned.GetComponent<BallMovement>().angle = this.angle;
        */
        }
    }


    //Switch to update (not physic based)
    /*
    private void Update() 
    {
        if (cannon_Body != null)
        {
            cannon_Body.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
    */
}
