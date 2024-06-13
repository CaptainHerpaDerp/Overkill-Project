using GaiaElements;
using TeamColors;
using UnityEngine;


public class BlueSpecialBehaviour : SpecialBehaviour
{
    [SerializeField] private GameObject smokeObjectPrefab;
    [SerializeField] private FogPoint fogPointPrefab;

    [SerializeField] private Vector3 smokeOffset;

    [SerializeField] private float influenceRadius;

    [SerializeField] private int layerMask;

    [SerializeField] private float scaleModMin, scaleModMax;

    [SerializeField] private int spawnChance;

    public override void Activate()
    {
        if (onCooldown)
        {
            return;
        }

        // OverlapSphere is a physics function that returns all colliders within a sphere

        Collider[] colliders = Physics.OverlapSphere(transform.position, influenceRadius);

        print($"Red special activated: {colliders.Length}");

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Plant plant))
            {
                if (plant.TeamColor != ColorEnum.TEAMCOLOR.BLUE)
                    continue;

                if (Random.Range(0, spawnChance) != 0)
                    continue;
                    
                // Create a smoke screen box at the plant's position
                GameObject smokeScreenBox = Instantiate(smokeObjectPrefab, plant.transform.position + smokeOffset, Quaternion.identity);

                // Set the smoke screen box's scale to a random value between scaleModMin and scaleModMax
                smokeScreenBox.transform.localScale = Vector3.one * Random.Range(scaleModMin, scaleModMax);
            }
        }

        onCooldown = true;
        DoCooldown();
    }
}
