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
            StartCoroutine(StartQTEWithDelay());
        }
    }

    private void InteractWithNearbyObjects()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance))
        {
            BedBehavior bed = hit.collider.GetComponent<BedBehavior>();
            if (bed != null)
            {
                // Ensure QTEManager.Instance is not null before calling StartQTE
                if (QTEManager.Instance != null)
                {
                    QTEManager.Instance.StartQTE();
                }
                else
                {
                    Debug.LogError("QTEManager.Instance is null");
                }
            }
        }
    }
    private IEnumerator StartQTEWithDelay()
    {
        // Wait for half a second
        yield return new WaitForSeconds(0.5f);

        // Check if the QTEManager instance is available and start the QTE
        if (QTEManager.Instance != null)
        {
            QTEManager.Instance.StartQTE();
        }
        else
        {
            Debug.LogError("QTEManager.Instance is null.");
        }
    }


}