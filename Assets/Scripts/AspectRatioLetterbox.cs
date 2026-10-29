
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class AspectRatioLetterbox : MonoBehaviour
{
    private Camera cam;
    private const float TargetAspect = 16f / 9f;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.backgroundColor = Color.black;
        cam.clearFlags = CameraClearFlags.SolidColor;
        ApplyAspectRatio();
    }

    void ApplyAspectRatio()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / TargetAspect;

        if (scaleHeight < 1f)
        {
            // Black bars at the top and bottom
            cam.rect = new Rect(
                0f, (1f - scaleHeight) / 2f,
                1f, scaleHeight
            );
        }
        else
        {
            // Black bars on the left and right
            float scaleWidth = 1f / scaleHeight;

            cam.rect = new Rect(
                (1f - scaleWidth) / 2f, 0f,
                scaleWidth, 1f
            );
        }
    }

    void OnPreCull()
    {
        if (cam != null)
            ApplyAspectRatio();
    }
}
