using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class LifeformTraitsUI: MonoBehaviour, IPointerEnterHandler
{
    private static Dictionary<GeneticTraitType, Color> m_TraitColours;

    private const int traitCount = (int)GeneticTraitType.COUNT;

    [SerializeField]
    private LifeformGeneticsUI m_GeneticsUI;
    [SerializeField]
    private LifeformGenetics m_Genetics => m_GeneticsUI.GetComponent();
    [SerializeField]
    private Image m_TraitImage;
    [SerializeField]
    private Sprite m_TraitImageSprite;
    [SerializeField]
    private RectTransform m_CanvasRectTransform;
    [SerializeField]
    private RectTransform m_TraitImageRect;

    private void InitializeTraitColours()
    {
        m_TraitColours = new Dictionary<GeneticTraitType, Color>();
        for(int i = 0; i < traitCount; i++)
        {
            GeneticTraitType traitType = (GeneticTraitType)i;
            Color colour = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

            m_TraitColours.Add(traitType, colour);
        }
    }

    private void CreateUI()
    {
        int widthSegment = (int)Mathf.Floor(m_CanvasRectTransform.sizeDelta.x / traitCount);
        int width = (int)Mathf.Floor(m_CanvasRectTransform.sizeDelta.x);
        int height = (int)Mathf.Floor(m_CanvasRectTransform.sizeDelta.y);

        Texture2D traitTex = new Texture2D(width, height);

        // TODO: Investigate use of SetPixels with mips
        for(int i = 0; i < traitCount; i++) 
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

        m_TraitImage.sprite = sprite;
        m_TraitImageSprite = sprite;
    }
  
    // TODO: Better way to check when we are hovering over a specific part of the image?
    //  Other methods could be with multiple images? But then we might run into scaling issues.
    public void OnPointerEnter(PointerEventData pointer)
    {
        GameObject currentObj = pointer.pointerCurrentRaycast.gameObject;
        if(currentObj != gameObject)
          return;

        // We are hovering over this behaviour's gameObject
        Debug.Log("Hovering over " + gameObject.name);
    }

    void Start()
    {
        const int colourSeed = 49842019;
        Random.InitState(colourSeed);
        
        if(m_TraitColours == null)
            InitializeTraitColours();

        CreateUI();
    }
}
