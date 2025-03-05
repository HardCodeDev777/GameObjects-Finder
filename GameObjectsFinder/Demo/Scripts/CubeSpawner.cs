// This script is for demonstration purposes only and is not part of the utility itself.
// You are free to use, modify, and distribute this code without restrictions.

using UnityEngine;
using HardCodeDev.Attributes;

namespace HardCodeDev.Examples
{
    public class CubeSpawner : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private GameObject baseCube, tagCube, scriptCube;
        private System.Collections.Generic.List<GameObject> spawnedCubes = new System.Collections.Generic.List<GameObject>();

        [FuncButton]
        public void SpawnRandomCubes()
        {

            if (spawnedCubes.Count > 0)
            {
                foreach (var cube in spawnedCubes)
                {
                    try
                    {
                        DestroyImmediate(cube);
                    }
                    catch
                    {
                        Debug.LogError("Error destroying cubes!");
                    }
                }
                spawnedCubes.Clear();
            }

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                int cubesIndex = Random.Range(0, 3);
                switch (cubesIndex)
                {
                    case 0:
                        var spawnedCube = Instantiate(baseCube, spawnPoints[i].position, Quaternion.identity);
                        spawnedCubes.Add(spawnedCube);
                        break;
                    case 1:
                        var spawnedCube2 = Instantiate(tagCube, spawnPoints[i].position, Quaternion.identity);
                        spawnedCubes.Add(spawnedCube2);
                        break;
                    case 2:
                        var spawnedCube3 = Instantiate(scriptCube, spawnPoints[i].position, Quaternion.identity);
                        spawnedCubes.Add(spawnedCube3);
                        break;
                }
            }

            Debug.Log("Cubes spawned.");
        }

        [FuncButton]
        public void DeleteAllRandomCubes()
        {
            if (spawnedCubes.Count > 0)
            {
                foreach (var cube in spawnedCubes)
                {
                    try
                    {
                        DestroyImmediate(cube);
                    }
                    catch
                    {
                        Debug.LogError("Error destroying cubes!");
                    }
                }
                spawnedCubes.Clear();
            }
        }
    }
}