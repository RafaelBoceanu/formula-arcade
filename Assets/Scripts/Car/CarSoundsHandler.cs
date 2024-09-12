using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CarSoundsHandler : MonoBehaviour
{
    public AudioMixer audioMixer;

    public AudioSource tires;
    public AudioSource engine;
    public AudioSource hit;
    public AudioSource jump;
    public AudioSource land;

    float desiredEnginePitch = 0.5f;
    float tiresPitch = 0.5f;

    PlayerCarController carController;

    // Start is called before the first frame update
    void Start()
    {
        carController = GetComponent<PlayerCarController>();
        audioMixer.SetFloat("SFXVolume", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEngineSounds();
        UpdateTiresSounds();
    }

    void UpdateEngineSounds()
    {
        float velocityMagnitude = carController.GetVelocityMagnitude();

        float desiredEngineVolume = velocityMagnitude * 0.05f;
        desiredEngineVolume = Mathf.Clamp(desiredEngineVolume, 0.2f, 1.0f);
        engine.volume = Mathf.Lerp(engine.volume, desiredEngineVolume, Time.deltaTime * 10);

        desiredEnginePitch = velocityMagnitude * 0.2f;
        desiredEnginePitch = Mathf.Clamp(desiredEnginePitch, 0.5f, 2f);
        engine.pitch = Mathf.Lerp(engine.pitch, desiredEnginePitch, Time.deltaTime * 1.5f);
    }

    void UpdateTiresSounds()
    {
        if (carController.IsTireSkidding(out float lateralVelocity, out bool isBraking))
        {
            if (isBraking)
            {
                tires.volume = Mathf.Lerp(tires.volume, 1.0f, Time.deltaTime * 10);
                tiresPitch = Mathf.Lerp(tiresPitch, 0.5f, Time.deltaTime * 10);
            }
            else
            {
                tires.volume = Mathf.Abs(lateralVelocity) * 0.05f;
                tiresPitch = Mathf.Abs(lateralVelocity) * 0.1f;
            }
        }
        else
            tires.volume = Mathf.Lerp(tires.volume, 0, Time.deltaTime * 10);
    }

    public void PlayJumpSound()
    {
        jump.Play();
    }

    public void PlayLandSound()
    {
        land.Play();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        float relativeVelocity = collision.relativeVelocity.magnitude;

        float volume = relativeVelocity * 0.1f;

        hit.pitch = Random.Range(0.95f, 1.05f);
        hit.volume = volume;

        if (!hit.isPlaying)
            hit.Play();
    }
}
