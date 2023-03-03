using Assets.Dtos;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace mew
{
    public abstract class BaseAnt : UnitBase
    {
        public Action<BaseAnt> Clicked;

        private bool _initialyzed = false;

        protected Block _currentPos;
        protected Block _nextPos;

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

        public string[] PortionInfos => _scannerManager.PortionInfos;

        public float steeringForceConstant = 10;

        private void Awake()
        {
            _body = transform.GetChild(0);
            _head = transform.GetChild(1);

            _scannerManager = _head.GetComponentInChildren<AntScannerManager>();
            
            _dropPheroInterval = 1.0f / _dropPheroFrequency;
        }

        void Start()
        {
            _nest = transform.parent;
            _currentPos = EnvironmentManager.Instance.BlockFromWorldPoint(transform.position);

            PheromoneContainer = transform.parent.parent.parent.GetChild(1);
        }

        void Update()
        {
            if (_initialyzed == false)
                return;

            Move();
            DropPheromone();
            CheckCollectableCollisions();
        }

        public override void Initialyze(ScriptableUnitBase.Stats stats)
        {
            base.Initialyze(stats);

            _scannerManager.InitialyzeScanners();

            _initialyzed = true;
        }

        /* Get axis from body to head.
         In order to have that vector following the ant, one need to add ant position (so that position will always be the origin of the vector)*/
        public Vector3 BodyHeadAxis => (_head.position - _body.position).normalized;

        public float PhysicalLength => Vector3.Distance(_body.position, _head.position);

        // Update _nextPos in inherited class
        public virtual void Move()
        {
            _currentPos = _nextPos;
            transform.SetPositionAndRotation(_currentPos.WorldPosition + 0.5f * Vector3.up, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0));
        }

        public abstract void CheckCollectableCollisions();

        internal virtual void DropPheromone()
        {
            _dropPheroTimer -= Time.deltaTime;
            if (_dropPheroTimer > 0)
                return;

            _dropPheroTimer += _dropPheroInterval;

            // Spawn pheromone
            var scriptablePheromone = ResourceSystem.Instance.PheromoneOfTypeGet(GetPheroType());
            var pheromone = Instantiate(scriptablePheromone.PheromonePrefab, _currentPos.WorldPosition, Quaternion.identity, PheromoneContainer);
            pheromone.SetCaracteristics(scriptablePheromone.BaseCaracteristics);
        }

        private void OnMouseDown()
        {
            Clicked?.Invoke(this);
            Debug.Log($"Mouse was clicked on {name}");
        }

        protected abstract ScriptablePheromoneBase.PheromoneTypeEnum GetPheroType();
    }
}
