using System;
using System.Collections;
using UnityEngine;

public class SpecialBehaviour : MonoBehaviour
{
    // Activated when the special ability is triggered
    public Action<float> OnSpecialTriggered;

    // Activated when the special ability is refreshed
    public Action OnSpecialAbilityRefreshed;

    protected bool onCooldown = false;

    protected float cooldownTime;    

    public virtual void Activate()
    {

    }

    public virtual void DoCooldown()
    {
        OnSpecialTriggered?.Invoke(cooldownTime);
        onCooldown = true;
        StartCoroutine(DoCooldownCoroutine());
    }

    private IEnumerator DoCooldownCoroutine()
    {
        yield return new WaitForSeconds(cooldownTime);

        onCooldown = false;
        OnSpecialAbilityRefreshed?.Invoke();
    }
}
