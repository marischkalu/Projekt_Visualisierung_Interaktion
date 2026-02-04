using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GroundZero : MonoBehaviour
{
    [SerializeField] private LayerMask _affectedLayerMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnTriggerEnter(Collider other)
    {
        int collisionLayerMask = 1 << other.gameObject.layer;
        if ((collisionLayerMask & _affectedLayerMask) != 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}