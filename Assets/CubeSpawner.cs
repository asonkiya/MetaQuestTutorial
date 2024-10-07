using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cameraRig;

    public GameObject cubePrefab;

    public float spawnDistance = 5.0f;

    public float offsetRange = 2.0f;

    public float spawnInterval = 1.0f;

    public float moveSpeed = 2.0f;

    private void Start()
    {
        InvokeRepeating("SpawnCube", 0, spawnInterval);
    }

    private void SpawnCube()
    {
        if (cameraRig == null || cubePrefab == null)
        {
            Debug.LogWarning("CameraRig or CubePrefab is not set!");
            return;
        }

        Transform centerEye = cameraRig.GetComponent<OVRCameraRig>().centerEyeAnchor;

       
        Vector3 randomOffset = new Vector3(
            Random.Range(-offsetRange, offsetRange),
            Random.Range(-offsetRange, offsetRange),
            0
        );

        
        Vector3 spawnPosition = centerEye.position + centerEye.forward * spawnDistance + randomOffset;
        GameObject spawnedCube = Instantiate(cubePrefab, spawnPosition, Quaternion.identity);
        spawnedCube.transform.LookAt(centerEye);
        MoveTowardsCamera moveScript = spawnedCube.AddComponent<MoveTowardsCamera>();
        moveScript.Initialize(centerEye, moveSpeed);
    }
}


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
       
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        OVRGrabbable grabbable = collision.gameObject.GetComponent<OVRGrabbable>();

        if (grabbable != null && grabbable.isGrabbed)
        {
            
            Destroy(gameObject);
        }
    }
}
