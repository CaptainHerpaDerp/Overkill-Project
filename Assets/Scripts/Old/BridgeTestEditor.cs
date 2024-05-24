using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BridgeTest))]
public class BridgeTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
       // base.OnInspectorGUI();

        BridgeTest bridgeTest = (BridgeTest)target;

        GUILayout.Space(25);

        // Show the fields for the two BoxColliders

        #region Bridge Fields and Null Creation

        EditorGUILayout.BeginHorizontal();

        if (bridgeTest.bridge1Coll == null && GUILayout.Button("Create New"))
        {
            // Create a new GameObject with a BoxCollider component
            bridgeTest.bridge1Coll = new GameObject("Bridge 1").AddComponent<BoxCollider>();

            BoxCollider bridge1Coll = bridgeTest.bridge1Coll;

            bridge1Coll.isTrigger = true;

            Bridge newBridge = bridgeTest.bridge1Coll.gameObject.AddComponent<Bridge>();

            // Set the position of the collider to the position of the BridgeTest object
            bridgeTest.bridge1Coll.transform.position = bridgeTest.transform.position;

            // Set the size of the collider to the default dimensions
            bridgeTest.bridge1Coll.size = bridgeTest.defaultBridgeDimensions;

            bridgeTest.EntryPoint = newBridge;
        }

        // Set the size of the label to the width of the label text

        bridgeTest.bridge1Coll = (BoxCollider)EditorGUILayout.ObjectField("Bridge 1", bridgeTest.bridge1Coll, typeof(BoxCollider), true);

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        if (bridgeTest.bridge2Coll == null && GUILayout.Button("Create New"))
        {
            // Create a new GameObject with a BoxCollider component
            bridgeTest.bridge2Coll = new GameObject("Bridge 2").AddComponent<BoxCollider>();

            BoxCollider bridge2Coll = bridgeTest.bridge2Coll;

            bridge2Coll.isTrigger = true;

            Bridge newBridge = bridgeTest.bridge2Coll.gameObject.AddComponent<Bridge>();

            // Set the position of the collider to the position of the BridgeTest object
            bridgeTest.bridge2Coll.transform.position = bridgeTest.transform.position;

            // Set the size of the collider to the default dimensions
            bridgeTest.bridge2Coll.size = bridgeTest.defaultBridgeDimensions;

            bridgeTest.ExitPoint = newBridge;
        }

        bridgeTest.bridge2Coll = (BoxCollider)EditorGUILayout.ObjectField("Bridge 2", bridgeTest.bridge2Coll, typeof(BoxCollider), true);

        EditorGUILayout.EndHorizontal();

        #endregion

        GUILayout.Space(25);

        // Show the bridgetype enum field
        bridgeTest.bridgeType = (BridgeTest.BridgeType)EditorGUILayout.EnumPopup("Bridge Type", bridgeTest.bridgeType);

        GUILayout.Space(10);

        if (bridgeTest.bridgeType == BridgeTest.BridgeType.OneWay)
        {
            if (bridgeTest.EntryPoint == null)
            {
                bridgeTest.EntryPoint = bridgeTest.bridge1Coll.GetComponent<Bridge>();
            }

            if (bridgeTest.ExitPoint == null)
            {
                bridgeTest.ExitPoint = bridgeTest.bridge2Coll.GetComponent<Bridge>();
                bridgeTest.ExitPoint.Acessible = false;
            }

            EditorGUILayout.BeginHorizontal();

            // Show the field for the first bridge
            EditorGUILayout.LabelField($"Entrance = {bridgeTest.EntryPoint.gameObject.name}", EditorStyles.boldLabel);

            if (GUILayout.Button("<->"))
            {
                Bridge entryPoint = bridgeTest.EntryPoint;

                bridgeTest.EntryPoint = bridgeTest.ExitPoint;
                bridgeTest.ExitPoint = entryPoint;

                bridgeTest.EntryPoint.Acessible = true;
                bridgeTest.ExitPoint.Acessible = false;


                //Redraw the gizmos
                SceneView.RepaintAll();
            }
            EditorGUILayout.LabelField($"Exit = {bridgeTest.ExitPoint.gameObject.name}", EditorStyles.boldLabel);

            EditorGUILayout.EndHorizontal();
        }
    }
}
