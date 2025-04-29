//This allows us to create a dictionary of colors to game objects in the inspector
using UnityEngine;

[System.Serializable]
public class ColorToGameObject
{ //this is a class, rather than a struct, because structs are not serializable in Unity
    
    public Color color;
    public GameObject prefab;
    public RoomObjectType objType;
}

public enum RoomObjectType{
    Obstacle, Wall, Floor
}
