using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEManager : MonoBehaviour
{
    public enum DifficultyLevel
    {
        Level1,
        Level2,
        Level3,
        Level4
    }
    [SerializeField] private int maxFailuresAllowed = 2;
    public delegate void OnQTEFailure();
    public static event OnQTEFailure QTEFailedBeyondLimit;
    public DifficultyLevel currentDifficulty = DifficultyLevel.Level1;
    public float timeLimitPerQTE = 5f; // Time limit for each QTE
    public int numberOfQTEs = 3; // Number of QTEs for each level

    private List<KeyCode> keySymbols = new List<KeyCode> { KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.DownArrow };
    private Dictionary<KeyCode, string> keyCodeToSymbol = new Dictionary<KeyCode, string>
{
    { KeyCode.UpArrow, "up" },
    { KeyCode.DownArrow, "down" },
    { KeyCode.LeftArrow, "left" },
    { KeyCode.RightArrow, "right" }
};

    private List<KeyCode[]> qteSequences = new List<KeyCode[]>(); // List to store QTE sequences
    public static QTEManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep the manager across scenes.
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        GenerateQTESequences();
    }

    private void GenerateQTESequences()
    {
        switch (currentDifficulty)
        {
            case DifficultyLevel.Level1:
                GenerateRandomSequences(3, 3, 5);
                break;
            case DifficultyLevel.Level2:
                GenerateRandomSequences(3, 5, 7);
                break;
            case DifficultyLevel.Level3:
                GenerateRandomSequences(3, 6, 7);
                break;
            case DifficultyLevel.Level4:
                GenerateRandomSequences(3, 8, 6);
                break;
        }
    }
    private void GenerateRandomSequences(int sequenceLength, int timeLimitPerQTE, int numberOfSequences)
    {
        for (int i = 0; i < numberOfSequences; i++)
        {
            KeyCode[] sequence = new KeyCode[sequenceLength];
            for (int j = 0; j < sequenceLength; j++)
            {
                sequence[j] = keySymbols[Random.Range(0, keySymbols.Count)];
            }
            qteSequences.Add(sequence);
        }
        this.timeLimitPerQTE = timeLimitPerQTE;
    }

    public void StartQTE()
    {
        StartCoroutine(PerformQTE());
    }

    private IEnumerator PerformQTE()
    {
        int failedSequencesCount = 0;

        foreach (KeyCode[] sequence in qteSequences)
        {
            ShowSequenceToPlayer(sequence);
            bool sequenceCompleted = true;
            int currentIndex = 0;
            float timer = 0f;

            while (timer < timeLimitPerQTE && currentIndex < sequence.Length)
            {
                if (Input.GetKeyDown(sequence[currentIndex]))
                {
                    Debug.Log($"Correct input: {sequence[currentIndex]}");
                    currentIndex++; // Move to the next key in the sequence
                }
                else if (Input.anyKeyDown)
                {
                    sequenceCompleted = false;
                    Debug.Log("Wrong input received.");
                    break; // Exit the loop as the sequence has failed
                }

                timer += Time.deltaTime;
                yield return null; // Wait until the next frame to continue checking
            }

            if (!sequenceCompleted || currentIndex < sequence.Length)
            {
                failedSequencesCount++;
                if (failedSequencesCount > maxFailuresAllowed)
                {
                    Debug.Log("Failed the interaction. NPC will lose one life.");
                    QTEFailedBeyondLimit?.Invoke(); // Notify of QTE failure beyond limit.
                    break; // Exit the QTE sequence early due to excessive failures.
                }
            }

            yield return new WaitForSeconds(1f); // Delay between QTEs
        }

        if (failedSequencesCount <= maxFailuresAllowed)
        {
            Debug.Log("All QTEs completed successfully or with minor failures.");
        }
    }

    private void ShowSequenceToPlayer(KeyCode[] sequence)
    {
        string sequenceSymbols = string.Join(" ", sequence.Select(kc => keyCodeToSymbol.ContainsKey(kc) ? keyCodeToSymbol[kc] : kc.ToString()));
        Debug.Log("QTE sequence: " + sequenceSymbols);
        // Consider replacing this with actual UI display logic.
    }
}