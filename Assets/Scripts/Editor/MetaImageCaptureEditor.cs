using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MetaImageCapture))]
public class MetaImageCaptureEditor : Editor
{
    SerializedProperty GridSubsetCapture_property;
    SerializedProperty minWidthGridSubset_property;
    SerializedProperty maxWidthGridSubset_property;
    SerializedProperty minHeightGridSubset_property;
    SerializedProperty maxHeightGridSubset_property;

    MetaImageCapture _metaImageCapture_ref;

    public override void OnInspectorGUI()
    {
        _metaImageCapture_ref = (MetaImageCapture)target;

        CatchProperties();

        DrawBase();

        DrawMetaAllImageCaptureButton();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawMetaAllImageCaptureButton()
    {
        GridSubsetCapture_property.boolValue = EditorGUILayout.BeginToggleGroup(
            new GUIContent("Grid Image Capture", "Enables/Disables the possibility to capture images of the fields only on a" +
                            "subset of the grid on which we move the drone."), GridSubsetCapture_property.boolValue);
        
        EditorGUILayout.PropertyField(minWidthGridSubset_property,
            new GUIContent("Min Width", "Subset's minimum bound of the width of the grid on which we capture images."));
        EditorGUILayout.PropertyField(maxWidthGridSubset_property,
            new GUIContent("Max Width", "Subset's maximum bound of the width of the the grid on which we capture images."));
        EditorGUILayout.PropertyField(minHeightGridSubset_property,
            new GUIContent("Min Height", "Subset's minimum bound of the height of the grid on which we capture images."));
        EditorGUILayout.PropertyField(maxHeightGridSubset_property,
            new GUIContent("Max Height", "Subset's maximum bound of the height of the grid on which we capture images."));
        
        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.Space();

        EditorGUI.BeginDisabledGroup(GridSubsetCapture_property.boolValue);

        EditorGUI.EndDisabledGroup();
    }

    private void DrawBase()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fieldGenerator_ref"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("capturingCamera_ref"));
    }

    private void CatchProperties()
    {
        GridSubsetCapture_property = serializedObject.FindProperty("GridSubsetCapture");
        minWidthGridSubset_property = serializedObject.FindProperty("minWidthGridSubset");
        maxWidthGridSubset_property = serializedObject.FindProperty("maxWidthGridSubset");
        minHeightGridSubset_property = serializedObject.FindProperty("minHeightGridSubset");
        maxHeightGridSubset_property = serializedObject.FindProperty("maxHeightGridSubset");
    }

    private float Bool2Float(bool _b)
    {
        return _b ? 1f : 0f;
    }
}
