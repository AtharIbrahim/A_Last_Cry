using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    float velocityZ = 0.0f;
    float velocityX = 0.0f;
    public float acceleration = 2.0f;
    public float deacceleration = 2.0f;
    public float maxWalkVelocity = 0.5f;
    public float maxRunVelocity = 2.0f;
    public float moveSpeed = 2.0f; // Speed multiplier for movement

    int VelocityXHash;
    int VelocityZHash;

    public CinemachineVirtualCamera virtualCamera;
    private CinemachinePOV povComponent;

    // Jump System
    public LayerMask groundLayer; // Layer that represents the ground
    public Transform groundCheck; // Position to check if the player is grounded
    private Rigidbody rb;
    private bool isGrounded;
    public float jumpForce = 5.0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        VelocityXHash = Animator.StringToHash("Velocity X");
        VelocityZHash = Animator.StringToHash("Velocity Z");

        if (virtualCamera != null)
        {
            povComponent = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        }
    }

    void changeVelocity(float moveX, float moveZ, bool runPressed, float currentMaxVelocity)
    {
        // Check if movement is significant
        bool isMoving = (moveZ != 0 || moveX != 0);

        if (moveZ > 0 && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }
        if (moveZ < 0 && velocityZ > -currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * acceleration;
        }
        if (moveX < 0 && velocityX > -currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
        }
        if (moveX > 0 && velocityX < currentMaxVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }

        // decrease velocity forward
        if (moveZ == 0 && velocityZ > 0.0f)
        {
            velocityZ -= Time.deltaTime * deacceleration;
        }
        if (moveZ == 0 && velocityZ < 0.0f)
        {
            velocityZ += Time.deltaTime * deacceleration;
        }
        // decrease velocity left
        if (moveX == 0 && velocityX < 0.0f)
        {
            velocityX += Time.deltaTime * deacceleration;
        }
        // decrease velocity right
        if (moveX == 0 && velocityX > 0.0f)
        {
            velocityX -= Time.deltaTime * deacceleration;
        }
    }

    void lockOrResetVelocity(float moveX, float moveZ, bool runPressed, float currentMaxVelocity)
    {
        if (moveZ == 0 && velocityZ != 0.0f && (velocityZ > -0.05f && velocityZ < 0.05f))
        {
            velocityZ = 0.0f;
        }

        if (moveX == 0 && velocityX != 0.0f && (velocityX > -0.05f && velocityX < 0.05f))
        {
            velocityX = 0.0f;
        }

        // decelerate for forward
        if (moveZ > 0 && runPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }
        else if (moveZ > 0 && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deacceleration;
            if (velocityZ > currentMaxVelocity && velocityZ < (currentMaxVelocity + 0.05f))
            {
                velocityZ = currentMaxVelocity;
            }
        }
        else if (moveZ > 0 && velocityZ < currentMaxVelocity && velocityZ > (currentMaxVelocity - 0.05f))
        {
            velocityZ = currentMaxVelocity;
        }

        // decelerate for backward
        if (moveZ < 0 && runPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ = -currentMaxVelocity;
        }
        else if (moveZ < 0 && velocityZ < -currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * deacceleration;
            if (velocityZ < -currentMaxVelocity && velocityZ > (-currentMaxVelocity - 0.05f))
            {
                velocityZ = -currentMaxVelocity;
            }
        }
        else if (moveZ < 0 && velocityZ > -currentMaxVelocity && velocityZ < (-currentMaxVelocity + 0.05f))
        {
            velocityZ = -currentMaxVelocity;
        }

        // decelerate for left
        if (moveX < 0 && runPressed && velocityX < -currentMaxVelocity)
        {
            velocityX = -currentMaxVelocity;
        }
        else if (moveX < 0 && velocityX < -currentMaxVelocity)
        {
            velocityX += Time.deltaTime * deacceleration;
            if (velocityX < -currentMaxVelocity && velocityX > (-currentMaxVelocity - 0.05f))
            {
                velocityX = -currentMaxVelocity;
            }
        }
        else if (moveX < 0 && velocityX > -currentMaxVelocity && velocityX < (-currentMaxVelocity + 0.05f))
        {
            velocityX = -currentMaxVelocity;
        }

        // decelerate for right
        if (moveX > 0 && runPressed && velocityX > currentMaxVelocity)
        {
            velocityX = currentMaxVelocity;
        }
        else if (moveX > 0 && velocityX > currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * deacceleration;
            if (velocityX > currentMaxVelocity && velocityX < (currentMaxVelocity + 0.05f))
            {
                velocityX = currentMaxVelocity;
            }
        }
        else if (moveX > 0 && velocityX < currentMaxVelocity && velocityX > (currentMaxVelocity - 0.05f))
        {
            velocityX = currentMaxVelocity;
        }
    }

    void moveCharacter()
    {
        Vector3 forwardMovement = transform.forward * velocityZ * Time.deltaTime * moveSpeed;
        Vector3 rightMovement = transform.right * velocityX * Time.deltaTime * moveSpeed;
        Vector3 movement = forwardMovement + rightMovement;

        transform.Translate(movement, Space.World);
    }

    void rotateCharacter()
    {
        if (povComponent != null)
        {
            float yaw = povComponent.m_HorizontalAxis.Value;
            transform.rotation = Quaternion.Euler(0, yaw, 0);
        }
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        bool rtPressed = Input.GetAxis("Fire3") > 0.1f; // Adjust the threshold as needed
        bool runPressed = Input.GetKey(KeyCode.LeftShift) || rtPressed;

        float currentMaxVelocity = runPressed ? maxRunVelocity : maxWalkVelocity;

        changeVelocity(moveX, moveZ, runPressed, currentMaxVelocity);
        lockOrResetVelocity(moveX, moveZ, runPressed, currentMaxVelocity);

        moveCharacter();
        rotateCharacter();

        animator.SetFloat(VelocityZHash, velocityZ);
        animator.SetFloat(VelocityXHash, velocityX);

        isGrounded = Physics.CheckSphere(groundCheck.position, 0.0f, groundLayer);
        // Debug.Log("Is Grounded: " + isGrounded);

        // UpdateCameraShake();
    }
    // void UpdateCameraShake()
    // {
    //     // Get the player's speed
    //     float playerSpeed = new Vector3(velocityX, 0, velocityZ).magnitude;

    //     // Get the Perlin noise component from the virtual camera
    //     var perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

    //     if (perlin != null)
    //     {
    //         // Dynamically adjust the noise based on player's speed
    //         // Increase amplitude and frequency as player moves faster
    //         perlin.m_AmplitudeGain = Mathf.Lerp(0.1f, 0.5f, playerSpeed / maxRunVelocity);
    //         perlin.m_FrequencyGain = Mathf.Lerp(0.1f, 1.5f, playerSpeed / maxRunVelocity);
    //     }
    // }


}

// XboxLStickY
