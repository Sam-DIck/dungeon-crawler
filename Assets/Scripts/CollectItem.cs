using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItem : MonoBehaviour
{
    public DungeonGenerator generator;


    // Update is called once per frame
    void Update()
    {
        Vector2 boundsMin = new Vector2(generator.player.position.x - 0.5f, generator.player.position.y - 0.5f);
        Vector2 boundsMax = new Vector2(generator.player.position.x + 1, generator.player.position.y + 1);
        for (int x = Mathf.FloorToInt(boundsMin.x - 0.7085f); x <= Mathf.CeilToInt(boundsMax.x + 0.7085f); x++)
        {
            for (int y = Mathf.FloorToInt(boundsMin.y - 1); y <= Mathf.CeilToInt(boundsMax.y + 1); y++)
            {
                Vector2Int itemPos = new Vector2Int(x, y);
                if (generator.items.ContainsKey(itemPos))
                {
                    Debug.Log("Item Removed");
                    generator.items.Remove(itemPos);
                    return;
                }
            }
        }
    }
}
