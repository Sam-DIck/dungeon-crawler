using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightEngine : MonoBehaviour
{
    public static LightEngine instance { get; private set; }
    [SerializeField]
    DungeonGenerator generator;
    public List<Light2D> instantiatedLights = new List<Light2D>();
    public GameObject lightPrefab;
    private void Awake()
    {
        if (generator == null)
            generator = GetComponent<DungeonGenerator>();
        instance = this;
    }
    public void AssignLighting(Vector2Int itemPos, ItemType item)
    {
        Vector2 boundsMin = new Vector2(itemPos.x - item.lightRange/2, itemPos.y - item.lightRange/2);
        Vector2 boundsMax = new Vector2(itemPos.x + item.lightRange/2, itemPos.y + item.lightRange/2);
        Light2D selectedLight = null;
        foreach (Light2D light in instantiatedLights)
        {
            Vector2 pos = light.transform.position;
            if (boundsMin.x<=pos.x && pos.x<=boundsMax.x &&
                boundsMin.y<=pos.y && pos.y <= boundsMax.y)
            {
                if (selectedLight == null)
                {
                    selectedLight = light;
                }
                else
                {
                    if ((new Vector2(selectedLight.transform.position.x, selectedLight.transform.position.y)-itemPos).magnitude > (new Vector2(pos.x, pos.y) - itemPos).magnitude)
                    {
                        selectedLight = light;
                    }
                }
            }
        }
        if (selectedLight != null)
        {
            selectedLight.transform.position += new Vector3(itemPos.x+1, itemPos.y+1);
            selectedLight.transform.position /= 2;
        }
        else
        {
            GameObject go = Instantiate(lightPrefab);
            go.transform.position = new Vector3(itemPos.x+1, itemPos.y+1);
            instantiatedLights.Add(go.GetComponent<Light2D>());
        }
    }
}
