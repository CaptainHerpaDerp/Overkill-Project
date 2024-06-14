using GaiaElements;
using TeamColors;
using UnityEngine;

public class RedSpecialBehaviour : SpecialBehaviour
{
    [SerializeField] private float influenceRadius;

    [SerializeField] private int layerMask;

    [SerializeField] private ParticleSystem particleGameObject;

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
                if (plant.TeamColor == ColorEnum.TEAMCOLOR.DEFAULT)
                {
                    plant.Activate(ColorEnum.TEAMCOLOR.RED);
                }
                else
                {
                    plant.TeamColor = ColorEnum.TEAMCOLOR.RED;  
                }

                particleGameObject.Play();

                onCooldown = true;
                DoCooldown();
            }
        }
    }
}
