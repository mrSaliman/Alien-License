using App.Scripts.Components;
using DG.Tweening;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace App.Scripts.Systems
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public sealed class AnimationSystem : ISystem 
    {
        public World World { get; set;}

        private Filter _filter;
        private Stash<TransformComponent> _transformStash;
        private Stash<AnimatedComponent> _animatedStash;
        private Stash<LevelDataComponent> _levelStash;
        private Stash<MovableComponent> _movableStash;

        public void OnAwake()
        {
            _filter = World.Filter.With<TransformComponent>().With<MovableComponent>().Build();
            _animatedStash = World.GetStash<AnimatedComponent>();
            _transformStash = World.GetStash<TransformComponent>();
            _levelStash = World.GetStash<LevelDataComponent>();
            _movableStash = World.GetStash<MovableComponent>();
        }

        public void OnUpdate(float deltaTime) 
        {
            if (_levelStash.IsEmpty())
            {
                return;
            }
            ref var levelData = ref _levelStash.data[0];

            foreach (var entity in _filter)
            {
                ref var movableComponent = ref _movableStash.Get(entity);
                if (movableComponent.currentPosition == movableComponent.nextPosition) continue;

                ref var transformComponent = ref _transformStash.Get(entity);
                var nextWorldPosition = movableComponent.nextPosition * levelData.cellSize;
                
                if (_animatedStash.Has(entity))
                {
                    ref var animatedComponent = ref _animatedStash.Get(entity);
                    transformComponent.transform.DOMove(
                        new Vector3(nextWorldPosition.x, 0, nextWorldPosition.y),
                        animatedComponent.animationTime).OnComplete(() => SyncPosition(entity));
                }
                else
                {
                    transformComponent.transform.position = new Vector3(nextWorldPosition.x, 0,
                        nextWorldPosition.y);
                    SyncPosition(entity);
                }
            }
        }

        private void SyncPosition(Entity entity)
        {
            ref var movableComponent = ref _movableStash.Get(entity);
            movableComponent.currentPosition = movableComponent.nextPosition;
        }

        public void Dispose()
        {

        }
    }
}