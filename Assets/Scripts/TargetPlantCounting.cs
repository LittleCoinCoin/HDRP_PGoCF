using UnityEngine;
using System.IO;
using System.Globalization;

public class TargetPlantCounting : MonoBehaviour
{
    public Camera DroneCamera;
    private Plane[] camFrustrumPlanes;
    
    public Field field_generator_ref;

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
            if ((_targetPlant.transform.position.x > boundaries[3] && _targetPlant.transform.position.x < boundaries[2]) &&
                (_targetPlant.transform.position.z > boundaries[1] && _targetPlant.transform.position.z < boundaries[0]))
            {
                counter++;

                //Vector2 plant_viewPort = DroneCamera.WorldToViewportPoint(_targetPlant.transform.position);

                Vector3 _correctedTransform = new Vector3(_targetPlant.transform.position.x,
                                                          _targetPlant.transform.position.y + _targetPlant.transform.localScale.y,
                                                          _targetPlant.transform.position.z);

                Vector2 _corrected_plant_ViewPort = DroneCamera.WorldToViewportPoint(_correctedTransform);

                sw.WriteLine(_targetPlant.transform.position.x.ToString("G", CultureInfo.InvariantCulture) + "," +
                             _targetPlant.transform.position.z.ToString("G", CultureInfo.InvariantCulture) + "," +
                             //plant_viewPort.x.ToString("G", CultureInfo.InvariantCulture) + "," +
                             //plant_viewPort.y.ToString("G", CultureInfo.InvariantCulture) + "," +
                             _corrected_plant_ViewPort.x.ToString("G", CultureInfo.InvariantCulture) + "," +
                             _corrected_plant_ViewPort.y.ToString("G", CultureInfo.InvariantCulture));
            }
        }

        sw.Close();

        return counter;
    }
}
