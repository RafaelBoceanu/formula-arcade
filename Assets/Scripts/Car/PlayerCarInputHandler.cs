using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarInputHandler : MonoBehaviour
{
    public int playerNumber = 1;

    PlayerCarController carController;

    // Start is called before the first frame update
    void Start()
    {
        carController = GetComponent<PlayerCarController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputVector = Vector2.zero;

        switch (playerNumber)
        {
            case 1:
                inputVector.x = Input.GetAxis("Horizontal_P1");
                inputVector.y = Input.GetAxis("Vertical_P1");
                break;
            case 2:
                inputVector.x = Input.GetAxis("Horizontal_P2");
                inputVector.y = Input.GetAxis("Vertical_P2");
                break;
            case 3:
                inputVector.x = Input.GetAxis("Horizontal_P3");
                inputVector.y = Input.GetAxis("Vertical_P3");
                break;
            case 4:
                inputVector.x = Input.GetAxis("Horizontal_P4");
                inputVector.y = Input.GetAxis("Vertical_P4");
                break;
        }

        carController.SetInputVector(inputVector);
    }
}
