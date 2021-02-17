using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Perception.GroundTruth;

public class DroneBehaviour : MonoBehaviour
{

    //reference to the field generator to get the caracteristics of the field when drone is in auto pilot
    public Field field_generator_ref;

    //Drone flying behaviour
    public enum FlyingMode { ManualFree, ManualGrid, ImageCapture};
    public FlyingMode flyingMode;

    //Drone flying parameters
    public float flying_speed = 7f;
    public float accelerated_flying_speed = 14f;
    private float current_flying_speed;

    //Smooth damp parameters
    //public float _smoothTime = 0;
    private Vector3 velocity = Vector3.zero;

    //automatic flight parameters
    public float x_overlapping;
    public float y_overlapping;

    public TargetPlantCounting plantCounter;

    // Start is called before the first frame update
    void Start()
    {
        field_generator_ref = FindObjectOfType<Field>();
    }

    // Update is called once per frame
    void Update()
    {
        if (flyingMode == FlyingMode.ManualFree)
        {
            Fly_Movement();
        }

        if (flyingMode == FlyingMode.ManualGrid)
        {
            GridMovementRules();
        }

        if (flyingMode == FlyingMode.ImageCapture)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                StartCoroutine(AllImageCapture());
                //GetComponentInChildren<PerceptionCamera>().RequestCapture();
            }
        }
    }

    /// <summary>
    /// Place the drone at the starting position for the image acquisition (bottom left corner of the field)
    /// </summary>
    public void StartPosition()
    {

        float[] visionBoundaries = GetComponentInChildren<CameraVision>().VisionBoundariesOnField();

        Vector3 visionBoundaries_offset = new Vector3((visionBoundaries[2] - visionBoundaries[3])/2,
                                                      0,
                                                      (visionBoundaries[0] - visionBoundaries[1])/2);

        transform.position = new Vector3(-field_generator_ref.field_size /2,
                                         transform.position.y,
                                         -field_generator_ref.field_size /2);

        transform.position += visionBoundaries_offset;
    }

    /// <summary>
    /// Moves the drone on a grid defined by the camera vision and the proportion of overlapping
    /// between the images captured from two adjacent node of the grid.
    /// </summary>
    /// <param name="_horizontal">Can be equal to -1 (moving left), 0 (no movement) or 1 (moving right).</param>
    /// <param name="_vertical">Can be equal to -1 (moving down), 0 (no movement) or 1 (moving up).</param>
    public void OnGridMovement(int _horizontal, int _vertical)
    {
        
        float[] visionBoundaries = GetComponentInChildren<CameraVision>().VisionBoundariesOnField();

        Vector3 visionBoundaries_lengths = new Vector3((visionBoundaries[2] - visionBoundaries[3]),
                                                      0,
                                                      (visionBoundaries[0] - visionBoundaries[1]));

        Vector3 directionVector = new Vector3(_horizontal * (1-x_overlapping),
                                                0,
                                                _vertical * (1-y_overlapping));

        Vector3 movementVector = Vector3.Scale(directionVector, visionBoundaries_lengths);

        //Debug.Log(visionBoundaries_lengths);
        //Debug.Log(directionVector);
        //Debug.Log(movementVector);

        transform.position += movementVector;
    }

    /// <summary>
    /// moving the drone on a grid according to the arrows of the keyboard
    /// </summary>
    private void GridMovementRules()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnGridMovement(0, 1);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnGridMovement(0, -1);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnGridMovement(1, 0);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnGridMovement(-1, 0);
        }
    }

    /// <summary>
    /// Only capture images on a subpart of the grid on which the drone can move
    /// </summary>
    /// <param name="_minWidth">Starting coordinate along the width of the field of the capture</param>
    /// <param name="_maxWidth">Ending coordinate along the width of the field of the capture</param>
    /// <param name="_minHeight">Starting coordinate along the height of the field of the capture</param>
    /// <param name="_maxHeight">Ending coordinate along the height of the field of the capture</param>
    public void CaptureGridSubset(int _minWidth, int _maxWidth, int _minHeight, int _maxHeight)
    {
        //Position initialized at the bottom left corner of the field
        StartPosition();

        //Moves to the first position where the image capture should begin
        for (int i = 0; i < _minWidth; i++)
        {
            OnGridMovement(1, 0);
        }
        for (int i = 0; i < _minHeight; i++)
        {
            OnGridMovement(0, 1);
        }
        
        //actually do the image capture
        bool forward = true;
        int y_direction = 0;
        gameObject.GetComponentInChildren<CaptureImage>().counter = 0;
        gameObject.GetComponentInChildren<CaptureImage>().SetTimeFolder();
        for (int i = 0; i < _maxWidth-_minWidth; i++)
        {
            if (i > 0)
            {
                OnGridMovement(1, 0);
            }
            gameObject.GetComponentInChildren<CaptureImage>().TakePicture();

            y_direction = forward ? 1 : -1;

            for (int j = 0; j < _maxHeight-_minHeight-1; j++)
            {
                OnGridMovement(0, y_direction);
                gameObject.GetComponentInChildren<CaptureImage>().TakePicture();
            }
            forward = !forward;
        }

        gameObject.GetComponentInChildren<CaptureImage>().parameter_saved = false;
    }

    /// <summary>
    /// Take all possible images at the coordinates the grid that covers the field
    /// </summary>
    public IEnumerator AllImageCapture()
    {
        
        StartPosition();

        float[] visionBoundaries = GetComponentInChildren<CameraVision>().VisionBoundariesOnField();

        float frustrum_height = visionBoundaries[0] - visionBoundaries[1];
        float frustrum_width = visionBoundaries[2] - visionBoundaries[3];

        float field_height = field_generator_ref.height * field_generator_ref.field_size;
        float field_width = field_generator_ref.height * field_generator_ref.field_size;

        int y_steps = Mathf.RoundToInt((field_generator_ref.height * field_generator_ref.field_size - frustrum_height) /
                                        (frustrum_height * (1-y_overlapping)));
        
        int x_steps = Mathf.RoundToInt((field_generator_ref.width * field_generator_ref.field_size - frustrum_width) /
                                        (frustrum_width * (1-x_overlapping)));
        

        //Debug.Log($"y_step is : {y_steps} ({field_generator_ref.height * field_generator_ref.field_size - frustrum_height} / {(frustrum_height) * (1 - y_overlapping)})");
        //Debug.Log($"x_step is : {x_steps} ({field_generator_ref.width * field_generator_ref.field_size - frustrum_width} / {(frustrum_width) * (1 - x_overlapping)})");
        bool forward = true;
        int y_direction = 0;
        gameObject.GetComponentInChildren<CaptureImage>().counter = 0;
        gameObject.GetComponentInChildren<CaptureImage>().SetTimeFolder();
        for (int i = 0; i <= x_steps; i++)
        {
            if (i > 0)
            {
                OnGridMovement(1, 0);
            }
            yield return new WaitForEndOfFrame();
            //gameObject.GetComponentInChildren<CaptureImage>().TakePicture();
            plantCounter.With_Perception();
            yield return new WaitForEndOfFrame();


            y_direction = forward ? 1 : -1;

            for (int j = 0; j < y_steps; j++)
            {
                OnGridMovement(0, y_direction);
                //gameObject.GetComponentInChildren<CaptureImage>().TakePicture();
                yield return new WaitForEndOfFrame();
                plantCounter.With_Perception();
                yield return new WaitForEndOfFrame();
            }

            forward = !forward;
        }

        //gameObject.GetComponentInChildren<CaptureImage>().parameter_saved = false;
    }

    /// <summary>
    /// Manages the movements of the camera. The camera moves on the X-Z plane.
    /// </summary>
    private void Fly_Movement()
    {
        //Get keyboard input for movement
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");

        //Calculate the movement
        Vector3 mov = Vector3.zero;
        mov += transform.right * xMov;
        mov += transform.forward * zMov;

        if (Input.GetKey(KeyCode.W))
        {
            mov += Vector3.down;
        }

        if (Input.GetKey(KeyCode.X))
        {
            mov += Vector3.up;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            current_flying_speed = accelerated_flying_speed;
        }

        else
        {
            current_flying_speed = flying_speed;
        }

        transform.position += mov * current_flying_speed * Time.deltaTime;
    }
}
