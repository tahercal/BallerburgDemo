using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Sprite))]
public class TerraformingPhysic : MonoBehaviour
{
    #region Const strings
    private const string ERROR_NOSPRITE = "No Sprite found on this GameObject";
    #endregion

    #region Variables
    [SerializeField][Range(0.1f, 100f)] private float forceAppliedPerPixel = 1f;

    private List<TemporaryBall> _currentBullets = new List<TemporaryBall>();
    private Sprite _sprite = null;
    #endregion

    #region Life cycle
    private void Awake()
    {
        if (!gameObject.TryGetComponent<Sprite>(out _sprite))
            Debug.LogError(ERROR_NOSPRITE, gameObject);
    }

    private void Update()
    {
        if(_sprite && _sprite.texture)
        {
            foreach(TemporaryBall temporaryBall in _currentBullets)
            {
                if (!temporaryBall)
                    continue;

                //Get hit pixels
                Vector2Int[] hitPixels = this.GetTerrainHitPixels(_sprite, temporaryBall);

                //Test
                foreach(Vector2Int vector2Int in hitPixels)
                {
                    Color pixelColor = _sprite.texture.GetPixel(vector2Int.x, vector2Int.y);
                    pixelColor.a = 0;
                    _sprite.texture.SetPixel(vector2Int.x, vector2Int.y, pixelColor);
                }
            }
        }

    }
    #endregion

    /// <summary>
    /// Get reference of the balls inside the collider
    /// </summary>
    /// <param name="collision"></param>
    #region Triggers
    private void OnTriggerEnter2D(Collider2D collision)
    {
        TemporaryBall triggeredBall = null;

        if (collision && collision.TryGetComponent<TemporaryBall>(out triggeredBall) && !_currentBullets.Contains(triggeredBall))
            _currentBullets.Add(triggeredBall);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        TemporaryBall triggeredBall = null;

        if (collision && collision.TryGetComponent<TemporaryBall>(out triggeredBall) && _currentBullets.Contains(triggeredBall))
            _currentBullets.Remove(triggeredBall);
    }
    #endregion

    #region Privates
    Vector2Int[] GetTerrainHitPixels(Sprite terrainSprite, TemporaryBall temporaryBall)
    {
        List<Vector2Int> hitPixels = new List<Vector2Int>();
        
        if (terrainSprite && terrainSprite.texture && temporaryBall != null && temporaryBall.Sprite != null && temporaryBall.Sprite.texture != null)
        {
            for (int i = 0; i < temporaryBall.Sprite.texture.width; i++)
            {
                for (int j = 0; j < temporaryBall.Sprite.texture.height; j++)
                {
                    if (temporaryBall.Sprite.texture.GetPixel(i, j).a == 0)
                        continue;

                    Vector2Int pixelPosition = new Vector2Int((int)temporaryBall.Sprite.rect.x + i, (int)temporaryBall.Sprite.rect.y + j);
                    if (terrainSprite)
                    {
                        if (pixelPosition.x > terrainSprite.rect.x && pixelPosition.x < (terrainSprite.rect.x + terrainSprite.rect.width) &&
                            pixelPosition.y > terrainSprite.rect.y && pixelPosition.y < (terrainSprite.rect.y + terrainSprite.rect.height))
                        {
                            hitPixels.Add(new Vector2Int(pixelPosition.x - (int)_sprite.rect.x, pixelPosition.y - (int)_sprite.rect.y));
                        }
                    }
                }
            }
        }

        return hitPixels.Count > 0 ? hitPixels.ToArray() : null;
    }
    #endregion
}
