using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody carRigidbody; // Reference to the Rigidbody component of the car
    public Transform exitPoint; // Point for the player to exit the car (to the left of the car)
    public float maxSpeed = 30f; // Maximum speed of the car
    public float accelerationFactor = 0.25f; // Factor controlling acceleration speed
    public float braking = 30f; // Braking rate of the car
    public float maxTurnAngle = 25f; // Maximum angle the car can turn
    public bool isPlayerInsideCar = false; // Flag to indicate whether the player is inside the car

    private float currentSpeed = 0f; // Current speed of the car
    private float horizontalInput; // Input for steering
    private float verticalInput; // Input for acceleration

    private const float groundFriction = 2f; // Friction when car is on the ground
    private const float airFriction = 0.5f; // Friction when car is airborne
    private const float maxGroundDist = 0.2f; // Maximum distance to consider the car on the ground
    private const float maxAngularVelocity = 2f; // Maximum angular velocity of the car
    private const float angularDrag = 1f; // Angular drag to simulate resistance to angular acceleration
    private const float turnSpeedModifier = 5f; // Modifier for turn speed based on current speed
    public CameraShaker cameraShaker;
    public AudioClip drivingSound;

    void Update()
    {
        if (isPlayerInsideCar)
        {
            // Get input for steering and acceleration
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }
        else
        {
            horizontalInput = 0f;
            verticalInput = 0f;
        }
    }

    void FixedUpdate()
    {
        if (isPlayerInsideCar)
        {
            // Calculate acceleration
            if (verticalInput > 0)
            {
                // Drive forward
                currentSpeed += maxSpeed * accelerationFactor * Mathf.Pow(1 - currentSpeed / maxSpeed, 2) * Time.fixedDeltaTime;
            }
            else if (verticalInput < 0)
            {
                // Drive backward (reverse)
                currentSpeed -= maxSpeed * accelerationFactor * Mathf.Pow(1 - Mathf.Abs(currentSpeed) / maxSpeed, 2) * Time.fixedDeltaTime;
                // Invert the horizontal input when reversing
                horizontalInput *= -1f;
            }
            else
            {
                // Apply braking force or rolling resistance
                float brakeForce = braking * Mathf.Clamp01(Mathf.Abs(currentSpeed) / maxSpeed);
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, brakeForce * Time.fixedDeltaTime);
            }

            // Clamp the speed within the maximum speed limits
            currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

            // Calculate movement direction based on input
            Vector3 movement = transform.forward * currentSpeed * Time.fixedDeltaTime;

            // Apply movement force to the car Rigidbody
            carRigidbody.MovePosition(carRigidbody.position + movement);

            // Calculate rotation based on input and speed
            float turnAngle = horizontalInput * maxTurnAngle * Mathf.Clamp01(Mathf.Abs(currentSpeed) / maxSpeed) * turnSpeedModifier;

            // Apply rotation to the car Rigidbody
            carRigidbody.MoveRotation(carRigidbody.rotation * Quaternion.Euler(0f, turnAngle * Time.fixedDeltaTime, 0f));

            // Apply friction
            ApplyFriction();
        }
    }


    void ApplyFriction()
    {
        // Check if the car is on the ground
        bool isGrounded = Physics.Raycast(transform.position, -transform.up, maxGroundDist);

        // Apply appropriate friction
        float friction = isGrounded ? groundFriction : airFriction;

        // Apply friction force
        Vector3 frictionForce = -carRigidbody.velocity.normalized * friction * Time.fixedDeltaTime;
        carRigidbody.AddForce(frictionForce, ForceMode.VelocityChange);

        // Apply angular drag
        carRigidbody.angularDrag = angularDrag;

        // Limit maximum angular velocity
        carRigidbody.angularVelocity = Vector3.ClampMagnitude(carRigidbody.angularVelocity, maxAngularVelocity);

        // If the car is nearly stationary, apply additional rolling resistance to slow down gradually
        if (Mathf.Approximately(currentSpeed, 0f))
        {
            float rollingResistance = groundFriction * 2.0f; // Adjust this value as needed for faster deceleration
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, rollingResistance * Time.fixedDeltaTime);
        }
    }


    public void SetPlayerInsideCar(bool value)
    {
        isPlayerInsideCar = value;
        if (value == true)
        {
            // Reset car's momentum
            currentSpeed = 0f;
            carRigidbody.velocity = Vector3.zero;
            carRigidbody.angularVelocity = Vector3.zero;
        }
    }

    public void ExitCar()
    {
        // Set the player outside the car
        isPlayerInsideCar = false;
        // Move the player to the exit point (to the left of the car)
        transform.position = exitPoint.position;
    }
}