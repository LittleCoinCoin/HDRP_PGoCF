using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Field))]
public class FieldEditor : Editor
{
    Field field_ref;

    SerializedProperty open_field_parameters_property;
    Texture2D field_parameters_panel_texture;

    SerializedProperty open_plant_parameters_property;
    Texture2D plant_parameters_panel_texture;

    SerializedProperty open_weed_parameters_property;
    Texture2D weed_parameters_panel_texture;

    SerializedProperty open_CR_parameters_property;
    Texture2D CR_parameters_panel_texture;

    SerializedProperty open_sun_parameters_property;
    Texture2D sun_parameters_panel_texture;

    SerializedProperty open_rendering_parameters_property;
    Texture2D rendering_parameters_panel_texture;

    SerializedProperty open_drone_parameters_property;
    Texture2D drone_parameters_panel_texture;

    Texture2D close_h;
    Texture2D open_h;
    GUIStyle popupStyle;

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        CatchProperties();

        CatchTextureButtons();

        UpdateParametersPanelTextures();

        field_ref = (Field)target;

        FieldParametersManager();

        EditorGUILayout.Space();

        PlantParametersManager();

        EditorGUILayout.Space();

        WeedParametersManager();

        EditorGUILayout.Space();

        CropRowsParametersManager();

        EditorGUILayout.Space();

        SunParametersManager();

        EditorGUILayout.Space();

        DroneParametersManager();

        EditorGUILayout.Space();

        RenderingParametersManager();


        serializedObject.ApplyModifiedProperties();


        EditorGUILayout.Space();

        GenerateButtonManager();

        EditorGUILayout.Space();

        SaveParametersManager();

        EditorGUILayout.Space();

