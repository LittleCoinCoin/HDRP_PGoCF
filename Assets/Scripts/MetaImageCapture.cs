using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaImageCapture : MonoBehaviour
{
    public Field fieldGenerator_ref;

    public Camera capturingCamera_ref;
    public string rootFolder;
    public string metaCapture_FolderName;

    public bool GridSubsetCapture = false;
    public int minWidthGridSubset;
    public int maxWidthGridSubset;
    public int minHeightGridSubset;
    public int maxHeightGridSubset;

    [Min(1)] public int numberOfFields;

    

}
