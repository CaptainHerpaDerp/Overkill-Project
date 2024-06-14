using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BadgeType { MostDeaths, MostKills, MostCreaturesTamed, MostTimeWinning, MostCamper, MostMover }

public class BadgeAssigner : MonoBehaviour
{

    /* The list of the parents with layout groups. 
     Positions of the children of these parents should be used as placement marks for the badges */
    [SerializeField] private List<GameObject> playerBadgePositionGroups;

    [HideInInspector] public List<GameObject> BadgePrefabs = new();
    [HideInInspector] public List<BadgeType> BadgeTypes = new();

    private Dictionary<int, int> ChildActivationIndex = new();

    [SerializeField] private float newBadgeSpeed = 2.5f;

    private void Start()
    {
        for (int i = 0; i < playerBadgePositionGroups.Count; i++)
        {
            ChildActivationIndex.Add(i, 0);
        }

        StartCoroutine(RandomlyAssignBadges());

    }

    private IEnumerator RandomlyAssignBadges()
    {
        while (true)
        {
            for (int i = 0; i < 10; i++)
            {
                int parentIndex = Random.Range(0, playerBadgePositionGroups.Count);

                if (ChildActivationIndex[parentIndex] >= playerBadgePositionGroups[parentIndex].transform.childCount)
                {
                    i--;
                    continue;
                }

                Transform randomParent = playerBadgePositionGroups[parentIndex].transform;

                AwardBadgeMovement badge = Instantiate(BadgePrefabs[Random.Range(0, BadgePrefabs.Count)], Vector3.zero, Quaternion.identity, parent: this.transform).GetComponent<AwardBadgeMovement>();
                badge.AnimateToPosition(randomParent.GetChild(ChildActivationIndex[parentIndex]).position);
                badge.transform.localPosition = Vector3.zero;

                ChildActivationIndex[parentIndex]++;

                yield return new WaitForSeconds(newBadgeSpeed); 
            }

            yield break;
        }
    }
}


