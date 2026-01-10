using UnityEngine;

public class MaskPainter : MonoBehaviour
{
    public int maskResolution = 1024;
    public float paintRadius = 0.05f; // UV-Radius

    private Texture2D maskTexture;
    private Material material;

    void Start()
    {
        material = GetComponent<Renderer>().material;

        maskTexture = new Texture2D(maskResolution, maskResolution, TextureFormat.RGBA32, false);
        maskTexture.wrapMode = TextureWrapMode.Clamp;

        // Maske initial schwarz
        Color[] pixels = new Color[maskResolution * maskResolution];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.black;

        maskTexture.SetPixels(pixels);
        maskTexture.Apply();

        material.SetTexture("_MaskTex", maskTexture);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("ColorBall"))
            return;

        ContactPoint hit = collision.contacts[0];

        if (Physics.Raycast(hit.point + hit.normal * 0.01f, -hit.normal, out RaycastHit rayHit))
        {
            PaintAtUV(rayHit.textureCoord);
        }
    }

    void PaintAtUV(Vector2 uv)
    {
        int centerX = (int)(uv.x * maskResolution);
        int centerY = (int)(uv.y * maskResolution);
        int radius = (int)(paintRadius * maskResolution);

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                int px = centerX + x;
                int py = centerY + y;

                if (px < 0 || px >= maskResolution || py < 0 || py >= maskResolution)
                    continue;

                float dist = Mathf.Sqrt(x * x + y * y);
                if (dist > radius)
                    continue;

                maskTexture.SetPixel(px, py, Color.white);
            }
        }

        maskTexture.Apply();
    }
}