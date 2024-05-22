using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovingColorCollision : MonoBehaviour
{
    private PlayerMovingColorChange movingChangeScript;

    private void Start()
    {
        movingChangeScript = GetComponentInParent<PlayerMovingColorChange>();
        
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
