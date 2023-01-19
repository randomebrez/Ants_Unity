using System.Linq;
using UnityEngine;

namespace Assets._Scripts.Utilities
{
    public static class StaticCompute
    {
        private const float Delta = 0.00001f;

        public static float ComputeNormalLaw(float x, float mu, float sigma)
        {
            var temp = (x - mu) / sigma;
            var expoArg = temp * temp;
            var constant = 1f / Mathf.Sqrt(2 * sigma * Mathf.PI);
            return constant * Mathf.Exp(-0.5f * expoArg);
        }

        public static float ComputeExponentialProbability(float x, float offset, float argumentMultiplicator)
        {
            var expoArg = (x - offset) / (x - (1 + Delta));
            if (x < offset)
                expoArg = 0;
            var tempResult = Mathf.Exp(argumentMultiplicator * expoArg);
            if (tempResult > 1)
                tempResult = 1;
            return 1 - tempResult;
        }

        public static float ComputeTanh(float x)
        {
            var pos = Mathf.Exp(x);
            var neg = Mathf.Exp(-x);
            return (pos - neg) / (pos + neg);
        }

        public static float ComputeSigmoid(float x, float param)
        {
            var denominator = 1 + Mathf.Exp(-param * x);
            return 1 / denominator;
        }

        public static (float, float[]) GetPortionProbabilities(float[] bonuses, float[] maluses, float baseMu = 0, float baseSigma = 270, float totalAngle = 360)
        {
            var portionNumber = bonuses.Count();
            var probabilities = new float[portionNumber];
            var deltaTheta = totalAngle / portionNumber;

            for (int i = 0; i < portionNumber; i++)
            {
                var currentAngle = (-totalAngle / 2f) + (i + 0.5f) * deltaTheta;
                var baseProbability = ComputeNormalLaw(currentAngle, baseMu, baseSigma);
                var probability = baseProbability * (1 + bonuses[i] - maluses[i]);
                probabilities[i] = probability;
            }

            return NormalizeSum(probabilities);
        }

        public static (float, float[]) NormalizeSum(float[] numbers)
        {
            var sum = 0f;
            for (int i = 0; i < numbers.Count(); i++)
                sum += numbers[i];

            var sumRes = 0f;
            for (int i = 0; i < numbers.Count(); i++)
                numbers[i] = numbers[i] / sum;

            return (sumRes, numbers);
        }
    }
}
