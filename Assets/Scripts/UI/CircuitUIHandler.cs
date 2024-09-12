using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircuitUIHandler : MonoBehaviour
{
    [Header("Circuit Details")]
    public Image circuitFlag;

    Animator animator = null;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetupCircuit(CircuitData circuitData)
    {
        circuitFlag.sprite = circuitData.FlagUISprite;
    }

    public void StartFlagAppearAnim(bool isFlagAppearingOnRight)
    {
        if (isFlagAppearingOnRight)
        {
            animator.Play("Car UI Right Appear Anim");
        }
        else
        {
            animator.Play("Car UI Left Appear Anim");
        }
    }

    public void StartFlagDisappearAnim(bool isFlagAppearingOnRight)
    {
        if (isFlagAppearingOnRight)
        {
            animator.Play("Car UI Right Disappear Anim");
        }
        else
        {
            animator.Play("Car UI Left Disappear Anim");
        }
    }

    public void OnFlagDissapearAnimComplete()
    {
        Destroy(gameObject);
    }
}
