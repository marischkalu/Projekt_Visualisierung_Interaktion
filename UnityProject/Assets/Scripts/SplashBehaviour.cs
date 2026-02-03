using UnityEngine;
using System.Collections;

public class SplashBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DestroyGameObject());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DestroyGameObject()
    {
        yield return new WaitForSeconds(4);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
