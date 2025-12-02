using UnityEngine;
using UnityEngine.UI;

public class CorrectColorIndicator : MonoBehaviour
{
    [SerializeField] private ColorBottle _colorBottle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Image>().color = _colorBottle.CorrectColor; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
