using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCircuit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("CircuitSpawnPoint");

        CircuitData[] circuitDatas = Resources.LoadAll<CircuitData>("CircuitData/");

        Transform spawnPointPosition = spawnPoint.transform;

        int selectedCircuit = PlayerPrefs.GetInt("SelectCircuit");

        foreach (CircuitData circuitData in circuitDatas)
        {
            if (circuitData.CircuitUniqueID == selectedCircuit)
            {
                GameObject circuitInstance = Instantiate(circuitData.CircuitPrefab, spawnPointPosition.position, spawnPointPosition.rotation);
            }
        }
    }
}
