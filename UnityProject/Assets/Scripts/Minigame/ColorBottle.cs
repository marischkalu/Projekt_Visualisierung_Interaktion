using UnityEngine;
using System.Collections.Generic;
using System;
using System.Net.NetworkInformation;
using UnityEngine.UI;

public class ColorBottle : MonoBehaviour
{


    private int _maxContentCount = 5;
    public bool IsFinalized;

    //private Vector3 _startPosition;
    public List<Color> StartColorContent = new List<Color>();
    public List<Color> ColorContent;
    public Color CorrectColor;

    private List<GameObject> _visualPieces = new List<GameObject>();

    void Start()
    {
        //_startPosition = transform.position;
        //ColorContent = new List<Color>(StartColorContent);
        //CreateVisual();
    }


    void Update()
    {
        // Lift when selected
        if (BottleManager.SelectedBottle == this)
            GetComponent<Image>().color = Color.white;

            //transform.position = _startPosition + new Vector3(0, 0.1f, 0);
        else
            GetComponent<Image>().color = Color.lightBlue;
    }

    public void BottleClicked()
    {
        Debug.Log(IsFinalized);
        if (IsFinalized) return;
        if (BottleManager.SelectedBottle == null)
        {
            BottleManager.SelectBottle(this);
        }
        else if (BottleManager.SelectedBottle == this)
        {
            BottleManager.DeselectBottle();
        }
        else
        {
            if (ColorContent.Count >= _maxContentCount) return;
            if (BottleManager.SelectedBottle.ColorContent.Count <= 0) return;

            ColorContent.Add(BottleManager.SelectedBottle.ColorContent[BottleManager.SelectedBottle.ColorContent.Count - 1]);
            BottleManager.SelectedBottle.ColorContent.RemoveAt(BottleManager.SelectedBottle.ColorContent.Count - 1);

            CreateVisual();
            BottleManager.SelectedBottle.CreateVisual();
            BottleManager.DeselectBottle();
        }
    }

    public void Finalize()
    {
        IsFinalized = true;
        if (BottleManager.SelectedBottle == this) BottleManager.DeselectBottle();
        //FinalizeEvent?.Invoke();
    }

    public void MixFlask()
    {
        if (IsFinalized) return;
        MixColors();
        CreateVisual();
        //_bottleManager.DeselectBottle();
    }

    void MixColors()
    {
        float newColorRedVal = 0;
        float newColorGreenVal = 0;
        float newColorBlueVal = 0;

        float totalPoD = 0;

        foreach (Color colorComponent in ColorContent)
        {
            float powerOfDarkness = (1 - colorComponent.r) + (1 - colorComponent.g) + (1 - colorComponent.b);
            totalPoD += powerOfDarkness;

            newColorRedVal += colorComponent.r * powerOfDarkness;
            newColorGreenVal += colorComponent.g * powerOfDarkness;
            newColorBlueVal += colorComponent.b * powerOfDarkness;
        }

        Debug.Log($"{newColorRedVal}, {newColorGreenVal}, {newColorBlueVal}");


        newColorRedVal /= totalPoD;
        newColorGreenVal /= totalPoD;
        newColorBlueVal /= totalPoD;

        Color mixedColor = new Color(newColorRedVal, newColorGreenVal, newColorBlueVal);

        for (int i = 0; i < ColorContent.Count; i++)
        {
            ColorContent[i] = mixedColor;
        }
    }



    public void CreateVisual()
    {
        foreach (GameObject g in _visualPieces)
        {
            if (g != null) Destroy(g);
        }

        _visualPieces.Clear();

        for (int i = 0; i < ColorContent.Count; i++)
        {
            GameObject piece = new GameObject();
            
            piece.transform.parent = transform.parent;

            piece.AddComponent<Image>();

            Image pieceImage = piece.GetComponent<Image>();
            RectTransform pieceRectTransform = piece.GetComponent<RectTransform>();
            RectTransform bottleRectTransform = this.gameObject.GetComponent<RectTransform>();
            pieceImage.raycastTarget = false;
            pieceImage.color = ColorContent[i];

            CopyRectTransform(bottleRectTransform, pieceRectTransform);

            float bottleHeight = bottleRectTransform.sizeDelta.y;
            float pieceHeight = bottleHeight / _maxContentCount;

            pieceRectTransform.anchoredPosition = new Vector2(
                pieceRectTransform.anchoredPosition.x,
                pieceRectTransform.anchoredPosition.y + pieceHeight * i - ((bottleHeight / 2) - (pieceHeight / 2))
                );
            pieceRectTransform.sizeDelta = new Vector2(bottleRectTransform.sizeDelta.x, (bottleRectTransform.sizeDelta.y / 5));

            _visualPieces.Add(piece);
        }
    }

    void CopyRectTransform(RectTransform source, RectTransform target)
    {
        // Anchors
        target.anchorMin = source.anchorMin;
        target.anchorMax = source.anchorMax;

        // Pivot
        target.pivot = source.pivot;

        // Position
        target.anchoredPosition = source.anchoredPosition;

        // Rotation
        target.localRotation = source.localRotation;

        // Scale
        target.localScale = source.localScale;

        // Size
        //target.sizeDelta = source.sizeDelta;
    }

    public void ResetBottle()
    {
        IsFinalized = false;
        ColorContent.Clear();
        ColorContent = new List<Color>(StartColorContent);
        CreateVisual();
    }

}
