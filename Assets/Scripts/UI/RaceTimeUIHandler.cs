using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceTimeUIHandler : MonoBehaviour
{
    TMP_Text timeText;

    float lastRaceTimeUpdate = 0;

    private void Awake()
    {
        timeText = GetComponent<TMP_Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateTimeCo());
    }

    IEnumerator UpdateTimeCo()
    {
        while (true)
        {
            float raceTime = GameManager.instance.GetRaceTime();

            if (lastRaceTimeUpdate != raceTime)
            {
                int raceTimeMinutes = (int)Mathf.Floor(raceTime / 60);
                int raceTimeSeconds = (int)Mathf.Floor(raceTime % 60);

                timeText.text = $"{raceTimeMinutes:00}:{raceTimeSeconds:00}";

                lastRaceTimeUpdate = raceTime;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
