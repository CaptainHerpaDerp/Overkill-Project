using Core;
using Players;
using System.Collections;
using UnityEngine;

namespace Players
{
    public class PurpleSpecialBehaviour : SpecialBehaviour
    {
        public Transform laserOrigin; 
        public float laserRange = 100f;
        public LayerMask collisionMask; // Layer mask for detecting collisions

        // The line renderer for the laser
        [SerializeField] private LineRenderer laserLineRenderer;

        // The particle object at the end of the laser
        [SerializeField] private GameObject laserEndEffect;

        [SerializeField] private float laserTime;
        [SerializeField] private float laserPushback;

        // The desired width of the laser at its fullest
        [SerializeField] private float laserWidth;

        [SerializeField] private Player parentPlayer;

        // The two materials for the line renderer, one to be applied when aiming and one when firing
        [SerializeField] private Material aimMat, fireMat;

        private Coroutine TrajectoryCoroutine;

        // TEMP
        [SerializeField] private LaserTargetFinder targetFinder;

        // The number of segments to use for the curve
        [SerializeField] private int numSegments = 50;

        // The sharpness of the turn
        [SerializeField] private float turnSharpness = 1.0f;

        // The time the endpoint of the laser should be visible
        private const float endpointLifeTime = 1f;

        private void OnDisable()
        {
            targetFinder.gameObject.SetActive(false);
        }

        public override bool Activate()
        {
            if (!targetFinder.gameObject.activeInHierarchy)
            {
                targetFinder.gameObject.SetActive(true);
            }

            if (onCooldown)
            {
                return false;
            }

            if (targetFinder.GetPlayerTarget(laserRange) == null)
            {
                return false;
            }

            playerModelController.PlayAnimation(Players.AnimationState.Special);        

            TrajectoryCoroutine ??= StartCoroutine(FireLaserCoroutine());

            return true;
        }

        private IEnumerator FireLaserCoroutine()
        {
            laserLineRenderer.enabled = true;

            // Apply the aiming material to the line renderer
            laserLineRenderer.material = aimMat;

            Transform laserTarget = targetFinder.GetPlayerTarget(laserRange).transform;

            // Create a laser end effect instance at the position of the target
            GameObject endEffect = Instantiate(laserEndEffect, laserTarget.position, Quaternion.identity);
            StartCoroutine(DestoryEndEffect(endEffect));

            // Calculate the curve points
            Vector3[] curvePoints = CalculateBezierCurve(laserOrigin.position, laserTarget.position, turnSharpness);

            curvePoints[curvePoints.Length - 1] = laserTarget.position + (laserTarget.position - curvePoints[curvePoints.Length - 2]) * .5f;
            // Update the line renderer positions
            laserLineRenderer.positionCount = curvePoints.Length;
            laserLineRenderer.SetPositions(curvePoints);

            // Calculate the angle between the forward direction and the transform of the target
            float angle = Vector3.Angle(laserOrigin.forward, (laserTarget.position - laserOrigin.position).normalized);

            // If the angle is less than 10 degrees, the sharpness of the curve should be 0, otherwise it should be 1   
            //turnSharpness = angle < straightLaserAngle ? 0 : 1;

            laserTarget.GetComponent<Rigidbody>().AddForce((laserTarget.position - laserOrigin.position).normalized * laserPushback);

            TrajectoryCoroutine = null;

            SoundManager.Instance.PlayPurpleAbilityAtPoint(transform.position);

            // Apply the firing material to the line renderer
            laserLineRenderer.material = fireMat;

            // Show the laser for a short time
            StartCoroutine(ShowFireLaser(laserTime));

            // Do the cooldown
            DoCooldown();

            yield break;
        }


        // Calculate the points of the Bezier curve
        private Vector3[] CalculateBezierCurve(Vector3 startPoint, Vector3 endPoint, float sharpness)
        {

            Vector3 vectorToTarget = endPoint - startPoint;
            Vector3 controlPointOffset = Vector3.up * sharpness * vectorToTarget.magnitude;
            Debug.Log($"distance = {vectorToTarget.magnitude}; offset = {controlPointOffset}");

            Vector3 controlPoint = startPoint + vectorToTarget / 2 + controlPointOffset;

            Vector3[] curvePoints = new Vector3[numSegments + 1];
            curvePoints[0] = startPoint;

            for (int i = 1; i < numSegments + 1; i++)
            {
                float t = (i - 1) / (float)numSegments;
                curvePoints[i] = CalculateQuadraticBezierPoint(t, startPoint, controlPoint, endPoint);
            }

            return curvePoints;
        }

        // Calculate a point on a quadratic Bezier curve
        private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector3 p = uu * p0; // (1-t)^2 * P0
            p += 2 * u * t * p1; // 2(1-t)t * P1
            p += tt * p2; // t^2 * P2

            return p;
        }

        // Temporarily show the laser when the player is holding the trigger
        private IEnumerator ShowFireLaser(float showTime)
        {
            float time = 0;
            // Set the laser's width to 0 to start
            laserLineRenderer.startWidth = 0;
            laserLineRenderer.endWidth = 0;
            float width = 0;

            // Half the time so we have half the time to show the laser and half the time to hide it
            float startTime = showTime / 3;
            float endTime = showTime - startTime;

            // First, increase the width of the laser to show it
            while (time < startTime)
            {
                float t = time / startTime;

                width = Mathf.Lerp(0, laserWidth, t);
                laserLineRenderer.startWidth = width;
                laserLineRenderer.endWidth = width;

                time += Time.deltaTime;
                yield return null;
            }

            // Reset the time
            time = 0;

            // Then, decrease the width of the laser to hide it
            while (time < endTime)
            {
                float t = time / endTime;

                width = Mathf.Lerp(laserWidth, 0, t);
                laserLineRenderer.startWidth = width;
                laserLineRenderer.endWidth = width;

                time += Time.deltaTime;
                yield return null;
            }

            // Hide the line
            laserLineRenderer.enabled = false;

            // Reset the width of the laser
            laserLineRenderer.startWidth = 1;

            yield break;
        }

        private IEnumerator DestoryEndEffect(GameObject objToDestroy)
        {
            yield return new WaitForSeconds(endpointLifeTime);
            Destroy(objToDestroy);
        }
    }
}