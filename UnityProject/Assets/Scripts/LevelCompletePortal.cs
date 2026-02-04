using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompletePortal : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayerMask;
    [SerializeField] private LevelManager _levelManager;

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
        if ((collisionLayerMask & _playerLayerMask) != 0)
        {
            _levelManager.LevelCompleted();
        }
    }

}
