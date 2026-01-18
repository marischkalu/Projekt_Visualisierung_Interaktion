using UnityEngine;
using UnityEngine.UI;

public class BallUI : MonoBehaviour
{
    [SerializeField] private Transform _ballsParent;
    private Image[] _ballDots;
    [SerializeField] private Color _fullColor = Color.green;
    [SerializeField] private Color _emptyColor = Color.gray;
    private int _orbCount = 0;


    [SerializeField] Outline _ammoOrbHighlight;
    [SerializeField] Outline _ammoColorHighlight;

    void Start()
    {
        // get all children images automatically
        PlayerController.UpdateOrbCountEvent += OnUpdateOrbCount;
        PlayerController.SwitchAmmoEvent += OnSwitchAmmo;
        _ballDots = _ballsParent.GetComponentsInChildren<Image>();
        UpdateDots();
        OnSwitchAmmo();
    }

    void Update()
    {

    }

    void OnUpdateOrbCount(int newOrbCount)
    {
        _orbCount = newOrbCount;
        UpdateDots();
    }

    void UpdateDots()
    {
        for (int i = 0; i < _ballDots.Length; i++)
        {
            if (i < _orbCount)
                _ballDots[i].color = _fullColor;
            else
                _ballDots[i].color = _emptyColor;
        }
    }

    void OnSwitchAmmo()
    {
        _ammoOrbHighlight.enabled = (PlayerController.CurrentAmmo == PlayerController.Ammo.Orb);
        _ammoColorHighlight.enabled = (PlayerController.CurrentAmmo == PlayerController.Ammo.Color);
    }

    void OnDestroy()
    {
        PlayerController.UpdateOrbCountEvent -= OnUpdateOrbCount;
        PlayerController.SwitchAmmoEvent -= OnSwitchAmmo;
    }
}