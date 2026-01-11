using UnityEngine;

public class WorldSphereInteractor : MonoBehaviour
{
    [SerializeField] private float radius = 3f;

    void Update()
    {
        Shader.SetGlobalVector("_Position", transform.position);
        Shader.SetGlobalFloat("_Radius", radius);
    }
}
