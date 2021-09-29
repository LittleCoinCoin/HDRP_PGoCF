using System.Collections;
using UnityEngine;
using UnityEngine.Perception.GroundTruth;

/// <summary>
/// Handles the logic to capture complete fields or a subset of the field.
/// </summary>
public class MetaImageCapture : MonoBehaviour
{
    public Field fieldGenerator_ref;

    public Camera capturingCamera_ref;
    
    public bool GridSubsetCapture = false;
    public int minWidthGridSubset;
    public int maxWidthGridSubset;
    public int minHeightGridSubset;
    public int maxHeightGridSubset;

    public bool CaptureOrder = false;
    public bool CaptureDone = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (fieldGenerator_ref.drone_ref.GetComponent<DroneBehaviour>().flyingMode == DroneBehaviour.FlyingMode.ImageCapture)
            {
                CaptureOrder = true;
            }
        }
    }

    public void CaptureImages()
    {
        if (GridSubsetCapture)
        {
            //CaptureSubsetImages_AllFields();
            StartCoroutine(CaptureSubsetImages_AllGrowthStages());
        }
        else
        {
            //CaptureAllImages_AllFields();
            StartCoroutine(CaptureAllImages_AllGrowthStages());
        }
    }

    public IEnumerator CaptureSubsetImages_AllGrowthStages()
    {
        Debug.Log("Capture Subset Images at all Growth Stages");
        for (int i = 1; i <= fieldGenerator_ref.field_monitoring_iterations; i++)
        {
            Debug.Log($"Capturing Images at Growth Stage {i}/{fieldGenerator_ref.field_monitoring_iterations}");
            fieldGenerator_ref.target_growth_stage = i;
            fieldGenerator_ref.ShowFieldAtTargetMonitoringStage();

            DatasetCapture.StartNewSequence();
            StartCoroutine(fieldGenerator_ref.drone_ref.GetComponent<DroneBehaviour>().CaptureGridSubset(
                                minWidthGridSubset, maxWidthGridSubset,
                                minHeightGridSubset, maxHeightGridSubset));

            yield return new WaitUntil(fieldGenerator_ref.drone_ref.GetComponent<DroneBehaviour>().ReturnCaptureDone);

            fieldGenerator_ref.drone_ref.GetComponent<DroneBehaviour>().CaptureDone = false;
        }
        CaptureDone = true;
    }

    public IEnumerator CaptureAllImages_AllGrowthStages()
    {
        Debug.Log("Capture All Images at all Growth Stages");
        for (int i = 1; i <= fieldGenerator_ref.field_monitoring_iterations; i++)
        {
            Debug.Log($"Capturing Images at Growth Stage {i}/{fieldGenerator_ref.field_monitoring_iterations}");
            fieldGenerator_ref.target_growth_stage = i;
            fieldGenerator_ref.ShowFieldAtTargetMonitoringStage();

            DatasetCapture.StartNewSequence();
            StartCoroutine(fieldGenerator_ref.drone_ref.GetComponent<DroneBehaviour>().AllImageCapture());

            yield return new WaitUntil(fieldGenerator_ref.drone_ref.GetComponent<DroneBehaviour>().ReturnCaptureDone);

            fieldGenerator_ref.drone_ref.GetComponent<DroneBehaviour>().CaptureDone = false;
        }
        CaptureDone = true;
    }
}
