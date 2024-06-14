using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BadgeType { MostDeaths, MostKills, MostCreaturesTamed, MostTimeWinning, MostCamper, MostMover }

public class BadgeAssigner : MonoBehaviour
{

    /* The list of the parents with layout groups. 
     Positions of the children of these parents should be used as placement marks for the badges */
    private List<GameObject> playerBadgePositionGroups;

    [HideInInspector] public List<GameObject> BadgePrefabs = new();
    [HideInInspector] public List<BadgeType> BadgeTypes = new();

    private IEnumerator RandomlyAssignBadges()
    {
        while (true)
        {
            for (int i = 0; i < 10; i++)
            {

            }

            yield return new WaitForSeconds(1);
        }
    }
}


