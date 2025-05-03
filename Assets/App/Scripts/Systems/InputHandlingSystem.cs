using App.Scripts.Components;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Providers;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace App.Scripts.Systems
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public sealed class InputHandlingSystem : ISystem 
    {
        public World World { get; set;}
        
        private Stash<MovableComponent> _movableStash;

        private Vector2 _startTouchPosition;
        private Vector2 _endTouchPosition;
        private Direction _direction;

        public void OnAwake() 
        {
            _movableStash = World.GetStash<MovableComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (Input.touchCount <= 0) return;
            var touch = Input.GetTouch(0);
            switch (touch.phase) {
                case TouchPhase.Began:
                    _startTouchPosition = touch.position;
                    break;
                case TouchPhase.Ended:
                    _endTouchPosition = touch.position;
                    ApplyInput(CalculateSwipeDirection(_startTouchPosition, _endTouchPosition));
                    break;
            }
        }

        private void ApplyInput(Direction direction)
        {
            var ray = Camera.main.ScreenPointToRay(_startTouchPosition);
            if (!Physics.Raycast(ray, out var hit)) return;
            var entity = hit.collider.GetComponent<EntityProvider>().Entity;

            if (!_movableStash.Has(entity)) return;
            
            ref var movableComponent = ref _movableStash.Get(entity);
            if (movableComponent.isBlocked ||
                (direction is Direction.Up or Direction.Down &&
                 movableComponent.direction == MovementDirection.Horizontal) ||
                (direction is Direction.Left or Direction.Right &&
                 movableComponent.direction == MovementDirection.Vertical))
            {
                return;
            }

            movableComponent.directionToMove = direction;
        }

        private static Direction CalculateSwipeDirection(Vector2 startTouchPosition, Vector2 endTouchPosition) {
            var delta = endTouchPosition - startTouchPosition;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                return delta.x > 0 ? Direction.Right : Direction.Left;
            }
            return delta.y > 0 ? Direction.Up : Direction.Down;
        }

        public void Dispose()
        {

        }
    }

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down,
        None
    }
}