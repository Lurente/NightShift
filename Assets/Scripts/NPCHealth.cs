using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    void Start()
    {
        // Initialize the NPC's health at the start.
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        // Check if the NPC is still alive.
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        // Handle the NPC's death here (e.g., disable the NPC, play death animation).
    }

    void OnEnable()
    {
        QTEManager.QTEFailedBeyondLimit += HandleQTEFailure;
    }

    void OnDisable()
    {
        QTEManager.QTEFailedBeyondLimit -= HandleQTEFailure;
    }

    private void HandleQTEFailure()
    {
        TakeDamage(1);
    }
}
