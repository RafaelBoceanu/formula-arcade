using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCars : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        CarData[] carDatas = Resources.LoadAll<CarData>("CarData/");

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform spawnPoint = spawnPoints[i].transform;

            int playerSelectedCarID = PlayerPrefs.GetInt($"P{i + 1}SelectedCarID");

            foreach (CarData carData in carDatas)
            {
                if (carData.CarUniqueID == playerSelectedCarID)
                {
                    GameObject carInScene = Instantiate(carData.CarPrefab, spawnPoint.position, spawnPoint.rotation);

                    int playerNumber = i + 1;

                    carInScene.GetComponent<PlayerCarInputHandler>().playerNumber = i + 1;

                    if (PlayerPrefs.GetInt($"P{playerNumber}SelectedCarID_IsAI") == 1)
                    {
                        carInScene.GetComponent<PlayerCarInputHandler>().enabled = false;
                        carInScene.GetComponent<CarAIHandler>().playerNumber = i + 1;
                        carInScene.gameObject.name = $"Player {playerNumber}";
                        carInScene.tag = "AI";
                    }
                    else
                    {
                        carInScene.GetComponent<CarAIHandler>().enabled = false;
                        carInScene.GetComponent<PathFinding>().enabled = false;
                        carInScene.GetComponent<PlayerCarInputHandler>().playerNumber = i + 1;
                        carInScene.gameObject.name = $"Player {playerNumber}";
                        carInScene.tag = "Player";
                    }

                    break;
                }
            }
        }
    }
}