        ImageCaptureManager();

    }

    private void FieldParametersManager()
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(field_parameters_panel_texture, popupStyle, GUILayout.Width(20), GUILayout.Height(15)))
        {
            open_field_parameters_property.boolValue = !open_field_parameters_property.boolValue;
        }
        EditorGUILayout.LabelField("Field parameters", EditorStyles.boldLabel);
        //GUILayout.Toggle(open_field_parameters, "show");
        EditorGUILayout.EndHorizontal();
        
        if (EditorGUILayout.BeginFadeGroup(Bool2Float(open_field_parameters_property.boolValue)))
        {
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("field_ref"),
                    new GUIContent("Field 3D object", "The surface representing the ground."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("field_holder"),
                    new GUIContent("Container",
                    "An empty game object witht the tag \"Field_Holder\" to contain all the elements instantiated during the generation of the whole crop field."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("height"),
                    new GUIContent("Height", "The number of \"Field 3D object\" instantiated on the Z axis."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("width"),
                    new GUIContent("Width", "The number of \"Field 3D object\" instantiated on the X axis."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("field_texture_average_granularity"),
                    new GUIContent("Average texture grain", "The average tiling divider of the texture of the field."));
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("field_texture_granularity_random"),
                        new GUIContent("Variability", "Introduces variability in the tiling divider of the texture of the field."));

                }
            }
        }
        EditorGUILayout.EndFadeGroup();

        EditorGUILayout.EndVertical();
    }

    private void PlantParametersManager()
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(plant_parameters_panel_texture, popupStyle, GUILayout.Width(20), GUILayout.Height(15)))
        {
            open_plant_parameters_property.boolValue = !open_plant_parameters_property.boolValue;
        }
        EditorGUILayout.LabelField("Plant parameters", EditorStyles.boldLabel);
        //GUILayout.Toggle(open_field_parameters, "show");
        EditorGUILayout.EndHorizontal();
        if (EditorGUILayout.BeginFadeGroup(Bool2Float(open_plant_parameters_property.boolValue)))
        {
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("plant_ref"), new GUIContent("Plant 3D object", "The plant model."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("plant_growing_probability"), new GUIContent("Growth probability",
                    "Probability that the plant is indeed instantiated on the surface of the field. It simulates whether the seed planted at the begining germinated."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("plant_average_Yscale"),
                    new GUIContent("Average size", "Modifies the average scale of the \"Plant 3D object\" on the Y axis."));
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("plant_Yscale_random"),
                        new GUIContent("Variability", "Introduces variability among plants in regards to their scale on the Y axis."));

                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("plant_average_radius"),
                    new GUIContent("Average radius", "Modifies the average scale of the \"Plant 3D object\" on the X and Z axis."));
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("plant_radius_random"),
                        new GUIContent("Variability", "Introduces variability among plants in regards to their scale on the X and Z axis."));

                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("crop_plants_average_spacing"),
                    new GUIContent("Average spacing", "Simulates the general space between plants in the same crops rows."));
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("crop_plants_spacing_random"), new GUIContent("Variability",
                    "Introduces variability in the space between plants in the same crops row. This variability is introduced between two consecutive plants."));

                }
            }
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();
    }

    private void WeedParametersManager()
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(weed_parameters_panel_texture, popupStyle, GUILayout.Width(20), GUILayout.Height(15)))
        {
            open_weed_parameters_property.boolValue = !open_weed_parameters_property.boolValue;
        }
        EditorGUILayout.LabelField("Weed parameters", EditorStyles.boldLabel);
        //GUILayout.Toggle(open_field_parameters, "show");
        EditorGUILayout.EndHorizontal();
        if (EditorGUILayout.BeginFadeGroup(Bool2Float(open_weed_parameters_property.boolValue)))
        {
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("weed_ref"),
                    new GUIContent("Weed 3D object", "The weed model."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_weed_growing_probability"),
                    new GUIContent("Growth Probability", "The probability for the weed to grow in the area that is above the density threshold"));


                EditorGUILayout.PropertyField(serializedObject.FindProperty("weed_average_Yscale"),
                    new GUIContent("Average size", "Modifies the average scale of the \"Weed 3D object\" on the Y axis."));
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("weed_Yscale_random"), new GUIContent("Variability",
                    "Introduces variability among weeds in regards to their scale on the Y axis."));

                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("weed_average_radius"),
                    new GUIContent("Average radius", "Modifies the average scale of the \"Weed 3D object\" on the X and Z axis."));
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("weed_radius_random"), new GUIContent("Variability",
                    "Introduces variability among weeds in regards to their scale on the X and Z axis."));

                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_weed_local_random"),
                    new GUIContent("Position Variability", "Modifies the position of \"Weed 3D object\" on the X and Z axis after Perlin Noise instanciation."));

                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.LabelField("Perlin noise parameters", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_weed_PN_MapHeight"),
                    new GUIContent("Map Height", "The height of the texture on which we are going to map the Perlin Noise." +
                    "It does not have to be equal to the field height; in actual fact, it is recommanded to be way greater."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_weed_PN_MapWidth"),
                    new GUIContent("Map Width", "The width of the texture on which we are going to map the Perlin Noise." +
                    "It does not have to be equal to the field width; in actual fact, it is recommanded to be way greater."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_weed_PN_Seed"),
                    new GUIContent("Seed", "The seed of the pseudo random number generator"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_weed_PN_Octaves"),
                    new GUIContent("Octaves", ""));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_weed_PN_NoiseScale"),
                    new GUIContent("Noise scale", ""));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_weed_PN_Persistance"),
                    new GUIContent("Persistance", ""));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_weed_PN_Lacunarity"),
                    new GUIContent("Lacunarity", ""));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_weed_PN_Offset"),
                    new GUIContent("Offset", "X-axis and Y-axis offset of the perlin noise generated"));
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_weed_PN_Offset_random"), new GUIContent("Variability",
                    "Introduces variability on the offset of the Perlin Noise."));
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_weed_growth_threshold"),
                    new GUIContent("Density threshold", "Perlin noise threshold under which the weed cannot grow"));

                SerializedProperty _weedPN_EnablePreviewProperty = serializedObject.FindProperty("_weed_PN_enablePreview");
                EditorGUILayout.PropertyField(_weedPN_EnablePreviewProperty, new GUIContent("Enable preview", "Displays the preview of the Perlin Noise distribution"));


                //EditorGUILayout.PropertyField(serializedObject.FindProperty("_weed_PN_Texture"),
                //    new GUIContent("Distribution preview", "Preview of the perlin noise projected as a texture"));

                if (EditorGUILayout.BeginFadeGroup(Bool2Float(_weedPN_EnablePreviewProperty.boolValue)))
                {
                    GUILayout.BeginHorizontal();
                    SerializedProperty _weedPN_AutoUpdateProperty = serializedObject.FindProperty("_weed_PN_autoPreview");
                    EditorGUILayout.PropertyField(_weedPN_AutoUpdateProperty, new GUIContent("Auto preview", "Apply the changes everytimes a parameter of the Perlin Noise of this section is modified"));

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (_weedPN_AutoUpdateProperty.boolValue)
                        {
                            serializedObject.ApplyModifiedProperties();//mandatory
                            field_ref.Preview_WeedPerlinNoise();
                        }
                    }

                    if (GUILayout.Button("Preview"))
                    {
                        field_ref.Preview_WeedPerlinNoise();
                    }
                    GUILayout.EndHorizontal();

                    if (field_ref._weed_PN_Texture != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(new GUIContent(field_ref._weed_PN_Texture));
                        GUILayout.Label(new GUIContent(field_ref._weed_PN_Texture_Thresholded));
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndFadeGroup();
            }
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();
    }

    private void CropRowsParametersManager()
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(CR_parameters_panel_texture, popupStyle, GUILayout.Width(20), GUILayout.Height(15)))
        {
            open_CR_parameters_property.boolValue = !open_CR_parameters_property.boolValue;
        }
        EditorGUILayout.LabelField("Crops Rows parameters", EditorStyles.boldLabel);
        //GUILayout.Toggle(open_field_parameters, "show");
        EditorGUILayout.EndHorizontal();
        if (EditorGUILayout.BeginFadeGroup(Bool2Float(open_CR_parameters_property.boolValue)))
        {
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("plant_row_holder"), new GUIContent("Container",
                    "An empty game object witht the tag \"Plant_Row_Holder\" to contain all the plants instantiated during the generation of the crops row."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("crop_rows_average_direction"), new GUIContent("Average angle",
                    "Simulates the general orientation of the crops rows of the field"));
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("crop_rows_direction_random"), new GUIContent("Variability",
                    "Introduces variability in the direction of the row. This variability is introduced between two consecutive plants of an identical row."));
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("crop_rows_average_spacing"), new GUIContent("Average spacing",
                    "Simulates the general space between crops rows."));
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("crop_rows_spacing_random"), new GUIContent("Variability",
                    "Introduces variability in the space between crops rows. This variability is introduced between two consecutive crops rows."));

                }
            }
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();
    }

    private void SunParametersManager()
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(sun_parameters_panel_texture, popupStyle, GUILayout.Width(20), GUILayout.Height(15)))
        {
            open_sun_parameters_property.boolValue = !open_sun_parameters_property.boolValue;
        }
        EditorGUILayout.LabelField("Sun parameters", EditorStyles.boldLabel);
        //GUILayout.Toggle(open_field_parameters, "show");
        EditorGUILayout.EndHorizontal();

        if (EditorGUILayout.BeginFadeGroup(Bool2Float(open_sun_parameters_property.boolValue)))
        {
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("directional_light_ref"), new GUIContent("Sun Light", "The light object simulating the sun."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("sun_average_elevation"), new GUIContent("Average elevation",
                    "Modifies the average elevation of the sun. If we consider the Z axis pointing toward the North, then a value between between 0 and 90 degrees simulates the North Hemisphere; and" +
                    " a value betwwen 90 and 180 degrees simulates a sun in the South Hemisphere"));
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("sun_elevation_random"), new GUIContent("Variability", "Introduces variability on the sun elevation."));

                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("sun_average_azimuth"), new GUIContent("Average azimuth",
                    "This does not exactly correspond to the real definition of the azimuth but it has the same effect. If we consider the Z axis pointing toward the North (and then X axis toward the East)" +
                    " then, a value between 0 and 90 degrees puts the sun on the afternoon time and a value between 0 and -90 degrees puts the sun in the morning time."));
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("sun_azimuth_random"), new GUIContent("Variability",
                    "Introduces variability on the sun azimuth."));
                }
            }
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();
    }

    private void DroneParametersManager()
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(drone_parameters_panel_texture, popupStyle, GUILayout.Width(20), GUILayout.Height(15)))
        {
            open_drone_parameters_property.boolValue = !open_drone_parameters_property.boolValue;
        }
        EditorGUILayout.LabelField("Drone parameters", EditorStyles.boldLabel);
        //GUILayout.Toggle(open_field_parameters, "show");
        EditorGUILayout.EndHorizontal();

        if (EditorGUILayout.BeginFadeGroup(Bool2Float(open_drone_parameters_property.boolValue)))
        {
            //EditorGUI.BeginChangeCheck();
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("drone_ref"),
                    new GUIContent("Drone", "The gameobject defining the drone (must contain the camera equipped with CaptureImage.cs and CameraVision.cs)."));

                //EditorGUILayout.PropertyField(serializedObject.FindProperty("drone_smoothTime"),
                //                    new GUIContent("Movement time", "The time ine seconds the drone will take to move from one image capture position to another. If equal to 0 then the drone will teleport"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("drone_altitude"),
                                    new GUIContent("Altitude", "The focal length of the simulated lens of the camera."));

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Camera parameters", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("camera_sensor_size"),
                    new GUIContent("Sensor size", "The size of the simulated sensor of the camera."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("camera_focal_length"),
                                    new GUIContent("Focal length", "The focal length of the simulated lens of the camera."));

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Image acquisition parameters", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("image_capture_horizontal_overlapping"),
                                    new GUIContent("X overlapping", "The overlapping happening between two images captured next to each others horizontally (drone movinf to the right or the left)."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("image_capture_vertical_overlapping"),
                                    new GUIContent("Z overlapping", "The overlapping happening between two images captured next to each others vertically (drone moving forward or backward)."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("image_size"),
                                    new GUIContent("Image Size", "The size of the image captured."));

            }

            //SerializedProperty drone_AutoUpdateProperty = serializedObject.FindProperty("autoUdate_droneParameters");
            //EditorGUILayout.PropertyField(drone_AutoUpdateProperty, new GUIContent("Auto update", "Apply the changes everytimes a parameter of this section is modified"));

            //if (EditorGUI.EndChangeCheck())
            //{
            //    if (drone_AutoUpdateProperty.boolValue)
            //    {
            //        field_ref.ApplyDroneParameters();
            //    }
            //}

            if (GUILayout.Button("Apply"))
            {
                field_ref.ApplyDroneParameters();
            }

            if (GUILayout.Button("Start Position"))
            {
                field_ref.drone_ref.GetComponent<DroneBehaviour>().StartPosition();
            }

        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();
    }

    private void RenderingParametersManager()
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(rendering_parameters_panel_texture, popupStyle, GUILayout.Width(20), GUILayout.Height(15)))
        {
            open_rendering_parameters_property.boolValue = !open_rendering_parameters_property.boolValue;
        }
        EditorGUILayout.LabelField("Labelling rendering parameters", EditorStyles.boldLabel);
        //GUILayout.Toggle(open_field_parameters, "show");
        EditorGUILayout.EndHorizontal();
        if (EditorGUILayout.BeginFadeGroup(Bool2Float(open_rendering_parameters_property.boolValue)))
        {
            EditorGUI.BeginChangeCheck();
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("labellingMode"), new GUIContent("Activate Labelling", "Switches the rendering of the crops field to use the labelling materials."));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("castShadow_labellingMode"), new GUIContent("Cast Shadows", "Manages whether the objects related to the labelling mode will cast shadows."));

            }

            GUILayout.BeginHorizontal();
            SerializedProperty rendering_AutoUpdateProperty = serializedObject.FindProperty("autoUpdate_labellingMode");
            EditorGUILayout.PropertyField(rendering_AutoUpdateProperty, new GUIContent("Auto update", "Apply the changes everytimes a parameter of this section is modified"));

            if (EditorGUI.EndChangeCheck())
            {
                if (rendering_AutoUpdateProperty.boolValue)
                {
                    serializedObject.ApplyModifiedProperties();//mandatory
                    field_ref.Render();
                }
            }

            if (GUILayout.Button("Render"))
            {
                field_ref.Render();
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();
    }

    private void GenerateButtonManager()
    {
        if (GUILayout.Button("Generate Field"))
        {
            field_ref.Generator();
        }
    }

    private void SaveParametersManager()
    {
        if (GUILayout.Button("Save Parameters"))
        {
            field_ref.SaveParameters("C:/Users/eliot/Documents/Scolarité/AgroParisTech/3A/Stage_Tournesols/Unity");
        }
    }

    private void ImageCaptureManager()
    {
        if (GUILayout.Button("Capture All images"))
        {
            field_ref.drone_ref.GetComponent<DroneBehaviour>().AllImageCapture();
        }
    }

    private float Bool2Float(bool _b)
    {
        return _b? 1 : 0;
    }

    private void CatchProperties()
    {
        open_field_parameters_property = serializedObject.FindProperty("open_field_parameters");
        open_plant_parameters_property = serializedObject.FindProperty("open_plant_parameters");
        open_weed_parameters_property = serializedObject.FindProperty("open_weed_parameters");
        open_CR_parameters_property = serializedObject.FindProperty("open_CR_parameters");
        open_sun_parameters_property = serializedObject.FindProperty("open_sun_parameters");
        open_rendering_parameters_property = serializedObject.FindProperty("open_rendering_parameters");
        open_drone_parameters_property = serializedObject.FindProperty("open_drone_parameters");
    }

    /// <summary>
    /// Load the textures for the dropdown custom buttons
    /// </summary>
    private void CatchTextureButtons()
    {
        close_h = Resources.Load<Texture2D>("GUI/Triangle_Close_H");
        open_h = Resources.Load<Texture2D>("GUI/Triangle_Open_H");
        popupStyle = GUI.skin.FindStyle("ToolbarButton");
    }

    private void UpdateParametersPanelTextures()
    {
        field_parameters_panel_texture = UpdateParametersPanelTexture(open_field_parameters_property, field_parameters_panel_texture);

        plant_parameters_panel_texture = UpdateParametersPanelTexture(open_plant_parameters_property, plant_parameters_panel_texture);

        weed_parameters_panel_texture = UpdateParametersPanelTexture(open_weed_parameters_property, weed_parameters_panel_texture);

        CR_parameters_panel_texture = UpdateParametersPanelTexture(open_CR_parameters_property, CR_parameters_panel_texture);

        sun_parameters_panel_texture = UpdateParametersPanelTexture(open_sun_parameters_property, sun_parameters_panel_texture);

        rendering_parameters_panel_texture = UpdateParametersPanelTexture(open_rendering_parameters_property, rendering_parameters_panel_texture);

        drone_parameters_panel_texture = UpdateParametersPanelTexture(open_drone_parameters_property, drone_parameters_panel_texture);

    }

    private Texture2D UpdateParametersPanelTexture(SerializedProperty _openingStatus_property, Texture2D _buttonTexture)
    {
        if (_openingStatus_property.boolValue)
        {
            _buttonTexture = open_h;
        }
        else
        {
            _buttonTexture = close_h;
        }

        return _buttonTexture;
    }
}
