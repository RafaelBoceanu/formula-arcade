using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardInfoHandler : MonoBehaviour
{
    public GameObject leaderboardItemPrefab;
    bool initialized = false;
    SetLeaderboardInfo[] setLeaderboardInfo;

    Canvas canvas;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;

        GameManager.instance.OnGameStateChanged += OnGameStateChanged;
    }

    // Start is called before the first frame update
    void Start()
    {
        initialized = false;
    }

    void Update()
    {
        if ( !initialized )
        {
            initialized = true;
            VerticalLayoutGroup leaderboardLayoutGroup = GetComponentInChildren<VerticalLayoutGroup>();

            LapCounter[] lapCounterArray = FindObjectsOfType<LapCounter>();

            setLeaderboardInfo = new SetLeaderboardInfo[lapCounterArray.Length];

            for (int i = 0; i < lapCounterArray.Length; i++)
            {
                GameObject leaderboardInfoGameObject = Instantiate(leaderboardItemPrefab, leaderboardLayoutGroup.transform);

                setLeaderboardInfo[i] = leaderboardInfoGameObject.GetComponent<SetLeaderboardInfo>();

                setLeaderboardInfo[i].SetPositionText($"{i + 1}.");
            }

        }
    }
    public void UpdateList(List<LapCounter> lapCounters)
    {
        if (setLeaderboardInfo != null)
        {
            for (int i = 0; i < lapCounters.Count; i++)
            {
                setLeaderboardInfo[i].SetPlayerNameText(lapCounters[i].gameObject.name);
            }
        }
    }

    void OnGameStateChanged(GameManager gameManager)
    {
        if (GameManager.instance.GetGameState() == GameStates.raceOver)
        {
            canvas.enabled = true;
        }
    }

    void OnDestroy()
    {
        GameManager.instance.OnGameStateChanged -= OnGameStateChanged;
    }
}
