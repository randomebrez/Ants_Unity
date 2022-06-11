using Assets.Dtos;
using UnityEngine;

namespace mew
{
    public abstract class BaseAnt : UnitBase
    {
        private const int ScannerSubdivision = 20;

        private bool _initialyzed = false;

        protected Vector3 _position;
        protected Vector3 _velocity;
        protected Vector3 _desiredDirection;

        private Transform _body;
        private Transform _head;
        protected Transform _nest;
        protected AntScannerManager _scannerManager;

        private Transform PheromoneContainer;

        protected int _dropPheroFrequency = 10;
        protected float _dropPheroInterval;
        protected float _dropPheroTimer;

        protected bool HasTarget => _target.Active && _target.Transform != null;
        protected Target _target = new Target();
        protected float _targetTreshold = 0.3f;

        public float[] Probabilities = new float[ScannerSubdivision];

        public float steeringForceConstant = 10;

        private void Awake()
        {
            _position = transform.position;
            _position.y = 1.25f;
            _body = transform.GetChild(0);
            _head = transform.GetChild(1);

            _scannerManager = _head.GetComponentInChildren<AntScannerManager>();
            

            _dropPheroInterval = 1.0f / _dropPheroFrequency;
        }

        void Start()
        {
            _nest = transform.parent;
            _desiredDirection = BodyHeadAxis.normalized;

            PheromoneContainer = transform.parent.parent.parent.GetChild(1);
        }

        void Update()
        {
            if (_initialyzed == false)
                return;

            Move();
            DropPheromone();

            if (HasTarget == false)
                return;

            if (TargetDistance() < Stats.TargetDestroyTreshold)
                OnTargetReach();
        }

        public override void Initialyze(ScriptableUnitBase.Stats stats)
        {
            base.Initialyze(stats);

            _scannerManager.InitialyzeScanners(ScannerSubdivision);
            _scannerManager.CreateMesh();

            _initialyzed = true;
        }

        /* Get axis from body to head.
         In order to have that vector following the ant, one need to add ant position (so that position will always be the origin of the vector)*/
        public Vector3 BodyHeadAxis => (_head.position - _body.position).normalized;

        public float PhysicalLength => Vector3.Distance(_body.position, _head.position);


        /* Set _desiredDirection in non abstract classes
         Once _desiredDirection is set ant movement always end like that*/
        public virtual void Move()
        {
            var desiredVelocity = _desiredDirection * Stats.MaxSpeed;
            var desiredSteeringForce = (desiredVelocity - _velocity) * Stats.SteerStrength;

            // get acceleration
            var acceleration = Vector3.ClampMagnitude(desiredSteeringForce, Stats.SteerStrength);

            var isHeadingForCollision = _scannerManager.IsHeadingForCollision();
            if (isHeadingForCollision.isIt)
            {
                var avoidForce = (isHeadingForCollision.avoidDir.normalized * Stats.MaxSpeed - _velocity) * Stats.SteerStrength;
                avoidForce.y = 0;
                acceleration += Vector3.ClampMagnitude(avoidForce, Stats.SteerStrength) * Stats.AvoidCollisionForce;
            }

            // calculate new velocity
            var newVelocity = Vector3.ClampMagnitude(_velocity + acceleration * Time.deltaTime, Stats.MaxSpeed);

            // calculate new position
            var newPos = _position + newVelocity * Time.deltaTime;

            // if ant ends in a obstacle, just rotate ant
            switch (_scannerManager.IsMoveValid(_position, newPos))
            {
                case true:
                    _position = newPos;
                    _velocity = newVelocity;
                    break;
                case false:
                    _velocity = Vector3.zero;
                    break;
            }

            var rotation = Vector3.SignedAngle(BodyHeadAxis, newVelocity, Vector3.up);

            // Apply it to the ant game object
            transform.SetPositionAndRotation(_position, Quaternion.Euler(0, transform.rotation.eulerAngles.y + rotation, 0));
        }

        protected Vector3 GetRandomDirection(bool carryFood)
        {
            _scannerManager.CarryingFood = carryFood;
            var inhibitedProbabilties = _scannerManager.ProbabilitiesGet;
            Probabilities = inhibitedProbabilties;

            var randomNumber = Random.value;
            var foundZone = false;
            var sum = 0f;
            var count = 0;

            while(foundZone == false && count < inhibitedProbabilties.Length)
            {
                sum += inhibitedProbabilties[count];
                if (randomNumber < sum)
                    foundZone = true;

                count++;
            }

            var deltaAngle = 360f / ScannerSubdivision;
            var startingAngle = -180f + (count - 1) * deltaAngle;

            var randomAngle = startingAngle + Random.value * deltaAngle;
            var randomNorm = Random.value;
            return Quaternion.Euler(0, transform.rotation.eulerAngles.y + randomAngle, 0) * BodyHeadAxis * randomNorm;
        }

        public abstract void OnTargetReach();

        public float TargetDistance()
        {
            if (!HasTarget)
                return float.MaxValue;

            var distance = _target.Transform.position - _position;
            return Mathf.Sqrt(distance.x * distance.x + distance.y * distance.y + distance.z * distance.z);
        }

        internal virtual void DropPheromone()
        {
            _dropPheroTimer -= Time.deltaTime;
            if (_dropPheroTimer > 0)
                return;
            
            _dropPheroTimer += _dropPheroInterval;

            // Spawn pheromone
            var scriptablePheromone = ResourceSystem.Instance.PheromoneOfTypeGet(GetPheroType());
            var pheromone = Instantiate(scriptablePheromone.PheromonePrefab, _body.position, Quaternion.identity, PheromoneContainer);
            pheromone.SetCaracteristics(scriptablePheromone.BaseCaracteristics);
        }

        protected abstract ScriptablePheromoneBase.PheromoneTypeEnum GetPheroType();
    }
}
