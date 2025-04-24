//This acts to hold the sprites for each prefab per theme
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SpriteSet", menuName = "Custom/Sprite Set")]
public class SpriteSetData : ScriptableObject
{
    public List<Sprite> sprites = new List<Sprite>();
}