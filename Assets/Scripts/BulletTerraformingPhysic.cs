using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BulletTerraformingPhysic : MonoBehaviour
{
    #region Const strings
    private const string SHADERATTRIBUTE_TEXTURE = "_MainTex";

    private const string WARNING_INVALIDSCALE = "The current sprite lossy scale is not valid, please change it to 1,1,1";

    private const string ERROR_NOSPRITERENDERER = "No SpriteRenderer found on this GameObject";
    private const string ERROR_NOSPRITE = "No Sprite found on this GameObject";
    private const string ERROR_NOTEXTURE2D = "No Texture found on this GameObject's SpriteRenderer";
    #endregion

    #region Structures
    private struct PixelCoor
    {
        public int x;
        public int y;

        public PixelCoor(int xCoor, int yCoor)
        {
            x = xCoor;
            y = yCoor;
        }
    }
    #endregion

    #region Variables
    [SerializeField] private Color terrainColor = Color.black;
    [SerializeField][Range(0.1f, 100f)] private float forceAppliedPerPixel = 1f;

    private List<TemporaryCircleBullet> _currentBullets = new List<TemporaryCircleBullet>();
    private SpriteRenderer _spriteRenderer = null;
    private Texture2D _modifiableTexture = null;
    #endregion

    #region Life cycle
    private void Awake()
    {
        if (transform.lossyScale != Vector3.one)
            Debug.LogWarning(WARNING_INVALIDSCALE, gameObject);

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

                    //Set Color
                    for(int i = 0; i < _modifiableTexture.width; i++)
                    {
                        for (int j = 0; j < _modifiableTexture.height; j++)
                            _modifiableTexture.SetPixel(i, j, terrainColor);
                    }
                    _modifiableTexture.Apply();
                }
                else
                    Debug.LogError(ERROR_NOTEXTURE2D, gameObject);
            }
            else
                Debug.LogError(ERROR_NOSPRITE, gameObject);
        }
        else
            Debug.LogError(ERROR_NOSPRITERENDERER, gameObject);
    }

    private void Update()
    {
        if (_currentBullets.Count == 0 || !_spriteRenderer || !_spriteRenderer.sprite || !_modifiableTexture)
            return;

        foreach (TemporaryCircleBullet temporaryCircleBullet in _currentBullets)
        {
            if (!temporaryCircleBullet || !temporaryCircleBullet.CircleCollider2D)
                continue;

            //Get hit pixels
            PixelCoor[] hitPixels = this.GetTerrainHitPixels(_spriteRenderer.sprite, _modifiableTexture, temporaryCircleBullet.CircleCollider2D);

            if (hitPixels != null)
            {
                foreach (PixelCoor pixelCoor in hitPixels)
                {
                    Color pixelColor = _modifiableTexture.GetPixel(pixelCoor.x, pixelCoor.y);
                    pixelColor.a = 0;
                    _modifiableTexture.SetPixel(pixelCoor.x, pixelCoor.y, pixelColor);
                }

                _modifiableTexture.Apply();
            }
        }

    }
    #endregion

    /// <summary>
    /// Get reference of the CircleBullet inside the collider
    /// </summary>
    /// <param name="collision"></param>
    #region Triggers
    private void OnTriggerEnter2D(Collider2D collision)
    {
        TemporaryCircleBullet triggeredCircleBullet = null;

        if (collision && collision.TryGetComponent<TemporaryCircleBullet>(out triggeredCircleBullet) && !_currentBullets.Contains(triggeredCircleBullet))
            _currentBullets.Add(triggeredCircleBullet);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        TemporaryCircleBullet triggeredCircleBullet = null;

        if (collision && collision.TryGetComponent<TemporaryCircleBullet>(out triggeredCircleBullet) && _currentBullets.Contains(triggeredCircleBullet))
            _currentBullets.Remove(triggeredCircleBullet);
    }
    #endregion

    #region Privates
    PixelCoor[] GetTerrainHitPixels(Sprite terrainSprite, Texture2D terrainTexture2D, CircleCollider2D circleCollider2D)
    {
        List<PixelCoor> hitPixels = new List<PixelCoor>();
        
        if (terrainSprite && terrainTexture2D && circleCollider2D)
        {
            float maxDistancePixelCircleBulletCollider = circleCollider2D.radius * Mathf.Max(circleCollider2D.transform.localScale.x, circleCollider2D.transform.localScale.y);

            //Set local zone
            Vector2 circleBulletRadius = new Vector2(maxDistancePixelCircleBulletCollider, maxDistancePixelCircleBulletCollider);
            
            Vector2 minCircleBulletRadiusLimitInTerrain = (((Vector2)transform.InverseTransformPoint(circleCollider2D.transform.position) - circleBulletRadius) * terrainSprite.pixelsPerUnit) + terrainSprite.pivot;
            minCircleBulletRadiusLimitInTerrain.x = Mathf.Clamp(minCircleBulletRadiusLimitInTerrain.x, 0, terrainTexture2D.width);
            minCircleBulletRadiusLimitInTerrain.y = Mathf.Clamp(minCircleBulletRadiusLimitInTerrain.y, 0, terrainTexture2D.height);

            Vector2 maxCircleBulletRadiusLimitInTerrain = (((Vector2)transform.InverseTransformPoint(circleCollider2D.transform.position) + circleBulletRadius) * terrainSprite.pixelsPerUnit) + terrainSprite.pivot;
            maxCircleBulletRadiusLimitInTerrain.x = Mathf.Clamp(maxCircleBulletRadiusLimitInTerrain.x, 0, terrainTexture2D.width);
            maxCircleBulletRadiusLimitInTerrain.y = Mathf.Clamp(maxCircleBulletRadiusLimitInTerrain.y, 0, terrainTexture2D.height);

            for (int i = Mathf.FloorToInt(minCircleBulletRadiusLimitInTerrain.x); i < Mathf.FloorToInt(maxCircleBulletRadiusLimitInTerrain.x); i++)
            {
                for (int j = Mathf.FloorToInt(minCircleBulletRadiusLimitInTerrain.y); j < Mathf.FloorToInt(maxCircleBulletRadiusLimitInTerrain.y); j++)
                {
                    Vector2 pixelWorldPosition = ((Vector2)transform.position) + (new Vector2(i , j) - terrainSprite.pivot) / terrainSprite.pixelsPerUnit;
                    if(Vector2.Distance(circleCollider2D.transform.position, pixelWorldPosition) <= maxDistancePixelCircleBulletCollider)
                        hitPixels.Add(new PixelCoor(i, j));
                }
            }
        }

        return hitPixels.Count > 0 ? hitPixels.ToArray() : null;
    }
    #endregion
}
