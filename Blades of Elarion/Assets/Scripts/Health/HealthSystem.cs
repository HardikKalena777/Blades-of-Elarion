using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int currentHealth;
    public GameObject hitVFX;
    public GameObject ragdoll;

    int maxHealth = 100;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        animator.SetTrigger("Hit");
        CameraShake.Instance.ShakeCamera(1f, 0.2f);

        if (currentHealth <= 0)
        {
            Die();
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
            Destroy(vfx, 1f); // Destroy the VFX after 2 seconds
        }
    }
}
