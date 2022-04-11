using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LifeformGeneticsUI: MonoBehaviour, IAmUIFor<LifeformGenetics>
{
    [SerializeField]
    private Lifeform m_Lifeform;
    [SerializeField]
    private LifeformGenetics m_Genetics => m_Lifeform.Genetics;
    [SerializeField]
    private Canvas m_Canvas;
    [SerializeField]
    private RectTransform m_CanvasRectTransform;

    private Image m_TraitImage;

    public LifeformGenetics GetComponent() => m_Genetics;
    public Canvas GetCanvas() => m_Canvas;
    public bool GetVisible() => m_Canvas.isActiveAndEnabled;

    public void SetVisible(bool visible)
    {
        m_Canvas.gameObject.SetActive(visible);
    }

    int GetImageWidthForTrait(GeneticTrait trait)
    {
        int dataSize = m_Genetics.GetDataSize();
        if(dataSize == 0)
          return 0;
        
        int canvasWidth = (int)m_CanvasRectTransform.sizeDelta.x;

        return (int)(((float)trait.Count() / dataSize) * canvasWidth);
    }

    Color GetImageColourForTrait(GeneticTrait trait)
    {
        return Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

    void GenerateImageForTraits()
    {
        GameObject imageObj = new GameObject("GeneticTraitsImage");
        
        RectTransform imageTrans = imageObj.AddComponent<RectTransform>();
        imageTrans.transform.SetParent(m_Canvas.transform);
        imageTrans.localScale = Vector3.one;
        imageTrans.anchoredPosition = Vector2.zero;
        imageTrans.sizeDelta = new Vector2((float)GeneticTraitType.COUNT, 1f);

        Texture2D traitTex = new Texture2D(1, 1);
        Color[] traitTexColour = new Color[(int)GeneticTraitType.COUNT];
        for(int i = 0; i < (int)GeneticTraitType.COUNT; i++)
        {
            GeneticTrait trait = m_Genetics.GetTrait((GeneticTraitType)i);
            Color traitColour = GetImageColourForTrait(trait);
            traitTexColour[i] = traitColour;
        }

        Image image = imageObj.AddComponent<Image>();
        image.sprite = Sprite.Create(traitTex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));

        imageObj.transform.SetParent(m_Canvas.transform);

        m_TraitImage = image;
    }

    void Start()
    {
        const int colourSeed = 49842019;
        Random.InitState(colourSeed);

        GenerateImageForTraits();
    }
}
