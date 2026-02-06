using UnityEngine;
using UnityEngine.UI;


public class SplashManager : MonoBehaviour
{
    [SerializeField] private Image _splash;


    [SerializeField] Outline _ammoColorHighlight;

    void Start()
    {

        PlayerController.SwitchAmmoEvent += OnSwitchAmmo;
        PlayerController.UpdateHudColorSplashesEvent += OnUpdateHudColorSplashesEvent;

        OnSwitchAmmo();
    }

    void Update()
    {

    }

    void OnUpdateHudColorSplashesEvent(Color color)
    {
        _splash.color = color;
    }


    void OnSwitchAmmo()
    {
        _ammoColorHighlight.enabled = (PlayerController.CurrentAmmo == PlayerController.Ammo.Color);
    }

    void OnDestroy()
    {
        PlayerController.SwitchAmmoEvent -= OnSwitchAmmo;
        PlayerController.UpdateHudColorSplashesEvent -= OnUpdateHudColorSplashesEvent;
    }
}
