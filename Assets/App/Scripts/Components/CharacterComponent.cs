using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;

namespace App.Scripts.Components
{
    [System.Serializable]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct CharacterComponent : IComponent
    {
        public bool sleepState;
        public int currentFurnitureId;
    }
    
}