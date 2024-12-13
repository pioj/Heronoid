using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace mierdergames
{
    [CreateAssetMenu(fileName = "LevelPack_XXX", menuName = "Heronoid/Levels/New Pack of Levels...", order = 2)]
    [Serializable]
    public class SO_LevelPack : ScriptableObject
    {
       [Header("NIVELES")]
       [SerializeField] public List<SO_Level> niveles;
       [SerializeField] public List<GameObject> themePrefabs;
       
       [Header("BOSS")]
       [SerializeField] public GameObject prefabBoss;
       [SerializeField] public List<SO_BossLevel> RandomBossLevels;
    }
}