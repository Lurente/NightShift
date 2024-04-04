using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform lobbyPoint;
    public Transform exitPoint;
    public float searchRadius = 1000f;
    public float interactionDistance = 2.2f;
    public LayerMask bedLayer;

    private Transform targetBed;
    private bool reachedDestination = false;
    private bool isWaitingForBed = false;

    public void CheckForAvailableBed()
    {
        if (isWaitingForBed)
        {
            // The NPC was waiting in the lobby; try to find an unoccupied bed again.
            FindNearestUnoccupiedBed();
        }
    }
    public void LeaveBedAndExit()
    {
        StartCoroutine(MoveToExitAndDestroy());
    }
    public void StartWaitingForBed()
    {
        isWaitingForBed = true;
        // Logic to move to the lobby or perform waiting actions
    }

    public void Start()
    {
        GameObject waitingRoom = GameObject.FindGameObjectWithTag("Lobby");
        GameObject exit = GameObject.FindGameObjectWithTag("exitPoint");
        if (waitingRoom != null)
        {
            lobbyPoint = waitingRoom.transform;
        }
        else
        {
            Debug.LogError("Failed to find the Waiting Room GameObject.");
        }
        if (exit != null)
        {
            exitPoint = exit.transform;
        }
        else
        {
            Debug.LogError("Failed to find the Waiting Room GameObject.");
        }
        FindNearestUnoccupiedBed();
    }

    private void FixedUpdate()
    {
        if (!reachedDestination && targetBed != null)
        {
            MoveToTarget(); // Moved into FixedUpdate for physics consistency.
        }
    }

    private void FindNearestUnoccupiedBed()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius, bedLayer);
        float closestDistance = Mathf.Infinity;
        Transform newTargetBed = null;

        foreach (Collider collider in colliders)
        {
            BedBehavior bed = collider.GetComponent<BedBehavior>();
            if (bed != null && !bed.IsOccupied)
            {
                float distanceToBed = Vector3.Distance(transform.position, collider.transform.position);
                if (distanceToBed < closestDistance)
                {
                    closestDistance = distanceToBed;
                    newTargetBed = collider.transform;
                }
            }
        }

        if (newTargetBed != null)
        {
            targetBed = newTargetBed;
            Debug.LogWarning("Found an unoccupied bed.");
            // Optionally, start moving towards the bed here or ensure Update handles it.
        }
        else
        {
            Debug.LogWarning("No unoccupied beds found, moving to the lobby.");
            MoveToLobby(); // Call a method to handle moving to the lobby
        }
    }


    private void MoveToTarget()
    {
        if (reachedDestination) return; // No need to move if the destination is reached

        Vector3 direction = (targetBed.position - transform.position).normalized;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);

        CheckDestinationReached();
    }
    private void CheckDestinationReached()
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetBed.position);
        if (distanceToTarget < interactionDistance)
        {
            ProcessArrival();
        }
    }
    private void ProcessArrival()
    {
        if (targetBed == lobbyPoint)
        {
            reachedDestination = true;
            Debug.LogWarning("Reached lobby.");
        }
        else
        {
            BedBehavior bed = targetBed.GetComponent<BedBehavior>();
            if (bed != null && !bed.IsOccupied)
            {
                reachedDestination = true;
                bed.OccupyBed(this);
                Debug.LogWarning("Reached bed.");
                StartCoroutine(LayDown());
            }
            else
            {
                Debug.LogWarning("The bed has been occupied.");
                targetBed = null; // Search for another bed or move to the lobby.
                FindNearestUnoccupiedBed();
            }
        }
    }
    private void MoveToLobby()
    {
        if (lobbyPoint == null)
        {
            Debug.LogError("Lobby point not set.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode.
#endif
            return;
        }
        if (lobbyPoint != null)
        {
            // Update the target position to be the lobby point and reset the reached destination flag
            targetBed = lobbyPoint;
            reachedDestination = false;
        }
        else
        {
            Debug.LogError("Lobby point not set.");
        }
    }

    private IEnumerator LayDown()
    {
        Debug.LogWarning("Started laying on bed.");
        // Define the position and rotation for lying down on the bed
        Vector3 layDownPosition = targetBed.position + Vector3.up * (targetBed.localScale.y); // Adjust position to be on the surface of the bed
        Quaternion layDownRotation = Quaternion.LookRotation(targetBed.forward, targetBed.up) * Quaternion.Euler(0, 90f, 90f); // Rotate 90 degrees upwards

        // Set the NPC's position and rotation instantly to the lay down position and rotation
        transform.position = layDownPosition;
        transform.rotation = layDownRotation;

        // Indicate that the NPC has completed lying down
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // You can add more logic here if needed, such as triggering animations or other behaviors
        yield return null; // This line is added to make the function a coroutine
    }
    private IEnumerator MoveToExitAndDestroy()
    {
        // Example movement towards the exit point
        while (Vector3.Distance(transform.position, exitPoint.position) > 1f) // Arbitrary small distance
        {
            transform.position = Vector3.MoveTowards(transform.position, exitPoint.position, 5f * Time.deltaTime); // Move towards the exit
            yield return null;
        }

        Destroy(gameObject); // Destroy the NPC object once it reaches the exit point
    }

}