
using UnityEngine;

public class MoveDetector: MonoBehaviour
{
    // 观察其他角色
    [SerializeField]
    public GameObject[] m_ObserveTargets;
    
    private GameObject m_ClosestTarget;
    
    void Update()
    {
        var currentPosition = transform.position;
        foreach (var target in m_ObserveTargets)
            if (Vector3.Distance(currentPosition, target.transform.position) < 10.0f)
            {
                m_ClosestTarget = target;
                return;
            }
    }
}
