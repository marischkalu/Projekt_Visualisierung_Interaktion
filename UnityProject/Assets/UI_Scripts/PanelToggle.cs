using UnityEngine;

public class PanelToggle : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private KeyCode _togglePanelKey = KeyCode.E;

    void Update()
    {
        if (Input.GetKeyDown(_togglePanelKey))
        {
            TogglePanel();
        }
    }

    void TogglePanel()
    {
        _panel.SetActive(!_panel.activeSelf);
    }
}
