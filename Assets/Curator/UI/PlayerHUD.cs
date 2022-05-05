using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD: MonoBehaviour
{
    [SerializeField]
    private Canvas m_Canvas;

    [SerializeField]
    private ResourceStorage m_PlayerResources;

    [SerializeField]
    private HorizontalLayoutGroup m_Layout;

    [SerializeField]
    private Dictionary<string, StatUI> m_Elements;

    private StatUI GetUIForResource(Resource r)
    {
        if(m_Elements.TryGetValue(r.Identifier, out StatUI ui))
            return ui;
        else
            return null;
    }

    private StatUI CreateUIForResource(Resource r)
    {
        GameObject layoutElement = new GameObject(string.Format("Resource_{0}", r.Identifier));
        layoutElement.transform.parent = m_Layout.gameObject.transform;

        StatUI ui = layoutElement.AddComponent<StatUI>();
        ui.Initialise(r);

        return ui;
    }

    private void HandleResourceAdd(Resource newResource)
    {
        StatUI ui = CreateUIForResource(newResource);
        m_Elements.Add(newResource.Identifier, ui);
    }

    private void HandleResourceUpdate(Resource resource)
    {
        resource.TriggerChange();
    }

    private void HandleResourceRemove(Resource oldResource)
    {
        StatUI ui = GetUIForResource(oldResource);
        GameObject.Destroy(ui.gameObject);
    }

    private void Start()
    {
        m_PlayerResources.OnResourceAdded += HandleResourceAdd;
        m_PlayerResources.OnResourceRemoved += HandleResourceRemove;
        m_PlayerResources.OnResourceUpdated += HandleResourceUpdate;
    }
}
