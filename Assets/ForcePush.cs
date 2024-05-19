using System.Collections.Generic;
using UnityEngine;

public class ForcePush : MonoBehaviour
{
    public float force = 10f;
    public ForceMode forceMode = ForceMode.Force;

    private HashSet<Rigidbody> rigidbodies = new();

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb;

        if (other.transform == this.transform.parent)
        {
            print("Ignoring parent");
            return;
        }

        if (!other.TryGetComponent(out rb))
            return;

        if (rigidbodies.Contains(rb))
        {
            return;
        }

        print ("Adding force to " + rb.name);
        rigidbodies.Add(rb);
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb;

        if (!other.TryGetComponent(out rb))
            return;

        if (rigidbodies.Contains(rb))
        {
            print ("Removing force from " + rb.name);
            rigidbodies.Remove(rb);
        }
    }

    private void Update()
    {

        foreach (var item in rigidbodies)
        {
            Vector3 diff = (item.transform.position - transform.position).normalized;
            item.AddForce(diff * force, forceMode);
        }
    }

}
