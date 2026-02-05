using _02.Scripts.Cotroller;
using Features.ExpCristal; // 새로 만든 네임스페이스
using UnityEngine;

// [L] Logic System 역할을 수행하지만, 기존 프리팹 연결 유지를 위해 클래스명 유지
[RequireComponent(typeof(ExpCristalVisualizer))] 
public class ExpCristal : MonoBehaviour
{
    [SerializeField] private PureDataExpCristal _pureData; // 인스펙터에서 할당 필요

    private RuntimeDataExpCristal _model;
    private IExpCristalVisualizer _visualizer;

    private void Awake()
    {
        // [Internal Binding] Visualizer 연결
        _visualizer = GetComponent<IExpCristalVisualizer>();
        if (_visualizer == null)
        {
            _visualizer = gameObject.AddComponent<ExpCristalVisualizer>();
        }

        // 데이터 안전장치
        if (_pureData == null)
        {
            // 기본 SO가 없다면 임시로 생성하여 에러 방지 (실제로는 인스펙터 할당 권장)
            _pureData = ScriptableObject.CreateInstance<PureDataExpCristal>();
            Debug.LogWarning("[ExpCristal] PureData가 할당되지 않아 기본값을 사용합니다.");
        }

        // 모델 초기화
        _model = new RuntimeDataExpCristal(_pureData);
    }

    private void OnEnable()
    {
        // 오브젝트 풀링 등으로 재사용될 때 초기화
        if (_model != null)
        {
            _model.Reset();
        }
    }

    // --- 기존 인터페이스 호환 ---
    public void ExpSetting(Controller controller)
    {
        Initialize(controller, _model.CurrentExpAmount);
    }

    public void SetTarget(Controller controller)
    {
        _model.SetTarget(controller);
    }

    public void SetValue(int amount)
    {
        _model.SetExpAmount(amount);
    }
    // ---------------------------

    public void Initialize(Controller target, int expAmount)
    {
        _model.SetTarget(target);
        _model.SetExpAmount(expAmount);
    }

    private void Update()
    {
        if (_model == null || _model.Target == null) return;

        float dt = Time.deltaTime;

        // 1. Process: 모델 상태 갱신 (속도 증가 등)
        _model.Tick(dt);

        // 2. Logic: 이동 계산
        Vector3 targetPos = _model.Target.transform.position;
        Vector3 currentPos = _visualizer.Position;
        
        // MoveTowards 로직 수행
        Vector3 newPos = Vector3.MoveTowards(currentPos, targetPos, _model.CurrentMoveSpeed * dt);

        // 3. Command: 비주얼 업데이트 명령
        _visualizer.Position = newPos;

        // 4. Decision: 획득 판정
        if (Vector3.Distance(newPos, targetPos) < _model.PureData.AcquisitionDistance)
        {
            Collect();
        }
    }

    private void Collect()
    {
        // 외부 시스템 알림
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.BroadcastExpEvent(_model.CurrentExpAmount);
        }

        // 상태 정리 및 비활성화
        _model.SetTarget(null); // 타겟 해제
        _visualizer.SetActive(false);
    }
}