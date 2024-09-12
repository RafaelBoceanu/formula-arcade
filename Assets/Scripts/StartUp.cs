using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUp
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InstantiatePrefab()
    {
        Debug.Log(" -- Instantiating global objects -- ");

        GameObject[] prefabsToInstantiate = Resources.LoadAll<GameObject>("InstantiateOnLoad/");

        foreach (GameObject prefab in prefabsToInstantiate)
        {
            Debug.Log($"Creating {prefab.name}");

            GameObject.Instantiate(prefab);
        }

        Debug.Log(" -- Instantiating global objects done -- ");
    }
}
