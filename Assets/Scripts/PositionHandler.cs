using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PositionHandler : MonoBehaviour
{
    LeaderboardInfoHandler leaderboardInfoHandler;
    bool initialized = false;

    public List<LapCounter> lapCounters = new List<LapCounter>();

    // Start is called before the first frame update
    void Start()
    {
        initialized = false;
    }

    void Update()
    {
        if (!initialized)
        {
            initialized = true;

            LapCounter[] lapCounterArray = FindObjectsOfType<LapCounter>();

            lapCounters = lapCounterArray.ToList<LapCounter>();

            foreach (LapCounter lapCounter in lapCounters)
                lapCounter.OnPassCheckpoint += OnPassCheckpoint;

            leaderboardInfoHandler = FindObjectOfType<LeaderboardInfoHandler>();

            if(leaderboardInfoHandler != null)
                leaderboardInfoHandler.UpdateList(lapCounters);
        }
    }

    void OnPassCheckpoint(LapCounter lapCounter)
    {
        lapCounters = lapCounters.OrderByDescending(s => s.GetNumberOfPassedCheckpoints()).ThenBy(s => s.GetTimeAtLastCheckpointPassed()).ToList();

        int carPosition = lapCounters.IndexOf(lapCounter) + 1;

        lapCounter.SetCarPosition(carPosition);

        if (leaderboardInfoHandler != null)
            leaderboardInfoHandler.UpdateList(lapCounters);
    }
}
