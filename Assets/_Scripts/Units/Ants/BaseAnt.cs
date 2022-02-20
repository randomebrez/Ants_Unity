using System.Collections.Generic;
using UnityEngine;

namespace mew
{
    public abstract class BaseAnt : UnitBase
    {
        protected Vector3 _position;
        protected Vector3 _velocity;
        protected Vector3 _desiredDirection;

        private Transform _body;
        private Transform _head;
        protected AntVision _visionField;

        protected bool HasTarget => _target != null;
        protected Transform _target = null;
        protected float _targetTreshold = 0.3f;

        public float SteerStrength = 3;
        private float _currentSteerStrength = 0f;
        public float WanderStrength = 1;

        public float expoMultiplicator = 100;
        public float steeringForceConstant = 10;

        void Start()
        {
            _position = transform.position;
            _position.y = 1.25f;
            _body = transform.GetChild(0);
            _head = transform.GetChild(1);

            _visionField = _head.GetComponent<AntVision>();

            _desiredDirection = BodyHeadAxis.normalized;
            _currentSteerStrength = SteerStrength;
        }

        void Update()
        {
            if (TargetDistance() < Stats.TargetDestroyTreshold)
                DestroyTarget();

            Move();
        }

        // Get axis from body to head.
        // In order to have that vector following the ant, one need to add ant position (so that position will always be the origin of the vector)
        public Vector3 BodyHeadAxis => (_head.position - _body.position).normalized;


        // Set _desiredDirection in non abstract classes
        // Once _desiredDirection is set ant movement always end like that
        public virtual void Move()
        {
            _currentSteerStrength = GetSteeringForce();
            var desiredVelocity = _desiredDirection * Stats.MaxSpeed;
            var desiredSteeringForce = (desiredVelocity - _velocity) * _currentSteerStrength;

            // get acceleration
            var acceleration = Vector3.ClampMagnitude(desiredSteeringForce, SteerStrength);

            // calculate new velocity
            var newVelocity = Vector3.ClampMagnitude(_velocity + acceleration * Time.deltaTime, Stats.MaxSpeed);

            // calculate new position
            var newPos = _position + newVelocity * Time.deltaTime;

            // if ant ends in a obstacle, just rotate ant
            switch (_visionField.IsMoveValid(_position, newPos))
            {
                case true:
                    _position = newPos;
                    _velocity = newVelocity;
                    break;
                case false:
                    _velocity = Vector3.zero;
                    break;
            }

            // calculate the rotation to go in the new direction
            var angle = Vector3.SignedAngle(BodyHeadAxis, newVelocity, Vector3.up);


            // Apply it to the ant game object
            transform.SetPositionAndRotation(_position, Quaternion.Euler(0, transform.rotation.eulerAngles.y + angle, 0));
        }

        protected Vector3 GetRandomDirection()
        {
            var openingPositions = new List<float>();
            var closingPositions = new List<float>();
            var distances = new List<float>();

            if (_visionField.Obstacles.Count > 0)
                //Debug.Log("Obstacles");

            foreach(var obstacle in _visionField.Obstacles)
            {
                var dist = Vector3.Distance(_head.position, obstacle.transform.position);
                var anglePortion = 2 * Stats.VisionAngle / dist + 0.1f;
                var obstacleAngle = transform.rotation.eulerAngles.y + Vector3.SignedAngle(BodyHeadAxis, obstacle.transform.position, Vector3.up);
                openingPositions.Add(obstacleAngle - anglePortion / 2);
                closingPositions.Add(obstacleAngle + anglePortion / 2);
                distances.Add(dist);
            }

            var (inhibitedPortions, probaSum) = GetInhibitedDirections(openingPositions, closingPositions, distances);

            var randomNumber = Random.value;
            var foundZone = false;
            var sum = 0f;
            var count = 0;
            while(foundZone == false && count < inhibitedPortions.Count)
            {
                sum += inhibitedPortions[count].probability / probaSum;
                if (randomNumber < sum)
                    foundZone = true;

                count++;
            }
            
            var randomAngle = (inhibitedPortions[count - 1].startingPoint + Random.value * inhibitedPortions[count - 1].portion);
            var randomNorm = Random.value;

            return Quaternion.Euler(0, transform.rotation.eulerAngles.y + randomAngle, 0) * BodyHeadAxis * randomNorm;
        }

        public virtual void DestroyTarget()
        {
            if (HasTarget)
                Destroy(_target.gameObject);
        }

        public float TargetDistance()
        {
            if (!HasTarget)
                return float.MaxValue;

            var distance = _target.position - _position;
            return Mathf.Sqrt(distance.x * distance.x + distance.y * distance.y + distance.z * distance.z);
        }

