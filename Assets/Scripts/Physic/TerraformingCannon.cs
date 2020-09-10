
using TMPro;
using UnityEngine;

public class TerraformingCannon : BulletTerraformingPhysic
{
    #region Const strings
    private const string TEXT_DIE = "R.I.P";
    private const string TEXT_PLAYER = "Player";
    #endregion

    #region Variables
    [SerializeField] private TMP_Text dieText = null;
    [SerializeField] [Range(1f, 10f)] private float textTime = 2f;
    [SerializeField] [Range(0, 10000)] int alphaPixelsBeforeDie = 500;

    private CannonController _cannonController = null;
    #endregion

    #region Life Cycle
    protected override void Awake()
    {
        base.Awake();

        _cannonController = gameObject.GetComponentInParent<CannonController>();
    }

    protected override void Update()
    {
        base.Update();

        if (!_cannonController)
            return;

        if (this.AlphaPixels > alphaPixelsBeforeDie)
        {
            GameObject.Destroy(_cannonController.gameObject);
            if (dieText)
            {
                if (dieText.text != string.Empty)
                    dieText.text += "\n";

                dieText.text += TEXT_DIE + " " + TEXT_PLAYER + " " + _cannonController.name;
            }
        }

    }
    #endregion
}
