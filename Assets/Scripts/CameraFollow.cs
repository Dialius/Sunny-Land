using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    // Optional: Bounds for the camera
    public Vector2 minBounds;
    public Vector2 maxBounds;
    public bool useBounds = false;
    public bool autoCalculateBounds = true; // New option for automatic bounds

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (autoCalculateBounds)
        {
            CalculateBounds();
        }
    }

    public void CalculateBounds()
    {
        UnityEngine.Tilemaps.Tilemap[] tilemaps = FindObjectsByType<UnityEngine.Tilemaps.Tilemap>(FindObjectsSortMode.None);
        
        if (tilemaps.Length == 0) return;

        Bounds bounds = new Bounds(tilemaps[0].localBounds.center, tilemaps[0].localBounds.size);
        
        foreach(var tm in tilemaps)
        {
            bounds.Encapsulate(tm.localBounds);
        }

        // Camera calculations
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        // Set min/max bounds based on the total tilemap bounds, adjusted for camera size
        minBounds = new Vector2(bounds.min.x + camWidth, bounds.min.y + camHeight);
        maxBounds = new Vector2(bounds.max.x - camWidth, bounds.max.y - camHeight);
        
        useBounds = true;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        float clampedX = smoothedPosition.x;
        float clampedY = smoothedPosition.y;

        if (useBounds)
        {
             clampedX = Mathf.Clamp(smoothedPosition.x, minBounds.x, maxBounds.x);
             clampedY = Mathf.Clamp(smoothedPosition.y, minBounds.y, maxBounds.y);
        }

        // Keep the camera's original Z position (usually -10)
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
