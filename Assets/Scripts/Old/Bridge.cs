using System;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public bool PlayerInCollider;

    public Action<GameObject> OnPlayerContact;

    public bool Acessible = true;

    private void OnTriggerEnter(Collider other)
    {
        if (Acessible == false)
            return;

        if (PlayerInCollider == true)
            return;

        if (other.CompareTag("Player"))
        {
            PlayerInCollider = true;
            OnPlayerContact?.Invoke(other.gameObject);
        }
    }

    // Ensure the player has exited the bridge before reactivating it
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInCollider = false;
        }
    }
}
