using GaiaElements;
using System;
using System.Collections;
using System.Collections.Generic;
using TeamColors;
using UnityEngine;

public class PlantPainter : MonoBehaviour{
    [Header("Plant paint spread properties")]
    public Color paintColor;
    public float initialPaintRadius = 1f;
   
    public float paintRadiusStep = .5f;
    public float maxPaintRadius = 3;
    public float captureDelay = .2f;
    public float spreadTextureDelay = .01f;

    [Header("Paint properties")]
    public float strength = 1;
    public float hardness = 1;
    private ColorEnum.TEAMCOLOR colorTextureID;
    public List<Texture2D> paintTextures;
    public Texture2D normalPaintMap;

    [Header("Paint Collider")] 
    public float paintColliderRadius;
    public LayerMask paintLayerMask;

    private float paintRadius;


    private void Awake()
    {
        transform.parent.GetComponent<Plant>().OnPlantOwnershipChanged += ChangePlantOwner;   
    }
    

    private void FixedUpdate() {
        //Paint();
    }


    private void ChangePlantOwner(int iD, ColorEnum.TEAMCOLOR newColor, ColorEnum.TEAMCOLOR oldColor) {
        paintRadius = initialPaintRadius;
        StartCoroutine(PaintCourutine(newColor));
    }



    private IEnumerator PaintCourutine(ColorEnum.TEAMCOLOR newColor) {

        yield return new WaitForSeconds(captureDelay);

        colorTextureID = newColor;
        //Debug.Log("newColor = " + paintTextures[(int)colorTextureID]);
        while (paintRadius < maxPaintRadius) {
            Paint();
            yield return new WaitForSeconds(spreadTextureDelay);
            paintRadius += paintRadiusStep;
        }
        
    }

    private void Paint() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, paintColliderRadius, paintLayerMask);

        foreach (Collider col in hitColliders)
        {
            Paintable p = col.GetComponent<Paintable>();
            if (p == null) {
                Debug.Log("paintable is null");
                continue;
            }
                
            //Debug.Log(paintTextures[(int)colorTextureID]);
            Vector3 closestPoint = col.ClosestPoint(transform.position);
            PaintManager.instance.paint(p, closestPoint, paintRadius, hardness, strength, paintTextures[(int)colorTextureID], normalPaintMap, paintColor);

        }
    }
}
