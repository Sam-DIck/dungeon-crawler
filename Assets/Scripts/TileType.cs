using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "TileType")]
public class TileType : ScriptableObject
{
    public Vertice.VerticeType topRight;
    public Vertice.VerticeType topLeft;
    public Vertice.VerticeType bottomRight;
    public Vertice.VerticeType bottomLeft;

    public Tile tile;
    public Tile colliderTile;
    
}
