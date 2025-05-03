using System.Collections.Generic;
using App.Scripts.Components;
using Sirenix.OdinInspector;
using UnityEngine;

namespace App.Scripts.GameField
{
    [CreateAssetMenu(fileName = "Level_01", menuName = "Game/Level Data")]
    public class LevelDataSO : ScriptableObject
    {
        [BoxGroup("Grid Settings")]
        [MinValue(1), MaxValue(10)]
        public Vector2Int gridSize = new Vector2Int(5, 5);

        [BoxGroup("Game Settings")]
        [MinValue(1)]
        public int maxMoves = 10;

        [BoxGroup("Objects"), ListDrawerSettings(ShowFoldout = true)]
        public List<MovableObjectData> movableObjects = new List<MovableObjectData>();

        [BoxGroup("Character")]
        public int characterObjectId = -1;

        [BoxGroup("Exits"), ListDrawerSettings(ShowFoldout = true)]
        public List<Vector2Int> exitPositions = new List<Vector2Int>();

        private void Validate()
        {
            if (gridSize.x <= 0 || gridSize.y <= 0)
            {
                Debug.LogError("Grid size must be at least 1x1.");
                return;
            }

            var tempGrid = new int[gridSize.x][];
            for (var index = 0; index < gridSize.x; index++)
            {
                tempGrid[index] = new int[gridSize.y];
            }

            foreach (var obj in movableObjects)
            {
                if (obj.size.x <= 0 || obj.size.y <= 0)
                {
                    Debug.LogWarning($"Object {obj.type} has invalid size ({obj.size.x}, {obj.size.y}).");
                    continue;
                }

                for (var x = obj.position.x; x < obj.position.x + obj.size.x; x++)
                {
                    for (var y = obj.position.y; y < obj.position.y + obj.size.y; y++)
                    {
                        if (x < 0 || x >= gridSize.x || y < 0 || y >= gridSize.y)
                        {
                            Debug.LogError($"Object {obj.type} at ({x}, {y}) is out of bounds.");
                            continue;
                        }

                        if (tempGrid[x][y] > 0)
                        {
                            Debug.LogWarning($"Overlap detected at ({x}, {y}) between objects.");
                        }
                        else
                        {
                            tempGrid[x][y] = 1;
                        }
                    }
                }
            }
        }

        [Button("Validate Level", ButtonSizes.Large)]
        public void RunValidation()
        {
            Validate();
        }
    }
}