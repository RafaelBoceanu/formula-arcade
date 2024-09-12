using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SelectCarUIHandler : MonoBehaviour
{
    [Header("Car Prefab")]
    public GameObject carPrefab;

    [Header("Spawn on")]
    public Transform spawnOnTransform;
    public TMP_Text isAIText;
    
    public bool isAI = false;

    bool isChangingCar = false;
    bool canCarBeChanged = true;

    CarData[] carDatas;

    int selectedCarIndex = 0;

    CarUIHandler carUIHandler = null;

    // Start is called before the first frame update
    void Start()
    {
        carDatas = Resources.LoadAll<CarData>("CarData/");

        StartCoroutine(SpawnCarCo(true));
    }

    // Update is called once per frame
    void Update()
    {
        CarRotation();

        if (isAI)
        {
            carUIHandler.carImage.sprite = carDatas[carDatas.Length - 1].CarUISprite;
            canCarBeChanged = false;
        }
        else
        {
            canCarBeChanged = true;
        }
        
        OnSelectCar();
    }

    public void OnPreviousCar()
    {
        if (isChangingCar)
            return;

        selectedCarIndex--;

        if (selectedCarIndex < 0)
            selectedCarIndex = carDatas.Length - 2;

        StartCoroutine(SpawnCarCo(true));
    }

    public void OnNextCar()
    {
        if (isChangingCar)
            return;

        selectedCarIndex++;

        if (selectedCarIndex > carDatas.Length - 2)
            selectedCarIndex = 0;

        StartCoroutine(SpawnCarCo(false));
    }

    public void OnSelectCar()
    {

        if(name != "P1SelectedCarID")
        {
            if(isAI)
            {
                PlayerPrefs.SetInt(name, carDatas[4].CarUniqueID);
                PlayerPrefs.SetInt($"{name}_IsAI", 1);
            }
            else
            {
                PlayerPrefs.SetInt(name, carDatas[selectedCarIndex].CarUniqueID);
                PlayerPrefs.SetInt($"{name}_IsAI", 0);
            }
        }
        else
        {
            PlayerPrefs.SetInt(name, carDatas[selectedCarIndex].CarUniqueID);
            PlayerPrefs.SetInt($"{name}_IsAI", 0);
        }

        PlayerPrefs.Save();
    }

    public void CarRotation()
    {
        switch (name)
        {
            case "P1SelectedCarID":
                if (Input.GetAxis("Horizontal_P1") < 0)
                {
                    OnPreviousCar();
                }
                else if (Input.GetAxis("Horizontal_P1") > 0)
                {
                    OnNextCar();
                }
                break;
            case "P2SelectedCarID":
                if (Input.GetAxis("Horizontal_P2") < 0)
                {
                    OnPreviousCar();
                }
                else if (Input.GetAxis("Horizontal_P2") > 0)
                {
                    OnNextCar();
                }
                break;
            case "P3SelectedCarID":
                if (Input.GetAxis("Horizontal_P3") < 0)
                {
                    OnPreviousCar();
                }
                else if (Input.GetAxis("Horizontal_P3") > 0)
                {
                    OnNextCar();
                }
                break;
            case "P4SelectedCarID":
                if (Input.GetAxis("Horizontal_P4") < 0)
                {
                    OnPreviousCar();
                }
                else if (Input.GetAxis("Horizontal_P4") > 0)
                {
                    OnNextCar();
                }
                break;
        }
    }

    public void IsAIOnOff()
    {
        isAI = !isAI;

        if (isAI)
            isAIText.gameObject.SetActive(true);
        else
            isAIText.gameObject.SetActive(false);

    }

    IEnumerator SpawnCarCo(bool isCarAppearingOnRight)
    {
        if (canCarBeChanged)
        {
            isChangingCar = true;

            if (carUIHandler != null)
                carUIHandler.StartCarDisappearAnim(!isCarAppearingOnRight);

            GameObject carInstance = Instantiate(carPrefab, spawnOnTransform);

            carUIHandler = carInstance.GetComponent<CarUIHandler>();
            carUIHandler.SetupCar(carDatas[selectedCarIndex]);
            carUIHandler.StartCarAppearAnim(isCarAppearingOnRight);

            yield return new WaitForSeconds(0.4f);

            isChangingCar = false;
        }
    }
}
