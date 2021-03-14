using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;



public class Field : MonoBehaviour
{
    //field size
    [Min(1)] public float height = 1;
    [Min(1)] public float width = 1;

    //Whether the synthetic datasets are generated for one field at one time or one field
    //at multiple times of the crop development
    [Min(1)] public int field_monitoring_iterations = 1;

    [Serializable]
    public struct AllPlantStatus
    {
        public bool[] _allPlantStatus;
    }
    public List<AllPlantStatus> field_monitoring_all_plants_status;

    //parameters to decide which growth stage of the field time monitoring we should reach
    [Min(1)] public int target_growth_stage = 1;
    private int counter_growth_stages;

    //Instantiation mode of the crop rows.
    public enum GenerationMode { LinearV1, LinearSeaderDrill }//, RS_Curved}
    public GenerationMode crops_rows_GenMode = GenerationMode.LinearV1;

    //Number of rows in the seeder drill
    [Min(1)] public int crop_rows_on_seeder_drill;

    //space between crop rows with Seader Drill
    [Min(0.001f)] public float crop_rows_spacing_in_seader_drill;
    [Min(0.001f)] public float crop_rows_average_spacing_between_seader_passes;
    [Range(0f,1f)] public float crop_rows_spacing_between_seader_passes_random;

    //space between rows for LinearV1
    [Min(0.001f)] public float crop_rows_average_spacing;
    [Range(0f, 1f)] public float crop_rows_spacing_random;

    //space between plants in rows for LinearV1
    [Min(0.001f)] public float crop_plants_average_spacing;
    [Range(0f, 1f)] public float crop_plants_spacing_random;

    //plants randomized position with Seader Drill
    public float crop_plant_seader_drill_spacing;
    public float crop_plant_position_radius;

    //angle of the crop rows on the fields
    [Range(0f, 45f)] public float crop_rows_average_direction;
    [Range(0f, 1f)] public float crop_rows_direction_random;

    //Empty GameObject to be parent of all the instanciated gameobjects making a field
    public GameObject field_holder;
    private GameObject instantiated_field_holder;

    //3D model of the field
    public GameObject field_ref;
    public float field_size = 10;//the size of the Unity built in Plane prefab.
    private Vector3 field_center;
    [Min(1f)] public float field_texture_average_granularity = 1f;
    [Range(0f, 1f)] public float field_texture_granularity_random = 1f;

    //Empty GameObject to be parent of all the plants gameobjects making a crop row
    public GameObject plant_row_holder;

    //3D models of the target plants accross different growth stages
    public GameObject[] diff_growth_stages_plant_refs = new GameObject[1];

    //plant growth probability Mode
    public enum PlantGrowthProbabilityDistribution {Constant, Custom_Curves}
    public PlantGrowthProbabilityDistribution[] plant_growth_proba_distribution = new PlantGrowthProbabilityDistribution[1];

    //probability that a plant grows in the constant mode
    [Range(0f, 1f)] public float[] plant_growing_probability = new float[1];

    //plant growth probability according to CustomCurves
    public AnimationCurve[] X_Growth_Distribution = new AnimationCurve[1];
    public AnimationCurve[] Z_Growth_Distribution = new AnimationCurve[1];

    //scaling the size of the 3D model of the plant
    [Min(0f)] public float[] plant_average_Yscale = new float[1];
    [Range(0f, 1f)] public float[] plant_Yscale_random = new float[1];
    [Min(0f)] public float[] plant_average_radius = new float[1];
    [Range(0f, 1f)] public float[] plant_radius_random = new float[1];

    //List of the plants in the field
    public List<GameObject> all_target_plants;
    public List<Vector2> all_plants_coordinates;
    public List<int> all_plants_rows_indeces;

    //3D model of the Weed
    public GameObject weed_ref;
    [Min(0f)] public float weed_average_Yscale = 1f;
    [Range(0f, 1f)] public float weed_Yscale_random = 0.5f;
    [Min(0f)] public float weed_average_radius = 1f;
    [Range(0f, 1f)] public float weed_radius_random = 0.5f;
    [Range(0f, 1f)] public float _weed_growth_threshold = 0.5f;
    [Range(0f, 1f)] public float _weed_growing_probability = 0.5f;
    [Min(0f)] public float _weed_average_local_position_modifier_x = 1f;
    [Range(0f, 1f)] public float _weed_local_position_modifier_x_random = 0.1f;
    [Min(0f)] public float _weed_average_local_position_modifier_z = 1f;
    [Range(0f, 1f)] public float _weed_local_position_modifier_z_random = 0.1f;

    //Perlin Noise parameters for the Weed map
    public int _weed_PN_MapHeight = 100;
    public int _weed_PN_MapWidth = 100;
    public int _weed_PN_Seed = 0;
    public int _weed_PN_Octaves = 1;
    public float _weed_PN_NoiseScale = 1;
    [Range(0, 1)]  public float _weed_PN_Persistance = 0f;
    public float _weed_PN_Lacunarity = 1f;
    public Vector2 _weed_PN_Offset = Vector2.zero;
    public Vector2 _weed_PN_Offset_random = Vector2.zero;
    public float[,] _weed_PN_NoiseMap;
    public float[,] _weed_PN_NoiseMap_Thresholded;
    public Texture2D _weed_PN_Texture;
    public Texture2D _weed_PN_Texture_Thresholded;
    private int _weed_PN_Texture_rescale = 140;
    public bool _weed_PN_enablePreview = false;
    public bool _weed_PN_autoPreview = false;

    //reference to the scene light to simulate the position of the sun
    public GameObject directional_light_ref;

    //equivalent of the elevation and azimuth for the sun position
    [Range(0f, 180f)] public float sun_average_elevation;
    [Range(0f, 1f)] public float sun_elevation_random;
    [Range(-90f, 90f)] public float sun_average_azimuth;
    [Range(0f, 1f)] public float sun_azimuth_random;

