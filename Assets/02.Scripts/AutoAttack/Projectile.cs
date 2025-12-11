using System.Collections;
using UnityEngine;

/// <summary>
/// 투사체의 이동과 화면 이탈 시 풀 반환 로직을 처리합니다.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class Projectile : MonoBehaviour
{
    public ProjectileWeapon projectilePoolManager;
    [Tooltip("투사체의 이동 속도")]
    public float speed = 10f;

    [Tooltip("화면 밖으로 나간 후 풀로 돌아가기까지의 대기 시간")]
    public float returnToPoolDelay = 2f;

    private Renderer m_Rend;
    private bool m_IsVisible = false;
    private Coroutine m_ReturnCoroutine;
    private GameObject m_ProjectileTarget;

    void Awake()
    {
        // 렌더러 컴포넌트를 미리 캐싱
        m_Rend = GetComponent<Renderer>();
    }

    void OnEnable()
    {
        // 오브젝트가 활성화될 때 상태 초기화
        m_IsVisible = true; 
        
        // 이전에 실행되던 반환 코루틴이 있다면 중지
        if (m_ReturnCoroutine != null)
        {
            StopCoroutine(m_ReturnCoroutine);
            m_ReturnCoroutine = null;
        }
    }

    void Update()
    {
        // 이동 로직 각각 무기에 맞게 수정
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));

        // 화면에 보이다가 안보이게 되는 시점을 감지
        if (m_IsVisible && !m_Rend.isVisible)
        {
            m_IsVisible = false;
            // 화면 밖으로 나갔으므로, 지연 시간 후 풀로 돌아가는 코루틴 시작
            m_ReturnCoroutine = StartCoroutine(ReturnToPoolAfterDelay());
            Debug.Log("투사체가 화면 밖으로 나갔습니다. 풀 반환 절차를 시작합니다.");
        }
        // 화면에 안보이다가 다시 보이게 되는 경우 (예: 화면 가장자리를 스쳐 지나갈 때)
        else if (!m_IsVisible && m_Rend.isVisible)
        {
            m_IsVisible = true;
            // 풀으로 돌아가는 코루틴을 중지
            if (m_ReturnCoroutine != null)
            {
                StopCoroutine(m_ReturnCoroutine);
                m_ReturnCoroutine = null;
                Debug.Log("투사체가 다시 화면에 나타났습니다. 풀 반환을 취소합니다.");
            }
        }
    }

    public void SetTarget(GameObject target)
    {
        m_ProjectileTarget = target;
        gameObject.transform.LookAt(target.transform);
    }

    /// <summary>
    /// 지정된 시간(returnToPoolDelay)이 지난 후, 게임 오브젝트를 비활성화하여 풀에 반환합니다.
    /// </summary>
    private IEnumerator ReturnToPoolAfterDelay()
    {
        yield return new WaitForSeconds(returnToPoolDelay);
        
        Debug.Log("시간이 다 되어 투사체를 풀로 반환합니다.");
        gameObject.SetActive(false); // 오브젝트 비활성화
    }
}
