using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class TemporaryCircleBullet : MonoBehaviour
{
    #region Const strings
    private const string ERROR_NOCOLLIDER = "No CircleCollider2D found on this GameObject";
    private const string ERROR_NORIGIDBODY2D = "No Rigidbody found on this GameObject";
    #endregion

    #region Variables
    public CircleCollider2D CircleCollider2D { get; private set; } = null;
    public Rigidbody2D Rigidbody2D { get; private set; } = null;
    #endregion

    #region Life cycle
    private void Awake()
    {
        CircleCollider2D = gameObject.GetComponent<CircleCollider2D>();
        if(!CircleCollider2D)
            Debug.LogError(ERROR_NOCOLLIDER, gameObject);

        Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        if (!Rigidbody2D)
            Debug.LogError(ERROR_NORIGIDBODY2D, gameObject);
    }
    #endregion
}