    //parameters for the drone
    public GameObject drone_ref;
    private Camera drone_camera;
    //public float drone_smoothTime = 0;
    public float drone_altitude = 30;
    public Vector2 camera_sensor_size = new Vector2 (36,24);
    public float camera_focal_length = 35;
    [Range(0, 0.99f)] public float image_capture_horizontal_overlapping = 0;
    [Range(0, 0.99f)] public float image_capture_vertical_overlapping = 0;
    public Vector2Int image_size = new Vector2Int(1920, 1080);

    //boolean to manage the rendering mode of the field: reality or labelling
    public bool labellingMode = false;
    public bool castShadow_labellingMode = false;
    public bool autoUpdate_labellingMode = false;

    //Parameters open/close booleans
    public bool open_field_parameters = true;
    public bool open_plant_parameters = true;
    public bool open_weed_parameters = true;
    public bool open_CR_parameters = true;
    public bool open_sun_parameters = true;
    public bool open_rendering_parameters = true;
    public bool open_drone_parameters = true;

    //Plant Parameters Growth Stages open/close boolean array
    public bool[] open_PGS_parameters = new bool[1];

    private void Start()
    {
        //Generator();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Generator();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    ///<summary>
    ///Procedural Generation of the field.
    ///Computes all the plants positions and living status for every monitoring stages.
    ///</summary>
    public void Generator()
    {
        //Debug.Log("Field Generator !");
        
        CheckSceneForField();
        SpawnField();

        //Generate initial plants coordinates
        switch (crops_rows_GenMode)
        {
            case GenerationMode.LinearV1:
                GeneratePlantCoordinates_LinearV1();
                //SpawnPlants_LinearV1();
                break;
            case GenerationMode.LinearSeaderDrill:
                GeneratePlantCoordinates_LinearSeaderDrill();
                //SpawnPlants_LinearSeaderDrill();
                break;
            //case (GenerationMode.RS_Curved):
            //    SpawnPlants_();
            //    break;
        }

        //iterate the growth probabilities to alter the structure of the field
        for (counter_growth_stages=0; counter_growth_stages<field_monitoring_iterations; ++counter_growth_stages)
        {
            ApplyProbabilityGrowthDistribution();
        }

        //spawn the GameObjects at the end
        SpawnPlants();

        SpawnWeeds();
        PlaceLight();
        Render();
        ApplyDroneParameters();

        ApplyPlantStatus();
    }

    /// <summary>
    /// Purpose is to show the state of the field at individual monitoring stages.
    /// You have to at least use the Generator function once in the editor before using 
    /// this function.
    /// </summary>
    public void ShowFieldAtTargetMonitoringStage()
    {
        CheckSceneForField();
        SpawnField();

        //spawn the GameObjects at the end
        SpawnPlants();

        SpawnWeeds();
        PlaceLight();
        Render();
        ApplyDroneParameters();

        ApplyPlantStatus();
    }

    /// <summary>
    /// Look for all the gameobjects of the scene with the tag "Field_Holder" and destroy them.
    /// </summary>
    private void CheckSceneForField()
    {
        GameObject[] field_objects = GameObject.FindGameObjectsWithTag("Field_Holder");
        if (field_objects != null)
        {
            foreach(GameObject _g in field_objects)
            {
                 destroy(_g);
            }
        }
        instantiated_field_holder = Instantiate(field_holder);
        instantiated_field_holder.transform.position = Vector3.zero;
    }

    /// <summary>
    /// Spawn the field and manage its scale.
    /// </summary>
    private void SpawnField()
    {
        //instantiating the field, managing scale and location
        GameObject field_part = Instantiate(field_ref, Vector3.zero, Quaternion.identity);
        field_part.transform.localScale = new Vector3(width, 1f, height);
        field_part.transform.parent = instantiated_field_holder.transform;

        field_center = new Vector3((width * field_size - field_size) / 2, 0f, (height * field_size - field_size) / 2);
        field_part.transform.localPosition = field_center;

        ////Manage Texture granularity by acessing shader property
        Renderer _field_part_renderer = field_part.GetComponent<Renderer>();
        _field_part_renderer.sharedMaterial.shader = Shader.Find("Shader Graphs/DryRockyDirt");
        float _gran_rd = AveragePlusRandom(field_texture_average_granularity, field_texture_granularity_random);
        _field_part_renderer.sharedMaterial.SetVector("_Vector2Tiling", new Vector2(width / _gran_rd, height / _gran_rd));
    }

    /// <summary>
    /// Generates initial positions of the plants in the field according to the 
    /// strategy used in the early stages of the project.
    /// </summary>
    private void GeneratePlantCoordinates_LinearV1()
    {
        all_plants_coordinates = new List<Vector2>();
        field_monitoring_all_plants_status = new List<AllPlantStatus>();

        float x_plant = 0;
        float z_plant = 0;

        float b = -width * field_size - field_size / 2;

        while (b < height * field_size - field_size / 2)
        {
            x_plant = -field_size / 2;
            z_plant = b;

            int row_plant_first_index = all_plants_coordinates.Count;
            int row_plant_last_index = all_plants_coordinates.Count;

            while (x_plant < width * field_size - field_size / 2)
            {
                float _rad = Mathf.Deg2Rad * AveragePlusRandom(crop_rows_average_direction, crop_rows_direction_random);
                float _hyp = AveragePlusRandom(crop_plants_average_spacing, crop_plants_spacing_random);

                x_plant += Mathf.Cos(_rad) * _hyp;
                z_plant += Mathf.Sin(_rad) * _hyp;

                if (CheckInsideField(x_plant, z_plant))
                {
                    all_plants_coordinates.Add(new Vector2(x_plant, z_plant));
                    row_plant_last_index++;
                }
            }

            if (row_plant_last_index - row_plant_first_index > 0)
            {
                all_plants_rows_indeces.Add(row_plant_last_index);
            }

            b += AveragePlusRandom(crop_rows_average_spacing, crop_rows_spacing_random) / Mathf.Cos(Mathf.Deg2Rad * crop_rows_average_direction);
        }
    }

    /// <summary>
    /// Generates initial positions of the plants in the field according to the 
    /// strategy mimicing a Seader Drill attached to a tractor
    /// </summary>
    private void GeneratePlantCoordinates_LinearSeaderDrill()
    {
        all_plants_coordinates = new List<Vector2>();
        field_monitoring_all_plants_status = new List<AllPlantStatus>();

        float x_plant = 0;
        float z_plant = 0;

        float _rad = Mathf.Deg2Rad * AveragePlusRandom(crop_rows_average_direction, crop_rows_direction_random);
        float a = Mathf.Tan(_rad);
        float width_target = UnityEngine.Random.Range((1 - 0.025f) * width * field_size - field_size / 2, width * field_size - field_size / 2);
        float b = -a * width_target - field_size / 2;//the b parameter at which we should begin.

        int crop_rows_counter = 1;
        while (b < height * field_size)
        {
            int row_plant_first_index = all_plants_coordinates.Count;
            int row_plant_last_index = all_plants_coordinates.Count;

            x_plant = Mathf.Max(-b / a - field_size / 2, -field_size / 2);
            if (x_plant == -field_size / 2)
            {
                z_plant = b - field_size / 2;
            }
            else
            {
                z_plant = -field_size / 2;
            }
            while (x_plant + Mathf.Cos(_rad) * crop_plant_seader_drill_spacing < width * field_size - field_size / 2 &&
                   z_plant + Mathf.Sin(_rad) * crop_plant_seader_drill_spacing < height * field_size - field_size / 2)
            {
                x_plant += Mathf.Cos(_rad) * crop_plant_seader_drill_spacing;
                z_plant += Mathf.Sin(_rad) * crop_plant_seader_drill_spacing;

                float _x_plant = x_plant + UnityEngine.Random.Range(-crop_plant_position_radius, crop_plant_position_radius);
                float _z_plant = z_plant + UnityEngine.Random.Range(-crop_plant_position_radius, crop_plant_position_radius);
                all_plants_coordinates.Add(new Vector2(_x_plant, _z_plant));
            }

            if (row_plant_last_index - row_plant_first_index > 0)
            {
                all_plants_rows_indeces.Add(row_plant_last_index);
            }

            if (crop_rows_counter == crop_rows_on_seeder_drill)
            {
                _rad = Mathf.Deg2Rad * AveragePlusRandom(crop_rows_average_direction, crop_rows_direction_random);
                a = Mathf.Tan(_rad);
                b += AveragePlusRandom(crop_rows_average_spacing_between_seader_passes,
                                        crop_rows_spacing_between_seader_passes_random) / Mathf.Cos(_rad);
                crop_rows_counter = 1;
            }
            else
            {
                b += crop_rows_spacing_in_seader_drill;
                crop_rows_counter++;
            }
        }
    }

    /// <summary>
    /// Fills in the all_plants_living_status arrays.
    /// </summary>
    private void ApplyProbabilityGrowthDistribution()
    {
        AllPlantStatus allPlantStatus = new AllPlantStatus();
        allPlantStatus._allPlantStatus = new bool[all_plants_coordinates.Count];
        for (int i = 0; i < all_plants_coordinates.Count; ++i)
        {
            if (counter_growth_stages > 0)
            {
                if (field_monitoring_all_plants_status[counter_growth_stages - 1]._allPlantStatus[i])//if plant is alive at the previous growth step
                {
                    allPlantStatus._allPlantStatus[i] = IsPlantAlive(all_plants_coordinates[i].x, all_plants_coordinates[i].y);
                }
            }
            else
            {
                allPlantStatus._allPlantStatus[i] = IsPlantAlive(all_plants_coordinates[i].x, all_plants_coordinates[i].y);
            }
        }
        field_monitoring_all_plants_status.Add(allPlantStatus);
    }

    /// <summary>
    /// Computes if the plants is considered alive or not depending on the PlantGrowthProbabilityDistribution
    /// </summary>
    /// <param name="_x">float, plant GameObject X coordinates</param>
    /// <param name="_z">float, plant GameObject Z coordinates</param>
    /// <returns>Returns a boolean. true if the plants is considered alive and false otherwise.</returns>
    private bool IsPlantAlive(float _x, float _z)
    {
        bool plant_alive = false;

        switch (plant_growth_proba_distribution[counter_growth_stages])
        {
            case PlantGrowthProbabilityDistribution.Constant:
                if (UnityEngine.Random.Range(0f, 1f) < plant_growing_probability[counter_growth_stages])
                {
                    plant_alive = true;
                }
                break;

            case PlantGrowthProbabilityDistribution.Custom_Curves:
                float x_normalized_coord = (_x + field_size / 2) / (field_size * width);
                float z_normalized_coord = (_z + field_size / 2) / (field_size * height);

                float test_value = UnityEngine.Random.Range(0f, 1f);

                plant_alive = (test_value < X_Growth_Distribution[counter_growth_stages].Evaluate(x_normalized_coord)) &&
                              (test_value < Z_Growth_Distribution[counter_growth_stages].Evaluate(z_normalized_coord));
                break;
        }

        return plant_alive;
    }

    /// <summary>
    /// Spawn the plants organized as rows on the field in a linear fashion v1
    /// </summary>
    /// <remarks> DEPRECATED! To be Removed </remarks>
    private void SpawnPlants_LinearV1()
    {
        all_target_plants = new List<GameObject>();

        float x_plant = 0;
        float z_plant = 0;

        float b = -width * field_size - field_size / 2;
        
        while (b < height * field_size - field_size / 2)
        {
            GameObject plant_row = Instantiate(plant_row_holder) as GameObject;
            plant_row.transform.parent = instantiated_field_holder.transform;

            x_plant = -field_size / 2;
            z_plant = b;

            while (x_plant < width * field_size - field_size / 2)
            {
                float _rad = Mathf.Deg2Rad * AveragePlusRandom(crop_rows_average_direction, crop_rows_direction_random);
                float _hyp = AveragePlusRandom(crop_plants_average_spacing, crop_plants_spacing_random);

                x_plant += Mathf.Cos(_rad) * _hyp;
                z_plant += Mathf.Sin(_rad) * _hyp;

                if (CheckInsideField(x_plant, z_plant))
                {
                    SpawnPlant(x_plant, z_plant, plant_row);
                }
            }

            if (plant_row.transform.childCount == 0)
            {
                destroy(plant_row);
            }

            b += AveragePlusRandom(crop_rows_average_spacing, crop_rows_spacing_random) / Mathf.Cos(Mathf.Deg2Rad * crop_rows_average_direction) ;
        }
        
    }

    /// <summary>
    /// This version simulates the seeder drill that spans several crop crows.
    /// So, the direction and spacing of the crop rows are different only between
    /// series of crop rows.
    /// </summary>
    /// <remarks> DEPRECATED! To be Removed </remarks>
    private void SpawnPlants_LinearSeaderDrill()
    {
        all_target_plants = new List<GameObject>();

        float x_plant = 0;
        float z_plant = 0;

        float _rad = Mathf.Deg2Rad * AveragePlusRandom(crop_rows_average_direction, crop_rows_direction_random);
        float a = Mathf.Tan(_rad);
        float width_target = UnityEngine.Random.Range((1-0.025f) * width * field_size - field_size / 2, width * field_size - field_size / 2);
        float b = -a * width_target -field_size / 2;//the b parameter at which we should begin.

        int crop_rows_counter = 1;
        while (b < height * field_size)
        {
            GameObject plant_row = Instantiate(plant_row_holder) as GameObject;
            plant_row.transform.parent = instantiated_field_holder.transform;

            x_plant = Mathf.Max(-b/a -field_size / 2, -field_size/2);
            if (x_plant == -field_size / 2)
            {
                z_plant = b - field_size / 2;
            }
            else
            {
                z_plant = -field_size / 2;
            }
            while (x_plant + Mathf.Cos(_rad) * crop_plant_seader_drill_spacing < width * field_size - field_size / 2 &&
                   z_plant + Mathf.Sin(_rad) * crop_plant_seader_drill_spacing < height * field_size - field_size / 2)
            {
                x_plant += Mathf.Cos(_rad) * crop_plant_seader_drill_spacing;
                z_plant += Mathf.Sin(_rad) * crop_plant_seader_drill_spacing;
                
                SpawnPlant(x_plant, z_plant, plant_row);
            }

            if (plant_row.transform.childCount == 0)
            {
                destroy(plant_row);
            }
            
            if (crop_rows_counter == crop_rows_on_seeder_drill)
            {
                _rad = Mathf.Deg2Rad * AveragePlusRandom(crop_rows_average_direction, crop_rows_direction_random);
                a = Mathf.Tan(_rad);
                b += AveragePlusRandom(crop_rows_average_spacing_between_seader_passes,
                                        crop_rows_spacing_between_seader_passes_random) / Mathf.Cos(_rad);
                crop_rows_counter = 1;
            }
            else
            {
                b += crop_rows_spacing_in_seader_drill;
                crop_rows_counter++;
            }
        }
    }

    /// <summary>
    /// Spawn all the plants.
    /// Loops through the plants living status and activates models depending on the value. 
    /// </summary>
    private void SpawnPlants()
    {
        all_target_plants = new List<GameObject>();

        int row_count = 0;
        GameObject plant_row = Instantiate(plant_row_holder) as GameObject;
        plant_row.transform.parent = instantiated_field_holder.transform;

        for (int i = 0; i<all_plants_coordinates.Count; i++)
        {
            if (i == all_plants_rows_indeces[row_count])
            {
                ++row_count;
                plant_row = Instantiate(plant_row_holder) as GameObject;
                plant_row.transform.parent = instantiated_field_holder.transform;
            }

            SpawnPlant(all_plants_coordinates[i].x, all_plants_coordinates[i].y, plant_row);
        }
    }

    /// <summary>
    /// Set active the plants GameObjects based on the _allPlantStatus array of the 
    /// </summary>
    public void ApplyPlantStatus()
    {
        for (int i = 0; i < all_plants_coordinates.Count; i++)
        {
            all_target_plants[i].SetActive(
                field_monitoring_all_plants_status[target_growth_stage - 1]._allPlantStatus[i]);
        }
    }

    /// <summary>
    /// Spawn one plant on the X-Z plan in the scene
    /// </summary>
    /// <param name="_x">float, plant GameObject X coordinates</param>
    /// <param name="_z">float, plant GameObject Z coordinates</param>
    /// <param name="_parent_row">GameObject, the crops row that it is part of.</param>
    private void SpawnPlant(float _x, float _z, GameObject _parent_row)
    {
        GameObject plant = Instantiate(diff_growth_stages_plant_refs[target_growth_stage-1],
                            new Vector3(_x, 0, _z), Quaternion.identity) as GameObject;
        
        float _plant_radius = AveragePlusRandom(plant_average_radius[target_growth_stage-1],
                                                plant_radius_random[target_growth_stage - 1]);
        float _plant_size = AveragePlusRandom(plant_average_Yscale[target_growth_stage - 1],
                                              plant_Yscale_random[target_growth_stage - 1]);
        plant.transform.localScale = new Vector3( _plant_radius, _plant_size, _plant_radius);
        plant.transform.Rotate(Vector3.up, UnityEngine.Random.Range(0, 360));
        plant.transform.parent = _parent_row.transform;
        
        all_target_plants.Add(plant);
    }

    /// <summary>
    /// Spawn the weeds in the field based on Perlin Noise.
    /// </summary>
    private void SpawnWeeds()
    {
        _weed_PN_NoiseMap = GenerateNoiseMap(_weed_PN_MapWidth, _weed_PN_MapHeight, _weed_PN_Seed,
            _weed_PN_Octaves, _weed_PN_NoiseScale, _weed_PN_Persistance, _weed_PN_Lacunarity, _weed_PN_Offset, _weed_PN_Offset_random);
        
        float x_weed = 0f;
        float z_weed = 0f;
        for (int i = 0; i < _weed_PN_MapWidth; i++)
        {
            for (int j = 0; j < _weed_PN_MapHeight; j++)
            {
                if (_weed_PN_NoiseMap[i, j] > _weed_growth_threshold)
                {
                    if (UnityEngine.Random.Range(0f, 1f) < _weed_growing_probability)
                    {
                        float _var_x = AveragePlusRandom(_weed_average_local_position_modifier_x, _weed_local_position_modifier_x_random);
                        float pre_x_weed = ClampValueToMaxMin(i + UnityEngine.Random.Range(-_var_x, _var_x),0f, _weed_PN_MapWidth);

                        float _var_z = AveragePlusRandom(_weed_average_local_position_modifier_z, _weed_local_position_modifier_z_random);
                        float pre_y_weed = ClampValueToMaxMin(j + UnityEngine.Random.Range(-_var_z, _var_z), 0f, _weed_PN_MapHeight);

                        x_weed = pre_x_weed / _weed_PN_MapWidth * width * field_size - field_size / 2;
                        z_weed = pre_y_weed / _weed_PN_MapHeight * height * field_size - field_size / 2;
                        GameObject _weed = Instantiate(weed_ref, new Vector3(x_weed, 0, z_weed), Quaternion.identity);
                        _weed.transform.eulerAngles = new Vector3(-90, UnityEngine.Random.Range(0, 360), 0);
                        float _weed_radius = AveragePlusRandom(weed_average_radius, weed_radius_random);
                        float _weed_size = AveragePlusRandom(weed_average_Yscale, weed_Yscale_random);
                        _weed.transform.localScale = new Vector3(_weed_radius, _weed_radius, _weed_size); //because of the rotation, the radius is on X and Y axis and the size on the Z axis
                        _weed.transform.parent = instantiated_field_holder.transform;
                    }
                }
            }
        }

        if (_weed_PN_enablePreview)
        {
            Preview_WeedPerlinNoise(false);
        }
    }

    /// <summary>
    /// Generates the preview textures for the Perlin noise distribution of the weed in the field. The previews are displayed
    /// in the inspector if the option "Enable preview" checked.
    /// </summary>
    /// <param name="_Generate_Noise_Map"> Call the GenerateNoiseMap method prior to visalization</param>
    public void Preview_WeedPerlinNoise(bool _Generate_Noise_Map)
    {
        if (_Generate_Noise_Map)
        {
            _weed_PN_NoiseMap = GenerateNoiseMap(_weed_PN_MapWidth, _weed_PN_MapHeight, _weed_PN_Seed,
                                                _weed_PN_Octaves, _weed_PN_NoiseScale, _weed_PN_Persistance,
                                                _weed_PN_Lacunarity, _weed_PN_Offset, _weed_PN_Offset_random);
        }
        _weed_PN_Texture = GenerateTextureNoiseMap(_weed_PN_NoiseMap);

        _weed_PN_NoiseMap_Thresholded = _weed_PN_NoiseMap;
        for (int i = 0; i < _weed_PN_MapWidth; i++)
        {
            for (int j = 0; j < _weed_PN_MapHeight; j++)
            {
                if (_weed_PN_NoiseMap[i, j] > _weed_growth_threshold)
                {
                    _weed_PN_NoiseMap[i, j] = 1;
                }
                else
                {
                    _weed_PN_NoiseMap[i, j] = 0;
                }
            }
        }
        _weed_PN_Texture_Thresholded = GenerateTextureNoiseMap(_weed_PN_NoiseMap_Thresholded);

        int _newW = 0;
        int _newH = 0;
        if (width > height)
        {
            _newW = _weed_PN_Texture_rescale;
            _newH = Mathf.RoundToInt((float)height / width * _weed_PN_Texture_rescale);
        }
        else if (width < height)
        {
            _newW = Mathf.RoundToInt((float)width / height * _weed_PN_Texture_rescale);
            _newH = _weed_PN_Texture_rescale;
        }
        else
        {
            _newW = _weed_PN_Texture_rescale;
            _newH = _weed_PN_Texture_rescale;
        }
        TextureScale.Bilinear(_weed_PN_Texture, _newW, _newH);
        TextureScale.Bilinear(_weed_PN_Texture_Thresholded, _newW, _newH);
    }

    /// <summary>
    /// Places the directional light representing the sun
    /// </summary>
    private void PlaceLight()
    {
        float elevation = AveragePlusRandom(sun_average_elevation, sun_elevation_random);

        float azimuth = AveragePlusRandom(sun_average_azimuth, sun_azimuth_random);
        directional_light_ref.transform.eulerAngles = new Vector3(elevation, 0, 0);//control elevation
        directional_light_ref.transform.Rotate(Vector3.up, azimuth); //control azimuth (but modified since we do not calculate from the north)
    }

    /// <summary>
    /// Manages how to render the crops field depending on the value of labellingMode
    /// </summary>
    public void Render()
    {
        if (labellingMode)
        {
            ApplyLabellingMode(instantiated_field_holder);

            if (castShadow_labellingMode)
            {
                ApplyCastShadow(instantiated_field_holder);
            }
            else
            {
                ApplyDontCastShadow(instantiated_field_holder);
            }
        }
        else
        {
            ApplyCastShadow(instantiated_field_holder);
            ApplyRealityMode(instantiated_field_holder);
        }
    }

    /// <summary>
    /// The drone parameters section includes parameters coming from other components.
    /// We need this specific method to update them all.
    /// </summary>
    public void ApplyDroneParameters()
    {
        drone_ref.transform.position = new Vector3(drone_ref.transform.position.x, drone_altitude, drone_ref.transform.position.z) ;

        drone_ref.GetComponent<DroneBehaviour>().field_generator_ref = this;
        drone_ref.GetComponent<DroneBehaviour>().x_overlapping = image_capture_horizontal_overlapping;
        drone_ref.GetComponent<DroneBehaviour>().y_overlapping = image_capture_vertical_overlapping;
        //drone_ref.GetComponent<DroneBehaviour>()._smoothTime = drone_smoothTime;

        drone_camera = drone_ref.GetComponentInChildren<Camera>();
        drone_camera.sensorSize = camera_sensor_size;
        drone_camera.focalLength = camera_focal_length;

        drone_camera.GetComponent<CaptureImage>().captureWidth = image_size.x;
        drone_camera.GetComponent<CaptureImage>().captureHeight = image_size.y;
    }

    /// <summary>
    /// Applies the method FromRealToLabelling to all children of <paramref name="_parent"/> which have a ChangeMaterial component attached
    /// </summary>
    /// <param name="_parent">The parent of the children on which we apply the labelling mode</param>
    public void ApplyLabellingMode(GameObject _parent)
    {
        if (_parent != null)
        {
            Transform[] _T = _parent.GetComponentsInChildren<Transform>();
            foreach (Transform _t in _T)
            {
                ChangeMaterial _cm = _t.gameObject.GetComponent<ChangeMaterial>();
                if (_cm != null)
                {
                    _cm.FromRealToLabelling();
                }
            }
        }
    }

    /// <summary>
    /// Applies the method FromLabellingToReal to all children of <paramref name="_parent"/> which have a ChangeMaterial component attached
    /// </summary>
    /// <param name="_parent">The parent of the children on which we apply the reality mode</param>
    public void ApplyRealityMode(GameObject _parent)
    {
        if (_parent != null)
        {
            Transform[] _T = _parent.GetComponentsInChildren<Transform>();
            foreach (Transform _t in _T)
            {
                ChangeMaterial _cm = _t.gameObject.GetComponent<ChangeMaterial>();
                if (_cm != null)
                {
                    _cm.FromLabellingToReal();
                }
            }
        }
    }

    /// <summary>
    /// Applies the method DontCastShadow to all children of <paramref name="_parent"/> which have a ChangeMaterial component attached
    /// </summary>
    /// <param name="_parent">The parent of the children for which we disable the shadow casting</param>
    public void ApplyDontCastShadow(GameObject _parent)
    {
        if (_parent != null)
        {
            Transform[] _T = _parent.GetComponentsInChildren<Transform>();
            foreach (Transform _t in _T)
            {
                ChangeMaterial _cm = _t.gameObject.GetComponent<ChangeMaterial>();
                if (_cm != null)
                {
                    _cm.DontCastShadow();
                }
            }
        }
    }

    /// <summary>
    /// Applies the method CastShadow to all children of <paramref name="_parent"/> which have a ChangeMaterial component attached
    /// </summary>
    /// <param name="_parent">The parent of the children for which we disable the shadow casting</param>
    public void ApplyCastShadow(GameObject _parent)
    {
        if (_parent != null)
        {
            Transform[] _T = _parent.GetComponentsInChildren<Transform>();
            foreach (Transform _t in _T)
            {
                ChangeMaterial _cm = _t.gameObject.GetComponent<ChangeMaterial>();
                if (_cm != null)
                {
                    _cm.CastShadow();
                }
            }
        }
    }

    /// <summary>
    /// Generates an array filled with Perlin Noise.
    /// </summary>
    public float[,] GenerateNoiseMap(int MapWidth, int MapHeight, int Seed,
        int Octaves, float NoiseScale, float Persistance, float Lacunarity, Vector2 Offset, Vector2 Offset_Random)
    {
        return PerlinNoiseGenerator.GenerationNoiseMap(
            MapWidth, MapHeight, Seed, Octaves, NoiseScale, Persistance, Lacunarity, Offset, Offset_Random);
    }

    /// <summary>
    /// Generates a Texture to visualize the Perlin Noise.
    /// </summary>
    /// <param name="_noiseMap">The Perlin noise map generated with the method "GenerateNoiseMap" </param>
    /// <returns></returns>
    public Texture2D GenerateTextureNoiseMap(float[,] _noiseMap)
    {
        return TextureGenerator.TextureFromNoiseMap(_noiseMap);
    }

    /// <summary>
    /// Generates a Perlin noise map and the Texture to visualize it.
    /// </summary>
    /// <returns></returns>
    public Texture2D GenerateTextureNoiseMap(int MapWidth, int MapHeight, int Seed,
        int Octaves, float NoiseScale, float Persistance, float Lacunarity, Vector2 Offset, Vector2 Offset_Random)
    {
        float[,] MapNoise = new float[MapWidth, MapHeight];

        MapNoise = GenerateNoiseMap(MapWidth, MapHeight, Seed, Octaves, NoiseScale, Persistance, Lacunarity, Offset, Offset_Random);

        return TextureGenerator.TextureFromNoiseMap(MapNoise);
    }

    /// <summary>
    /// Returns a random value within the bin bounded by <paramref  name="_random"/>
    /// centered on <paramref  name="_average"/>.
    /// </summary>
    /// <param name="_average"></param>
    /// <param name="_random"></param>
    /// <returns></returns>
    private float AveragePlusRandom(float _average, float _random)
    {
        return _average + UnityEngine.Random.Range(-_random* _average, _random* _average);
    }

    /// <summary>
    /// Makes sure to return a value between <paramref name="_min"/> and <paramref name="_max"/>.
    /// </summary>
    /// <param name="_value"></param>
    /// <param name="_max"></param>
    /// <param name="_min"></param>
    /// <returns>
    /// Returns <paramref name="_min"/> if <paramref name="_value"/> less than <paramref name="_min"/>
    /// Returns <paramref name="_max"/> if <paramref name="_value"/> greater than <paramref name="_min"/>
    /// Returns <paramref name="_value"/> otherwise.
    /// </returns>
    private float ClampValueToMaxMin(float _value, float _min, float _max)
    {
        if (_value > _max)
        {
            return _max;
        }
        else if (_value < _min)
        {
            return _min;
        }
        else
        {
            return _value;
        }
    }

    /// <summary>
    /// Checks wheter a position on the X-Z plane is within the field area.
    /// </summary>
    /// <param name="_x">Coordinate x of the X-Z plane</param>
    /// <param name="_z">Coordinate z of the X-Z plane</param>
    /// <returns>True if inside the field, False otherwise.</returns>
    private bool CheckInsideField(float _x, float _z)
    {
        return (_x > -field_size / 2 && _x < width * field_size - field_size / 2) &&
                (_z > -field_size / 2 && _z < height * field_size - field_size / 2);
    }

    /// <summary>
    /// Destroy a gameobject according to the 2 built-in method of Unity if we are in the editor or in Play mode.
    /// </summary>
    /// <param name="_g">The game object to destroy</param>
    public void destroy(GameObject _g)
    {
        if (Application.isEditor)
        {
            DestroyImmediate(_g);
        }
        else
        {
            Destroy(_g);
        }
    }

    //public void SaveParameters(string _savePath)
    //{
    //    Dictionary<string, object> _dictRoot = new Dictionary<string, object>();

    //    //Dictionary for the field parameters
    //    Dictionary<string, object>  _dictFieldParameters = FieldParametersToDictionary();

    //    //Dictionary for the plant parameters
    //    Dictionary<string, object> _dictPlantParameters = PlantParametersToDictionary();

    //    //Dictionary for the weed parameters (Perlin Noise included)
    //    Dictionary<string, object> _dictWeedParameters = WeedParametersToDictionary();

    //    //Dictionary for the crops rows parameters
    //    Dictionary<string, object> _dictCRParameters = CropRowsParametersToDictionary();

    //    //Dictionary for the sun parameters
    //    Dictionary<string, object> _dictSunParameters = SunParametersToDictionary();

    //    //Dictionaries for drone parameters
    //    Dictionary<string, object> _dictDroneParameters = DroneParametersToDictionary();


    //    _dictRoot.Add("Field_Parameters", _dictFieldParameters);
    //    _dictRoot.Add("Plant_Parameters", _dictPlantParameters);
    //    _dictRoot.Add("Weed_Parameters", _dictWeedParameters);
    //    _dictRoot.Add("Crops_Rows_Parameters", _dictCRParameters);
    //    _dictRoot.Add("Sun_Parameters", _dictSunParameters);
    //    _dictRoot.Add("Drone_Parameters", _dictDroneParameters);

    //    XmlSave.Save(_savePath, "Field_Generator_Parameters.xml", _dictRoot);
    //}

    //private Dictionary<string, object> FieldParametersToDictionary()
    //{
    //    Dictionary<string, object> _dictFieldParameters = new Dictionary<string, object>();
    //    //_dictFieldParameters.Add("field_ref", field_ref.name);
    //    //_dictFieldParameters.Add("field_holder", field_holder.name);
    //    Dictionary<string, object> _dictFieldDimensions = new Dictionary<string, object>();
    //    _dictFieldDimensions.Add("height", height.ToString("G", CultureInfo.InvariantCulture));
    //    _dictFieldDimensions.Add("width", width.ToString("G", CultureInfo.InvariantCulture));
    //    _dictFieldParameters.Add("scale", _dictFieldDimensions);

    //    Dictionary<string, object> _dictFieldTexture = new Dictionary<string, object>();
    //    _dictFieldTexture.Add("field_texture_average_granularity", field_texture_average_granularity.ToString("G", CultureInfo.InvariantCulture));
    //    _dictFieldTexture.Add("field_texture_granularity_random", field_texture_granularity_random.ToString("G", CultureInfo.InvariantCulture));
    //    _dictFieldParameters.Add("texture", _dictFieldTexture);

    //    return _dictFieldParameters;
    //}

    //private Dictionary<string, object> PlantParametersToDictionary()
    //{
    //    Dictionary<string, object> _dictPlantParameters = new Dictionary<string, object>();
    //    _dictPlantParameters.Add("plant_ref", plant_ref.name);
    //    _dictPlantParameters.Add("plant_growing_probability", plant_growing_probability.ToString("G", CultureInfo.InvariantCulture));

    //    Dictionary<string, object> _dictPlantYScale = new Dictionary<string, object>();
    //    _dictPlantYScale.Add("plant_average_Yscale", plant_average_Yscale.ToString("G", CultureInfo.InvariantCulture));
    //    _dictPlantYScale.Add("plant_Yscale_random", plant_Yscale_random.ToString("G", CultureInfo.InvariantCulture));
    //    _dictPlantParameters.Add("Yscale", _dictPlantYScale);

    //    Dictionary<string, object> _dictPlantCrops = new Dictionary<string, object>();
    //    _dictPlantCrops.Add("crop_plants_average_spacing", crop_plants_average_spacing.ToString("G", CultureInfo.InvariantCulture));
    //    _dictPlantCrops.Add("crop_plants_spacing_random", crop_plants_spacing_random.ToString("G", CultureInfo.InvariantCulture));
    //    _dictPlantParameters.Add("crop_plants", _dictPlantCrops);

    //    return _dictPlantParameters;
    //}

    //private Dictionary<string, object> WeedParametersToDictionary()
    //{
    //    Dictionary<string, object> _dictWeedParameters = new Dictionary<string, object>();
    //    _dictWeedParameters.Add("weed_ref", weed_ref.name);
    //    _dictWeedParameters.Add("_weed_growing_probability", _weed_growing_probability.ToString("G", CultureInfo.InvariantCulture));

    //    Dictionary<string, object> _dictWeedYScale = new Dictionary<string, object>();
    //    _dictWeedYScale.Add("weed_average_Yscale", weed_average_Yscale.ToString("G", CultureInfo.InvariantCulture));
    //    _dictWeedYScale.Add("weed_Yscale_random", weed_Yscale_random.ToString("G", CultureInfo.InvariantCulture));
    //    _dictWeedParameters.Add("Yscale", _dictWeedYScale);

    //    Dictionary<string, object> _dictWeedRadius = new Dictionary<string, object>();
    //    _dictWeedRadius.Add("weed_average_radius", weed_average_radius.ToString("G", CultureInfo.InvariantCulture));
    //    _dictWeedRadius.Add("weed_radius_random", weed_radius_random.ToString("G", CultureInfo.InvariantCulture));
    //    _dictWeedParameters.Add("crop_plants", _dictWeedRadius);


    //    //Perlin Noise
    //    Dictionary<string, object> _dictWeedPN = new Dictionary<string, object>();

    //    Dictionary<string, object> _dictNoiseDimensions = new Dictionary<string, object>();
    //    _dictNoiseDimensions.Add("_weed_PN_MapHeight", _weed_PN_MapHeight.ToString("G", CultureInfo.InvariantCulture));
    //    _dictNoiseDimensions.Add("_weed_PN_MapWidth", _weed_PN_MapWidth.ToString("G", CultureInfo.InvariantCulture));
    //    _dictWeedPN.Add("map_dimensions", _dictNoiseDimensions);

    //    _dictWeedPN.Add("_weed_PN_Seed", _weed_PN_Seed.ToString("G", CultureInfo.InvariantCulture));
    //    _dictWeedPN.Add("_weed_PN_Octaves", _weed_PN_Octaves.ToString("G", CultureInfo.InvariantCulture));
    //    _dictWeedPN.Add("_weed_PN_NoiseScale", _weed_PN_NoiseScale.ToString("G", CultureInfo.InvariantCulture));
    //    _dictWeedPN.Add("_weed_PN_Persistance", _weed_PN_Persistance.ToString("G", CultureInfo.InvariantCulture));
    //    _dictWeedPN.Add("_weed_PN_Lacunarity", _weed_PN_Lacunarity.ToString("G", CultureInfo.InvariantCulture));

    //    Dictionary<string, object> _dictNoiseOffset = new Dictionary<string, object>();
    //    _dictNoiseOffset.Add("X", _weed_PN_Offset.x.ToString("G", CultureInfo.InvariantCulture));
    //    _dictNoiseOffset.Add("Y", _weed_PN_Offset.y.ToString("G", CultureInfo.InvariantCulture));
    //    _dictWeedPN.Add("_weed_PN_Offset", _dictNoiseDimensions);

    //    _dictWeedPN.Add("_weed_growth_threshold", _weed_growth_threshold.ToString("G", CultureInfo.InvariantCulture));

    //    _dictWeedParameters.Add("perlin_noise", _dictWeedPN);

    //    return _dictWeedParameters;
    //}

    //private Dictionary<string, object> CropRowsParametersToDictionary()
    //{
    //    Dictionary<string, object> _dictCRParameters = new Dictionary<string, object>();

    //    Dictionary<string, object> _dictCRDirection = new Dictionary<string, object>();
    //    _dictCRDirection.Add("crop_rows_average_direction", crop_rows_average_direction.ToString("G", CultureInfo.InvariantCulture));
    //    _dictCRDirection.Add("crop_rows_direction_random", crop_rows_direction_random.ToString("G", CultureInfo.InvariantCulture));
    //    _dictCRParameters.Add("direction", _dictCRDirection);

    //    Dictionary<string, object> _dictCRSpacing = new Dictionary<string, object>();
    //    _dictCRSpacing.Add("crop_rows_average_spacing", crop_rows_average_spacing.ToString("G", CultureInfo.InvariantCulture));
    //    _dictCRSpacing.Add("crop_rows_spacing_random", crop_rows_spacing_random.ToString("G", CultureInfo.InvariantCulture));
    //    _dictCRParameters.Add("spacing", _dictCRSpacing);

    //    return _dictCRParameters;
    //}

    //private Dictionary<string, object> SunParametersToDictionary()
    //{
    //    Dictionary<string, object> _dictSunParameters = new Dictionary<string, object>();

    //    Dictionary<string, object> _dictSunElevation= new Dictionary<string, object>();
    //    _dictSunElevation.Add("sun_average_elevation", sun_average_elevation.ToString("G", CultureInfo.InvariantCulture));
    //    _dictSunElevation.Add("sun_elevation_random", sun_elevation_random.ToString("G", CultureInfo.InvariantCulture));
    //    _dictSunParameters.Add("elevation", _dictSunElevation);

    //    Dictionary<string, object> _dictSunAzimuth = new Dictionary<string, object>();
    //    _dictSunAzimuth.Add("sun_average_azimuth", sun_average_azimuth.ToString("G", CultureInfo.InvariantCulture));
    //    _dictSunAzimuth.Add("sun_azimuth_random", sun_azimuth_random.ToString("G", CultureInfo.InvariantCulture));
    //    _dictSunParameters.Add("spacing", _dictSunAzimuth);

    //    return _dictSunParameters;
    //}

    //private Dictionary<string, object> DroneParametersToDictionary()
    //{
    //    Dictionary<string, object> _dictDroneParameters = new Dictionary<string, object>();

    //    _dictDroneParameters.Add("drone_altitude", drone_altitude.ToString("G", CultureInfo.InvariantCulture));

    //    Dictionary<string, object> _dictCamera = new Dictionary<string, object>();
    //    Dictionary<string, object> _dictCameraSensor = new Dictionary<string, object>();
    //    _dictCameraSensor.Add("x", camera_sensor_size.x.ToString("G", CultureInfo.InvariantCulture));
    //    _dictCameraSensor.Add("y", camera_sensor_size.y.ToString("G", CultureInfo.InvariantCulture));
    //    _dictCamera.Add("camera_sensor_size", _dictCameraSensor);

    //    _dictCamera.Add("camera_focal_length", camera_focal_length.ToString("G", CultureInfo.InvariantCulture));

    //    _dictDroneParameters.Add("camera", _dictCamera);


    //    Dictionary<string, object> _dictImageAcquisition = new Dictionary<string, object>();
    //    _dictImageAcquisition.Add("image_capture_horizontal_overlapping", image_capture_horizontal_overlapping.ToString("G", CultureInfo.InvariantCulture));
    //    _dictImageAcquisition.Add("image_capture_vertical_overlapping", image_capture_vertical_overlapping.ToString("G", CultureInfo.InvariantCulture));

    //    _dictDroneParameters.Add("spacing", _dictImageAcquisition);

    //    return _dictDroneParameters;
    //}

}
