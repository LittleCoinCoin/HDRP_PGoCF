using UnityEngine;
using UnityEngine.Perception.GroundTruth;
using System.Collections.Generic;

/// <summary>
/// Handles the logic to annotate all images visible by the camera.
/// This uses custom annotations compliant with UnityPerception's
/// framework to be automatically added to the dataset labelling file.
/// </summary>
public class TargetPlantCounting : MonoBehaviour
{
    public Camera DroneCamera;
    
    public Field field_generator_ref;

    public GameObject _debug_object;
    private List<GameObject> _debug_positions_objects = new List<GameObject>();

    public struct PlantAnnotation
    {
        public uint instance_id;
        public Vector2 ViewPort;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log($"{CountTargetPlants()} plants visible on Drone camera");
        }
    }

    /// <summary>
    /// Counts the number of plants present in the screen space
    /// </summary>
    /// <returns> An integer : the number of plants</returns>
    public int CountTargetPlants()
    {
        int counter = 0;
        float[] boundaries = DroneCamera.GetComponent<CameraVision>().VisionBoundariesOnField();

        foreach (GameObject _targetPlant in field_generator_ref.all_target_plants)
        {
            if ((_targetPlant.transform.position.x > boundaries[3] && _targetPlant.transform.position.x < boundaries[2]) &&
                (_targetPlant.transform.position.z > boundaries[1] && _targetPlant.transform.position.z < boundaries[0]))
            {
                counter++;
            }
        }
        return counter;
    }

    public void With_Perception()
    {
        int counter = 0;
        float[] boundaries = DroneCamera.GetComponent<CameraVision>().VisionBoundariesOnField();
        
        DroneCamera.GetComponent<CustomAnnotation>().DefineViewPortAnnotation();

        List<PlantAnnotation> capture = new List<PlantAnnotation>();

        foreach (GameObject _targetPlant in field_generator_ref.all_target_plants)
        {
            if (_targetPlant.activeSelf &&
                (_targetPlant.transform.position.x > boundaries[3] && _targetPlant.transform.position.x < boundaries[2]) &&
                (_targetPlant.transform.position.z > boundaries[1] && _targetPlant.transform.position.z < boundaries[0]))
            {
                Vector3 _correctedTransform = new Vector3(_targetPlant.transform.position.x,
                                                          _targetPlant.transform.position.y + _targetPlant.transform.localScale.y * _targetPlant.GetComponent<MeshFilter>().sharedMesh.bounds.size.y,//_targetPlant.transform.localScale.y,
                                                          _targetPlant.transform.position.z);

                Vector2 _corrected_plant_ViewPort = DroneCamera.WorldToViewportPoint(_correctedTransform);

                if (0 < _corrected_plant_ViewPort.x && _corrected_plant_ViewPort.x < 1 &&
                     0 < _corrected_plant_ViewPort.y && _corrected_plant_ViewPort.y < 1)
                {
                    counter++;

                    capture.Add(new PlantAnnotation {
                                        instance_id = _targetPlant.GetComponent<Labeling>().instanceId,
                                        ViewPort = _corrected_plant_ViewPort}
                    );
                }
            }
        }

        DroneCamera.GetComponent<PerceptionCamera>().RequestCapture();
        DroneCamera.GetComponent<CustomAnnotation>().ApplyViewPortAnnotation(capture);
    }

    public void ShowDebugPositions()
    {
        DestroyDebugPositions();

        float[] boundaries = DroneCamera.GetComponent<CameraVision>().VisionBoundariesOnField();
        _debug_positions_objects = new List<GameObject>();
        foreach (GameObject _targetPlant in field_generator_ref.all_target_plants)
        {
            if (_targetPlant.activeSelf &&
                (_targetPlant.transform.position.x > boundaries[3] && _targetPlant.transform.position.x < boundaries[2]) &&
                (_targetPlant.transform.position.z > boundaries[1] && _targetPlant.transform.position.z < boundaries[0]))
            {

                //Debug.Log(_targetPlant.GetComponent<MeshFilter>().sharedMesh.bounds.size.y);
                Vector3 _correctedTransform = new Vector3(_targetPlant.transform.position.x,
                                                          _targetPlant.transform.position.y + 
                                                          _targetPlant.transform.localScale.y * _targetPlant.GetComponent<MeshFilter>().sharedMesh.bounds.size.y,//_targetPlant.transform.localScale.y,
                                                          _targetPlant.transform.position.z);

                GameObject _debug = Instantiate(_debug_object, _correctedTransform, Quaternion.identity);
                _debug_positions_objects.Add(_debug);
            }
        }
    }

    public void DestroyDebugPositions()
    {
        foreach(GameObject _g in _debug_positions_objects)
        {
            DestroyImmediate(_g);
        }
    }
}
