using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "ItemType")]
public class ItemType : ScriptableObject
{
    public Tile tile;
    
    public float lightRange;
    public float protectionRange;
    public Vertice.VerticeType defualtTile;
    public bool emitsLight { get { return lightRange > 0; } }
    public bool protectsTiles { get { return protectionRange > 0; } }
}
