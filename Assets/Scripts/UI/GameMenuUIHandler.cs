using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuUIHandler : MonoBehaviour
{
    Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();

        canvas.enabled = false;

        GameManager.instance.OnGameStateChanged += OnGameStateChanged;
    }

    public void OnPlayAgainWithSameCars()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnPlayAgainWithDifferentCars()
    {
        SceneManager.LoadScene("Menu");
    }

    IEnumerator ShowMenuCo()
    {
        yield return new WaitForSeconds(1);

        canvas.enabled = true;
    }

    void OnGameStateChanged(GameManager gameManager)
    {
        if (GameManager.instance.GetGameState() == GameStates.raceOver)
            StartCoroutine(ShowMenuCo());
    }

    void OnDestroy()
    {
        GameManager.instance.OnGameStateChanged -= OnGameStateChanged;
    }
}
