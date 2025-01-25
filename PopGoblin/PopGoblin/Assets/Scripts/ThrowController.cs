using UnityEngine;
using UnityEngine.UI;

public class ThrowController : MonoBehaviour
{
    [Header("Throw Settings")]
    [Tooltip("Maximum hold time before full power.")]
    public float maxHoldTime = 2f;

    [Tooltip("Maximum force when at 100% power.")]
    public float maxThrowForce = 10f;

    [Tooltip("Angle (in degrees) to throw.")]
    public float throwAngle = 45f;

    [Header("References")]
    [Tooltip("Item to throw (remember Rigidbody).")]
    public Rigidbody objectToThrow;

    [Tooltip("UI Slider for throw bar.")]
    public Slider powerSlider;

    private float currentHoldTime = 0f;
    private bool isHolding = false;

    void Update()
    {
        // holding left mouse button down (change for input controllers)
        if (Input.GetMouseButtonDown(0))
        {
            isHolding = true;
            currentHoldTime = 0f;
        }

        // increment hold time (charge power)
        if (Input.GetMouseButton(0) && isHolding)
        {
            currentHoldTime += Time.deltaTime;
            currentHoldTime = Mathf.Clamp(currentHoldTime, 0f, maxHoldTime);

            // Update the UI Slider
            if (powerSlider)
            {
                float normalizedPower = currentHoldTime / maxHoldTime;
                powerSlider.value = normalizedPower;
            }
        }

        // On release, throw the object
        if (Input.GetMouseButtonUp(0) && isHolding)
        {
            isHolding = false;

            float normalizedPower = currentHoldTime / maxHoldTime;
            ThrowObject(normalizedPower);

            // Reset
            currentHoldTime = 0f;
            if (powerSlider)
                powerSlider.value = 0f;
        }
    }

    // Applies force to the object based on the normalized power.
    // <param name="power">Value between 0 and 1 representing how charged up the throw is.</param>
    private void ThrowObject(float power)
    {
        if (objectToThrow == null) return;

        // Calculate final throw force
        float throwForce = maxThrowForce * power;

        // Convert angle to radians
        float angleInRadians = throwAngle * Mathf.Deg2Rad;

        // Calculate directional vector based on angle
        // Assuming 'forward' is the horizontal axis and 'up' is vertical:
        Vector3 forceDirection = new Vector3(
            Mathf.Cos(angleInRadians),
            Mathf.Sin(angleInRadians),
            0f
        );

        // diff orientation i.e z use:
        // Vector3 forceDirection = new Vector3(0f, Mathf.Sin(angleInRadians), Mathf.Cos(angleInRadians));

        // Make sure object is free to move before yeeting
        objectToThrow.isKinematic = false;

        // Apply impulse force so it yeets in one go
        objectToThrow.AddForce(forceDirection * throwForce, ForceMode.Impulse);

         DestroyAfterTime destroyScript = objectToThrow.GetComponent<DestroyAfterTime>();
        if (destroyScript != null)
        {
            // Start the destruction timer only after we've thrown it
            destroyScript.BeginDestructionTimer();
        }
    }
}
