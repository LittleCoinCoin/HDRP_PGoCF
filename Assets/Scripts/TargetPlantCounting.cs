using UnityEngine;
using UnityEngine.Perception.GroundTruth;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

public class TargetPlantCounting : MonoBehaviour
{
    public Camera DroneCamera;
    private Plane[] camFrustrumPlanes;
    
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

    /// <summary>
    /// Counts the number of plants present in the screen space and generate a csv file
    /// with all the coordinates of the plants both in world format (x and z) and viewport (x and y)
    /// </summary>
    /// <param name="_path_plant_position_file">Path where to write the csv file. The name of the file should be included
    /// in that path.</param>
    /// <returns>An integer : the number of plants</returns>
    public int CountTargetPlants(string _path_plant_position_file)
    {
        int counter = 0;
        float[] boundaries = DroneCamera.GetComponent<CameraVision>().VisionBoundariesOnField();

        FileStream plant_pos_file = new FileStream(_path_plant_position_file, FileMode.Create);
        StreamWriter sw = new StreamWriter(plant_pos_file);

        foreach (GameObject _targetPlant in field_generator_ref.all_target_plants)
        {
            if (_targetPlant.activeSelf &&
                (_targetPlant.transform.position.x > boundaries[3] && _targetPlant.transform.position.x < boundaries[2]) &&
                (_targetPlant.transform.position.z > boundaries[1] && _targetPlant.transform.position.z < boundaries[0]))
            {
                counter++;

                //Vector2 plant_viewPort = DroneCamera.WorldToViewportPoint(_targetPlant.transform.position);

                Vector3 _correctedTransform = new Vector3(_targetPlant.transform.position.x,
                                                          _targetPlant.transform.position.y + _targetPlant.transform.localScale.y * _targetPlant.GetComponent<MeshFilter>().sharedMesh.bounds.size.y,//_targetPlant.transform.localScale.y,
                                                          _targetPlant.transform.position.z);

                Vector2 _corrected_plant_ViewPort = DroneCamera.WorldToViewportPoint(_correctedTransform);

                Vector3 _NE_bounding_box = new Vector3(_targetPlant.transform.position.x + _targetPlant.transform.localScale.x * _targetPlant.GetComponent<MeshFilter>().sharedMesh.bounds.size.x,
                                                    _targetPlant.transform.position.y + _targetPlant.transform.localScale.y * _targetPlant.GetComponent<MeshFilter>().sharedMesh.bounds.size.y,
                                                    _targetPlant.transform.position.z + _targetPlant.transform.localScale.z * _targetPlant.GetComponent<MeshFilter>().sharedMesh.bounds.size.z);

                Vector3 _SW_bounding_box = new Vector3(_targetPlant.transform.position.x - _targetPlant.transform.localScale.x * _targetPlant.GetComponent<MeshFilter>().sharedMesh.bounds.size.x,
                                                    _targetPlant.transform.position.y + _targetPlant.transform.localScale.y * _targetPlant.GetComponent<MeshFilter>().sharedMesh.bounds.size.y,
                                                    _targetPlant.transform.position.z - _targetPlant.transform.localScale.z * _targetPlant.GetComponent<MeshFilter>().sharedMesh.bounds.size.z);

                Vector2 _onScreen_NE_Bounding_Box = DroneCamera.WorldToViewportPoint(_NE_bounding_box);
                Vector2 _onScreen_SW_Bounding_Box = DroneCamera.WorldToViewportPoint(_SW_bounding_box);

                sw.WriteLine(_targetPlant.transform.position.x.ToString("G", CultureInfo.InvariantCulture) + "," + //this is the virtual world x position
                             _targetPlant.transform.position.z.ToString("G", CultureInfo.InvariantCulture) + "," + //this is the virtual world y position
                             _corrected_plant_ViewPort.x.ToString("G", CultureInfo.InvariantCulture) + "," + //this is the screen x position
                             _corrected_plant_ViewPort.y.ToString("G", CultureInfo.InvariantCulture) + "," + //this is the screen world y position
                             (_onScreen_NE_Bounding_Box.x - _onScreen_SW_Bounding_Box.x).ToString("G", CultureInfo.InvariantCulture) + "," + //this is the plant width on the screen
                             (_onScreen_NE_Bounding_Box.y - _onScreen_SW_Bounding_Box.y).ToString("G", CultureInfo.InvariantCulture) + "," + //this is the plant height on the screen
                             _targetPlant.transform.eulerAngles.y.ToString("G", CultureInfo.InvariantCulture) // rotation of the plant to rotate the box accordingly.
                             ); 
            }

        }

        sw.Close();
        return counter;
    }

    public void With_Perception()
    {

        int counter = 0;
        float[] boundaries = DroneCamera.GetComponent<CameraVision>().VisionBoundariesOnField();

        DroneCamera.GetComponent<CustomizePerception>().DefineViewPortAnnotation();

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

        DroneCamera.GetComponent<CustomizePerception>().ApplyViewPortAnnotation(capture);
        DroneCamera.GetComponent<PerceptionCamera>().RequestCapture();
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
