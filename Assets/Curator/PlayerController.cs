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
    private float m_MaxRaycastDistance = 1000f;

    [SerializeField]
    private LayerMask m_RaycastMask;

    [SerializeField]
    private List<IAmUI> m_ActiveUI;

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
            Debug.Log("Closed UI");
            m_ActiveUI.Remove(lifeformInspectorUI);
        };
        m_ActiveUI.Add(lifeformInspectorUI);

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
        m_ActiveUI = new List<IAmUI>();

        if(m_EventSystem == null)
          m_EventSystem = EventSystem.current;
    }

    public bool CheckUIAtCursor(IAmUI ui)
    {
        GraphicRaycaster raycaster = ui.GetRaycaster();
        if(raycaster == null)
          return false;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        PointerEventData pointerData = new PointerEventData(m_EventSystem) 
        {
            position = Input.mousePosition,
        };
        raycaster.Raycast(pointerData, raycastResults);

        return raycastResults.Any();
    }

    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            // Handle events in the UI?
            foreach(IAmUI ui in m_ActiveUI)
              if(CheckUIAtCursor(ui))
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
