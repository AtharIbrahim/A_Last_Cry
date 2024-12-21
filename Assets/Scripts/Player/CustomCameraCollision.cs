using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCameraCollision : MonoBehaviour
{
   
     public Transform target; // The player or camera target
    public float smoothSpeed = 0.125f; // Smooth camera movement speed
    public Vector3 offset; // The offset of the camera from the target
    public float minDistance = 1f; // Minimum distance between camera and target
    public float maxDistance = 4f; // Maximum distance between camera and target
    public LayerMask collisionLayer; // Layer(s) to check for collisions (e.g., walls, objects)

    private Vector3 currentVelocity;

    void Update()
    {
        // Calculate the desired position of the camera based on the target and offset
        Vector3 desiredPosition = target.position + offset;

        // Perform a raycast to check for obstacles between the target and the camera's desired position
        RaycastHit hit;
        if (Physics.Raycast(target.position, desiredPosition - target.position, out hit, maxDistance, collisionLayer))
        {
            // If there's an obstacle, adjust the camera's position to avoid collision
            float distance = Mathf.Clamp(hit.distance, minDistance, maxDistance); // Clamp the distance to avoid going too close or too far
            desiredPosition = target.position + (desiredPosition - target.position).normalized * distance;
        }

        // Smoothly move the camera to the desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothSpeed);

        // Optionally, make the camera always look at the target
        transform.LookAt(target);
    }
        
}
