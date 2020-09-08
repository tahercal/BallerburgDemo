using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BallMovement : MonoBehaviour
{
    #region Const strings
    private const string ERROR_NORIGIDBODY = "No Rigidbody found on this GameObject";
    private const string ERROR_NOCIRCLECOLLIDER2D = "No CircleCollider2D found on this GameObject";
    #endregion

    //Cannon Responsability
    /*
    //Adaptation for scripts and access security
    [Header("Parameters")]
    public float force;
    public float angle;
    */

    public Rigidbody2D Rigidbody2D { get; private set; }
    public CircleCollider2D CircleCollider2D { get; private set; }

    //private float velocity;

    //Get variales
    private void Awake()
    {
        this.Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        if (!this.Rigidbody2D)
            Debug.LogError(ERROR_NORIGIDBODY);

        this.CircleCollider2D = gameObject.GetComponent<CircleCollider2D>();
        if (!this.CircleCollider2D)
            Debug.LogError(ERROR_NOCIRCLECOLLIDER2D);
    }

    //Cannon Responsability
    /*
    private void Start()
    {
        Rigidbody2D.AddForce(transform.right * force * Time.deltaTime, ForceMode2D.Impulse);
    }
    */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Ball triggered with an object, destroy ball");
        Destroy(gameObject);
    }
}
