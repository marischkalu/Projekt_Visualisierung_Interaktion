using UnityEngine;
using System;

public class Collectible : MonoBehaviour
{
    public static event Action CollectibleCollisionEvent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag != "Player") return;
        CollectibleCollisionEvent?.Invoke();
        Destroy(this.gameObject);
    }
}
