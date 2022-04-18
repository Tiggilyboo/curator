using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(SkinnedMeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class LifeformModel: MonoBehaviour
{
    [SerializeField]
    Lifeform m_Lifeform;
    [SerializeField]
    LifeformGenetics m_Genetics => m_Lifeform.Genetics;

    [SerializeField]
    private Mesh m_Mesh;

    [SerializeField]
    private MeshFilter m_MeshFilter;
    [SerializeField]
    private SkinnedMeshRenderer m_MeshRenderer;
    [SerializeField]
    private MeshCollider m_MeshCollider;

    private void Start()
    {

        if(m_MeshCollider == null)
          m_MeshCollider = GetComponent<MeshCollider>();
        if(m_MeshFilter == null)
          m_MeshFilter = GetComponent<MeshFilter>();
        if(m_MeshRenderer == null)
        {
            m_MeshRenderer = GetComponent<SkinnedMeshRenderer>();
            m_MeshRenderer.rootBone = gameObject.transform;
        }

        m_MeshFilter.sharedMesh = m_Mesh;
        m_MeshCollider.sharedMesh = m_Mesh;
        m_MeshRenderer.sharedMesh = m_Mesh;
    }
}
