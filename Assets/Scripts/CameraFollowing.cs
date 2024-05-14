using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraFollowing : MonoBehaviour {
    
    [SerializeField] 
    private Transform actualDoor;
    
    [SerializeField] 
    private Transform teleportDoor;

    [SerializeField] 
    private Transform playerCam;

    private void LateUpdate() {
        Vector3 playerOffset = actualDoor.position - playerCam.position;

        transform.position = teleportDoor.position - playerOffset;
        transform.rotation = playerCam.rotation;
    }
}
