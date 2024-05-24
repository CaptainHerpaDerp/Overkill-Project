using Creatures;
using GaiaElements;
using System.Collections;
using System.Collections.Generic;
using TeamColors;
using UnityEngine;

public class GreenCreatureBehaviour : Creature
{
    [SerializeField] private SphereCollider pulseCollider;
    [SerializeField] private GameObject sphereObject;
    [SerializeField] private MeshRenderer sphereRenderer;
    [SerializeField] private float alphaFadeSpeed;
    [SerializeField] private float maxPulseRadius;
    [SerializeField] private float pulseSpeed;
    [SerializeField] private float pulseCooldown;

    private Coroutine pulseCoroutine;
   
    public bool forceActive = false;

    //Temp
    public void Update()
    {
        if (forceActive)
        {
            Act();
            forceActive = false;
        }
    }

    public override void Act()
    {
        if (pulseCoroutine != null)
            return;

        else
        {
            pulseCoroutine = StartCoroutine(DoPulse());
        }
    }

    protected override void TriggerColorChange(ColorEnum.TEAMCOLOR newColor)
    {
        StopAllCoroutines();
        pulseCoroutine = null;

        base.TriggerColorChange(newColor);
    }

    private IEnumerator DoPulse()
    {
        while (true)
        {
            if (pulseCollider.radius >= maxPulseRadius)
            {
                sphereObject.transform.localScale = new Vector3(pulseCollider.radius * 2, pulseCollider.radius * 2, pulseCollider.radius * 2);

                StartCoroutine(FadeOutSphere());

                yield return new WaitForSeconds(pulseCooldown);

                Color newColor = sphereRenderer.material.color;
                // Reset the alhpa
                newColor.a = 1;
                sphereRenderer.material.color = newColor;
                pulseCollider.radius = 0;
             }

            sphereObject.transform.localScale = new Vector3(pulseCollider.radius * 2, pulseCollider.radius * 2, pulseCollider.radius * 2);

            pulseCollider.radius += pulseSpeed * Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator FadeOutSphere()
    {
        while(true)
        {
            if (sphereRenderer.material.color.a > 0)
            {
                Color newColor = sphereRenderer.material.color;
                newColor.a -= alphaFadeSpeed;
                sphereRenderer.material.color = newColor;
                yield return new WaitForFixedUpdate();
            }
            else
            {
                sphereObject.transform.localScale = Vector3.zero;
                yield break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Plant plant))
        {
            if (plant.TeamColor == ColorEnum.TEAMCOLOR.DEFAULT)
            {
                plant.Activate(ColorEnum.TEAMCOLOR.GREEN);
            }
            else
            {
                TriggerPlantColorChange(plant, ColorEnum.TEAMCOLOR.GREEN);
            }

            print("contact");
        }
    }
}
