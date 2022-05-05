using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StatUI: MonoBehaviour, IAmUIFor<Resource>
{
    [SerializeField]
    private Resource m_Resource;
    
    [SerializeField]
    private List<Text> m_TextElements;

    [SerializeField]
    private Image m_ImageElement;

    [SerializeField]
    private Font m_Font;

    [SerializeField]
    private Canvas m_Canvas;

    [SerializeField]
    private GraphicRaycaster m_Raycaster;

    [SerializeField]
    private HorizontalLayoutGroup m_Layout;
    
    public Resource GetComponent() => m_Resource;

    public Canvas GetCanvas() => m_Canvas;
    public GraphicRaycaster GetRaycaster() => m_Raycaster;
    public bool GetVisible() => m_Canvas.isActiveAndEnabled;
    
    public void SetVisible(bool visible) 
    {
        m_Canvas.gameObject.SetActive(visible);
    }

    public Text CreateStatTextFor(string text)
    {
        GameObject titleObj = new GameObject("StatUI_TextElement");
        Text title = titleObj.AddComponent<Text>();
        title.font = m_Font;
        title.text = text;

        return title;
    }

    public void SetImageSpriteFrom(Texture2D texture)
    {
        Rect spriteRect = new Rect(0, 0, texture.width, texture.height);
        Sprite sprite = Sprite.Create(texture, spriteRect, new Vector2(0.5f, 0.5f));

        m_ImageElement.sprite = sprite;
    }

    public void Initialise(Resource resource)
    {
        m_Resource = resource;

        Text caption = CreateStatTextFor(resource.Identifier);
        Text text = CreateStatTextFor(resource.Quantity.ToString());

        m_Resource.OnResourceChange += (r) => {
            caption.text = r.Identifier;
            text.text = r.Quantity.ToString();
            SetImageSpriteFrom(r.Image);
        };
        m_TextElements = new List<Text> {
            caption, 
            text
        };
        foreach(Text e in m_TextElements)
            e.gameObject.transform.SetParent(m_Layout.transform);

        SetVisible(true);
    }
    
    private void Start()
    {
        if(m_TextElements == null)
          m_TextElements = new List<Text>();
    }
}