        private (List<(float startingPoint, float portion, float probability)> portions, float probaSum) GetInhibitedDirections(List<float> openingPositions, List<float> closingPositions, List<float> distances)
        {
            if (openingPositions.Count == 0)
                return (new List<(float ,float, float)> { (-178, 356, 1) }, 1);

            openingPositions.Sort();
            closingPositions.Sort();

            var results = new List<(float startingPoint, float portion, float distance)>();

            var openingIndex = 0;
            int? currentOpenerIndex = null;
            var closingIndex = 0;

            var distSum = 0f;

            // Start circle from angle -180 to 180;
            // Portions have reduced probability if between an open position and a close position
            // Portions have unchanged probability if between a close position and an open position
            //      and from minimum angle to first open position, and from last close position to maximum angle


            //first portion, unchanged probability
            var firstProba = (openingPositions[0] + 178) / 356;
            results.Add((-178, openingPositions[0] + 178, firstProba));

            // to ensure to have a proba law in the end
            var probaTotal = firstProba;


            while (openingIndex + closingIndex < openingPositions.Count + closingPositions.Count)
            {
                if (currentOpenerIndex.HasValue == false)
                {
                    currentOpenerIndex = openingIndex;
                    openingIndex++;
                    continue;
                }

                if (openingIndex == openingPositions.Count)
                {
                    // for the last portion 
                    // --> sum all remaining distances
                    // --> add an inhibited portion
                    // --> Add last unchanged portion

                    for(int j = closingIndex; j < closingPositions.Count; j++)
                    {
                        distSum += distances[j];
                    }
                    closingIndex = closingPositions.Count;

                    var inhibitedPortion = closingPositions[closingIndex - 1] - openingPositions[currentOpenerIndex.Value];
                    var followingUnchangedPortion = 178 - closingPositions[closingPositions.Count - 1];

                    var baseInhibitedProbability = inhibitedPortion / 356;
                    var baseFollowingProbability = followingUnchangedPortion / 356;

                    var distMean = distSum / (closingPositions.Count - currentOpenerIndex.Value);
                    //Linear
                    //var inhibitedProbability = baseInhibitedProbability * distMean / Stats.VisionRadius;

                    //Exponential
                    var inhibitedProbability = baseInhibitedProbability * (1 - Mathf.Exp(expoMultiplicator * distMean / (distMean - Stats.VisionRadius - 0.1f)));

                    results.Add((openingPositions[currentOpenerIndex.Value], inhibitedPortion, inhibitedProbability));
                    results.Add((closingPositions[closingPositions.Count - 1], followingUnchangedPortion, baseFollowingProbability));
                    var probaToAdd = baseFollowingProbability + inhibitedProbability;
                    probaTotal += baseFollowingProbability + inhibitedProbability;
                    continue;
                }

                if (openingIndex != closingIndex)
                {
                    if (openingPositions[openingIndex] < closingPositions[closingIndex])
                        openingIndex++;
                    else
                    {
                        distSum += distances[closingIndex]/ Stats.VisionRadius;
                        closingIndex++;
                    }
                }
                else
                {
                    // When we close a portion of changed probability
                    // --> Add it with below 1 probability multiplicator
                    //--> Add a portion of unchanged probability from last close to next open
                    var inhibitedPortion = closingPositions[closingIndex] - openingPositions[currentOpenerIndex.Value];
                    var followingUnchangedPortion = openingPositions[openingIndex] - closingPositions[closingIndex];

                    var baseInhibitedProbability = inhibitedPortion / 356;
                    var baseFollowingProbability = followingUnchangedPortion / 356;

                    var distMean = distSum / (closingIndex - currentOpenerIndex.Value + 1);
                    // Linear
                    //var inhibitedProbability = baseInhibitedProbability * distMean / Stats.VisionRadius;

                    //Exponential
                    var inhibitedProbability = baseInhibitedProbability * (1 - Mathf.Exp(expoMultiplicator * distMean / (distMean - Stats.VisionRadius - 0.1f)));

                    results.Add((openingPositions[currentOpenerIndex.Value], inhibitedPortion, inhibitedProbability));
                    results.Add((closingPositions[closingIndex], followingUnchangedPortion, baseFollowingProbability));

                    probaTotal += baseFollowingProbability + inhibitedProbability;

                    currentOpenerIndex = null;

                    distSum = 0;
                }
            }

            return (results, probaTotal);
        }

        private float GetSteeringForce()
        {
            var obstaclesNumber = _visionField.Obstacles.Count;

            if (obstaclesNumber == 0)
                return SteerStrength;

            var normalizedDistance = GetNormalizedDistance();

            var expoArgument = 1 - 1 / normalizedDistance;
            var expo = 1 - Mathf.Exp(expoArgument);
            var SteeringForceMultiplicator = (steeringForceConstant - 1) * expo + 1;
            return SteeringForceMultiplicator * SteerStrength;
        }

        private float GetNormalizedDistance()
        {
            var result = 0f;
            var obstacles = _visionField.Obstacles;
            foreach(var obstacle in obstacles)
            {
                result += Vector3.Distance(_head.position, obstacle.transform.position) / Stats.VisionRadius;
            }
            var temp = result / obstacles.Count;
            return temp > 1 ? 1 : temp;
        }
    }
}
