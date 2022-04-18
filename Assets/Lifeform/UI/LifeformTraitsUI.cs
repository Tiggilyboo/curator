using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class LifeformTraitsUI: MonoBehaviour
{
    private static Dictionary<GeneticTraitType, Color> m_TraitColours;

    [SerializeField]
    private LifeformGeneticsUI m_GeneticsUI;
    [SerializeField]
    private LifeformGenetics m_Genetics => m_GeneticsUI.GetComponent();
    [SerializeField]
    private LifeformTraitsImage m_TraitImage;
    [SerializeField]
    private RectTransform m_CanvasRectTransform;

    public LifeformTraitsImage GetLifeformTraitsImage() => m_TraitImage;

    private int GetTraitCount()
    {
        return m_Genetics.GetTraitCount();
    }

    private void InitializeTraitColours()
    {
        m_TraitColours = new Dictionary<GeneticTraitType, Color>();
        for(int i = 0; i < GetTraitCount(); i++)
        {
            GeneticTraitType traitType = (GeneticTraitType)i;
            Color colour = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

            m_TraitColours.Add(traitType, colour);
        }
    }

    private int GetTraitUnitWidth()
    {
        int widthSegment = (int)Mathf.Floor(m_CanvasRectTransform.sizeDelta.x / GetTraitCount());
        return widthSegment;
    }
    
    // TODO: Ideal to move to utilities / extensions somewhere
    // This is simplified if it is JUST and overlay type, not rendered to a mesh / plane that is not the screen!

    private void CreateTraitImageUI()
    {
        int widthSegment = GetTraitUnitWidth();
        int width = (int)Mathf.Floor(m_CanvasRectTransform.sizeDelta.x);
        int height = (int)Mathf.Floor(m_CanvasRectTransform.sizeDelta.y);

        Texture2D traitTex = new Texture2D(width, height);

        // TODO: Investigate use of SetPixels with mips
        for(int i = 0; i < GetTraitCount(); i++) 
        {
            GeneticTrait trait = m_Genetics.GetTrait((GeneticTraitType)i);
            Color traitColour = m_TraitColours[trait.Identifier];

            int segment = i * widthSegment;
            for(int x = segment; x < segment + widthSegment; x++)
                for(int y = 0; y < height; y++)
                    traitTex.SetPixel(x, y, traitColour);
        }
        traitTex.Apply();

        Rect spriteRect = new Rect(0, 0, traitTex.width, traitTex.height);
        Sprite sprite = Sprite.Create(traitTex, spriteRect, new Vector2(0.5f, 0.5f));

        m_TraitImage.Initialize(m_Genetics, sprite);
    }

    private void CreateUI()
    {
        CreateTraitImageUI();
    }
    
    public void SetVisible(bool visible)
    {
        m_TraitImage.SetVisible(visible);
    }

    private void Start()
    {
        const int colourSeed = 49842019;
        Random.InitState(colourSeed);
        
        if(m_TraitColours == null)
            InitializeTraitColours();

        CreateUI();
    }
}
