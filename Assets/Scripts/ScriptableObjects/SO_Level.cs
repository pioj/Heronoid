using UnityEngine;

namespace mierdergames
{
    [CreateAssetMenu(fileName = "Level_XX-xx", menuName = "Heronoid/Levels/New Level...", order = 0)]
    public class SO_Level : ScriptableObject
    {
        [SerializeField] public BaseLevel level;
    }
}