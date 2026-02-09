using System;
using System.Collections;
using _02.Scripts.AutoAttack;
using _02.Scripts.Cotroller;
using Features.Projectile;
using UnityEngine;

// [L] Logic System for Projectile
[RequireComponent(typeof(ProjectileVisualizer))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private PureDataProjectile pureData;

    private RuntimeDataProjectile model;
    private IProjectileVisualizer visualizer;
    
    // 외부 의존성
    private Controller ownerController;
    private Weapon ownerWeapon;
    private LayerMask targetLayer;
    private Camera mainCamera;

    private void Awake()
    {
        visualizer = GetComponent<IProjectileVisualizer>();
        if (visualizer == null) visualizer = gameObject.AddComponent<ProjectileVisualizer>();
        
        mainCamera = Camera.main;

        // PureData 안전장치
        if (pureData == null)
        {
            pureData = ScriptableObject.CreateInstance<PureDataProjectile>();
            // Debug.LogWarning("[Projectile] PureData가 설정되지 않아 기본값을 사용합니다.");
        }

        model = new RuntimeDataProjectile(pureData);
    }

    private void OnEnable()
    {
        if (model != null) model.Reset();
        if (visualizer != null) visualizer.OnTriggerEnterEvent += HandleCollision;
    }

    private void OnDisable()
    {
        if (visualizer != null) visualizer.OnTriggerEnterEvent -= HandleCollision;
    }

    public void ProjectileSetting(Controller controller, Weapon weapon, LayerMask layer)
    {
        ownerController = controller;
        ownerWeapon = weapon;
        targetLayer = layer;
    }

    public void SetTarget(GameObject target)
    {
        if (model == null) return;
        model.SetTarget(target);
        if (target != null)
        {
            visualizer.LookAt(target.transform);
        }
    }

    private void Update()
    {
        if (model == null) return;
        float dt = Time.deltaTime;

        // 1. Move
        Vector3 moveAmount = visualizer.Forward * (model.PureData.Speed * dt);
        visualizer.Position += moveAmount;

        // 2. Check Screen Bounds (기존 로직 유지)
        CheckScreenBounds(dt);
    }

    private void CheckScreenBounds(float dt)
    {
        if (model.IsInScreen)
        {
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(visualizer.Position);
            if (viewportPos.x < -0.1f || viewportPos.x > 1.1f || viewportPos.y < -0.1f || viewportPos.y > 1.1f)
            {
                model.IsInScreen = false;
                // Debug.Log("투사체 화면 이탈");
            }
        }
        else
        {
            // 화면 밖에서 일정 시간 경과 후 비활성화
            model.TimeSinceOutOfScreen += dt;
            if (model.TimeSinceOutOfScreen >= model.PureData.ReturnToPoolDelay)
            {
                visualizer.SetActive(false);
            }
        }
    }

    private void HandleCollision(Collider other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            var enemy = other.GetComponent<Controller>();
            if (enemy != null)
            {
                // DLV Refactoring: Use Model data directly
                int damage = ownerWeapon.Model.FinalDamage;

                var damageEvent = new BattleManager.DamageEventStruct
                {
                    damageAmount = damage,
                    senderWeapon = ownerWeapon,
                    sender = ownerController,
                    receiver = enemy
                };
                
                // 싱글톤 접근은 추후 매니저 리팩토링 시 수정 고려
                BattleManager.Instance?.ProcessDamage(damageEvent);
            }
        }
    }
}

