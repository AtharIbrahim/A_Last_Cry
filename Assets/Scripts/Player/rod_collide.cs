using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rod_collide : MonoBehaviour
{
   public float rotationAngle = 90f;  // The rotation angle to open the door
    public float rotationSpeed = 2f;   // The speed of door rotation
    private bool isPlayerNearby = false;  // Whether the player is near the door
    private bool isDoorOpen = false;   // Track whether the door is open or not
    private Quaternion closedRotation; // The door's initial (closed) rotation

    public GameObject interactionText;  // Reference to the UI text for interaction prompt
    public GameObject Key;  // Reference to the UI text for interaction prompt
    public GameObject KeyNotText;  // Reference to the UI text for interaction prompt
    private Transform player;           // Reference to the player
    private Vector3 doorPosition;      // The position of the door (pivot point)

    void Start()
    {
        closedRotation = transform.rotation;  // Store the initial door rotation
        doorPosition = transform.position;    // Store the position of the door
        player = GameObject.FindGameObjectWithTag("Player").transform;  // Find the player by tag

        if (interactionText != null)
        {
            interactionText.SetActive(false);  // Hide the interaction text at the start
        }
    }

    void Update()
    {
        bool accept = Input.GetButton("Submit");  // Check for "Submit" button press from Input Manager
        if (isPlayerNearby && accept && Key.activeSelf)  // Check if player is nearby, button is pressed, and key is active
        {
            if (isDoorOpen)
            {
                StartCoroutine(CloseDoor());  // Close the door
            }
            else
            {
                StartCoroutine(OpenDoor());  // Open the door
            }
        }
        else if (isPlayerNearby && accept && !Key.activeSelf)  // If "E" is pressed and key is not active
        {
            KeyNotText.SetActive(true);  // Show the "Key not available" text
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Check if player collides with door
        {
            isPlayerNearby = true;
            if (interactionText != null)
            {
                interactionText.SetActive(true);  // Show interaction text
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))  // Check if player leaves the door area
        {
            isPlayerNearby = false;
            KeyNotText.SetActive(false);
            if (interactionText != null)
            {
                interactionText.SetActive(false);  // Hide interaction text
            }
        }
    }

    // Coroutine to smoothly open the door
    private System.Collections.IEnumerator OpenDoor()
    {
        Vector3 playerDirection = player.position - doorPosition;  // Vector from door to player
        float targetAngle = rotationAngle * (Vector3.Dot(playerDirection, transform.forward) < 0 ? 1 : -1);
        float timeElapsed = 0f;
        Quaternion initialRotation = transform.rotation;

        while (timeElapsed < 1f)
        {
            timeElapsed += Time.deltaTime * rotationSpeed;
            float angle = Mathf.LerpAngle(initialRotation.eulerAngles.y, targetAngle, timeElapsed);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);  // Ensure final rotation is exactly as intended
        isDoorOpen = true;  // Mark the door as open
    }

    // Coroutine to smoothly close the door
    private System.Collections.IEnumerator CloseDoor()
    {
        Vector3 playerDirection = player.position - doorPosition;  // Vector from door to player
        float targetAngle = rotationAngle * (Vector3.Dot(playerDirection, transform.forward) < 0 ? 1 : -1);
        float timeElapsed = 0f;
        Quaternion initialRotation = transform.rotation;

        while (timeElapsed < 1f)
        {
            timeElapsed += Time.deltaTime * rotationSpeed;
            float angle = Mathf.LerpAngle(initialRotation.eulerAngles.y, 0f, timeElapsed);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, 0f);  // Ensure final rotation is exactly as intended
        isDoorOpen = false;  // Mark the door as closed
    }
}
