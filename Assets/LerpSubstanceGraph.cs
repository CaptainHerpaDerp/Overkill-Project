using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Adobe.Substance;
using Adobe.Substance.Runtime;
using Adobe.Substance.Input;
using Adobe.SubstanceEditor;
using static UnityEngine.Rendering.DebugUI;

public class LerpSubstanceGraph : MonoBehaviour
{
    [SerializeField] private float time;

    // Reference a substance graph asset
    [SerializeField] private SubstanceGraphSO substanceGraph;

    private void Update()
    {
        //// Set the $"time"parameter in the substance graph to the value of the "time" variable
        //substanceGraph.SetFloatParameter("time", time);

        //var substanceInput = substanceGraph.Input.
        //if (substanceInput != null && substanceInput is SubstanceInputFloat inputFloat)
        //{
        //    inputFloat.Data = value;
        //}
    }

}
