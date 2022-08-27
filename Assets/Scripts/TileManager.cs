using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public TileType[] tileTypes;
    [SerializeField]
    TileType errorTile;
    // Start is called before the first frame update

    public TileType GetTileType(Vertice topRight, Vertice topLeft, Vertice bottomRight, Vertice bottomLeft)
    {
        TileType tiletype = tileTypes.Where((TileType tiletype) => tiletype.topRight == topRight.type &&
                                                                   tiletype.topLeft == topLeft.type &&
                                                                   tiletype.bottomLeft == bottomLeft.type &&
                                                                   tiletype.bottomRight == bottomRight.type)
                                     .FirstOrDefault();
        if (tiletype is null)
        {
            return errorTile;
        }
        return tiletype;
    }
}



