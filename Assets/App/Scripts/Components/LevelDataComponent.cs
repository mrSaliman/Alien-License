using System;
using System.Collections.Generic;
using App.Scripts.GameField;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace App.Scripts.Components
{
    [Serializable]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct LevelDataComponent : IComponent
    {
        public Vector2Int gridSize;
        public int maxMoves;
        public List<MovableObjectData> movableObjects;
        public int characterObjectId;
        public List<Vector2Int> exitPositions;
        public bool[,] OccupationGrid;
        public Vector2 cellSize;
    }

    [Serializable]
    public struct MovableObjectData
    {
        public Vector2Int position;
        public Vector2Int size;
        public MovementDirection direction;
        public ObjectType type;
    }

    public enum ObjectType
    {
        Sofa,
        Bed,
        Desk,
        Aquarium,
        SmartSpeaker,
        Obstacle
    }
}