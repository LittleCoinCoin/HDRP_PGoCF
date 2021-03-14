﻿using System;
using UnityEngine;
using UnityEngine.Perception.Randomization.Scenarios;

/// <summary>
/// Custom constants to capture a number of fields generated by the 
/// Field Generator method.
/// </summary>
[Serializable]
public class CustomConstants : ScenarioConstants
{
    public Field ref_Field;
    public MetaImageCapture ref_MetaImageCature;

    [Min(1)] public int NumberOfFields = 1;
}