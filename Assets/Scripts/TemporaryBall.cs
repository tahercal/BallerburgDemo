using UnityEngine;

[RequireComponent(typeof(Sprite))]
[RequireComponent(typeof(Rigidbody))]
public class TemporaryBall : MonoBehaviour
{
    #region Const strings
    private const string ERROR_NOSPRITE = "No Sprite found on this GameObject";
    private const string ERROR_NORIGIDBODY = "No Rigidbody found on this GameObject";
    #endregion

    #region Variables
    public Sprite Sprite { get; private set; } = null;
    public Rigidbody Rigidbody { get; private set; } = null;
    #endregion

    #region Life cycle
    private void Awake()
    {
        Sprite = gameObject.GetComponent<Sprite>();
        if(!Sprite)
            Debug.LogError(ERROR_NOSPRITE, gameObject);

        Rigidbody = gameObject.GetComponent<Rigidbody>();
        if (!Sprite)
            Debug.LogError(ERROR_NOSPRITE, gameObject);
    }
    #endregion
}
