using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantAnimationListener : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private MeshRenderer meshRenderer;

    public void OnPlantAnimationEnd()
    {
        animator.enabled = false; 
    }

    public void OnPlantAnimationEndAndHide()
    {
        animator.enabled = false;
        meshRenderer.enabled = false;
    }
}
