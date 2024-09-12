using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LapCounter : MonoBehaviour
{
    public TMP_Text positionText;

    int passedCheckpointNumber = 0;
    int numberOfPassedCheckpoints = 0;
    int lapsCompleted = 0;
    int carPosition = 0;
    float timeAtLastCheckpointPassed = 0;
    float hideDelayTimeUI;
    bool hideRoutineRunning = false;
    bool raceCompleted = false;

    const int lapsToComplete = 4;

    LapCounterUIHandler lapCounterUIHandler;

    public event Action<LapCounter> OnPassCheckpoint;

    private void Start()
    {
        if(CompareTag("Player") || CompareTag("AI"))
        {
            lapCounterUIHandler = FindObjectOfType<LapCounterUIHandler>();
            lapCounterUIHandler.SetLapText($"LAP {lapsCompleted + 1}/{lapsToComplete}");
        }
    }

    public void SetCarPosition(int position)
    {
        carPosition = position;
    }

    public int GetNumberOfPassedCheckpoints()
    { 
        return numberOfPassedCheckpoints; 
    }

    public float GetTimeAtLastCheckpointPassed()
    {
        return timeAtLastCheckpointPassed;
    }

    IEnumerator ShowPositionCo(float delayToPositionHide)
    {
        hideDelayTimeUI += delayToPositionHide;

        positionText.text = carPosition.ToString();

        positionText.gameObject.SetActive(true);

        if (!hideRoutineRunning)
        {
            hideRoutineRunning = true;

            yield return new WaitForSeconds(hideDelayTimeUI);
            positionText.gameObject.SetActive(false);

            hideRoutineRunning = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            if (raceCompleted)
                return;

            Checkpoint checkpoint = collision.GetComponent<Checkpoint>();

            if (passedCheckpointNumber + 1 == checkpoint.checkPointNumber)
            {
                passedCheckpointNumber = checkpoint.checkPointNumber;

                numberOfPassedCheckpoints++;

                timeAtLastCheckpointPassed = Time.time;

                if (checkpoint.isFinishLine)
                {
                    passedCheckpointNumber = 0;
                    lapsCompleted++;

                    if (lapsCompleted >= lapsToComplete)
                        raceCompleted = true;

                    if (!raceCompleted && lapCounterUIHandler != null)
                        lapCounterUIHandler.SetLapText($"LAP {lapsCompleted + 1}/{lapsToComplete}");
                }

                OnPassCheckpoint?.Invoke(this);

                if (raceCompleted)
                {
                    StartCoroutine(ShowPositionCo(100));

                    if (CompareTag("Player"))
                    {
                        GameManager.instance.OnRaceCompleted();

                        GetComponent<PlayerCarInputHandler>().enabled = false;
                        GetComponent<CarAIHandler>().enabled = true;
                        GetComponent<PathFinding>().enabled = true;
                    }
                }
                else
                    StartCoroutine(ShowPositionCo(2));
            }

        }
    }
}
