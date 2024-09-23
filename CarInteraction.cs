using UnityEngine;

public class CarInteraction : MonoBehaviour
{
    public GameObject car; // Reference to the car GameObject
    public KeyCode enterExitKey = KeyCode.E; // Key to enter/exit the car
    public KeyCode switchCameraKey = KeyCode.V; // Key to switch between cameras
    public KeyCode rearViewCameraKey = KeyCode.C; // Key to switch to the rear view camera
    public AudioClip carStartSound; // Sound effect for starting the car
    public AudioClip hornSound; // Sound effect for car horn

    public Camera playerCamera; // Reference to the player's camera
    public Camera thirdPersonCarCamera; // Reference to the third-person car camera
    public Camera firstPersonCarCamera; // Reference to the first-person car camera
    public Camera rearViewCamera; // Reference to the rear view camera

    private bool isPlayerInsideCar = false; // Flag to track whether the player is inside the car
    private CarController carController;
    private FirstPersonController playerController; // Reference to the player's controller
    private AudioSource audioSource; // Reference to the AudioSource component

    void Start()
    {
        carController = car.GetComponent<CarController>();
        playerController = GetComponent<FirstPersonController>();
        audioSource = GetComponent<AudioSource>(); // Get AudioSource component from the player GameObject

        // Disable car cameras by default
        if (thirdPersonCarCamera != null)
            thirdPersonCarCamera.gameObject.SetActive(false);
        if (firstPersonCarCamera != null)
            firstPersonCarCamera.gameObject.SetActive(false);
        if (rearViewCamera != null)
            rearViewCamera.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(enterExitKey))
        {
            if (!isPlayerInsideCar)
            {
                // Check if the player is close enough to the car
                float distance = Vector3.Distance(transform.position, car.transform.position);
                if (distance < 3f)
                {
                    EnterCar();
                }
            }
            else
            {
                ExitCar();
            }
        }

        // Switch camera when the player is inside the car and presses the switchCameraKey
        if (isPlayerInsideCar && Input.GetKeyDown(switchCameraKey))
        {
            SwitchCamera();
        }

        // Toggle rear view camera when the player is inside the car and presses/releases the rearViewCameraKey
        if (isPlayerInsideCar)
        {
            if (Input.GetKeyDown(rearViewCameraKey))
            {
                ToggleRearViewCamera(true); // Enable rear view camera
            }
            else if (Input.GetKeyUp(rearViewCameraKey))
            {
                ToggleRearViewCamera(false); // Disable rear view camera
            }
        }

        // Play horn sound when the player is inside the car and presses the horn key
        if (isPlayerInsideCar && Input.GetKeyDown(KeyCode.H))
        {
            if (hornSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(hornSound);
            }
        }
    }

    void EnterCar()
    {
        // Disable player's collider and Rigidbody
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;

        // Disable player's controller
        playerController.enabled = false;

        // Play car start sound effect
        if (carStartSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(carStartSound);
        }

        // Play driving sound effect and set it to loop
        if (carController.drivingSound != null && audioSource != null)
        {
            audioSource.clip = carController.drivingSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        // Parent the player to the car
        transform.SetParent(car.transform);

        // Set the player's position and rotation relative to the car
        transform.localRotation = Quaternion.identity;

        // Enable third-person car camera by default
        if (thirdPersonCarCamera != null)
            thirdPersonCarCamera.gameObject.SetActive(true);

        // Disable first-person car camera
        if (firstPersonCarCamera != null)
            firstPersonCarCamera.gameObject.SetActive(false);

        // Disable rear view camera
        if (rearViewCamera != null)
            rearViewCamera.gameObject.SetActive(false);

        // Disable player camera
        if (playerCamera != null)
            playerCamera.gameObject.SetActive(false);

        // Update CarController to indicate the player is inside the car
        carController.SetPlayerInsideCar(true);

        isPlayerInsideCar = true;
    }

    void ExitCar()
    {
        // Enable player's collider
        Collider playerCollider = GetComponent<Collider>();
        if(playerCollider != null)
            playerCollider.enabled = true;
            
        // Enable player's Rigidbody
        Rigidbody playerRigidbody = GetComponent<Rigidbody>();
        if(playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false;
        }

        // Enable player's controller
        playerController.enabled = true;

        // Stop and reset the audio source
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.loop = false;
        }

        // Unparent the player from the car
        transform.SetParent(null);

        // Calculate the exit position to the left of the car
        Vector3

 exitPosition = car.transform.position - car.transform.right * 2f + Vector3.up * 1.5f; // Adjust the y-coordinate as needed

        // Set the player's position to the exit position
        transform.position = exitPosition;

        // Enable player camera
        if (playerCamera != null)
            playerCamera.gameObject.SetActive(true);

        // Disable car cameras
        if (thirdPersonCarCamera != null)
            thirdPersonCarCamera.gameObject.SetActive(false);
        if (firstPersonCarCamera != null)
            firstPersonCarCamera.gameObject.SetActive(false);
        if (rearViewCamera != null)
            rearViewCamera.gameObject.SetActive(false);

        // Update CarController to indicate the player is outside the car
        carController.SetPlayerInsideCar(false);

        isPlayerInsideCar = false;
    }

    void SwitchCamera()
    {
        if (isPlayerInsideCar && thirdPersonCarCamera != null && firstPersonCarCamera != null)
        {
            if (thirdPersonCarCamera.gameObject.activeSelf)
            {
                thirdPersonCarCamera.gameObject.SetActive(false);
                firstPersonCarCamera.gameObject.SetActive(true);
            }
            else
            {
                firstPersonCarCamera.gameObject.SetActive(false);
                thirdPersonCarCamera.gameObject.SetActive(true);
            }
        }
    }

    void ToggleRearViewCamera(bool enable)
    {
        if (isPlayerInsideCar && rearViewCamera != null)
        {
            rearViewCamera.gameObject.SetActive(enable);
        }
    }
}