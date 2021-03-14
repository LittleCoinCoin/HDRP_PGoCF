using System.Collections;
using UnityEngine;
using UnityEngine.Perception.GroundTruth;

public class MetaImageCapture : MonoBehaviour
{
    public Field fieldGenerator_ref;

    public Camera capturingCamera_ref;

    //These parameters are no longer needed since we use Unity Perception.
    //To be removed.
    //public string rootFolder;
    //public string metaCapture_FolderName;

    public bool GridSubsetCapture = false;
    public int minWidthGridSubset;
    public int maxWidthGridSubset;
    public int minHeightGridSubset;
    public int maxHeightGridSubset;

    //[Min(1)] public int numberOfFields;

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

    //These methods are no longer needed after upgrade to Unity Perrception.
    //TO BE REMOVED

    //public void CaptureSubsetImages_AllFields()
    //{
    //    for (int i = 0; i < numberOfFields; i++)
    //    {
    //        fieldGenerator_ref.drone_ref.GetComponent<DroneBehaviour>().CaptureGridSubset(
    //            minWidthGridSubset, maxWidthGridSubset,
    //            minHeightGridSubset, maxHeightGridSubset);
    //        fieldGenerator_ref.Generator();
    //    }
    //}

    //public void CaptureAllImages_AllFields()
    //{
    //    for (int i = 0; i < numberOfFields; i++)
    //    {
    //        DatasetCapture.StartNewSequence();
    //        StartCoroutine(fieldGenerator_ref.drone_ref.GetComponent<DroneBehaviour>().AllImageCapture());
    //        fieldGenerator_ref.Generator();
    //        EditorApplication.isPlaying = true;
            
    //    }
    //}

    public IEnumerator CaptureSubsetImages_AllGrowthStages()
    {
        for (int i = 1; i <= fieldGenerator_ref.field_monitoring_iterations; i++)
        {
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
        for (int i = 1; i <= fieldGenerator_ref.field_monitoring_iterations; i++)
        {
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
