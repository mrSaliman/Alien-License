using System.Collections.Generic;
using App.Scripts.Components;
using App.Scripts.GameField;
using Scellecs.Morpeh;
using Sirenix.Serialization;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace App.Scripts.Systems
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public sealed class LevelGenerationSystem : IInitializer 
    {
        public World World { get; set;}
        
        [OdinSerialize] private LevelDataSO _currentLevelSo;
        
        private Stash<MovableComponent> _movableStash;
        private Stash<LevelDataComponent> _levelStash;
        private Stash<WakeUpTriggerComponent> _wakeUpStash;
        private Stash<DisappearingObjectComponent> _disappearingObjectStash;
        private Stash<CharacterComponent> _characterStash;
        private Stash<GridPositionComponent> _gridPosStash;
        private Stash<AnimatedComponent> _animatedStash;
        private Stash<ExitZoneComponent> _exitZoneStash;
        private Stash<MoveCounterComponent> _moveCounterStash;

        public void OnAwake()
        {
            _levelStash = World.GetStash<LevelDataComponent>();
            _movableStash = World.GetStash<MovableComponent>();
            _wakeUpStash = World.GetStash<WakeUpTriggerComponent>();
            _disappearingObjectStash = World.GetStash<DisappearingObjectComponent>();
            _characterStash = World.GetStash<CharacterComponent>();
            _gridPosStash = World.GetStash<GridPositionComponent>();
            _animatedStash = World.GetStash<AnimatedComponent>();
            _exitZoneStash = World.GetStash<ExitZoneComponent>();
            _moveCounterStash = World.GetStash<MoveCounterComponent>();

            if (_levelStash.IsNotEmpty())
                _levelStash.RemoveAll();
            
            var levelEntity = World.CreateEntity();
            ref var levelData = ref _levelStash.Add(levelEntity);
            levelData.gridSize = _currentLevelSo.gridSize;
            levelData.maxMoves = _currentLevelSo.maxMoves;
            levelData.characterObjectId = _currentLevelSo.characterObjectId;
            levelData.exitPositions = new List<Vector2Int>(_currentLevelSo.exitPositions);
            levelData.movableObjects = new List<MovableObjectData>(_currentLevelSo.movableObjects);
            levelData.OccupationGrid = new bool[levelData.gridSize.x, levelData.gridSize.y];

            GenerateMovableObjects(ref levelData);

            GenerateExitZones(ref levelData);

            InitializeMoveCounter(ref levelData);
        }

        private void GenerateMovableObjects(ref LevelDataComponent levelData)
        {
            const float animationTime = 0.5f;
            for (var i = 0; i < levelData.movableObjects.Count; i++) 
            {
                var objData = levelData.movableObjects[i];

                var entity = World.CreateEntity();

                ref var movable = ref _movableStash.Add(entity);
                movable.size = objData.size;
                movable.direction = objData.direction;
                movable.directionToMove = Direction.None;
                movable.nextPosition = objData.position;
            
                ref var gridPos = ref _gridPosStash.Get(entity);
                gridPos.position = Vector2Int.zero;
            
                _disappearingObjectStash.Add(entity);
                ref var anim = ref _animatedStash.Add(entity);
                anim.animationTime = animationTime;

                if (objData.type is ObjectType.Aquarium or ObjectType.SmartSpeaker) _wakeUpStash.Add(entity);

                MarkOccupied(ref levelData, objData);

                if (i == levelData.characterObjectId)
                {
                    _characterStash.Add(entity);
                }
            }
        }

        private static void MarkOccupied(ref LevelDataComponent levelData, MovableObjectData objData)
        {
            for (var x = 0; x < objData.size.x; x++) {
                for (var y = 0; y < objData.size.y; y++) {
                    var cellX = objData.position.x + x;
                    var cellY = objData.position.y + y;
                    if (cellX < levelData.gridSize.x && cellY < levelData.gridSize.y) {
                        levelData.OccupationGrid[cellX, cellY] = true;
                    }
                }
            }
        }

        private void GenerateExitZones(ref LevelDataComponent levelData) {
            foreach (var exitPos in levelData.exitPositions) {
                var exitEntity = World.CreateEntity();

                ref var gridPosition = ref _gridPosStash.Add(exitEntity);
                gridPosition.position = exitPos;

                _exitZoneStash.Add(exitEntity);
            }
        }

        private void InitializeMoveCounter(ref LevelDataComponent levelData) {
            var moveCounterEntity = World.CreateEntity();
            ref var moveCounter = ref _moveCounterStash.Add(moveCounterEntity);
            moveCounter.remainingMoves = levelData.maxMoves;
        }

        public void Dispose()
        {

        }
    }
}