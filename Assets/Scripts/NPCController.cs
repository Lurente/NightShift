using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public float moveSpeed = 2f; // Movement speed of the NPC
    public float searchRadius = 1000f; // Radius to search for unoccupied beds
    public float interactionDistance = 2.2f; // Distance to consider the NPC close enough to interact with the bed
    public LayerMask bedLayer; // Layer mask to filter beds

    private Transform targetBed; // Reference to the nearest unoccupied bed
    private bool reachedDestination = false;

    private void Start()
    {
        FindNearestUnoccupiedBed();
        if (targetBed != null)
        {
            MoveTowardsBed();
        }
        else
        {
            Debug.LogWarning("No unoccupied beds found.");
        }
    }

    private void Update()
    {
        // Check if the NPC has reached its destination (the bed)
        if (!reachedDestination && targetBed != null)
        {
            MoveTowardsBed();
        }
    }

    private void FindNearestUnoccupiedBed()
    {
        Debug.LogWarning("Looking for a bed.");
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius, bedLayer);
        float closestDistance = Mathf.Infinity;
        foreach (Collider collider in colliders)
        {
            BedBehavior bed = collider.GetComponent<BedBehavior>();
            if (bed != null && !bed.IsOccupied) // Corrected method name here
            {
                Debug.LogWarning("Found a bed.");
                float distanceToBed = Vector3.Distance(transform.position, collider.transform.position);
                if (distanceToBed < closestDistance)
                {
                    closestDistance = distanceToBed;
                    targetBed = collider.transform;
                }
            }
        }
    }

    private void MoveTowardsBed()
    {
        // Calculate the direction to the bed
        Vector3 direction = (targetBed.position - transform.position).normalized;

        // Move the NPC towards the bed using Rigidbody's velocity
        // Adjust moveSpeed according to your needs
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = direction * moveSpeed;

        // Check if the NPC has reached the bed
        float distanceToBed = Vector3.Distance(transform.position, targetBed.position);
        if (distanceToBed < interactionDistance)
        {
            reachedDestination = true;
            // Occupy the bed
            BedBehavior bed = targetBed.GetComponent<BedBehavior>();
            if (bed != null)
            {
                bed.OccupyBed();
                Debug.LogWarning("Reached bed.");
                StartCoroutine(layDown());
            }
        }
    }
    private IEnumerator layDown()
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

}