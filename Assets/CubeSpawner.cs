using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    // Reference to the OVRCameraRig to access the center camera
    public GameObject cameraRig;

    // Cube prefab to spawn
    public GameObject cubePrefab;

    // Spawn distance from the camera
    public float spawnDistance = 5.0f;

    // Offset range for random spawning position
    public float offsetRange = 2.0f;

    // Spawn rate in seconds
    public float spawnInterval = 1.0f;

    // Speed at which cubes move towards the camera
    public float moveSpeed = 2.0f;

    private void Start()
    {
        // Start spawning cubes at intervals
        InvokeRepeating("SpawnCube", 0, spawnInterval);
    }

    private void SpawnCube()
    {
        // Check if cameraRig and cubePrefab are set
        if (cameraRig == null || cubePrefab == null)
        {
            Debug.LogWarning("CameraRig or CubePrefab is not set!");
            return;
        }

        // Get the camera's center position and forward direction
        Transform centerEye = cameraRig.GetComponent<OVRCameraRig>().centerEyeAnchor;

        // Calculate a random offset
        Vector3 randomOffset = new Vector3(
            Random.Range(-offsetRange, offsetRange),
            Random.Range(-offsetRange, offsetRange),
            0
        );

        // Set the spawn position with offset
        Vector3 spawnPosition = centerEye.position + centerEye.forward * spawnDistance + randomOffset;

        // Spawn a new cube
        GameObject spawnedCube = Instantiate(cubePrefab, spawnPosition, Quaternion.identity);

        // Make the cube face towards the camera
        spawnedCube.transform.LookAt(centerEye);

        // Attach a script to move the cube towards the camera
        MoveTowardsCamera moveScript = spawnedCube.AddComponent<MoveTowardsCamera>();
        moveScript.Initialize(centerEye, moveSpeed);
    }
}

// Script to handle the movement and destruction of cubes
public class MoveTowardsCamera : MonoBehaviour
{
    private Transform target;
    private float speed;

    public void Initialize(Transform targetTransform, float moveSpeed)
    {
        target = targetTransform;
        speed = moveSpeed;
    }

    private void Update()
    {
        // Move the cube towards the target (camera)
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object is being held by the player
        OVRGrabbable grabbable = collision.gameObject.GetComponent<OVRGrabbable>();

        if (grabbable != null && grabbable.isGrabbed)
        {
            // Destroy the cube when hit by a grabbed object
            Destroy(gameObject);
        }
    }
}
