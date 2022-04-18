using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LifeformTraitsImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image m_Image;
    [SerializeField]
    private Sprite m_Sprite;
    [SerializeField]
    private RectTransform m_ImageRectTransform;
    [SerializeField]
    private LifeformGenetics m_Genetics;

    [Header("Hover UI")]
    [SerializeField]
    private HoverUI m_HoverUIPrefab;
    [SerializeField]
    private Font m_Font;
    [SerializeField]
    private HoverUI m_Hover;
    [SerializeField]
    private GeneticTraitType m_CurrentTraitType;
    [SerializeField]
    private GeneticTrait m_GeneticTrait;
    [SerializeField]
    private bool m_PointerHovering;

    public bool IsPointerHovering => m_PointerHovering;
    public HoverUI GetActiveHoverUI() => m_Hover;

    public void Initialize(LifeformGenetics genetics, Sprite sprite)
    {
        m_Genetics = genetics;
        m_Sprite = sprite;
        m_Image.sprite = m_Sprite;
    }

    public void SetVisible(bool visible)
    {
        m_Image.enabled = visible;
        m_Hover?.Close();
    }

    private void CreateHoverForTraitType(GeneticTraitType traitType, float x, float y)
    {
        GeneticTrait hoverTrait = m_Genetics.GetTrait(traitType);

        // We are hovering over this behaviour's gameObject
        HoverUI hover = Instantiate<HoverUI>(m_HoverUIPrefab);
        hover.transform.SetParent(transform.parent.parent);
        hover.OnClose += () => {
          GameObject.Destroy(hover.gameObject);
          m_Hover = null;
        };

        hover.transform.position = new Vector3(x, y, 0f);

        Text title = CreateHoverTitleElementFor(hoverTrait);
        Text text = CreateHoverTextElementFor(hoverTrait);

        hover.Initialise(new []{ 
            title, 
            text,
        });
        hover.SetVisible(true);

        // Only allow a single hover to show in this UI
        if(m_Hover != null)
          m_Hover.Close();

        m_Hover = hover;
    }
    
    // TODO: Better way to check when we are hovering over a specific part of the image?
    //  Other methods could be with multiple images? But then we might run into scaling issues.
    public void OnPointerEnter(PointerEventData pointer)
    {
        m_PointerHovering = true;

        GameObject currentObj = pointer.pointerCurrentRaycast.gameObject;
        if(currentObj != gameObject)
          return;

        GeneticTraitType currentTraitType = GetTraitHoverType(pointer.position.x);

        if(m_Hover == null || m_CurrentTraitType != currentTraitType)
        {
            CreateHoverForTraitType(currentTraitType, pointer.position.x, pointer.position.y);
            m_CurrentTraitType = currentTraitType;
        }
    }

    public void OnPointerExit(PointerEventData pointer)
    {
        m_PointerHovering = false;
    }
    
    private Rect GetScreenCorners()
    {
        var corners = new Vector3[4];
        m_ImageRectTransform.GetWorldCorners(corners);

        return new Rect(
            corners[0].x,
            corners[1].y,
            corners[2].x - corners[0].x,
            corners[2].y - corners[0].y);
    }
    
    private GeneticTraitType GetTraitHoverType(float pointerX)
    {
        // Determine what part of the image we are hovering over
        int traitCount = m_Genetics.GetTraitCount();
        float traitUnitX = (float)(m_ImageRectTransform.sizeDelta.x / traitCount);
        float uiOffset = GetScreenCorners().xMin;
        pointerX -= uiOffset;

        int pointerTraitIndex = (int)(pointerX / traitUnitX);

        return (GeneticTraitType)pointerTraitIndex;
    }

    private string GetHoverTitleFor(GeneticTrait trait)
    {
        return trait.Identifier.ToString();
    }

    private Text CreateHoverTitleElementFor(GeneticTrait trait)
    {
        GameObject titleObj = new GameObject("HoverUI_Title");
        Text title = titleObj.AddComponent<Text>();
        title.font = m_Font;
        title.text = GetHoverTitleFor(trait);
        title.color = Color.black;
        title.alignment = TextAnchor.MiddleCenter;
        title.fontSize = 16;
        title.fontStyle = FontStyle.Bold;

        return title;
    }

    private string GetHoverTextFor(GeneticTrait trait)
    {
        IEnumerable<byte> traitData = m_Genetics.GetDataForTrait(trait);
        string traitRaw = string.Join(" ", traitData);
        string traitValue = "";

        string formatValueRate(float v, float r)
        {
            return string.Format("{0} [{1}]", v, r);
        }

        // TODO: Should this each be owned by a formatting property of GeneticTrait or?
        switch(trait.Identifier)
        {
          case GeneticTraitType.Energy:
            traitValue = formatValueRate(m_Genetics.GetMaxEnergy(), m_Genetics.GetEnergyRate());
            break;
          case GeneticTraitType.Hunger:
            traitValue = formatValueRate(m_Genetics.GetMaxHunger(), m_Genetics.GetHungerRate());
            break;
          case GeneticTraitType.Age:
            traitValue = string.Format("{0}", m_Genetics.GetMaxAge());
            break;
          case GeneticTraitType.Speed:
            traitValue = string.Format("{0}", m_Genetics.GetMoveRate());
            break;
          case GeneticTraitType.Eyesight:
            traitValue = string.Format("{0}", m_Genetics.GetEyesightDistance());
            break;
          case GeneticTraitType.Breed:
            traitValue = string.Format("{0}", m_Genetics.GetBreedRate());
            break;
          default:
            throw new NotImplementedException();
        }

        return string.Format("{0}\n{1}", traitValue, traitRaw);
    }

    private Text CreateHoverTextElementFor(GeneticTrait trait)
    {
        GameObject textObj = new GameObject("HoverUI_Text");
        Text text = textObj.AddComponent<Text>();
        text.font = m_Font;
        text.text = GetHoverTextFor(trait);
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleLeft;
        text.fontSize = 14;
        text.fontStyle = FontStyle.Normal;

        return text;
    }

    public void CloseHover() => m_Hover.Close();
    public bool IsHovering => m_PointerHovering;

    // Update is called once per frame
    void Update()
    {
        if(!m_PointerHovering)
            return;

        float x = Input.mousePosition.x;
        GeneticTraitType currentTraitType = GetTraitHoverType(x);
        if(m_Hover == null) 
        {
            CreateHoverForTraitType(currentTraitType, x, Input.mousePosition.y);
            m_CurrentTraitType = currentTraitType;
        }
        else
        {
            GeneticTrait trait = m_Genetics.GetTrait(m_CurrentTraitType);
            m_Hover.SetPosition(Input.mousePosition);
            m_Hover.UpdateTextElement(0, GetHoverTitleFor(trait));
            m_Hover.UpdateTextElement(1, GetHoverTextFor(trait));
        }
    }
}
