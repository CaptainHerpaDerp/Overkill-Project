using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantAnimationListener : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void OnPlantAnimationEnd()
    {
        animator.enabled = false;
    }
}
