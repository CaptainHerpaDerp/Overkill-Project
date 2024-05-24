using System;
using System.Collections;
using UnityEngine;
using GaiaElements;
namespace Creatures
{
    public class GreenExpandingSphere : MonoBehaviour
    {
        public Action<Plant> PlantTriggered;

        [SerializeField] private float maxRadius = 10;
        [SerializeField] private float pulseDuration = 15;
        [SerializeField] private float pulseCooldown = 5;
        [SerializeField] private float pulseSpeed = 1;

        bool started = false;

        [SerializeField] private SphereCollider sphereCollider;

        private float currentRadius => sphereCollider.radius;

        public void StartPulse()
        {
            if (!started)
            {
                started = true;
                StartCoroutine(DoPulse());
            }
        }

        private IEnumerator DoPulse()
        {
            float startTime = Time.time;
            float endTime = startTime + pulseDuration;
            float startRadius = transform.localScale.x;
            float endRadius = maxRadius;

            while (true)
            {
                if (currentRadius >= maxRadius)
                {
                    sphereCollider.radius = 1;
                    yield return new WaitForSeconds(pulseCooldown);
                    continue;
                }

                sphereCollider.radius += Time.deltaTime * pulseSpeed;

                yield return new WaitForFixedUpdate();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Plant plant;
            if (other.TryGetComponent(out plant))
            {
                Debug.Log("Plant Triggered");
                PlantTriggered?.Invoke(plant);
            }
            else
            {
                Debug.LogError("Plant component not found");
            }
        }
    }
}