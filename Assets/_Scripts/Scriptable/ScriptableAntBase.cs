using UnityEngine;

namespace mew
{
    [CreateAssetMenu(menuName = "BasicAnt")]
    public class ScriptableAntBase : ScriptableUnitBase
    {
        public AntTypeEnum AntType;

        public BaseAnt AntPrefab;

        public enum AntTypeEnum
        {
            Worker = 0,
            BodyGuard = 1
        }
    }
}
