using UnityEngine;

public class SnowMove : MonoBehaviour
{
    [Header("Scroll Speed Settings")]
    [SerializeField] private float scrollSpeedX = 0.1f;
    [SerializeField] private float scrollSpeedY = 0.1f;

    private Renderer snowRenderer;

    private void Awake()
    {
        // Cache the Renderer component to avoid multiple GetComponent calls.
        snowRenderer = GetComponent<Renderer>();

        // Check if Renderer exists
        if (snowRenderer == null)
        {
            Debug.LogError("SnowMove: Renderer component is missing from the object.");
        }
    }

    private void Update()
    {
        // Only update the texture offset if the Renderer exists.
        if (snowRenderer != null)
        {
            UpdateTextureOffset();
        }
    }

    /// <summary>
    /// Updates the texture offset based on the scroll speeds.
    /// </summary>
    private void UpdateTextureOffset()
    {
        // Use Time.deltaTime to ensure smooth, frame-independent scrolling
        float offsetX = Time.deltaTime * scrollSpeedX;
        float offsetY = Time.deltaTime * scrollSpeedY;
        snowRenderer.material.mainTextureOffset += new Vector2(offsetX, offsetY);
    }
}
