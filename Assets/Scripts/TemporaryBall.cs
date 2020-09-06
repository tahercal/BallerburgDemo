using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class TemporaryBall : MonoBehaviour
{
    #region Const strings
    private const string ERROR_NOSPRITE = "No Sprite found on this GameObject";
    private const string ERROR_NORIGIDBODY2D = "No Rigidbody found on this GameObject";
    #endregion

    #region Variables
    public SpriteRenderer SpriteRenderer { get; private set; } = null;
    public Rigidbody2D Rigidbody2D { get; private set; } = null;
    #endregion

    #region Life cycle
    private void Awake()
    {
        SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if(!SpriteRenderer)
            Debug.LogError(ERROR_NOSPRITE, gameObject);

        Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        if (!SpriteRenderer)
            Debug.LogError(ERROR_NOSPRITE, gameObject);
    }
    #endregion
}
