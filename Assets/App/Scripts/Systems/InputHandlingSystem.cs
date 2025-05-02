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
        
        private Filter _filter;
        private Stash<MovableComponent> _movableStash;

        private Vector2 _startTouchPosition;
        private Vector2 _endTouchPosition;
        private Direction _direction;

        public void OnAwake() 
        {
            _filter = World.Filter.With<MovableComponent>().Build();
            _movableStash = World.GetStash<MovableComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (Input.touchCount <= 0) return;
            var touch = Input.GetTouch(0);
            switch (touch.phase) {
                case TouchPhase.Began:
                    // Запоминаем начальную позицию касания
                    _startTouchPosition = touch.position;
                    break;
                case TouchPhase.Ended:
                    // Вычисляем конечную позицию и направление свайпа
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
                
            if (_movableStash.Has(entity)) {
                // entity moves to direction
            }
        }

        private static Direction CalculateSwipeDirection(Vector2 startTouchPosition, Vector2 endTouchPosition) {
            var delta = endTouchPosition - startTouchPosition;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                // Горизонтальное движение
                return delta.x > 0 ? Direction.Right : Direction.Left;
            }

            // Вертикальное движение
            return delta.y > 0 ? Direction.Up : Direction.Down;
        }

        public void Dispose()
        {

        }
    }

    enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }
}