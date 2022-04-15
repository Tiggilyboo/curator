using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController: MonoBehaviour
{
    [SerializeField]
    private GameObject m_Player;

    [SerializeField]
    private NavMeshAgent m_NavAgent;
    
    [SerializeField]
    private Camera m_Camera;

    [SerializeField]
    private EventSystem m_EventSystem;

    [SerializeField]
    private GraphicRaycaster m_GraphicRaycaster;
  
    [SerializeField]
    private float m_MaxRaycastDistance = 1000f;

    [SerializeField]
    private LayerMask m_RaycastMask;

    private bool HandleTerrainClick(RaycastHit hit, GameObject terrainObj)
    {
        m_NavAgent.SetDestination(hit.point);

        return true;
    }

    private bool HandleLifeformClick(GameObject lifeformObj)
    {
        LifeformUI ui = lifeformObj.GetComponent<LifeformUI>(); 
        LifeformInspectorUI lifeformInspectorUI = ui.OpenInspector();
        lifeformInspectorUI.OnClose += () => {
            m_GraphicRaycaster = null;
        };
        m_GraphicRaycaster = lifeformInspectorUI.GetRaycaster();

        return true;
    }

    // TODO: This seems so primitive...
    private bool HandleGameObjectClick(RaycastHit hit, GameObject hitObj)
    {
        switch(hitObj.tag)
        {
            case "Player": // Again where do we access these uniformly...?
              return false;
            case "Lifeform":
              return HandleLifeformClick(hitObj);
            case "Terrain":
              return HandleTerrainClick(hit, hitObj);
            default:
              return false;
        }
    }

    void Start()
    {
        if(m_EventSystem == null)
          m_EventSystem = EventSystem.current;
    }

    public bool CheckUIAtCursor()
    {
        if(m_GraphicRaycaster == null)
          return false;

        PointerEventData pointerData = new PointerEventData(m_EventSystem);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        m_GraphicRaycaster.Raycast(pointerData, raycastResults);

        return raycastResults.Any();
    }

    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            // Handle events in the UI?
            if(CheckUIAtCursor())
              return;

            /// Proceed with phyics based raycast checks
            Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics
              .RaycastAll(ray, m_MaxRaycastDistance, m_RaycastMask)
              .OrderBy(h => h.distance)
              .ToArray();

            foreach(RaycastHit hit in hits) 
            {
                if(HandleGameObjectClick(hit, hit.collider.gameObject))
                  return;
            }
        }
    }
}
