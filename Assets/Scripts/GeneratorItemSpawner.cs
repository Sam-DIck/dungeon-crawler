using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorItemSpawner : MonoBehaviour
{
    public ItemType itemToSpawn;
    public DungeonGenerator generator;
    public int numberToSpawn = 10;
    public float minRange = 10;
    public float maxRange = 100;

    private void Awake()
    {
        if(generator == null)
        {
            generator = GetComponent<DungeonGenerator>();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
            Vector2 pos = Random.insideUnitCircle.normalized * Random.Range(minRange, maxRange);
            Vector2Int itemPos = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
            if (!generator.items.ContainsKey(itemPos))
            {
                generator.items.Add(itemPos, itemToSpawn);
                if (itemToSpawn.protectsTiles)
                {
                    for (int x = Mathf.FloorToInt(itemPos.x - itemToSpawn.lightRange); x <= Mathf.CeilToInt(itemPos.x + itemToSpawn.lightRange); x++)
                    {
                        for (int y = Mathf.FloorToInt(itemPos.y - itemToSpawn.lightRange); y <= Mathf.CeilToInt(itemPos.y + itemToSpawn.lightRange); y++)
                        {
                            Vector2Int tilePos = new Vector2Int(x, y);
                            if (generator.vertices.ContainsKey(tilePos))
                            {
                                Vertice vert = generator.vertices[tilePos];
                                vert.hasLight = true;
                                vert.type = itemToSpawn.defualtTile;
                                generator.vertices[tilePos] = vert;
                            }
                        }
                    }
                }
                if (itemToSpawn.emitsLight)
                {
                    LightEngine.instance.AssignLighting(itemPos, itemToSpawn);
                }
            }
        }
    }
}
