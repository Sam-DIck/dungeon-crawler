using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonRenderer : MonoBehaviour
{
    private DungeonGenerator generator;
    public TileManager tileManager;
    public Tilemap wallTilemap;
    public Tilemap wallColliderTileMap;
    public Tilemap itemTilemap;
    // Start is called before the first frame update
    void Awake()
    {
        if (generator == null)
            generator = GetComponent<DungeonGenerator>();
        if (tileManager == null)
            tileManager = GetComponent<TileManager>();

        

    }
    private void OnEnable()
    {
        generator.OnChange += DrawDungeon;
    }
    private void OnDisable()
    {
        generator.OnChange -= DrawDungeon;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DrawDungeon()
    {
        System.DateTime t = System.DateTime.Now;
        //wallTilemap.ClearAllTiles();
        wallColliderTileMap.ClearAllTiles();
        Vector2 boundsMin = new Vector2(generator.player.position.x - generator.range, generator.player.position.y - generator.range);
        Vector2 boundsMax = new Vector2(generator.player.position.x + generator.range, generator.player.position.y + generator.range);
        for (int x = Mathf.FloorToInt(boundsMin.x - 1); x <= Mathf.CeilToInt(boundsMax.x + 1); x++)
        {
            for (int y = Mathf.FloorToInt(boundsMin.y - 1); y <= Mathf.CeilToInt(boundsMax.y + 1); y++)
            {
                Vector2Int vertPos = new Vector2Int(x, y);
                if (generator.vertices.ContainsKey(vertPos) &&
                    generator.vertices.ContainsKey(vertPos + Vector2Int.down) &&
                    generator.vertices.ContainsKey(vertPos + Vector2Int.left) &&
                    generator.vertices.ContainsKey(vertPos + Vector2Int.down + Vector2Int.left))
                {
                    TileType tiletype = tileManager.GetTileType(generator.vertices[vertPos],
                                            generator.vertices[vertPos + Vector2Int.left],
                                            generator.vertices[vertPos + Vector2Int.down],
                                            generator.vertices[vertPos + Vector2Int.down + Vector2Int.left]);
                    wallTilemap.SetTile(new Vector3Int(x - 1, y - 1, 0), tiletype.tile);
                    wallColliderTileMap.SetTile(new Vector3Int(x - 1, y - 1, 0), tiletype.colliderTile);
                }

                if (generator.items.ContainsKey(vertPos))
                {
                    itemTilemap.SetTile(new Vector3Int(vertPos.x, vertPos.y, 0), generator.items[vertPos].tile);
                }
            }
        } 
        ShadowCaster2DGenerator.GenerateTilemapShadowCasters(wallColliderTileMap.GetComponent<CompositeCollider2D>(), true, boundsMin-Vector2.one, boundsMax+Vector2.one);
        Debug.Log("Draw" + (System.DateTime.Now - t).Milliseconds.ToString());
    }
}
