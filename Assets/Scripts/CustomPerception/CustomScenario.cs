using UnityEngine;
using UnityEngine.Perception.Randomization.Scenarios;

/// <summary>
/// This Scenario handles the capture of the images of the number of fields
/// defined in the Scenario Constants. For each field, we capture every 
/// monitoring stages defined by the Field Generator.
/// </summary>
public class CustomScenario : PerceptionScenario<CustomConstants>
{
    private int current_field_index;

    /// <summary>
    /// In MetaImageCapture, CaptureOrder is true is the user has pressed the 
    /// I key of the keyboard.
    /// </summary>
    protected override bool isScenarioReadyToStart
    {
        get
        {
            return constants.ref_MetaImageCature.CaptureOrder;
        }
    }

    /// <summary>
    /// On start of the scenario, we initialize the counter of the field that
    /// have been captured.
    /// </summary>
    protected override void OnStart()
    {
        current_field_index = 0;
    }

    /// <summary>
    /// In this scenario we defined 1 Iteration = 1 Field. So, on the start
    /// of the iteration, we call for the generation of a new field before 
    /// calling the method to capture the images.
    /// </summary>
    protected override void OnIterationStart()
    {
        Debug.Log($"Capturing images of field {current_field_index+1}/{constants.NumberOfFields}");
        constants.ref_Field.Generator();
        constants.ref_MetaImageCature.CaptureImages();
    }

    /// <summary>
    /// The iteration is complete if the images of all the growth monitoring
    /// stages have been captured.
    /// </summary>
    protected override bool isIterationComplete
    {
        get
        {
            return constants.ref_MetaImageCature.CaptureDone;
        }
    }

    /// <summary>
    /// On the end of an iteration, we reset the boolean indicating that the
    /// iteration has finished and we increment the counter of the fields captured
    /// </summary>
    protected override void OnIterationEnd()
    {
        constants.ref_MetaImageCature.CaptureDone = false;
        current_field_index += 1;
    }

    /// <summary>
    /// The scenrario takes end when the counter of field captured exceeds the number
    /// of field set in the Constants.
    /// </summary>
    protected override bool isScenarioComplete
    {
        get
        {
            return current_field_index >= constants.NumberOfFields;
        }
    }
}
    
