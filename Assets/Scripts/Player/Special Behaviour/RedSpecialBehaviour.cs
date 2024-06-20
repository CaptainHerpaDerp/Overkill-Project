using GaiaElements;
using System.Collections;
using TeamColors;
using UnityEngine;
using Core;
using Players;

public class RedSpecialBehaviour : SpecialBehaviour
{
    [SerializeField] private float influenceRadius;

    [SerializeField] private int layerMask;

    [SerializeField] private ParticleSystem particleGameObject;
    [SerializeField] private RedExpandingSphereCollider sphereCollider;

    [Header("The time before the ability takes effect")]
    [SerializeField] private float activationDelay;

    public override bool Activate()
    {
        if (onCooldown)
        {
            return false;
        }

        StartCoroutine(DoAbility());

        return true;
    }

    private IEnumerator DoAbility()
    {
        onCooldown = true;

        playerModelController.PlayAnimation(Players.AnimationState.Special);

        yield return new WaitForSeconds(activationDelay);

        SoundManager.Instance.PlayRedAbilityAtPoint(transform.position);

        ParticleSystem newSystem = Instantiate(particleGameObject.gameObject, transform.position, particleGameObject.transform.rotation).GetComponent<ParticleSystem>();
        RedExpandingSphereCollider newSphereCollider = Instantiate(sphereCollider.gameObject, transform.position, sphereCollider.transform.rotation).GetComponent<RedExpandingSphereCollider>();

        newSystem.Play();
        StartCoroutine(DestroyParticlesOnFinish(newSystem));
        DoCooldown();
    }

    // Track a particle system and destroy it when it finishes
    private IEnumerator DestroyParticlesOnFinish(ParticleSystem system)
    {
        yield return new WaitUntil(() => system.isStopped);
        Destroy(system.gameObject);
    }
}
