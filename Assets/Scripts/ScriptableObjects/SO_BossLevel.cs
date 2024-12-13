using UnityEngine;

namespace mierdergames
{
    [CreateAssetMenu(fileName = "BossLevel_XX-xx", menuName = "Heronoid/Levels/New BossLevel...", order = 1)]
    public class SO_BossLevel : ScriptableObject
    {
        [SerializeField] public BossLevel level;
    }
}