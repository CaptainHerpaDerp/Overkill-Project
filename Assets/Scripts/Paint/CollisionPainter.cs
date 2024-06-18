using GaiaElements;
using System;
using System.Collections;
using System.Collections.Generic;
using TeamColors;
using UnityEngine;

public class CollisionPainter : MonoBehaviour{
    [Header("Paint properties")]
    public Color paintColor;
    public float paintRadius = 1;
    public float strength = 1;
    public float hardness = 1;
    public Texture2D paintTexture;
    public Texture2D normalPaintMap;

    [Header("Paint Collider")] 
    public float paintColliderRadius;

    private void FixedUpdate() {
        //Paint();
    }

    private void OnEnable()
    {
        Paint();
    }
    private void Paint() {

        Debug.Log("hey");

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, paintColliderRadius);

        foreach (Collider col in hitColliders)
        {
            Paintable p = col.GetComponent<Paintable>();
            if (p == null)
                continue;
            Vector3 closestPoint = col.ClosestPoint(transform.position);
            PaintManager.instance.paint(p, closestPoint, paintRadius, hardness, strength, paintTexture, normalPaintMap, paintColor);
        }
    }
}
