using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{
    public class LaserTargetFinder : MonoBehaviour
    {
        [SerializeField] private Player[] targetPlayers;
        [SerializeField] private Player exclusionPlayer;

        [SerializeField] private float minAngle;

        private void OnEnable()
        {
            targetPlayers = GameObject.FindObjectsOfType<Player>();
        }

        public Player GetPlayerTarget(float pDistance)
        {
            float smallestAngle = 360;
            Player returnPlayer = null;

            foreach (Player player in targetPlayers)
            {
                if (player == exclusionPlayer)
                {
                    continue;
                }

                // Get the forward vector of the current object
                Vector3 forward = transform.forward;

                // Calculate the direction vector from the object to the target
                Vector3 toTarget = (player.transform.position - transform.position).normalized;

                // Calculate the angle between the forward vector and the direction to the target
                float angle = Vector3.Angle(forward, toTarget);
                float distance = Vector3.Distance(transform.position, player.transform.position);

                if (angle < smallestAngle && angle < minAngle && distance <= pDistance)
                {
                    smallestAngle = angle;
                    returnPlayer = player;
                }
            }

            return returnPlayer;
        }
    }
}