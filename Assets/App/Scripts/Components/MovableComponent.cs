using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace App.Scripts.Components
{
    [System.Serializable]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct MovableComponent : IComponent
    {
        public Vector2Int size;
        public MovementDirection direction;
        public bool isBlocked;   
    }
    
    public enum MovementDirection {
        Horizontal,   
        Vertical,     
        Both        
    }
}