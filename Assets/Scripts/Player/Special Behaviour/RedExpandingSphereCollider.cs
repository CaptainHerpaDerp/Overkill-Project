using GaiaElements;
using System.Collections;
using TeamColors;
using UnityEngine;

public class RedExpandingSphereCollider : MonoBehaviour
{
    [SerializeField] private SphereCollider pulseCollider;
    [SerializeField] private float maxPulseRadius;
    [SerializeField] private float pulseSpeed;

    public void Start() 
    {
        StartCoroutine(DoPulse());
    }

    private IEnumerator DoPulse()
    {
        while (true)
        {
            if (pulseCollider.radius >= maxPulseRadius)
            {
                pulseCollider.radius = 0;
                Destroy(gameObject);
                yield break;
            }

            pulseCollider.radius += pulseSpeed * Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Plant plant))
        {
            if (plant.TeamColor == ColorEnum.TEAMCOLOR.DEFAULT)
            {
                plant.Activate(ColorEnum.TEAMCOLOR.RED);
            }
            else
            {
                plant.TeamColor = ColorEnum.TEAMCOLOR.RED;
            }
        }
    }
}
