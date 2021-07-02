using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

/// <summary>
/// This scripts computes the limits of the area of the field visible by the camera.
/// There are also debug functions drawing red decals on the outer part of the area.
/// The debug is accessible through a button in the inspector.
/// The camera must be perpendicular to the field.
/// </summary>
public class CameraVision : MonoBehaviour
{
    public Field field_generator_ref;
    public Camera camera_ref;
    public Transform droneTransform;

    public GameObject DebugObject;
    private List<GameObject> DebugObject_List = new List<GameObject>();

    public GameObject DecalProjector_ref;
    private List<GameObject> DecalProjectort_List = new List<GameObject>();
    

    private void Start()
    {
        field_generator_ref = FindObjectOfType<Field>();
    }

    private Vector2 Compute_FOV_HeightWidth_2()
    {
        float y_sensor_m = camera_ref.sensorSize.y / 2f * 0.001f;
        float x_sensor_m = camera_ref.sensorSize.x / 2f * 0.001f;
        //Debug.Log(y_sensor_m);
        //Debug.Log(-camera_ref.focalLength*0.001f + droneTransform.position.y);
        //Debug.Log((-camera_ref.focalLength * 0.001f + droneTransform.position.y )/ (-camera_ref.focalLength * 0.001f));
        //Debug.Log((-camera_ref.focalLength * 0.001f + droneTransform.position.y) / (-camera_ref.focalLength * 0.001f));
        float z = y_sensor_m * ((-camera_ref.focalLength * 0.001f + droneTransform.position.y) / (-camera_ref.focalLength * 0.001f));
        float x = z * (16f / 9f);

        return new Vector2(-x, -z) + new Vector2(x_sensor_m, y_sensor_m);
        //So, to compensate, I add them here even though it shoud not be necessary...
    }

    /// <summary>
    /// Compute the North, South, East, West coordinates of the boundaries of the crops field that is visible on camera.
    /// </summary>
    /// <returns>An array with values (N, S, E, O) in the plane defined by the virtual crop field</returns>
    /// <remarks>
    /// Works under the assumption that the drone takes pictures from a flat angle to the field and 
    /// the virtual field is at coordinate y = 0
    /// </remarks>
    public float[] VisionBoundariesOnField()
    {
        Vector2 HW = Compute_FOV_HeightWidth_2();
        //Debug.Log(HW);

        Vector3 S = droneTransform.position + new Vector3(0f, -droneTransform.position.y, -HW.y);
        Vector3 N = droneTransform.position + new Vector3(0f, -droneTransform.position.y, HW.y);
        Vector3 E = droneTransform.position + new Vector3(HW.x, -droneTransform.position.y, 0f);
        Vector3 O = droneTransform.position + new Vector3(-HW.x, -droneTransform.position.y, 0f);

        return new float[] { N.z, S.z, E.x, O.x };
    }

    public void destroyDecal()
    {
        if (DecalProjectort_List.Count > 0)
        {
            foreach (GameObject _go in DecalProjectort_List)
            {
                //DecalProjectort_List.Remove(_go);
                field_generator_ref.destroy(_go);
            }
            DecalProjectort_List = new List<GameObject>();
        }
    }

    /// <summary>
    /// Instanciate deacals that indicate the limits of the cameras field of view (FOV)
    /// </summary>
    public void Debug_FOV_HW_2()
    {
        if (DecalProjectort_List.Count == 0)
        {
            Vector2 HW = Compute_FOV_HeightWidth_2();
            Debug.Log(HW);

            Vector3 S = droneTransform.position + new Vector3(0f, -droneTransform.position.y, -HW.y);
            Vector3 N = droneTransform.position + new Vector3(0f, -droneTransform.position.y, HW.y);
            Vector3 E = droneTransform.position + new Vector3(HW.x, -droneTransform.position.y, 0f);
            Vector3 O = droneTransform.position + new Vector3(-HW.x, -droneTransform.position.y, 0f);
            //The offsets on Vector3 E and O at the x positions comes from the vertical gatefit option of the camera (chosen in the inspector).
            //If we had chosen the horizontal gatefit, the offset should have been on the Vector3 N and S at the z position.

            GameObject go_S = Instantiate(DecalProjector_ref, -Vector3.one, Quaternion.identity) as GameObject;

            //go_S.transform.eulerAngles = new Vector3(0, 180, 0);
            go_S.GetComponent<DecalProjector>().size = new Vector3(2 * HW.x, 1, 1);
            go_S.transform.position = S;
            go_S.transform.parent = droneTransform;

            GameObject go_N = Instantiate(DecalProjector_ref, -Vector3.one, Quaternion.identity) as GameObject;
            go_N.transform.eulerAngles = new Vector3(0, 180, 0);
            go_N.GetComponent<DecalProjector>().size = new Vector3(2 * HW.x, 1, 1);
            go_N.transform.position = N;
            go_N.transform.parent = droneTransform;

            GameObject go_E = Instantiate(DecalProjector_ref, -Vector3.one, Quaternion.identity) as GameObject;
            go_E.transform.eulerAngles = new Vector3(0, -90, 0);
            go_E.GetComponent<DecalProjector>().size = new Vector3(2 * HW.y, 1, 1);
            go_E.transform.position = E;
            go_E.transform.parent = droneTransform;

            GameObject go_O = Instantiate(DecalProjector_ref, -Vector3.one, Quaternion.identity) as GameObject;
            go_O.transform.eulerAngles = new Vector3(0, 90, 0);
            go_O.GetComponent<DecalProjector>().size = new Vector3(2 * HW.y, 1, 1);
            go_O.transform.position = O;
            go_O.transform.parent = droneTransform;


            DecalProjectort_List.Add(go_S);
            DecalProjectort_List.Add(go_N);
            DecalProjectort_List.Add(go_E);
            DecalProjectort_List.Add(go_O);
        }
    }
}
