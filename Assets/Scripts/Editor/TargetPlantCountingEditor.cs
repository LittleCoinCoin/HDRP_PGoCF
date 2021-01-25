using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TargetPlantCounting))]
public class TargetPlantCountingEditor : Editor
{
    TargetPlantCounting _ref_targetPlantCounting;

    public override void OnInspectorGUI()
    {
        _ref_targetPlantCounting = (TargetPlantCounting)target;

        base.OnInspectorGUI();

        if (GUILayout.Button("Show Debug Positions"))
        {
            _ref_targetPlantCounting.ShowDebugPositions();
        }

        if (GUILayout.Button("Destroy Debug Positions"))
        {
            _ref_targetPlantCounting.DestroyDebugPositions();
        }
    }
}
