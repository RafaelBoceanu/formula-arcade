using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerCarController : MonoBehaviour
{
    public float driftMultiplier = 0.95f;
    public float accelerationMultiplier = 30.0f;
    public float steeringMultiplier = 3.5f;
    public float maxSpeed = 20;

    public SpriteRenderer carSprite;
    public SpriteRenderer shadowSprite;

    public AnimationCurve jumpCurve;
    public ParticleSystem landingParticles;

    float acceleration = 0;
    float steering = 0;

    float rotationAngle = 0;

    float velocityVsUp = 0;

    public bool isJumping = false;

    Rigidbody2D carRigidbody;
    PolygonCollider2D carCollider;
    CarSoundsHandler carSoundsHandler;

    // Start is called before the first frame update
    void Start()
    {
        carRigidbody = GetComponent<Rigidbody2D>();
        carCollider = GetComponentInChildren<PolygonCollider2D>();
        carSoundsHandler = GetComponent<CarSoundsHandler>();
        rotationAngle = transform.rotation.eulerAngles.z;
    }

    void FixedUpdate()
    {
        if (GameManager.instance.GetGameState() == GameStates.countdown)
            return;

        EngineInput();

        DisableOrthogonalVelocity();

        SteeringInput();
    }

    void EngineInput()
    {
        if (isJumping && acceleration < 0)
            acceleration = 0;

        // Calculate how much speed we get in terms of forward velocity
        velocityVsUp = Vector2.Dot(transform.up, carRigidbody.velocity);

        // Limit to maximum speed when moving forward
        if (velocityVsUp > maxSpeed && acceleration > 0)
            return;

        // Limit to 50% of maxmimum speed when reversing
        if (velocityVsUp < -maxSpeed * 0.5f && acceleration < 0)
            return;

        // Limit the speed in any direction while accelerating
        if (carRigidbody.velocity.sqrMagnitude > maxSpeed * maxSpeed && acceleration > 0 && !isJumping)
            return;

        // Add drag if there is no acceleration
        if (acceleration == 0)
            carRigidbody.drag = Mathf.Lerp(carRigidbody.drag, 3.0f, Time.fixedDeltaTime * 3);
        else
            carRigidbody.drag = 0;

        // Create the power of the engine
        Vector2 engineInputVector = transform.up * acceleration * accelerationMultiplier;

        // Add the power to the car
        carRigidbody.AddForce(engineInputVector, ForceMode2D.Force);
    }

    void SteeringInput()
    {
        // Minimum speed that allows for steering
        float minSpeedForSteering = carRigidbody.velocity.magnitude / 8;

        // Change the angle based on input
        rotationAngle -= steering * steeringMultiplier * minSpeedForSteering;

        // Add steering to the car
        carRigidbody.MoveRotation(rotationAngle);
    }

    void DisableOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidbody.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(carRigidbody.velocity, transform.right);

        carRigidbody.velocity = forwardVelocity + rightVelocity * driftMultiplier;
    }    

    float GetLateralVelocity()
    {
        if (carRigidbody == null)
            return 0;

        return Vector2.Dot(transform.right, carRigidbody.velocity);
    }

    public bool IsTireSkidding(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = GetLateralVelocity();
        isBraking = false;

        if (isJumping)
            return false;

        if (acceleration < 0 && velocityVsUp > 0)
        {
            isBraking = true;
            return true;
        }

        if (Mathf.Abs(GetLateralVelocity()) > 4.0f)
            return true;

        return false;
    }

    public void SetInputVector(Vector2 inputVector)
    {
        steering = inputVector.x;
        acceleration = inputVector.y;
    }

    public float GetVelocityMagnitude()
    {
        if (carRigidbody == null)
            return 0;

        return carRigidbody.velocity.magnitude;
    }    

    public void Jump(float jumpHeight, float jumpPush)
    {
        if (!isJumping)
            StartCoroutine(JumpCo(jumpHeight, jumpPush));
    }

    private IEnumerator JumpCo(float jumpHeight, float jumpPush)
    {
        isJumping = true;

        float jumpStartTime = Time.time;
        float jumpDuration = carRigidbody.velocity.magnitude * 0.05f;

        jumpHeight = jumpHeight * carRigidbody.velocity.magnitude * 0.05f;
        jumpHeight = Mathf.Clamp(jumpHeight, 0.0f, 1.0f);

        carCollider.enabled = false;

        carSoundsHandler.PlayJumpSound();

        carSprite.sortingLayerName = "Flying";
        shadowSprite.sortingLayerName = "Flying";

        carRigidbody.AddForce(10 * jumpPush * carRigidbody.velocity.normalized, ForceMode2D.Impulse);

        while (isJumping)
        {
            float jumpPercentage = (Time.time - jumpStartTime) / jumpDuration;
            jumpPercentage = Mathf.Clamp01(jumpPercentage);

            carSprite.transform.localScale = Vector3.one + Vector3.one * jumpCurve.Evaluate(jumpPercentage) * jumpHeight;

            shadowSprite.transform.localScale = carSprite.transform.localScale * 0.75f;

            shadowSprite.transform.localPosition = new Vector3(1, -1, 0.0f) * 3 * jumpCurve.Evaluate(jumpPercentage) * jumpHeight;

            if (jumpPercentage == 1.0f)
                break;

            yield return null;
        }

        if (Physics2D.OverlapCircle(transform.position, 1.0f))
        {
            isJumping = false;

            Jump(0.2f, 0.6f);
        }
        else
        {
            carSprite.transform.localScale = Vector3.one;

            shadowSprite.transform.localPosition = Vector3.zero;
            shadowSprite.transform.localScale = carSprite.transform.localScale;

            carCollider.enabled = true;

            carSprite.sortingLayerName = "Default";
            shadowSprite.sortingLayerName = "Default";

            if (jumpHeight > 0.2f)
            {
                landingParticles.Play();

                carSoundsHandler.PlayLandSound();
            }

            isJumping = false;
        }
    }

    //Add Acceleration COROUTINE HERE
    private IEnumerator AccelerationCo()
    {
        accelerationMultiplier = 40;

        yield return new WaitForSeconds(1f);

        accelerationMultiplier = 10;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Jump")
        {
            JumpData jumpData = collision.GetComponent<JumpData>();
            Jump(jumpData.jumpHeight, jumpData.jumpPush);
        }

        if (collision.gameObject.tag == "Accelerate")
        {
            StartCoroutine(AccelerationCo());
        }

        if (collision.gameObject.tag == "Gravel")
        {
            maxSpeed = 1;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Gravel")
        {
            maxSpeed = 10;
        }
    }
}
