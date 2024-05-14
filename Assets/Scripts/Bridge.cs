using UnityEngine;

public class Bridge : MonoBehaviour
{
    [SerializeField] private Bridge otherBridge;

    [SerializeField] private Transform teleportationSpot;

    [SerializeField] private GameObject objectToEnable;

    public bool IsBridgeActive;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && IsBridgeActive)
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

            // Get the difference between the player and the bridge
            Vector3 difference = other.transform.position - transform.position;

            other.transform.parent.gameObject.transform.position = otherBridge.transform.position + difference;
            otherBridge.IsBridgeActive = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsBridgeActive = true;
        }
    }
}
