using System.Collections.Generic;
using UnityEngine;

namespace TeamColors
{
    /// <summary>
    /// Keeps track of the player's position and team color.
    /// </summary>
    public class AnimalLocator : MonoBehaviour
    {
        public static AnimalLocator Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("AnimalLocator already exists in the scene. Deleting this instance.");
                Destroy(this);
            }
        }

        private Dictionary<Transform, ColorEnum.TEAMCOLOR> animalTransformPairs = new Dictionary<Transform, ColorEnum.TEAMCOLOR>();

        public Transform GetClosestTransformOfTeam(Vector3 fromPosition, ColorEnum.TEAMCOLOR teamColor)
        {
               Transform closestTransform = null;
                float closestDistance = float.MaxValue;
    
                foreach (var animalTransformPair in animalTransformPairs)
            {
                    if (animalTransformPair.Value == teamColor)
                {
                        float distance = Vector3.Distance(fromPosition, animalTransformPair.Key.position);
    
                        if (distance < closestDistance)
                    {
                            closestDistance = distance;
                            closestTransform = animalTransformPair.Key;
                        }
                    }
                }
    
                return closestTransform;        
        }

        public void RegisterAnimalOfTeam(Transform animalTransform, ColorEnum.TEAMCOLOR teamColor)
        {
            print("Registered animal of team " + teamColor);
            animalTransformPairs.Add(animalTransform, teamColor);
        }

        public void ChangeTeamOfAnimal(Transform animalTransform, ColorEnum.TEAMCOLOR newTeamColor)
        {
            if (animalTransformPairs.ContainsKey(animalTransform))
            {
                animalTransformPairs[animalTransform] = newTeamColor;
            }
        }
    }
}