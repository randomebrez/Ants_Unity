using Assets.Dtos;
using System;
using UnityEngine;
using NeuralNetwork.Interfaces.Model;
using NeuralNetwork.Managers;

namespace mew
{
    public abstract class BaseAnt : UnitBase
    {
        public Action<BaseAnt> Clicked;

        private bool _initialyzed = false;

        protected Block _currentPos;
        protected Block _nextPos;

        public BrainManager BrainManager { get; private set; }

        private Transform _body;
        private Transform _head;
        protected Transform _nest;
        protected AntScannerManager _scannerManager;

        private Transform _pheromoneContainer;

        protected int _dropPheroFrequency = 10;
        protected float _dropPheroInterval;
        protected float _dropPheroTimer;

        protected bool HasTarget => _target.Active && _target.Transform != null;
        protected Target _target = new Target();
        protected float _targetTreshold = 0.3f;

        public string[] PortionInfos => _scannerManager.PortionInfos;
        public string NestName => _nest.name;

        private void Awake()
        {
            _body = transform.GetChild(0);
            _head = transform.GetChild(1);

            _scannerManager = _head.GetComponentInChildren<AntScannerManager>();
            
            _dropPheroInterval = 1.0f / _dropPheroFrequency;
        }

        void Start()
        {
            _nest = transform.parent.parent;
            _currentPos = EnvironmentManager.Instance.BlockFromWorldPoint(transform.position);
        }

        void Update()
        {
            if (_initialyzed == false)
                return;

            Move();
            DropPheromone();
            CheckCollectableCollisions();
        }

        public override void Initialyze(ScriptableUnitBase.Stats stats, Brain brain)
        {
            base.Initialyze(stats, brain);
            BrainManager = new BrainManager(brain);            
            _scannerManager.InitialyzeScanners();
            _pheromoneContainer = EnvironmentManager.Instance.GetPheromoneContainer();
            _initialyzed = true;
        }

        /* Get axis from body to head.
         In order to have that vector following the ant, one need to add ant position (so that position will always be the origin of the vector)*/
        public Vector3 BodyHeadAxis => (_head.position - _body.position).normalized;

        public float PhysicalLength => Vector3.Distance(_body.position, _head.position);

        private void OnMouseDown()
        {
            Clicked?.Invoke(this);
            Debug.Log($"Mouse was clicked on {name}");
        }

        // Abstract Methods
        public abstract void CheckCollectableCollisions();

        public abstract float GetUnitScore();

        protected abstract ScriptablePheromoneBase.PheromoneTypeEnum GetPheroType();

        // Virtual Methods

        /*Update _nextPos in inherited class*/
        public virtual void Move()
        {
            //if (_nextPos == null)
            //    Debug.Log($"{name} : NextPos was null");
            //
            //if (_currentPos == null)
            //    Debug.Log($"{name} : CurrentPos was null");

            var rotation = Vector3.SignedAngle(BodyHeadAxis, _nextPos.WorldPosition - _currentPos.WorldPosition, Vector3.up);
            _currentPos = _nextPos;

            transform.SetPositionAndRotation(_currentPos.WorldPosition + 0.5f * Vector3.up, Quaternion.Euler(0, transform.rotation.eulerAngles.y + rotation, 0));
        }

        internal virtual void DropPheromone()
        {
            _dropPheroTimer -= Time.deltaTime;
            if (_dropPheroTimer > 0)
                return;

            _dropPheroTimer += _dropPheroInterval;

            // Spawn pheromone
            var scriptablePheromone = ResourceSystem.Instance.PheromoneOfTypeGet(GetPheroType());
            var pheromone = Instantiate(scriptablePheromone.PheromonePrefab, _currentPos.WorldPosition, Quaternion.identity, _pheromoneContainer);
            pheromone.SetCaracteristics(scriptablePheromone.BaseCaracteristics);
        }
    }
}
