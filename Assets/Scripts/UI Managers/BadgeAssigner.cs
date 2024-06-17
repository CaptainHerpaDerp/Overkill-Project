using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameManagement;
using UI;

namespace UIManagement
{
    public enum BadgeType { MostDeaths, MostKills, MostCreaturesTamed, MostTimeWinning, MostCamper, MostMover }

    public class BadgeAssigner : MonoBehaviour
    {
        /* The list of the parents with layout groups. 
         Positions of the children of these parents should be used as placement marks for the badges */
        [SerializeField] private List<GameObject> playerBadgePositionGroups;

        [Header("The list of the parents with placement numbers")]
        [SerializeField] private List<GameObject> placementNumbersParent;

        [Header("The delay between each revealed placement")]
        [SerializeField] private float placementDisplayDelay = 1.5f;

        // An inspector editor will show these two lists as a dictionary
        [HideInInspector] public List<GameObject> BadgePrefabs = new();
        [HideInInspector] public List<BadgeType> BadgeTypes = new();

        private Dictionary<BadgeType, GameObject> badgeTypeGameObjectPairs = new();

        // A dictionary to keep track of the next child index of the layout group badge placements
        private Dictionary<int, int> ChildActivationIndex = new();

        [Header("The time that the badge will take to move to its position (match with badge animation time")]
        [SerializeField] private float newBadgeSpeed = 2.5f;

        private AchievementManager achievementManager;

        private void Start()
        {
            achievementManager = AchievementManager.Instance;

            for (int i = 0; i < playerBadgePositionGroups.Count; i++)
            {
                ChildActivationIndex.Add(i, 0);
            }

            for (int i = 0; i < placementNumbersParent.Count; i++)
            {
                placementNumbersParent[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < BadgeTypes.Count; i++)
            {
                badgeTypeGameObjectPairs.Add(BadgeTypes[i], BadgePrefabs[i]);
            }
        }

        private void OnEnable()
        {
            GameManager.Instance.OnAssignBadges += StartBadgeAssign;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnAssignBadges -= StartBadgeAssign;
        }

        private void StartBadgeAssign(float startDelay)
        {
            StartCoroutine(AssignPlacementsAndBadges(startDelay));
        }

        private IEnumerator AssignPlacementsAndBadges(float startDelay)
        {
            yield return new WaitForSeconds(startDelay);

            // Show all placement numbers with a delay in between each
            for (int i = 0; i < placementNumbersParent.Count; i++)
            {
                placementNumbersParent[i].gameObject.SetActive(true);
                yield return new WaitForSeconds(placementDisplayDelay);
            }           

            // Get length of AchievementType enum
            int achievementTypeLength = System.Enum.GetValues(typeof(AchievementType)).Length;

            while (true)
            {
                for (int i = 0; i < achievementTypeLength; i++)
                {
                    print("Assigning badge: " + (AchievementType.MostDeaths + i));

                    int winnerIndex = achievementManager.GetIndexOfAchievementWinner(AchievementType.MostDeaths + i);

                    // Get the index of the enum value
                    int badgeIndex = (int)BadgeTypes[i];

                    // If 1 is returned, no player has achieved this badge
                    if (winnerIndex == -1)
                    {
                        print("no winner for badge: " + (AchievementType.MostDeaths + i));
                        continue;
                    }

                    // The value of the badge (eg. Kill Count)
                    string badgeValue = achievementManager.GetAchievementValue((AchievementType.MostDeaths + i), winnerIndex);

                    if (ChildActivationIndex[winnerIndex] >= playerBadgePositionGroups[winnerIndex].transform.childCount)
                    {
                        Debug.LogError("The badge placement index is out of range. Please check the layout group child count.");
                    }
                    
                    AwardBadgeMovement badge = Instantiate(badgeTypeGameObjectPairs[(BadgeType)badgeIndex], Vector3.zero, Quaternion.identity, parent: this.transform).GetComponent<AwardBadgeMovement>();
                    badge.AnimateToPosition(playerBadgePositionGroups[winnerIndex].transform.GetChild(ChildActivationIndex[winnerIndex]).position);
                    badge.transform.localPosition = Vector3.zero;
                    badge.SetBadgeText(badgeValue);

                    ChildActivationIndex[winnerIndex]++;

                    yield return new WaitForSeconds(newBadgeSpeed);
                }

          
                yield break;
            }
        }
    }
}
