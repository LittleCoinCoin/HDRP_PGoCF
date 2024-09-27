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

        if(GUILayout.Button("Count Target Plants"))
		{
			Debug.Log($"{_ref_targetPlantCounting.CountTargetPlants()} plants visible on Drone camera");
		}

		if (GUILayout.Button("Show Debug Positions"))
        {
            _ref_targetPlantCounting.ShowDebugPositions();
        }

        if (GUILayout.Button("Destroy Debug Positions"))
        {
            _ref_targetPlantCounting.DestroyDebugPositions();
        }

        GUILayout.Space(10);

		if (GUILayout.Button("Capture with Perception"))
		{
			_ref_targetPlantCounting.With_Perception();
		}
	}
}
