using UnityEngine;
using UnityEngine.UI;


public class SplashManager : MonoBehaviour
{
    [SerializeField] private Image _splashOne;
    [SerializeField] private Image _splashTwo;


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

    void OnUpdateHudColorSplashesEvent(Color[] colors)
    {
        _splashOne.color = colors[0];
        _splashTwo.color = colors[1];
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
