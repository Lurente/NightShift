using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedBehavior : MonoBehaviour
{
    public bool IsOccupied = false;
    public bool IsReserved = false;
    public float maxOccupancyDuration = 60f; // Max wait time in seconds
    private Coroutine occupancyCoroutine;
    public NPCController currentOccupant;
    public delegate void BedAvailabilityHandler();
    private void Awake()
    {
        IsOccupied = false; // Initialize the bed as unoccupied at the start
    }
    public bool TryReserveBed()
    {
        if (!IsOccupied && !IsReserved)
        {
            IsReserved = true;
            Debug.Log("Bed reserved.");
            // Optionally, update the bed's appearance or state to indicate it's reserved
            return true;
        }
        return false;
    }

    public void OccupyBed(NPCController npc)
    {
        if (!IsOccupied)
        {
            IsOccupied = true;
            currentOccupant = npc;
            if (occupancyCoroutine != null)
            {
                StopCoroutine(occupancyCoroutine);
            }
            occupancyCoroutine = StartCoroutine(OccupancyDuration());
        }
    }

    private IEnumerator OccupancyDuration()
    {
        yield return new WaitForSeconds(maxOccupancyDuration);
        Debug.LogWarning("GRRRR TIRED OF WAITING!");
        ReleaseBed();
    }
    public void ReleaseBed()
    {
        IsOccupied = false;
        if (currentOccupant != null)
        {
            currentOccupant.LeaveBedAndExit();
            currentOccupant = null;
        }
        StopCoroutine(occupancyCoroutine); // Assuming this is defined elsewhere to manage occupancy time
        LobbyQueue.NotifyNextNPC();
    }

    public void AttemptToUseBed()
{
    // Start the QTE only if the bed is occupied, meaning there's a patient present.
    // Avoid starting new interactions if the bed is merely reserved for someone else.
    if (IsOccupied && !IsReserved)
    {
        Debug.Log("Interacting with patient in the bed.");
        QTEManager.Instance.StartQTE(); // Start the QTE for patient interaction.
    }
    else if (!IsOccupied && IsReserved)
    {
        Debug.Log("The bed is reserved for another patient.");
        // Optionally handle the case where the bed is reserved but not yet occupied.
    }
    else if (!IsOccupied)
    {
        Debug.Log("The bed is currently unoccupied.");
        // Handle the case where there's no patient to interact with.
    }
}
}
