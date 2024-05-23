using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BridgeTest : MonoBehaviour
{
    public enum BridgeType
    {
        OneWay,
        TwoWay
    }

    // Size of a newly created bridge
    public Vector3 defaultBridgeDimensions = new Vector3(1.5f, 3.0f, 0.1f);

    public BoxCollider bridge1Coll, bridge2Coll;

    public BridgeType bridgeType;

    public Bridge EntryPoint, ExitPoint;

    public void Start()
    {
        EntryPoint.OnPlayerContact += (GameObject obj) =>
        {
            TeleportPlayer(obj, EntryPoint, ExitPoint);
            ExitPoint.PlayerInCollider = true;
        };

        ExitPoint.OnPlayerContact += (GameObject obj) =>
        {
            TeleportPlayer(obj, ExitPoint, EntryPoint);
            EntryPoint.PlayerInCollider = true;
        };
    }

    private void TeleportPlayer(GameObject obj, Bridge fromBridge, Bridge toBridge)
    {
        Vector3 diff = obj.transform.position - fromBridge.transform.position;

        obj.transform.position = toBridge.transform.position + diff;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Draw a sphere at the position of the bridge
        //Gizmos.DrawWireSphere(transform.position, 0.3f);

        if (EntryPoint != null && ExitPoint != null)
        {            
            if (bridgeType == BridgeType.OneWay)
            {
                // Draw a green box around the entry point
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(EntryPoint.transform.position, .5f);

                // Draw a red box around the entry point
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(ExitPoint.transform.position, .5f);

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(EntryPoint.transform.position, ExitPoint.transform.position);

                // Draw an arrow using Handles
                Vector3 center = (EntryPoint.transform.position + ExitPoint.transform.position) / 2;
                Vector3 direction = (ExitPoint.transform.position - EntryPoint.transform.position).normalized;

                float arrowSize = 3f;

                Handles.color = Color.yellow;
                Handles.ArrowHandleCap(0, center - (direction * arrowSize), Quaternion.LookRotation(direction), arrowSize, EventType.Repaint);
            }

            else
            {
                // Draw a green box around the entry point
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(EntryPoint.transform.position, .5f);

                // Draw a red box around the entry point
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(ExitPoint.transform.position, .5f);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(EntryPoint.transform.position, ExitPoint.transform.position);

                // Draw an arrow using Handles
                Vector3 center = (EntryPoint.transform.position + ExitPoint.transform.position) / 2;
                Vector3 direction = (ExitPoint.transform.position - EntryPoint.transform.position).normalized;
                Vector3 negativeDirection = (EntryPoint.transform.position - ExitPoint.transform.position).normalized;

                float arrowSize = 3f;

                Handles.color = Color.green;
                Handles.ArrowHandleCap(0, (center - (direction * arrowSize) + direction * 1), Quaternion.LookRotation(direction), arrowSize, EventType.Repaint);
                Handles.ArrowHandleCap(0, (center - (negativeDirection * arrowSize) + negativeDirection * 1), Quaternion.LookRotation(negativeDirection), arrowSize, EventType.Repaint);
            }
        }
    }

}
