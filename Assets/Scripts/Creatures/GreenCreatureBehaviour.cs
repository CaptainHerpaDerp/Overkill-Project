using Creatures;
using GaiaElements;
using System.Collections;
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

    // Particle System to be enabled on teleport
    [SerializeField] private GameObject particleGameObject;

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

    public override void OnTeleported()
    {
        print("green creature teleported");

        // Restart the behaviour
        StopBehaviour();

        Color newColor = sphereRenderer.material.color;
        newColor.a = 1;
        sphereRenderer.material.color = newColor;

        // Activate the particle system
        if (particleGameObject != null) 
            particleGameObject.SetActive(true);

        pulseCoroutine ??= StartCoroutine(DoPulse(2));
    }

    public override void Act()
    {
        this.gameObject.SetActive(true);

        if (pulseCoroutine != null)
            return;

        else
        {
            print("started coroutine");
            pulseCoroutine = StartCoroutine(DoPulse());
        }
    }

    public override void StopBehaviour()
    {
        sphereObject.transform.localScale = Vector3.zero;
        pulseCollider.radius = 0;

        StopAllCoroutines();
        pulseCoroutine = null;
    }

    protected override void TriggerColorChange(ColorEnum.TEAMCOLOR newColor)
    {
        print("changed color");

        StopAllCoroutines();
        pulseCoroutine = null;

        base.TriggerColorChange(newColor);
    }

    private IEnumerator DoPulse(int delay = 0)
    {
        yield return new WaitForSeconds(delay);

        while (true)
        {
            if (pulseCollider.radius >= maxPulseRadius)
            {
                sphereObject.transform.localScale = new Vector3(pulseCollider.radius * 2, pulseCollider.radius * 2, pulseCollider.radius * 2);

                StartCoroutine(FadeOutSphere());

                yield return new WaitForSeconds(pulseCooldown);

                Color newColor = sphereRenderer.material.color;
                // Reset the alpha
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
            //plant.PlantSpreadCreep = true;
            //plant.PlayerParentTransform = PlayerLocator.Instance.GetTransformOfTeam(ColorEnum.TEAMCOLOR.GREEN);

            if (plant.TeamColor == ColorEnum.TEAMCOLOR.DEFAULT)
            {
                plant.Activate(ColorEnum.TEAMCOLOR.GREEN);
            }
            else
            {
                TriggerPlantColorChange(plant, ColorEnum.TEAMCOLOR.GREEN);
            }
        }
    }
}
