using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedBehavior : MonoBehaviour
{
    public bool IsOccupied { get; private set; } = false;

    private void Awake()
    {
        IsOccupied = false; // Initialize the bed as unoccupied at the start
    }

    public void OccupyBed()
    {
        if (!IsOccupied)
        {
            IsOccupied = true;
            Debug.Log("Bed is now occupied.");
            // Add any additional logic here for when the bed becomes occupied.
            // For example, changing the bed's appearance to indicate it's in use.
        }
    }

    public void ReleaseBed()
    {
        IsOccupied = false;
        Debug.Log("Bed is now available.");
        // Add any logic here for when the bed becomes available again.
        // This might include visual cues or enabling certain interactions.
    }

    // Optional: If you want beds to directly trigger QTEs upon interaction,
    // you might include a method like this:
    public void AttemptToUseBed()
    {
        if (!IsOccupied)
        {
            // Directly trigger a QTE through the QTEManager, or notify the player that the bed is available
            Debug.Log("Bed is available, starting QTE or other interaction.");
            QTEManager.Instance.StartQTE();
        }
        else
        {
            Debug.Log("The bed is occupied.");
            // Optionally, provide feedback or trigger a different interaction if the bed is occupied.
        }
    }
}
