using Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class FogPoint : MonoBehaviour
{
    private HashSet<Player> players = new();
    [SerializeField] private float duration;
    [SerializeField] private float inuenceRadius;
    [SerializeField] private float distanceModifier;
    [SerializeField] private float timeModifier;

    private float fogStrength;

    private void Start()
    {
        fogStrength = 1;

        StartCoroutine(ApplyFogEffects());
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Player player) && !players.Contains(player))
        {
            players.Add(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player) && players.Contains(player))
        {
            players.Remove(player);
            RemoveFogEffectFromPlayer(player);
        }
    }

    private IEnumerator ApplyFogEffects()
    {
        while (true)
        {
            foreach (Player player in players)
            {
                Volume volume = player.GetComponentInChildren<Volume>();
                for (int i = 0; i < volume.profile.components.Count; i++)
                {
                    if (volume.profile.components[i].name == "Fog")
                    {
                        Fog fog = (Fog)volume.profile.components[i];
                        fog.enabled.value = true;
                        fog.maxFogDistance.value = 2;
                    }
                } 
            }



            yield return new WaitForSeconds(0.25f);
        } 
    }

    private void RemoveFogEffectFromPlayer(Player player)
    {
        Volume volume = player.GetComponentInChildren<Volume>();
        for (int i = 0; i < volume.profile.components.Count; i++)
        {
            if (volume.profile.components[i].name == "Fog")
            {
                Fog fog = (Fog)volume.profile.components[i];
                fog.enabled.value = false;
            }
        }
    }
}
