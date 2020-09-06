using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TerraformingPhysic : MonoBehaviour
{
    #region Const strings
    private const string SHADERATTRIBUTE_TEXTURE = "_MainTex";

    private const string ERROR_NOSPRITERENDERER = "No SpriteRenderer found on this GameObject";
    private const string ERROR_NOSPRITE = "No Sprite found on this GameObject";
    private const string ERROR_NOTEXTUREID = "TextureID is empty, please add one";
    private const string ERROR_NOTEXTURE2D = "No Texture found on this GameObject's SpriteRenderer";
    #endregion

    #region Variables
    [SerializeField][Range(0.1f, 100f)] private float forceAppliedPerPixel = 1f;

    private List<TemporaryBall> _currentBullets = new List<TemporaryBall>();
    private SpriteRenderer _spriteRenderer = null;
    private Texture2D _modifiableTexture = null;
    #endregion

    #region Life cycle
    private void Awake()
    {
        if (gameObject.TryGetComponent<SpriteRenderer>(out _spriteRenderer))
        {
            if (_spriteRenderer.sprite != null)
            {

                if (_spriteRenderer.sprite.texture)
                {
                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                    _modifiableTexture = GameObject.Instantiate<Texture2D>(_spriteRenderer.sprite.texture);
                    block.SetTexture(SHADERATTRIBUTE_TEXTURE, _modifiableTexture);
                    _spriteRenderer.SetPropertyBlock(block);
                }
                else
                    Debug.LogError(ERROR_NOTEXTURE2D, gameObject);
            }
            else
                Debug.LogError(ERROR_NOSPRITE, gameObject);
        }
        else
            Debug.LogError(ERROR_NOSPRITERENDERER, gameObject);

        //TEXT
        if (_modifiableTexture)
        {

            for (int i = 0; i < _modifiableTexture.width; i++)
            {
                for (int j = 0; j < _modifiableTexture.height; j++)
                {
                    _modifiableTexture.SetPixel(i, j, Color.red);
                }
            }
            _modifiableTexture.Apply();
        }
        else
            Debug.LogError(ERROR_NOSPRITE, gameObject);
    }

    private void Update()
    {
        if(_spriteRenderer && _spriteRenderer.sprite && _spriteRenderer.sprite.texture)
        {

            foreach(TemporaryBall temporaryBall in _currentBullets)
            {
                if (!temporaryBall || !temporaryBall.SpriteRenderer || !temporaryBall.SpriteRenderer.sprite)
                    continue;

                //Get hit pixels
                Vector2Int[] hitPixels = this.GetTerrainHitPixels(_spriteRenderer.sprite, temporaryBall.SpriteRenderer.sprite);

                if(hitPixels == null)
                    continue;

                Debug.Log(hitPixels.Length);

                //Test
                foreach(Vector2Int vector2Int in hitPixels)
                {
                    Color pixelColor = _spriteRenderer.sprite.texture.GetPixel(vector2Int.x, vector2Int.y);
                    pixelColor.a = 0;
                    _spriteRenderer.sprite.texture.SetPixel(vector2Int.x, vector2Int.y, pixelColor);
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
    Vector2Int[] GetTerrainHitPixels(Sprite terrainSprite, Sprite temporaryBallSprite)
    {
        List<Vector2Int> hitPixels = new List<Vector2Int>();
        
        if (terrainSprite && terrainSprite.texture && temporaryBallSprite != null && temporaryBallSprite.texture != null)
        {
            for (int i = 0; i < temporaryBallSprite.texture.width; i++)
            {
                for (int j = 0; j < temporaryBallSprite.texture.height; j++)
                {
                    if (temporaryBallSprite.texture.GetPixel(i, j).a <= 0)
                        continue;

                    Vector2Int pixelPosition = new Vector2Int(Mathf.FloorToInt(temporaryBallSprite.rect.x) + i, Mathf.FloorToInt(temporaryBallSprite.rect.y) + j);
                    Debug.Log(pixelPosition);

                    if (terrainSprite)
                    {
                        if (pixelPosition.x > terrainSprite.rect.x && pixelPosition.x < (terrainSprite.rect.x + terrainSprite.rect.width) &&
                            pixelPosition.y > terrainSprite.rect.y && pixelPosition.y < (terrainSprite.rect.y + terrainSprite.rect.height))
                        {
                            pixelPosition = new Vector2Int(pixelPosition.x - Mathf.FloorToInt(terrainSprite.rect.x), pixelPosition.y - Mathf.FloorToInt(terrainSprite.rect.y));

                            if(terrainSprite.texture.GetPixel(pixelPosition.x, pixelPosition.y).a > 0)
                                hitPixels.Add(pixelPosition);
                        }
                    }
                }
            }
        }

        return hitPixels.Count > 0 ? hitPixels.ToArray() : null;
    }
    #endregion
}
