using System;
using UnityEngine;

public abstract class SpecialBehaviour : MonoBehaviour
{
    // Activated when the special ability is triggered
    public Action<float> OnSpecialTriggered;

    // Activated when the special ability is refreshed
    public Action OnSpecialAbilityRefreshed;

    public abstract void Activate();
}
