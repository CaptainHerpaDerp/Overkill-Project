using System.Collections;
using UnityEngine;

namespace Creatures
{
    public class CreatureMovingColorCollision : MonoBehaviour
    {
        private CreatureMovingColorChange movingChangeScript;

        [SerializeField] private SphereCollider sphereCollider;

        [SerializeField] private float convertSpeed;

        private void Start()
        {
            movingChangeScript = GetComponentInParent<CreatureMovingColorChange>();

            if (movingChangeScript == null)
            {
                Debug.LogError("MovingColorChange script not found in parent object.");
            }

            StartCoroutine(DetectPlantsInCollider());
        }

        // Get all plants within the collider
        private IEnumerator DetectPlantsInCollider()
        {
            while (true)
            {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereCollider.radius);

                foreach (var hitCollider in hitColliders)
                {
                    movingChangeScript.OnColorTriggerEnter(hitCollider);
                }

                yield return new WaitForSeconds(convertSpeed);
            }
        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    if (movingChangeScript != null)
        //    {
        //        movingChangeScript.OnColorTriggerEnter(other);
        //    }
        //    else
        //    {
        //        Debug.LogError("MovingColorChange script not found in parent object.");
        //    }
        //}
    }
}