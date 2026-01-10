using UnityEngine;

public class FastBallPainter : MonoBehaviour
{
    public Material paintMaterial; // Ein spezieller Brush-Shader (siehe unten)
    public float brushSize = 0.05f;

    private void OnCollisionEnter(Collision collision)
    {
        Renderer rend = collision.collider.GetComponent<Renderer>();
        if (rend == null) return;

        // Wir holen uns die RenderTexture vom Material des getroffenen Objekts
        RenderTexture mask = rend.material.GetTexture("_BaseMask") as RenderTexture;

        if (mask != null)
        {
            RaycastHit hit;
            Vector3 direction = collision.contacts[0].point - transform.position;

            if (Physics.Raycast(transform.position, direction, out hit))
            {
                // Wir übergeben dem Brush-Material die UV-Koordinaten
                paintMaterial.SetVector("_PaintUV", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));
                paintMaterial.SetFloat("_BrushSize", brushSize);

                // Ein temporärer Puffer, um das Bild zu aktualisieren
                RenderTexture temp = RenderTexture.GetTemporary(mask.width, mask.height);
                Graphics.Blit(mask, temp, paintMaterial);
                Graphics.Blit(temp, mask);
                RenderTexture.ReleaseTemporary(temp);
            }
        }
        Destroy(gameObject);
    }
}