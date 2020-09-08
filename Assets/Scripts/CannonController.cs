using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CannonController : MonoBehaviour
{
    #region Const
    private const int MIN_ANGLE = -180;
    private const int MAX_ANGLE = 180;
    #endregion

    //Accessibility Restriction
    [Header("Controls")]
    [SerializeField] private KeyCode upKey = KeyCode.W;
    [SerializeField] private KeyCode downKey = KeyCode.S;
    [SerializeField] private KeyCode fireKey = KeyCode.D;

    [Header("Parameters")]
    [SerializeField] private Vector2 angleClamp = Vector2.zero;
    [SerializeField]
    private float angle;
    [SerializeField] [Range(0.1f, 100f)] float rotationSpeed = 1f;
    [SerializeField][Range(1f, 100f)]
    private float forceTimeMultiplicator;
    [SerializeField]
    [Range(1f, 100f)]
    private float maxHandledFireTime = 5f;

    [Header("References")]

    [SerializeField]
    private Transform cannon_Body;

    [SerializeField]
    private Transform ball_SpawnPoint;

    [Header("Prefabs")]
    [SerializeField]
    private BallMovement cannonBall = null;

    Quaternion _originalRotation = Quaternion.identity;
    private bool _upKeyDown = false;
    private bool _downKeyDown = false;
    private float _currentAngle;

    private bool _fireKeyDown = false;
    private float _startFireTime = 0;

    #region Life Cycle
    /// <summary>
    /// Check angle settings
    /// </summary>
    private void OnValidate()
    {
        angleClamp.x = Mathf.Clamp(angleClamp.x, MIN_ANGLE, MAX_ANGLE);
        angleClamp.y = Mathf.Clamp(angleClamp.y, MIN_ANGLE, MAX_ANGLE);

        if (angleClamp.x > angleClamp.y)
            angleClamp.x = angleClamp.y;

        angle = Mathf.Clamp(angle, angleClamp.x, angleClamp.y);
    }

    private void Awake()
    {
        _currentAngle = angle;

        if (cannon_Body)
            _originalRotation = cannon_Body.rotation;
    }

    private void Update()
    {
        if (!cannon_Body)
            return;

        this.Rotate();

        this.Fire();
    }
    #endregion

    #region Private
    private void Rotate()
    {
        //Security Check
        if (!cannon_Body)
            return;

        if (!_upKeyDown && Input.GetKeyDown(upKey))
            _upKeyDown = true;
        else if (_upKeyDown && Input.GetKeyUp(upKey))
            _upKeyDown = false;

        if (!_downKeyDown && Input.GetKeyDown(downKey))
            _downKeyDown = true;
        else if (_downKeyDown && Input.GetKeyUp(downKey))
            _downKeyDown = false;

        //Compute smooth rotation
        if (_upKeyDown)
            _currentAngle += rotationSpeed * Time.deltaTime;
        if (_downKeyDown)
            _currentAngle -= rotationSpeed * Time.deltaTime;

        _currentAngle = Mathf.Clamp(_currentAngle, angleClamp.x, angleClamp.y);
        this.CannonRotation(_currentAngle);
    }

    private void Fire()
    {
        if (!cannon_Body)
            return;


        if (cannonBall && ball_SpawnPoint) // Increase security
        {
            if (!_fireKeyDown && Input.GetKeyDown(fireKey))
            {
                _fireKeyDown = true;
                _startFireTime = Time.time;
            }
            else if (_fireKeyDown)
            {
                float firedTime = Time.time - _startFireTime;
                if ((firedTime > maxHandledFireTime) || Input.GetKeyUp(fireKey))
                {
                    BallMovement cannonBallInstance = Instantiate<BallMovement>(cannonBall, ball_SpawnPoint.position, transform.rotation);
                    if (cannonBallInstance.Rigidbody2D != null)
                        cannonBallInstance.Rigidbody2D.AddForce(cannon_Body.transform.right * forceTimeMultiplicator * (firedTime), ForceMode2D.Impulse);

                    _fireKeyDown = false;
                    /*
                    ballSpawned.GetComponent<BallMovement>().force = this.force;
                    ballSpawned.GetComponent<BallMovement>().angle = this.angle;
                */
                }
            }
                
        }
    }

    private void CannonRotation(float angle)
    {
        if (!cannon_Body)
            return;

        angle = Mathf.Clamp(angle, angleClamp.x, angleClamp.y);
        cannon_Body.rotation = Quaternion.Euler(cannon_Body.rotation * Vector3.forward * angle) * _originalRotation;
    }
    #endregion

    //Switch to update (not physic based)
    /*
    private void FixedUpdate() 
    {
        if (cannon_Body != null)
        {
            cannon_Body.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
    */
}
