using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public int currentHealth;
    public GameObject hitVFX;
    public GameObject ragdoll;
    public Slider healthBar; // Reference to the health bar UI element

    public Volume volume; 
    int maxHealth = 100;
    Animator animator;

    private Vignette vignette; 

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (volume != null && volume.profile.TryGet(out Vignette vignetteComponent))
        {
            vignette = vignetteComponent;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        UpdateHealthUI(currentHealth, healthBar); // Update the health bar UI
        animator.SetTrigger("Hit");
        CameraShake.Instance.ShakeCamera(1f, 0.2f);
        HapticRumble.HR_Instance.Rumble(0.5f, 0.5f, 0.2f); // Trigger haptic feedback on hit

        UpdateVignetteIntensity();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateVignetteIntensity()
    {
        if (vignette != null)
        {
            float intensity = Mathf.Lerp(0f, 1f, 1f - (float)currentHealth / maxHealth);
            vignette.intensity.Override(intensity);
        }
    }

    private void UpdateHealthUI(int health, Slider healthBar)
    {
        if (healthBar != null)
        {
            healthBar.value = (float)health / maxHealth; // Correctly update the health bar value as a percentage
        }
    }

    private void Die()
    {
        Instantiate(ragdoll, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public void PlayHitVFX(Vector3 hitPosition)
    {
        if (hitVFX != null)
        {
            GameObject vfx = Instantiate(hitVFX, hitPosition, Quaternion.identity);
            Destroy(vfx, 1f); // Destroy the VFX after 1 second
        }
    }
}
