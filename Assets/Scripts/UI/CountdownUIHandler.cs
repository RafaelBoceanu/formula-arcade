using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownUIHandler : MonoBehaviour
{
    public TMP_Text countdownText;

    private void Awake()
    {
        countdownText.text = "";
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CountdownCo());        
    }

    IEnumerator CountdownCo()
    {
        yield return new WaitForSeconds(0.3f);

        int counter = 3;

        while (true)
        {
            if (counter != 0)
                countdownText.text = counter.ToString();
            else
            {
                countdownText.text = "GO!";

                GameManager.instance.OnRaceStart();

                break;
            }

            counter--;
            yield return new WaitForSeconds(1.0f);
        }

        yield return new WaitForSeconds(0.5f);

        gameObject.SetActive(false);
    }
}
