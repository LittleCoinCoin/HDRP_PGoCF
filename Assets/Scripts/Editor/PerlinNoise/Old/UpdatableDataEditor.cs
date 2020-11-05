using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(UpdatableData), true)]
public class UpdatableDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        UpdatableData data = (UpdatableData)target;
        if (GUILayout.Button("Update Values"))
        {
            data.NotifyOfUpdatedValue();
            EditorUtility.SetDirty(target);
        }
    }
}
