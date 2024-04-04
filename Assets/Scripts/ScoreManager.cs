using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score { get; private set; }

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        QTEManager.OnSuccessfulQTECompletion += HandleSuccessfulQTECompletion;
    }

    private void OnDisable()
    {
        QTEManager.OnSuccessfulQTECompletion -= HandleSuccessfulQTECompletion;
    }

    private void HandleSuccessfulQTECompletion()
    {
        Score += 100; // Example score increment
        Debug.Log($"Score Updated: {Score}");
        // Implement patient release logic here or call another method responsible for it.
    }
}
