using Assets.Dtos;
using System;
using UnityEngine;
using NeuralNetwork.Interfaces.Model;
using NeuralNetwork.Managers;
using Assets._Scripts.Utilities;

namespace mew
{
    public abstract class BaseAnt : UnitBase
    {
        // Events
        public Action<BaseAnt> Clicked;

        // Positions
        protected Block _currentPos;
        protected Block _nextPos;

        //Managers
        public BrainManager BrainManager { get; private set; }
        protected AntScannerManager _scannerManager;
        protected PheromoneManager _pheromoneManager;
        protected Transform _pheromoneContainer;

        // Private fields
        private bool _initialyzed = false;
        private Transform _body;
        private Transform _head;
        private Transform _nest;

        // Pheromones parameters
        protected int _dropPheroFrequency = 10;
        protected float _dropPheroInterval;
        protected float _dropPheroTimer;

        // Public methods
        public Vector3 BodyHeadAxis => (_head.position - _body.position).normalized;
        public float PhysicalLength => Vector3.Distance(_body.position, _head.position);
        public string[] PortionInfos => _scannerManager.PortionInfos;
        public string NestName => _nest.name;


        // Abstract Methods
        public abstract void CheckCollectableCollisions();

        public abstract float GetUnitScore();

        protected abstract ScriptablePheromoneBase.PheromoneTypeEnum GetPheroType();


        // Override methods
        public void Initialyze(ScriptableUnitBase.Stats stats, Brain brain, Transform pheromoneContainer)
        {
            Stats = stats;
            BrainManager = new BrainManager(brain);
            _scannerManager.InitialyzeScanners();
            _pheromoneContainer = pheromoneContainer;
            _initialyzed = true;
        }


        // Unity methods
        private void Awake()
        {
            _body = transform.GetChild(0);
            _head = transform.GetChild(1);

            _scannerManager = GetComponentInChildren<AntScannerManager>();
            _dropPheroInterval = 1.0f / _dropPheroFrequency;
        }

        void Start()
        {
            _nest = transform.parent.parent.parent;
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


        // Virtual Methods
        public virtual void Move()
        {
            // _nextPos must be updated in inherited classes
            if (_nextPos == null)
            {
                Debug.Log("Next pos null");
                return;
            }

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
            var newPheromone = Instantiate(scriptablePheromone.PheromonePrefab, _currentPos.WorldPosition + Vector3.up * scriptablePheromone.PheromonePrefab.transform.localScale.y, Quaternion.identity, _pheromoneContainer);
            var scalingFactor = GlobalParameters.NodeRadius / 10f;
            newPheromone.transform.localScale = new Vector3(scalingFactor, newPheromone.transform.localScale.y, scalingFactor);
            newPheromone.Initialyze(scriptablePheromone.BaseCaracteristics, _currentPos);
        }


        // Events
        private void OnMouseDown()
        {
            Clicked?.Invoke(this);
            Debug.Log($"Mouse was clicked on {name}");
        }
    }
}
