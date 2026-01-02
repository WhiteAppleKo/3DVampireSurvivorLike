using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 투사체의 이동과 화면 이탈 시 풀 반환 로직을 처리합니다.
/// </summary>
public class Projectile : MonoBehaviour
{
    public ProjectileWeapon projectilePoolManager;
    [Tooltip("투사체의 이동 속도")]
    public float speed = 10f;

    [Tooltip("화면 밖으로 나간 후 풀로 돌아가기까지의 대기 시간")]
    public float returnToPoolDelay = 2f;

    private Coroutine m_ReturnCoroutine;
    private GameObject m_ProjectileTarget;

    // Game 뷰의 메인 카메라를 참조하기 위한 변수
    private Camera m_MainCamera;
    // 투사체가 화면 안에 있었는지 상태를 추적하는 플래그
    private bool m_WasInScreen;
    private Weapon m_Weapon;
    private Controller m_Controller;
    [SerializeField] private LayerMask m_TargetLayer;

    void Awake()
    {
        // 메인 카메라를 찾아서 캐싱합니다.
        // Camera.main은 성능 부하가 있을 수 있으므로 Awake에서 한 번만 호출합니다.
        m_MainCamera = Camera.main;
    }

    void OnEnable()
    {
        // 오브젝트가 활성화될 때 화면 안에 있는 것으로 상태 초기화
        m_WasInScreen = true; 
        
        // 이전에 실행되던 반환 코루틴이 있다면 중지
        if (m_ReturnCoroutine != null)
        {
            StopCoroutine(m_ReturnCoroutine);
            m_ReturnCoroutine = null;
        }
    }

    void Update()
    {
        // 이동 로직
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));

        // 이전에 화면 안에 있었던 경우에만 화면 밖으로 나갔는지 검사
        if (m_WasInScreen)
        {
            // 월드 좌표를 메인 카메라의 뷰포트 좌표로 변환
            Vector3 viewportPos = m_MainCamera.WorldToViewportPoint(transform.position);

            // 뷰포트 좌표가 x,y축 모두 0과 1사이가 아니라면 화면 밖으로 나간 것
            // 약간의 여유(margin)를 두어 완전히 사라졌을 때를 감지할 수 있습니다.
            if (viewportPos.x < -0.1f || viewportPos.x > 1.1f || viewportPos.y < -0.1f || viewportPos.y > 1.1f)
            {
                m_WasInScreen = false;
                // 화면 밖으로 나갔으므로, 지연 시간 후 풀로 돌아가는 코루틴 시작
                m_ReturnCoroutine = StartCoroutine(ReturnToPoolAfterDelay());
                Debug.Log("투사체가 화면 밖으로 나갔습니다. 풀 반환 절차를 시작합니다.");
            }
        }
    }

    public void ProjectileSetting(Controller playerController, Weapon projectileWepon, LayerMask targetLayer)
    {
        m_Controller = playerController;
        m_Weapon = projectileWepon;
        m_TargetLayer = targetLayer;
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

    private void OnTriggerEnter(Collider other)
    {
        if((m_TargetLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            var enemy = other.GetComponent<Controller>();
            if (enemy != null)
            {
                var m_DamageEvent = new BattleManager.DamageEventStruct
                {
                    // 풀링 시 저장된 데미지 대신, 발사 시점의 최종 데미지를 실시간으로 가져옵니다.
                    damageAmount = m_Weapon.FinalStats.damage,
                    senderWeapon = m_Weapon,
                    sender = m_Controller,
                    receiver = enemy
                };
                BattleManager.Instance.BroadcastDamageEvent(m_DamageEvent);
            }
        }
    }
}
