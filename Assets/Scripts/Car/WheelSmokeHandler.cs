using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSmokeHandler : MonoBehaviour
{
    float smokeEmissionRate = 0;

    PlayerCarController carController;

    ParticleSystem wheelSmoke;
    ParticleSystem.EmissionModule wheelSmokeEmissionModule;

    // Start is called before the first frame update
    void Start()
    {
        carController = GetComponentInParent<PlayerCarController>();

        wheelSmoke = GetComponent<ParticleSystem>();

        wheelSmokeEmissionModule = wheelSmoke.emission;

        wheelSmokeEmissionModule.rateOverTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        smokeEmissionRate = Mathf.Lerp(smokeEmissionRate, 0, Time.deltaTime * 5);
        wheelSmokeEmissionModule.rateOverTime = smokeEmissionRate;

        if (carController.IsTireSkidding(out float lateralVelocity, out bool isBraking))
        {
            if (isBraking)
                smokeEmissionRate = 30;
            else smokeEmissionRate = Mathf.Abs(lateralVelocity) * 2;
        }
    }
}
