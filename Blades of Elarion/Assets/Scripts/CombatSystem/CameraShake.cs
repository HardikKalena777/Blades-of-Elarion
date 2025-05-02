using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }
    CinemachineCamera cinemachineCamera; // Updated to use CinemachineCamera
    float shakerTimer;
    float shakerTimerTotal;
    float startingIntensity;

    void Awake()
    {
        Instance = this;
        cinemachineCamera = GetComponent<CinemachineCamera>(); // Updated to use CinemachineCamera
    }

    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.AmplitudeGain = intensity; // Updated to use AmplitudeGain property

        startingIntensity = intensity;
        shakerTimerTotal = time;
        shakerTimer = time;
    }

    private void Update()
    {
        if (shakerTimer > 0)
        {
            shakerTimer -= Time.deltaTime;
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1f - shakerTimer / shakerTimerTotal); // Updated to use AmplitudeGain property
        }
    }
}
