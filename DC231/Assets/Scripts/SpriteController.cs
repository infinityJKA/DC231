using UnityEngine;
using System.Collections.Generic;

public class SpriteController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // Drag your SpriteRenderer here in the Inspector
    public List<SpriteSetData> spriteSets; // Assign sprite set data here in the Inspector
    public int dungeonLevel; //This would be the level number

    void Start()
    {
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        if (spriteSets.Count == 0)
        {
            Debug.LogWarning("No sprite sets assigned!");
            return;
        }

        int spriteSetIndex = Mathf.Clamp(dungeonLevel - 1, 0, spriteSets.Count - 1);
        SpriteSetData currentSpriteSet = spriteSets[spriteSetIndex];

        if (currentSpriteSet == null || currentSpriteSet.sprites.Count == 0)
        {
            Debug.LogWarning("No sprites in the selected sprite set!");
            return;
        }

        int spriteIndex = Random.Range(0, currentSpriteSet.sprites.Count);
        spriteRenderer.sprite = currentSpriteSet.sprites[spriteIndex];

        Debug.Log($"Assigned Sprite: {spriteRenderer.sprite.name}");
    }
}