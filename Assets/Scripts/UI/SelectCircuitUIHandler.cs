using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCircuitUIHandler : MonoBehaviour
{
    [Header("Circuit Prefab")]
    public GameObject circuitPrefab;

    [Header("Spawn on")]
    public Transform spawnOnTransform;

    bool isChangingCircuit = false;

    CircuitData[] circuitDatas;

    int selectedCircuitIndex = 0;

    CircuitUIHandler circuitUIHandler = null;

    // Start is called before the first frame update
    void Start()
    {
        circuitDatas = Resources.LoadAll<CircuitData>("CircuitData/");

        StartCoroutine(SpawnCircuitCo(true));
    }

    // Update is called once per frame
    void Update()
    {
        CircuitRotation();
        Debug.Log(circuitDatas[selectedCircuitIndex]);

        OnSelectCircuit();
    }

    public void OnPreviousCar()
    {
        if (isChangingCircuit)
            return;

        selectedCircuitIndex--;

        if (selectedCircuitIndex < 0)
            selectedCircuitIndex = circuitDatas.Length - 1;

        StartCoroutine(SpawnCircuitCo(true));
    }

    public void OnNextCar()
    {
        if (isChangingCircuit)
            return;

        selectedCircuitIndex++;

        if (selectedCircuitIndex > circuitDatas.Length - 1)
            selectedCircuitIndex = 0;

        StartCoroutine(SpawnCircuitCo(false));
    }

    public void OnSelectCircuit()
    {
        PlayerPrefs.SetInt("SelectCircuit", circuitDatas[selectedCircuitIndex].CircuitUniqueID);
        

        PlayerPrefs.Save();
    }

    public void CircuitRotation()
    {
        if ((Input.GetAxis("Horizontal_P1") < 0) || (Input.GetAxis("Horizontal_P2") < 0) || (Input.GetAxis("Horizontal_P3") < 0) || (Input.GetAxis("Horizontal_P4") < 0))
        {
            OnPreviousCar();
        }
        else if ((Input.GetAxis("Horizontal_P1") > 0) || (Input.GetAxis("Horizontal_P2") > 0) || (Input.GetAxis("Horizontal_P3") > 0) || (Input.GetAxis("Horizontal_P4") > 0))
        {
            OnNextCar();
        }
    }

    IEnumerator SpawnCircuitCo(bool isFlagAppearingOnRight)
    {
        isChangingCircuit = true;

        if (circuitUIHandler != null)
            circuitUIHandler.StartFlagDisappearAnim(!isFlagAppearingOnRight);

        GameObject circuitInstance = Instantiate(circuitPrefab, spawnOnTransform);

        circuitUIHandler = circuitInstance.GetComponent<CircuitUIHandler>();
        circuitUIHandler.SetupCircuit(circuitDatas[selectedCircuitIndex]);
        circuitUIHandler.StartFlagAppearAnim(isFlagAppearingOnRight);

        yield return new WaitForSeconds(0.4f);

        isChangingCircuit = false;
       
    }
}
