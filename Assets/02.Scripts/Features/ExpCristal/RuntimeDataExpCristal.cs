using _02.Scripts.Cotroller;
using UnityEngine;

namespace Features.ExpCristal
{
    public class RuntimeDataExpCristal
    {
        // 불변 데이터 참조
        public PureDataExpCristal PureData { get; private set; }
    
        // 가변 상태 데이터
        public float CurrentMoveSpeed { get; private set; }
        public int CurrentExpAmount { get; private set; }
        public Controller Target { get; private set; }
    
        // 생성자
        public RuntimeDataExpCristal(PureDataExpCristal data)
        {
            PureData = data;
            Reset();
        }

        // Setter (로직에서만 호출)
        public void SetTarget(Controller target)
        {
            Target = target;
        }
    
        public void SetExpAmount(int amount)
        {
            CurrentExpAmount = amount;
        }

        // 상태 초기화
        public void Reset()
        {
            if (PureData != null)
            {
                CurrentMoveSpeed = PureData.BaseMoveSpeed;
                CurrentExpAmount = PureData.BaseExpAmount;
            }
            Target = null;
        }

        // 자체 갱신 로직 (시간 흐름에 따른 상태 변화)
        public void Tick(float deltaTime)
        {
            if (Target != null && PureData != null)
            {
                CurrentMoveSpeed += PureData.Acceleration * deltaTime;
            }
        }
    }
}
