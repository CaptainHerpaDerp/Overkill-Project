using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureMovingColorCollision : MonoBehaviour
{
    private CreatureMovingColorChange movingChangeScript;

    private void Start()
    {
        movingChangeScript = GetComponentInParent<CreatureMovingColorChange>();
        
        if (movingChangeScript == null)
        {
            Debug.LogError("MovingColorChange script not found in parent object.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (movingChangeScript != null)
        {
            movingChangeScript.OnColorTriggerEnter(other);
        }
        
    }
}
