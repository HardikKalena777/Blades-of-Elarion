using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaRegenRate = 15f;
    public float regenDelay = 1.0f;

    private float currentStamina;
    private float lastStaminaUseTime;

    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;

    void Start()
    {
        currentStamina = maxStamina;
    }

    void Update()
    {
        RegenerateStamina();
    }

    public bool HasStamina(float amount)
    {
        return currentStamina >= amount;
    }

    public void UseStamina(float amount)
    {
        currentStamina = Mathf.Max(currentStamina - amount, 0f);
        lastStaminaUseTime = Time.time;
    }

    void RegenerateStamina()
    {
        if (Time.time - lastStaminaUseTime < regenDelay)
            return;

        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }
    }

    public void RefillStamina()
    {
        currentStamina = maxStamina;
    }
}
