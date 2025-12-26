using Mono.Cecil;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player;
    public float heightOffset;
    public float followSpeed = 5f;
    private Vector3 m_TargetPosition;
    void LateUpdate()
    {
        if (player == null) return;
        m_TargetPosition.x = player.position.x;
        m_TargetPosition.y = player.position.y + heightOffset;
        m_TargetPosition.z = player.position.z;
        transform.position = Vector3.Lerp(transform.position, m_TargetPosition, followSpeed * Time.deltaTime);
    }
}
