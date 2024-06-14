using GaiaElements;
using System.Collections;
using TeamColors;
using UnityEngine;

public class RedSpecialBehaviour : SpecialBehaviour
{
    [SerializeField] private float influenceRadius;

    [SerializeField] private int layerMask;

    [SerializeField] private ParticleSystem particleGameObject;
    [SerializeField] private RedExpandingSphereCollider sphereCollider;

    public override void Activate()
    {
        if (onCooldown)
        {
            return;
        }

        ParticleSystem newSystem = Instantiate(particleGameObject.gameObject, transform.position, particleGameObject.transform.rotation).GetComponent<ParticleSystem>();
        RedExpandingSphereCollider newSphereCollider = Instantiate(sphereCollider.gameObject, transform.position, sphereCollider.transform.rotation).GetComponent<RedExpandingSphereCollider>();

        newSystem.Play();
        StartCoroutine(DestroyParticlesOnFinish(newSystem));

        onCooldown = true;
        DoCooldown();
    }

    // Track a particle system and destroy it when it finishes
    private IEnumerator DestroyParticlesOnFinish(ParticleSystem system)
    {
        yield return new WaitUntil(() => system.isStopped);
        Destroy(system.gameObject);
    }
}
