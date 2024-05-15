using UnityEngine;

public class Bridge : MonoBehaviour
{
    [SerializeField] private Bridge otherBridge;

    [SerializeField] private GameObject objectToEnable;

    [Header("Determines if the player can enter through this bridge")]
    public bool IsBridgeUsable;

    // public for debugging purposes, should be private later
    [SerializeField] private bool isBridgeActive;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsBridgeUsable)
        {
            return;
        }

        if (other.CompareTag("Player") && isBridgeActive)
        {
            Debug.Log("Bridge");

            if (objectToEnable != null)
            {
                objectToEnable.SetActive(false);
            }

            if (otherBridge.objectToEnable != null)
            {
                otherBridge.objectToEnable.SetActive(true);
            }

            otherBridge.isBridgeActive = false;

            // Get the difference between the player and the bridge
            Vector3 difference = other.transform.position - transform.position;

           // other.transform.parent.gameObject.transform.position = otherBridge.transform.position + difference;
            other.transform.position = otherBridge.transform.position + difference;
        } 
    }

    // Ensure the player has exited the bridge before reactivating it
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isBridgeActive = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Draw a sphere at the position of the bridge
        Gizmos.DrawWireSphere(transform.position, 0.3f);

        if (otherBridge != null)
        Gizmos.DrawLine(transform.position, otherBridge.transform.position);
    }
}
