using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraVision))]
public class CameraVisionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CameraVision _cameraVision_ref = (CameraVision)target;

        if (GUILayout.Button("Debug Decals"))
        {
            _cameraVision_ref.Debug_FOV_HW_2();            
        }

        if (GUILayout.Button("Destroy Debug Decals"))
        {
            _cameraVision_ref.destroyDecal();
        }
    }
}
