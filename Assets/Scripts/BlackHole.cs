using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField] private GameObject parentPlayer;
    [SerializeField] private float pullForce = 10f;
    [SerializeField] private float radius = 10f;

    private void Start()
    {
        // Set the sphere collider radius to the push distance
        GetComponent<SphereCollider>().radius = radius;
    }

    private void OnTriggerStay(Collider other)
    {
        if (parentPlayer != null && other.gameObject == parentPlayer)
        {
            print("Ignoring parent");
            return;
        }

        // Pull the object towards the black hole

        if (other.TryGetComponent(out Rigidbody rb))
        {
            Vector3 direction = (transform.position - other.transform.position).normalized;
            rb.AddForce(direction * pullForce / Vector3.Distance(transform.position, other.transform.position), ForceMode.Force);
        }
    }
}
