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

    [SerializeField] protected float cooldownTime = 10;    

    public virtual bool Activate()
    {
        return false;
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
