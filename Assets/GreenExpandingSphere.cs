using System;
using UnityEngine;

public class GreenExpandingSphere : MonoBehaviour
{
    Action<Plant> PlantTriggered;

    private void OnTriggerEnter(Collider other)
    {
        Plant plant;
        if (other.TryGetComponent(out plant))
        {
            PlantTriggered?.Invoke(plant);
        }
    }
}
