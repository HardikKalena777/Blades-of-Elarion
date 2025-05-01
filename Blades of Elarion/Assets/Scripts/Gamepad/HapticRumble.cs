using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class HapticRumble : MonoBehaviour
{
    public float rumbleDuration = 0.2f; // Duration of the rumble in seconds
    public float lowRumbleFrequency = 0.5f; // Min Frequency of the rumble
    public float highRumbleFrequency = 0.5f; // Max Frequency of the rumble

    public static HapticRumble HR_Instance;

    private void Awake()
    {
        HR_Instance = this; // Singleton instance
    }

    public void Rumble(float lowRumbleFrequency, float highRumbleFrequency,float duration)
    {
        // Trigger gamepad vibration
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(lowRumbleFrequency,highRumbleFrequency); // Set low-frequency and high-frequency motor speeds
            Invoke(nameof(StopVibration), duration);
        }
    }

    private void StopVibration()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0f, 0f); // Stop vibration
        }
    }
}
