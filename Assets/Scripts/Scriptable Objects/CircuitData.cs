using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Circuit Data", menuName = "Circuit Data", order = 52)]
public class CircuitData : ScriptableObject
{
    [SerializeField]
    private int circuitUniqueID = 0;
    [SerializeField]
    private Sprite flagUISprite;
    [SerializeField]
    private GameObject circuitPrefab;

    public int CircuitUniqueID
    {
        get { return circuitUniqueID; }
    }

    public Sprite FlagUISprite
    {
        get { return flagUISprite; }
    }

    public GameObject CircuitPrefab
    {
        get { return circuitPrefab; }
    }
}
