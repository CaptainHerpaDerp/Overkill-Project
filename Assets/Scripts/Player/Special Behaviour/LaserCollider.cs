using Players;
using System.Collections.Generic;
using UnityEngine;

public class LaserCollider : MonoBehaviour
{
    public CapsuleCollider capsuleCollider;

    public HashSet<Player> PlayersInCollider = new();
     
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player) && !PlayersInCollider.Contains(player))
        {
            PlayersInCollider.Add(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player) && PlayersInCollider.Contains(player))
        {
            PlayersInCollider.Remove(player);
        }
    }
}
