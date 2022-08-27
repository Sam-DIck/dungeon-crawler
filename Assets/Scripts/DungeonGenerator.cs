using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public delegate void OnChangeDelegate();
    public OnChangeDelegate OnChange;
    [SerializeField]
    public Transform player;
    [SerializeField]
    public int range;
    Vector3 lastPlayerPos = new Vector3(100, 100);
    [SerializeField]
    float updateDistance;
    public Dictionary<Vector2Int, Vertice> vertices { get; private set; } = new Dictionary<Vector2Int, Vertice>();
    public Dictionary<Vector2Int, ItemType> items { get; private set; } = new Dictionary<Vector2Int, ItemType>();

    // Start is called before the first frame update
    void Start()
    {
        for(int x = -Mathf.FloorToInt(range/2); x<= Mathf.CeilToInt(range/2); x++)
        {
            for (int y = -Mathf.FloorToInt(range/2); y < Mathf.FloorToInt(range/2); y++)
            {
                vertices.Add(new Vector2Int(x, y), new Vertice(Vertice.VerticeType.None, true));
            }
        }
        DebugVertices(vertices);
        vertices = CompleteFill(vertices, new Vector2Int(-range, -range), new Vector2Int(range, range));
        vertices = ClearLoneVertices(vertices);
        DebugVertices(vertices);
        OnChange.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = player.position;
        if ((playerPosition - lastPlayerPos).magnitude >= updateDistance)
        {
            DateTime t = DateTime.Now;
            vertices = CompleteFill(vertices, new Vector2Int(Mathf.FloorToInt(playerPosition.x) - range, Mathf.FloorToInt(playerPosition.y) - range), new Vector2Int(Mathf.CeilToInt(playerPosition.x) + range, Mathf.CeilToInt(playerPosition.y) + range));
            vertices = RemoveOutOfRange(vertices, items, new Vector2Int(Mathf.FloorToInt(playerPosition.x) - range, Mathf.FloorToInt(playerPosition.y) - range), new Vector2Int(Mathf.CeilToInt(playerPosition.x) + range, Mathf.CeilToInt(playerPosition.y) + range));
            vertices = ClearLoneVertices(vertices);
            t = DateTime.Now;
            OnChange.Invoke();
            Debug.Log("Total: " + (DateTime.Now - t).Milliseconds.ToString());
            lastPlayerPos = playerPosition;
        }
    }

    static void DebugVertices(Dictionary<Vector2Int, Vertice> dict)
    {
        Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue);
        Vector2Int max = new Vector2Int(int.MinValue, int.MinValue);
        foreach (KeyValuePair<Vector2Int, Vertice> pair in dict)
        {
            min = Vector2Int.Min(min, pair.Key);
            max = Vector2Int.Max(max, pair.Key);
        }
        string str = string.Empty;
        for (int y = max.y; y >= min.y; y--)
        {
            for (int x = min.x; x <= max.x; x++)
            {
                Vertice vert;
                if (dict.TryGetValue(new Vector2Int(x, y), out vert))
                {
                    switch (vert.type)
                    {
                        case Vertice.VerticeType.None:
                            str += "~";
                            break;
                        case Vertice.VerticeType.Wall:
                            str += "#";
                            break;
                        default:
                            break;
                    }

                }
                else str += "?";
            }
            str += System.Environment.NewLine;
        }
        Debug.Log(str);
    }

    public static Vertice GetVertice(Dictionary<Vector2Int, Vertice> dict, Vector2Int pos)
    {
        Vertice.VerticeType newVertType = GetRandomTileType(50);

        Dictionary<Vector2Int, Vertice> neighbours = GetNeighbours(dict, pos);
        if (neighbours.IsWall(Vector2Int.up) || neighbours.IsWall(Vector2Int.down))
        {
            if (neighbours.IsWall(Vector2Int.left) || neighbours.IsWall(Vector2Int.right))
                newVertType = Vertice.VerticeType.Wall;
        }
        return new Vertice(newVertType);
    }
    public static Dictionary<Vector2Int, Vertice> CompleteFill(Dictionary<Vector2Int, Vertice> dict, Vector2Int boundsMin, Vector2Int boundsMax)
    {

        Dictionary<Vector2Int, Vertice> newDict = new Dictionary<Vector2Int, Vertice>(dict);
        for (int x = boundsMin.x; x <= boundsMax.x; x++)
        {
            for (int y = boundsMin.y; y <= boundsMax.y; y++)
            {
                if (!newDict.ContainsKey(new Vector2Int(x, y)))
                {
                    
                    newDict.Add(new Vector2Int(x, y), GetVertice(dict, new Vector2Int(x,y)));
                }
            }
        }
        return newDict;
    }
    public static Dictionary<Vector2Int, Vertice> GetNeighbours(Dictionary<Vector2Int, Vertice> dict, Vector2Int Key)
    {
        Dictionary<Vector2Int, Vertice> neighbours = new Dictionary<Vector2Int, Vertice>();
        for (int offsetX = -1; offsetX <= 1; offsetX++)
        {
            for (int offsetY = -1; offsetY <= 1; offsetY++)
            {
                if (dict.ContainsKey(Key + new Vector2Int(offsetX, offsetY)))
                {
                    neighbours.Add(new Vector2Int(offsetX, offsetY), dict[Key + new Vector2Int(offsetX, offsetY)]);
                }
            }
        }
        return neighbours;
    }

    public static bool CanRemove(Dictionary<Vector2Int, Vertice> dict, Dictionary<Vector2Int, ItemType> items, Vector2Int pos)
    {
        if (dict.ContainsKey(pos))
            return !dict[pos].hasLight;
        else return false;
    }
    public static Dictionary<Vector2Int, Vertice> RemoveOutOfRange(Dictionary<Vector2Int, Vertice> dict, Dictionary<Vector2Int, ItemType> items, Vector2Int boundsMin, Vector2Int boundsMax)
    {
        Dictionary<Vector2Int, Vertice> newDict = new Dictionary<Vector2Int, Vertice>(dict);
        for (int x = boundsMin.x-1; x <= boundsMax.x + 1; x++)
        {
            int y = boundsMin.y-1;
            if (CanRemove(dict, items, new Vector2Int(x, y)))
            {
                newDict.Remove(new Vector2Int(x, y));
            }
            y = boundsMax.y + 1;
            if (CanRemove(dict, items, new Vector2Int(x, y)))
            {
                newDict.Remove(new Vector2Int(x, y));
            }

        }
        for (int y = boundsMin.y; y <= boundsMax.y; y++)
        {
            int x = boundsMin.x - 1;
            if (CanRemove(dict, items, new Vector2Int(x, y)))
            {
                newDict.Remove(new Vector2Int(x, y));
            }
            x = boundsMax.x + 1;
            if (CanRemove(dict, items, new Vector2Int(x, y)))
            {
                newDict.Remove(new Vector2Int(x, y));
            }
        }



        //-----------------------------------------------------------------------------
        /*
        foreach (KeyValuePair<Vector2Int, Vertice> pair in dict)
        {
            if ((boundsMin.x <= pair.Key.x &&
                boundsMin.y <= pair.Key.y &&
                pair.Key.x  <= boundsMax.x &&
                pair.Key.y  <= boundsMax.y) || pair.Value.hasLight)
            {
                newDict.Add(pair);
            }
            else
            {
                foreach(KeyValuePair<Vector2Int, ItemType> itemPair in items)
                {
                    if (itemPair.Value.emitsLight)
                    {
                        Vector2Int itemPos = itemPair.Key;
                        float range = itemPair.Value.lightRange;
                        if ((itemPos.x - range <= pair.Key.x &&
                            itemPos.y - range <= pair.Key.y &&
                            pair.Key.x <= itemPos.x + range &&
                            pair.Key.y <= itemPos.y + range) || pair.Value.hasLight)
                        {
                            newDict.Add(pair);
                            break;
                        }
                    }
                }
            }
        }*/
        //DebugVertices(dict);
        //DebugVertices(newDict);

        return newDict;
        
    }
    public static Dictionary<Vector2Int, Vertice> ClearLoneVertices(Dictionary<Vector2Int, Vertice> dict)
    {
        Dictionary<Vector2Int, Vertice> newDict = new Dictionary<Vector2Int, Vertice>();
        foreach (KeyValuePair<Vector2Int, Vertice> pair in dict)
        {
            Dictionary<Vector2Int, Vertice> neighbours = GetNeighbours(dict, pair.Key);
            if (!neighbours.IsWall(Vector2Int.up) && !neighbours.IsWall(Vector2Int.down) &&
                !neighbours.IsWall(Vector2Int.right) && !neighbours.IsWall(Vector2Int.left) && !pair.Value.hasLight)
            {
                newDict.Add(pair.Key, new Vertice(Vertice.VerticeType.None));
            }
            else newDict.Add(pair);
        }
        return newDict;
    }
    public static Vertice.VerticeType GetRandomTileType(int wallChance)
    {
        if (0 > wallChance || wallChance > 100)
            throw new ArgumentOutOfRangeException(nameof(wallChance));
        int index = UnityEngine.Random.Range(0, 101);
        if (index < wallChance)
        {
            return Vertice.VerticeType.Wall;
        }
        else return Vertice.VerticeType.None;
    }
    void OnDrawGizmos()
    {
        foreach (KeyValuePair<Vector2Int, Vertice> pair in vertices)
        {
            if (pair.Value.hasLight)
            {
                if (pair.Value.type == Vertice.VerticeType.None)
                {
                    Gizmos.color = Color.blue;
                }
                else
                {
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawSphere(new Vector3(pair.Key.x, pair.Key.y), 0.1f);
            }
            else
            {
                if (pair.Value.type == Vertice.VerticeType.Wall)
                { 
                    Gizmos.color = Color.grey;
                Gizmos.DrawSphere(new Vector3(pair.Key.x, pair.Key.y), 0.1f);
                }

            }
            
        }
        Gizmos.color = Color.yellow;
        DrawBox(new Vector2(Mathf.FloorToInt(player.position.x - range), Mathf.FloorToInt(player.position.y - range)),
                new Vector2(Mathf.CeilToInt(player.position.x + range), Mathf.CeilToInt(player.position.y + range)));
        DrawBox(new Vector2(Mathf.FloorToInt(player.position.x - range)-1, Mathf.FloorToInt(player.position.y - range)-1),
                    new Vector2(Mathf.CeilToInt(player.position.x + range)+1, Mathf.CeilToInt(player.position.y + range)+1));
        Gizmos.color = Color.red;
        DrawBox(new Vector2(player.position.x - range, player.position.y - range),
                new Vector2(player.position.x + range, player.position.y + range));



    }
    public static void DrawBox(Vector2 boundsMin, Vector2 boundsMax)
    {
        Gizmos.DrawLine(new Vector3(boundsMin.x, boundsMin.y),
                        new Vector3(boundsMin.x, boundsMax.y));
        Gizmos.DrawLine(new Vector3(boundsMin.x, boundsMax.y),
                        new Vector3(boundsMax.x, boundsMax.y));
        Gizmos.DrawLine(new Vector3(boundsMax.x, boundsMax.y),
                        new Vector3(boundsMax.x, boundsMin.y));
        Gizmos.DrawLine(new Vector3(boundsMax.x, boundsMin.y),
                        new Vector3(boundsMin.x, boundsMin.y));
    }
}


public struct Vertice
{
    public enum VerticeType
    {
        None, Wall
    }
    public VerticeType type;
    public bool hasLight;

    public Vertice(VerticeType type, bool hasLight=false)
    {
        this.type = type;
        this.hasLight = hasLight;
    }
    
}

public static class DictionaryExtensions
{
    public static bool ContainsAndEquals<TKey, TValue>(this Dictionary<TKey,TValue> dictionary, TKey Key, Func<TValue, bool> match)
    {
        return dictionary.ContainsKey(Key) && match(dictionary[Key]);
    }
    public static bool ContainsAndEquals(this Dictionary<Vector2Int, Vertice> dictionary, Vector2Int Key, Vertice.VerticeType type)
    {
        return dictionary.ContainsAndEquals(Key, (vertice) => vertice.type == type);
    }
    public static bool IsWall(this Dictionary<Vector2Int, Vertice> dictionary, Vector2Int Key)
    {
        return dictionary.ContainsAndEquals(Key, Vertice.VerticeType.Wall);
    }
    public static void Add<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, KeyValuePair<TKey, TValue> pair)
    {
        dictionary.Add(pair.Key, pair.Value);
    }

    
}
