using System.Collections.Generic;
using Scellecs.Morpeh;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace App.Scripts
{
    public class Startup : SerializedMonoBehaviour {
        private World _world;

        [OdinSerialize] private List<IInitializer> initializers = new();
        [OdinSerialize] private List<ISystem> systems = new();
    
        private void Start() {
            _world = World.Default;
        
            var systemsGroup = _world.CreateSystemsGroup();
            foreach (var initializer in initializers)
            {
                systemsGroup.AddInitializer(initializer);
            }
            
            foreach (var system in systems)
            {
                systemsGroup.AddSystem(system);
            }
            
            _world.AddSystemsGroup(order: 0, systemsGroup);
        }
    }
}