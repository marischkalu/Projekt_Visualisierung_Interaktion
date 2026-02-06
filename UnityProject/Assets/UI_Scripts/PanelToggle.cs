using UnityEngine;
using UnityEngine.UI;
public class PanelToggle : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private KeyCode _togglePanelKey = KeyCode.E;

    void Start()
    {
        _panel.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(_togglePanelKey))
        {
            TogglePanels();
        }
    }

    void TogglePanels()
    {
        _panel.SetActive(!_panel.activeSelf);
    }
}
