using UnityEngine;

namespace Features.Player
{
    // [D] Input Data (Command Struct) - Rule 1 준수
    public struct PlayerInputCommand
    {
        public readonly Vector3 MoveDirection;
        public readonly bool IsRightClickPressed;
        public readonly Vector3 MouseWorldPosition;

        public PlayerInputCommand(Vector3 moveDir, bool rightClick, Vector3 mousePos)
        {
            MoveDirection = moveDir;
            IsRightClickPressed = rightClick;
            MouseWorldPosition = mousePos;
        }
    }
}
