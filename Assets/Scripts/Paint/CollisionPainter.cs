using System;
using UnityEngine;

public class CollisionPainter : MonoBehaviour{
    [Header("Paint properties")]
    public Color paintColor;
    public float paintRadius = 1;
    public float strength = 1;
    public float hardness = 1;
    public Texture2D paintTexture;

    [Header("Paint Collider")] 
    public float paintColliderRadius;


    private void FixedUpdate() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, paintColliderRadius);

        foreach (Collider col in hitColliders) {
            Paintable p = col.GetComponent<Paintable>();
            if (p == null) 
                continue;
            Vector3 closestPoint = col.ClosestPoint(transform.position);
            PaintManager.instance.paint(p, closestPoint, paintRadius, hardness, strength, paintTexture, paintColor);
        }
    }

    /*
    private void OnCollisionStay(Collision other) {
        Paintable p = other.collider.GetComponent<Paintable>();
        Debug.Log(p);
        if(p != null){
            Vector3 pos = other.contacts[0].point;
            PaintManager.instance.paint(p, pos, paintRadius, hardness, strength, paintColor);
        }
    }
    */
}
