using UnityEngine;
using UnityEngine.AI;

public class PlayerController: MonoBehaviour
{
    [SerializeField]
    private GameObject m_Player;

    [SerializeField]
    private NavMeshAgent m_NavAgent;
    
    [SerializeField]
    private Camera m_Camera;

    private void HandleTerrainClick(RaycastHit hit, GameObject terrainObj)
    {
        m_NavAgent.SetDestination(hit.point);
    }

    private void HandleLifeformClick(GameObject lifeformObj)
    {
        LifeformUI ui = lifeformObj.GetComponent<LifeformUI>(); 

        ui.OpenInspector();
    }

    // TODO: This seems so primitive...
    private void HandleGameObjectClick(RaycastHit hit, GameObject hitObj)
    {
        switch(hitObj.tag)
        {
            case "Player": // Again where do we access these uniformly...?
              break;
            case "Lifeform":
              HandleLifeformClick(hitObj);
              break;
            case "Terrain":
              HandleTerrainClick(hit, hitObj);
              break;
            default:
              return;
        }
    }

    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit))
                HandleGameObjectClick(hit, hit.collider.gameObject);
        }
    }
}
