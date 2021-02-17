using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Perception.GroundTruth;

public class CustomizePerception : MonoBehaviour
{

    private AnnotationDefinition plantAnnotationDefinition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DefineViewPortAnnotation()
    {
        plantAnnotationDefinition = DatasetCapture.RegisterAnnotationDefinition(
            "View_Port_Position",
            "The position relative to the screen",
            id: Guid.Parse("C0B4A22C-0420-4D9F-BAFC-954B8F7B35A7"));
    }

    public void ApplyViewPortAnnotation(List<TargetPlantCounting.PlantAnnotation> _capture)
    {
        //Vector3 _correctedTransform = new Vector3(_target.transform.position.x,
        //                                          _target.transform.position.y + _target.transform.localScale.y * _target.GetComponent<MeshFilter>().sharedMesh.bounds.size.y,
        //                                          _target.transform.position.z);

        //Vector2 _corrected_target_ViewPort = GetComponent<Camera>().WorldToViewportPoint(_correctedTransform);
        //Dictionary<string, object> _viewPortDict = new Dictionary<string, object>();
        //_viewPortDict.Add("ViewPort", _corrected_target_ViewPort);
        //_viewPortDict.Add("instance_id", _target.GetComponent<Labeling>().instanceId);

        var sensorHandle = GetComponent<PerceptionCamera>().SensorHandle;
        
        if (sensorHandle.ShouldCaptureThisFrame)
        {
            var asyncplantAnnotationDefinition = sensorHandle.ReportAnnotationAsync(plantAnnotationDefinition);

            asyncplantAnnotationDefinition.ReportValues(_capture);
                //new object[] { _corrected_target_ViewPort,
                //               _target.GetComponent<Labeling>().instanceId});
        }
    }
}
