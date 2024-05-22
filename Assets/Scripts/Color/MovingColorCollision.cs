using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingColorCollision : MonoBehaviour
{
    private MovingColorChange movingChangeScript;

    private void Start()
    {
        movingChangeScript = GetComponentInParent<MovingColorChange>();
        
        if (movingChangeScript == null)
        {
            Debug.LogError("MovingColorChange script not found in parent object.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hey");
        if (movingChangeScript != null)
        {
            movingChangeScript.OnColorTriggerEnter(other);
        }
        
    }
}
