using System;
using System.Collections.Generic;
using App.Scripts.Components;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace App.Scripts.Systems
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public sealed class MovementSystem : ISystem 
    {
        public World World { get; set;}

        private Filter _filter;
        private Stash<MovableComponent> _movableStash;
        private Stash<GridPositionComponent> _gridPosStash;
        private Stash<LevelDataComponent> _levelStash;
        private Dictionary<Direction, Vector2Int> _directionToVector;
        
        public void OnAwake()
        {
            InitializeDirectionMapping();
            
            _filter = World.Filter.With<MovableComponent>().Build();
            _movableStash = World.GetStash<MovableComponent>();
            _gridPosStash = World.GetStash<GridPositionComponent>();
            _levelStash = World.GetStash<LevelDataComponent>();
        }
        
        private void InitializeDirectionMapping()
        {
            _directionToVector = new Dictionary<Direction, Vector2Int>();

            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                var vector = dir switch
                {
                    Direction.Left => new Vector2Int(-1, 0),
                    Direction.Right => new Vector2Int(1, 0),
                    Direction.Up => new Vector2Int(0, 1),
                    Direction.Down => new Vector2Int(0, -1),
                    _ => new Vector2Int(0, 0)
                };

                _directionToVector[dir] = vector;
            }
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
                ref var gridPos = ref _gridPosStash.Get(entity);
                if (movableComponent.directionToMove == Direction.None) continue;
                if (gridPos.position != movableComponent.nextPosition) continue;
                var movementVector = _directionToVector[movableComponent.directionToMove];
                var currentPosition = gridPos.position + movableComponent.size * movementVector;
                while (!levelData.OccupationGrid[currentPosition.x, currentPosition.y])
                {
                    currentPosition += movementVector;
                }

                currentPosition -= movementVector * movableComponent.size;
                movableComponent.nextPosition = currentPosition;
                    
                FillOccupationGrid(gridPos.position, movableComponent.size, false, ref levelData);
                FillOccupationGrid(currentPosition, movableComponent.size, true, ref levelData);

                movableComponent.directionToMove = Direction.None;
            }
        }

        private static void FillOccupationGrid(Vector2Int start, Vector2Int size, bool value, ref LevelDataComponent levelData)
        {
            for (var i = start.x; i < start.x + size.x; i++)
            {
                for (var j = start.y; j < start.y + size.y; j++)
                {
                    levelData.OccupationGrid[i, j] = value;
                }
            }
        }

        public void Dispose()
        {

        }
    }
}