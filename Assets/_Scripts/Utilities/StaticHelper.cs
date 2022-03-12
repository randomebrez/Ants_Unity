using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Utilities
{
    public static class StaticHelper
    {
        private const float Delta = 0.00001f;
        public static void SetRecursiveLayer(List<GameObject> objects, int layerId)
        {
            if (objects.Count == 0)
                return;

            var newList = new List<GameObject>();

            foreach (var obj in objects)
            {
                obj.layer = layerId;

                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    newList.Add(obj.transform.GetChild(i).gameObject);
                }
            }

            SetRecursiveLayer(newList, layerId);
        }

        public static float ComputeExponentialProbability(float x, float offset, float argumentMultiplicator)
        {
            var expoArg = (x - offset) / (x - (1 + Delta));
            if (x < offset)
                expoArg = 0;
            var tempResult = Mathf.Exp(argumentMultiplicator * expoArg);
            if (tempResult > 1)
                return 0;
            return 1 - tempResult;
        }

        public static float ComputeNormalLaw(float x, float mu, float sigma)
        {
            var expoArg = ((x - mu) / sigma) * ((x - mu) / sigma);
            var constant = 1f / Mathf.Sqrt(2 * sigma * Mathf.PI);
            return constant * Mathf.Exp(-0.5f * expoArg);
        }
    }
}
