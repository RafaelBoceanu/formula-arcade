using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayQuitGame : MonoBehaviour
{
    public Canvas mainMenu;
    public Canvas helpMenu;
    public Canvas selectCarMenu;
    public Canvas selectCircuitMenu;

    public void SelectCar()
    {
        mainMenu.gameObject.SetActive(false);
        selectCarMenu.gameObject.SetActive(true);
    }

    public void SelectCircuit()
    {
        selectCarMenu.gameObject.SetActive(false);
        selectCircuitMenu.gameObject.SetActive(true);
    }

    public void GoToHelpMenu()
    {
        mainMenu.gameObject.SetActive(false);
        helpMenu.gameObject.SetActive(true);
    }

    public void GoBackToMainMenu()
    {
        mainMenu.gameObject.SetActive(true);
        selectCarMenu.gameObject.SetActive(false);
        helpMenu.gameObject.SetActive(false);
    }

    public void GoBackToSelectCar()
    {
        selectCarMenu.gameObject.SetActive(true);
        selectCircuitMenu.gameObject.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("LocalMultiplayerScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quit game!");
        Application.Quit();
    }
}
