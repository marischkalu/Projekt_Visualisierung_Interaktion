using UnityEngine;

public class BottleManager : MonoBehaviour
{
    public static ColorBottle SelectedBottle = null;

    public static void SelectBottle(ColorBottle bottle)
    {
        SelectedBottle = bottle;
    }

    public static void DeselectBottle()
    {
        SelectedBottle = null;
    }
}
