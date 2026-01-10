using UnityEngine;
using System.Collections.Generic;

public class ParticlePainter : MonoBehaviour
{
    public string maskProperty = "_MaskTex"; // Muss exakt wie die Reference im Shader sein!
    public int textureSize = 512;
    [Range(0.01f, 0.2f)] public float brushSize = 0.05f;

    private RenderTexture maskTexture;
    private Material targetMaterial;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    private MeshCollider meshCollider;

    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        targetMaterial = GetComponent<Renderer>().material;

        // Erstelle die Textur
        maskTexture = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.R8);
        maskTexture.filterMode = FilterMode.Bilinear;
        maskTexture.Create();

        // Initial auf Schwarz setzen
        ClearMask();

        // Dem Material zuweisen
        targetMaterial.SetTexture(maskProperty, maskTexture);
    }

    void OnParticleCollision(GameObject other)
    {
        ParticleSystem ps = other.GetComponent<ParticleSystem>();
        int numCollisionEvents = ps.GetCollisionEvents(gameObject, collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++)
        {
            Vector3 pos = collisionEvents[i].intersection;
            Vector3 normal = collisionEvents[i].normal;

            // Wir schießen einen Strahl von kurz vor der Oberfläche direkt auf das Objekt
            Ray ray = new Ray(pos + normal * 0.05f, -normal);
            RaycastHit hit;

            // Wir fragen explizit den MeshCollider dieses Objekts ab
            if (GetComponent<MeshCollider>().Raycast(ray, out hit, 0.1f))
            {
                Vector2 pixelUV = hit.textureCoord;
                Debug.Log("Treffer bei UV: " + pixelUV);
                PaintAt(pixelUV);
            }
        }
    }

    void PaintAt(Vector2 uv)
    {
        RenderTexture.active = maskTexture;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, textureSize, textureSize, 0);

        // WICHTIG: Benutzt du hier wirklich die "uv" Variable aus den Klammern oben?
        // Wenn hier (0,0) oder feste Zahlen stehen, landet alles in der Ecke.
        float x = uv.x * textureSize;
        float y = (1 - uv.y) * textureSize; // Unity UVs sind invertiert zu Pixel-Koordinaten

        float size = brushSize * textureSize;

        // Zeichne das Quadrat/Punkt an die berechnete Stelle (x, y)
        Graphics.DrawTexture(new Rect(x - size / 2, y - size / 2, size, size), Texture2D.whiteTexture);

        GL.PopMatrix();
        RenderTexture.active = null;
    }

    private void ClearMask()
    {
        RenderTexture.active = maskTexture;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = null;
    }
}