using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    private float interactionDistance = 2f; // Adjust as needed

    private void Update()
    {
        // Check for interaction with objects
        if (Input.GetKeyDown(KeyCode.F))
        {
          StartCoroutine(InteractWithNearbyObjects());
        }
    }

    private IEnumerator InteractWithNearbyObjects()
    {
        yield return new WaitForSeconds(0.5f);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance))
        {
            BedBehavior bed = hit.collider.GetComponent<BedBehavior>();
            if (bed != null)
            {
                // Ensure QTEManager.Instance is not null before calling StartQTE
                bed.AttemptToUseBed();
            }
        }
    }


}