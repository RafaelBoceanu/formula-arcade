using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarUIHandler : MonoBehaviour
{
    [Header("Car Details")]
    public Image carImage;

    Animator animator = null;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetupCar(CarData carData)
    {
        carImage.sprite = carData.CarUISprite;
    }

    public void StartCarAppearAnim(bool isCarAppearingOnRight)
    {
        if (isCarAppearingOnRight)
        {
            animator.Play("Car UI Right Appear Anim");
        }
        else
        {
            animator.Play("Car UI Left Appear Anim");
        }
    }

    public void StartCarDisappearAnim(bool isCarDisappearingOnRight)
    {
        if (isCarDisappearingOnRight)
        {
            animator.Play("Car UI Right Disappear Anim");
        }
        else
        {
            animator.Play("Car UI Left Disappear Anim");
        }
    }

    public void OnCarDissapearAnimComplete()
    {
        Destroy(gameObject);
    }
}
