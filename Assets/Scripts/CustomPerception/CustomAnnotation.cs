using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Perception.GroundTruth;

public class CustomAnnotation : MonoBehaviour
{
    private AnnotationDefinition plantAnnotationDefinition;
    
    public void DefineViewPortAnnotation()
    {
        plantAnnotationDefinition = DatasetCapture.RegisterAnnotationDefinition(
            "View_Port_Position",
            "The position relative to the screen",
            id: Guid.Parse("C0B4A22C-0420-4D9F-BAFC-954B8F7B35A7"));
    }

    public void ApplyViewPortAnnotation(List<TargetPlantCounting.PlantAnnotation> _capture)
    {
        SensorHandle sensorHandle = GetComponent<PerceptionCamera>().SensorHandle;
        
        if (sensorHandle.ShouldCaptureThisFrame)
        {
            //Debug.Log("ShouldCapture");
            AsyncAnnotation asyncplantAnnotationDefinition = sensorHandle.ReportAnnotationAsync(plantAnnotationDefinition);

            asyncplantAnnotationDefinition.ReportValues(_capture);
        }
    }
}
