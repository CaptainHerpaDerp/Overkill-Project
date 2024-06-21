using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameManagement;
using UI;
using Players;

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
        [SerializeField] private Transform badgeParent;

        private Dictionary<BadgeType, GameObject> badgeTypeGameObjectPairs = new();

        // A dictionary to keep track of the next child index of the layout group badge placements
        private Dictionary<int, int> ChildActivationIndex = new();

        [Header("The time that the badge will take to move to its position (match with badge animation time")]
        [SerializeField] private float newBadgeSpeed = 2.5f;

        [SerializeField] private float postBadgeEndTime;

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
            GameManager.Instance.OnGameReload += ResetBadgeAssigner;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnAssignBadges -= StartBadgeAssign;
            GameManager.Instance.OnGameReload -= ResetBadgeAssigner;
        }

        private void StartBadgeAssign(float startDelay, List<Player> placementList)
        {
            StartCoroutine(AssignPlacementsAndBadges(startDelay, placementList));
        }

        private void ResetBadgeAssigner()
        {
            foreach (GameObject placementNumber in placementNumbersParent)
            {
                placementNumber.gameObject.SetActive(false);
            }

            foreach (Transform child in badgeParent)
            {
                Destroy(child.gameObject);
            }

            //foreach (var kvp in ChildActivationIndex)
            //{
            //    ChildActivationIndex[kvp.Key] = 0;
            //}

            //foreach (KeyValuePair<int, int> kvp in ChildActivationIndex)
            //{
            //    ChildActivationIndex[kvp.Key] = 0;
            //}
        }

        private IEnumerator AssignPlacementsAndBadges(float startDelay, List<Player> placementList)
        {
            yield return new WaitForSeconds(startDelay);

            // Show all placement numbers with a delay in between each
            for (int i = 0; i < placementNumbersParent.Count; i++)
            {
                // Only show the placement number if the index of the placement number is less than or equal to the player count
                if (i <= placementList.Count)
                {
                    placementNumbersParent[i].gameObject.SetActive(true);
                    yield return new WaitForSeconds(placementDisplayDelay);
                }
            }           

            // Get length of AchievementType enum
            int achievementTypeLength = System.Enum.GetValues(typeof(AchievementType)).Length;

            while (true)
            {
                for (int i = 0; i < achievementTypeLength; i++)
                {
                    print("Assigning badge: " + (AchievementType.MostDeaths + i));
                    int playerIndex = achievementManager.GetIndexOfAchievementWinner(AchievementType.MostDeaths + i);
                    int playerPlacementIndex = placementList.IndexOf(achievementManager.GetAchievementWinner(AchievementType.MostDeaths + i));

                    // Get the index of the enum value
                    int badgeIndex = (int)BadgeTypes[i];

                    // If 1 is returned, no player has achieved this badge
                    if (playerIndex == -1)
                    {
                        print("no winner for badge: " + (AchievementType.MostDeaths + i));
                        continue;
                    }

                    // The value of the badge (eg. Kill Count)
                    string badgeValue = achievementManager.GetAchievementValue((AchievementType.MostDeaths + i), playerIndex);
                    
                    // If badgeValue is 0, no player has achieved this badge
                    if (badgeValue == "none")
                    {
                        print("no winner for badge: " + (AchievementType.MostDeaths + i));
                        continue;
                    }

                    if (ChildActivationIndex[playerIndex] >= playerBadgePositionGroups[playerIndex].transform.childCount)
                    {
                        Debug.LogError("The badge placement index is out of range. Please check the layout group child count.");
                    }

                    Debug.Log($"Awarding badge {(AchievementType.MostDeaths + i)} to player {playerIndex} with placement {playerPlacementIndex}");
                    
                    AwardBadgeMovement badge = Instantiate(badgeTypeGameObjectPairs[(BadgeType)badgeIndex], Vector3.zero, Quaternion.identity, parent: badgeParent).GetComponent<AwardBadgeMovement>();

                    // Move the badge to the placement index of the player 

                    badge.AnimateToPosition(playerBadgePositionGroups[playerPlacementIndex].transform.GetChild(ChildActivationIndex[playerIndex]).position);
                    badge.transform.localPosition = Vector3.zero;
                    badge.SetBadgeText(badgeValue);

                    ChildActivationIndex[playerIndex]++;
                        
                    yield return new WaitForSeconds(newBadgeSpeed);
                }

                yield return new WaitForSeconds(postBadgeEndTime);

                GameManager.Instance.RestartGame();

                yield break;
            }
        }
    }
}
