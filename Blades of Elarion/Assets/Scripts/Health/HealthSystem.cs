using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int currentHealth;
    public GameObject hitVFX;

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

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void PlayHitVFX(Vector3 hitPosition)
    {
        if (hitVFX != null)
        {
            GameObject vfx = Instantiate(hitVFX, hitPosition, Quaternion.identity);
            Destroy(vfx, 3f); // Destroy the VFX after 2 seconds
        }
    }
}
