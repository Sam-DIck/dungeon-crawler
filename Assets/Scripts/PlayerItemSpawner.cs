using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemSpawner : MonoBehaviour
{
    [SerializeField]
    ItemType itemToSpawn;
    private Controls _controls;
    private DungeonGenerator generator;
    [SerializeField]
    Transform playerTransform;
    bool lastpressed = false;
    // Start is called before the first frame update
    void Awake()
    {
        _controls = new Controls();
        generator = FindObjectOfType<DungeonGenerator>();
        if (playerTransform is null)
        {
            playerTransform = GetComponent<Transform>();
        }
    }
    private void OnEnable()
    {
        _controls.Player.Enable();
    }
    private void OnDisable()
    {
        _controls.Player.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (_controls.Player.Fire.IsPressed()&& !lastpressed)
        {
            Vector3 pos = playerTransform.position;
            Vector2Int itemPos = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
            if (!generator.items.ContainsKey(itemPos))
            {
                generator.items.Add(itemPos, itemToSpawn);
                for (int x = Mathf.FloorToInt(itemPos.x - itemToSpawn.lightRange); x <= Mathf.CeilToInt(itemPos.x + itemToSpawn.lightRange); x++)
                {
                    for (int y = Mathf.FloorToInt(itemPos.y - itemToSpawn.lightRange); y <= Mathf.CeilToInt(itemPos.y + itemToSpawn.lightRange); y++)
                    {
                        Vector2Int tilePos = new Vector2Int(x, y);
                        if (generator.vertices.ContainsKey(tilePos))
                        {
                            Vertice vert = generator.vertices[tilePos];
                            vert.hasLight = true;
                            generator.vertices[tilePos] = vert;
                        }
                    }
                }
                LightEngine.instance.AssignLighting(itemPos, itemToSpawn);

                /*foreach (KeyValuePair<Vector2Int, Vertice> pair in generator.vertices)
                {
                    if (itemPos.x - itemToSpawn.lightRange <= pair.Key.x &&
                            itemPos.y - itemToSpawn.lightRange <= pair.Key.y &&
                            pair.Key.x <= itemPos.x + itemToSpawn.lightRange &&
                            pair.Key.y <= itemPos.y + itemToSpawn.lightRange)
                    {
                        Vertice vert = pair.Value;
                        vert.hasLight = true;
                        generator.vertices[pair.Key] = vert;
                    }
                }*/

                generator.OnChange.Invoke();
            }
        }
        lastpressed = _controls.Player.Fire.IsPressed();
    }

}

