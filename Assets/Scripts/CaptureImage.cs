using UnityEngine;
using System;
using System.IO;

// Screen Recorder will save individual images of active scene in any resolution and of a specific image format
// including raw, jpg, png, and ppm.  Raw and PPM are the fastest image formats for saving.
//
// You can compile these images into a video using ffmpeg:
// ffmpeg -i screen_3840x2160_%d.ppm -y test.avi
//
//Original code from toddmoore at https://answers.unity.com/questions/22954/how-to-save-a-picture-take-screenshot-from-a-camer.html

public class CaptureImage : MonoBehaviour
{
    // 4k = 3840 x 2160   1080p = 1920 x 1080
    public int captureWidth = 1920;
    public int captureHeight = 1080;

    // optional game object to hide during screenshots (usually your scene canvas hud)
    public GameObject hideGameObject;

    // optimize for many screenshots will not destroy any objects so future screenshots will be fast
    public bool optimizeForManyScreenshots = true;

    // configure with raw, jpg, png, or ppm (simple raw format)
    public enum Format { RAW, JPG, PNG, PPM };
    public Format format = Format.PPM;

    // folder to write output (defaults to data path)
    public string RootFolder;
    private string TimeFolder;

    // private vars for screenshot
    private Rect rect;
    private RenderTexture renderTexture;
    private Texture2D screenShot;

    [HideInInspector] public int counter = 0; // image #

    //Reference to the field generator
    public Field field_generator_ref;

    //Reference to the target plant counter
    public TargetPlantCounting target_plant_counter;

    //bool to save the parameters
    public bool parameter_saved = false;

    private void Start()
    {
        field_generator_ref = FindObjectOfType<Field>();
        target_plant_counter = FindObjectOfType<TargetPlantCounting>();

        SetTimeFolder();
        parameter_saved = false;//always setting it back to false after updating the TimeFolder

    }

    private string folderDate()
    {
        DateTime time = DateTime.Now;
        int year = time.Year;
        int month = time.Month;
        int day = time.Day;
        int hour = time.Hour;
        int min = time.Minute;
        int sec = time.Second;

        return string.Format("/{0}_{1}_{2}_{3}_{4}_{5}", year, month, day, hour, min, sec);
    }

    public void SetTimeFolder()
    {
        TimeFolder = folderDate();
    }

    private bool CheckRootFolder()
    {
        bool _check = false;
        if (RootFolder.Length > 0)
        {
            _check = true;
        }
        return _check;
    }

    private void createDirectories()
    {
        if (!Directory.Exists(RootFolder + TimeFolder + "/Position_Files"))
        {
            System.IO.Directory.CreateDirectory(RootFolder + TimeFolder + "/Position_Files");
        }

        if (!Directory.Exists(RootFolder + TimeFolder + "/labelling"))
        {
            System.IO.Directory.CreateDirectory(RootFolder + TimeFolder + "/labelling");
        }

        if (!Directory.Exists(RootFolder + TimeFolder + "/virtual_reality"))
        {
            System.IO.Directory.CreateDirectory(RootFolder + TimeFolder + "/virtual_reality");
        }
    }

    // create a unique filename using a one-up variable
    private string uniqueFilename(int width, int height )
    {
        // use width, height, and counter for unique file name
        string filename = "";
        int nb_plants = target_plant_counter.CountTargetPlants(string.Format("{0}/screen_{1}x{2}_{3}.csv", RootFolder + TimeFolder + "/Position_Files", width, height, counter));
        if (field_generator_ref.labellingMode)
        {
            filename = string.Format("{0}/screen_{1}x{2}_{3}_{4}.{5}", RootFolder + TimeFolder + "/labelling", width, height, counter, nb_plants, format.ToString().ToLower());
        }

        else
        {
            filename = string.Format("{0}/screen_{1}x{2}_{3}_{4}.{5}", RootFolder + TimeFolder + "/virtual_reality", width, height, counter, nb_plants, format.ToString().ToLower());
        }

        // up counter for next call
        //++counter;

        // return unique filename
        return filename;
    }

    public void TakeScreenshot(string _filename)
    {
        //captureScreenshot = false;

        // hide optional game object if set
        if (hideGameObject != null) hideGameObject.SetActive(false);

        // create screenshot objects if needed
        if (renderTexture == null)
        {
            // creates off-screen render texture that can rendered into
            rect = new Rect(0, 0, captureWidth, captureHeight);
            renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
            screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        }

        // get main camera and manually render scene into rt
        Camera camera = this.GetComponent<Camera>(); // NOTE: added because there was no reference to camera in original script; must add this script to Camera
        camera.targetTexture = renderTexture;
        camera.Render();

        // read pixels will read from the currently active render texture so make our offscreen 
        // render texture active and then read the pixels
        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(rect, 0, 0);

        // reset active camera texture and render texture
        camera.targetTexture = null;
        RenderTexture.active = null;

        // get our unique filename
        //string filename = uniqueFilename((int)rect.width, (int)rect.height);

        // pull in our file header/data bytes for the specified image format (has to be done from main thread)
        byte[] fileHeader = null;
        byte[] fileData = null;

        switch (format)
        {
            case (Format.RAW):
                fileData = screenShot.GetRawTextureData();
                break;
            case (Format.PNG):
                fileData = screenShot.EncodeToPNG();
                break;
            case (Format.JPG):
                fileData = screenShot.EncodeToJPG();
                break;
            case (Format.PPM):
                // create a file header for ppm formatted file
                string headerStr = string.Format("P6\n{0} {1}\n255\n", rect.width, rect.height);
                fileHeader = System.Text.Encoding.ASCII.GetBytes(headerStr);
                fileData = screenShot.GetRawTextureData();
                break;
        }

        // create new thread to save the image to file (only operation that can be done in background)
        new System.Threading.Thread(() =>
        {
            // create file and write optional header with image bytes
            var f = System.IO.File.Create(_filename);
            if (fileHeader != null) f.Write(fileHeader, 0, fileHeader.Length);
            f.Write(fileData, 0, fileData.Length);
            f.Close();
            //Debug.Log(string.Format("Wrote screenshot {0} of size {1}", _filename, fileData.Length));
        }).Start();

        // unhide optional game object if set
        if (hideGameObject != null) hideGameObject.SetActive(true);

        // cleanup if needed
        if (optimizeForManyScreenshots == false)
        {
            Destroy(renderTexture);
            renderTexture = null;
            screenShot = null;
        }
    }

    public void TakePicture()
    {
        if (CheckRootFolder())
        {
            string filename = "";

            createDirectories();

            if (!parameter_saved)
            {
                //field_generator_ref.SaveParameters(RootFolder + TimeFolder);
                parameter_saved = true;
            }
            
            filename = uniqueFilename(captureWidth, captureHeight);
            TakeScreenshot(filename);

            //field_generator_ref.labellingMode = !field_generator_ref.labellingMode;//changing the labelling mode
            //field_generator_ref.Render();

            //filename = uniqueFilename(captureWidth, captureHeight);
            //TakeScreenshot(filename);

            //field_generator_ref.labellingMode = !field_generator_ref.labellingMode;//back to initial labelling mode
            //field_generator_ref.Render();

            counter++;//increasing counter after both photo modes have been captured.

        }
        else
        {
            Debug.LogError($"The root folder is apparently an empty string");
        }
    }

    void LateUpdate()
    {

        if (Input.GetKeyDown(KeyCode.G))
        {
            TakePicture();
        }
    }
}
