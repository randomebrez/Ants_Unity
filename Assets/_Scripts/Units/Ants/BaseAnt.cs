using Assets.Dtos;
using System;
using UnityEngine;
using NeuralNetwork.Managers;
using Assets._Scripts.Utilities;
using System.Collections.Generic;
using Assets._Scripts.Units.Ants;

namespace mew
{
    public abstract class BaseAnt : UnitBase
    {
        protected List<Color> _colors = new List<Color> { Color.black, Color.red, Color.yellow, Color.blue, Color.magenta, Color.cyan, Color.green, Color.gray, Color.white };
        // Events
        public Action<BaseAnt> Clicked;

        // Positions
        protected Block _currentPos;
        protected Block _nextPos;

        //Managers
        protected BrainManager _brainManager;
        protected AntScannerManager _scannerManager;
        protected PheromoneManager _pheromoneManager;
        protected Transform _pheromoneContainer;

        // Private fields
        private bool _initialyzed = false;
        private Transform _body;
        private Transform _head;
        private Transform _nest;

        protected int _age = 0;
        protected float _roundNumber = 0f;
        protected float _totalRotation = 0f;

        // Pheromones parameters
        protected int _dropPheroFrequency = 10;
        protected float _dropPheroInterval;
        protected float _dropPheroTimer;

        // Public methods
        public Vector3 BodyHeadAxis => (_head.position - _body.position).normalized;
        public float PhysicalLength => Vector3.Distance(_body.position, _head.position);
        public string[] PortionInfos => _scannerManager.PortionInfos;
        public string NestName => _nest.name;

        public AntBrains GetBrain()
        {
            return new AntBrains
            {
                MainBrain = _brainManager.GetBrain()
            };
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


        // Abstract Methods
        public abstract void CheckCollectableCollisions();

        public abstract float GetUnitScore();

        public abstract Dictionary<StatisticEnum, float> GetStatistics();

        protected abstract HashSet<StatisticEnum> RequiredStatistics();

        protected abstract ScriptablePheromoneBase.PheromoneTypeEnum GetPheroType();


        // Ant methods
        protected void SetHeadColor(Color color)
        {
            _head.GetComponent<MeshRenderer>().material.color = color;
        }

        protected void SetBodyColor(Color color)
        {
            _body.GetComponent<MeshRenderer>().material.color = color;
        }


        // Override methods
        public void Initialyze(ScriptableUnitBase.Stats stats, AntBrains brains, Transform pheromoneContainer)
        {
            Stats = stats;
            _brainManager = new BrainManager(brains.MainBrain);
            _scannerManager.InitialyzeScanners(brains.ScannerBrains);
            _pheromoneContainer = pheromoneContainer;
            SetHeadColor(_colors[0]);
            SetBodyColor(_colors[0]);
            _initialyzed = true;
        }


        // Virtual Methods
        public virtual void Move()
        {
            _age++;
            // _nextPos must be updated in inherited classes
            if (_nextPos == null)
            {
                Debug.Log("Next pos null");
                return;
            }

            var rotation = Vector3.SignedAngle(BodyHeadAxis, _nextPos.WorldPosition - _currentPos.WorldPosition, Vector3.up);
            _totalRotation += rotation;
            if (Mathf.Abs(_totalRotation) >= 360)
            {
                _totalRotation = 0f;
                _roundNumber++;
            }

            _currentPos = _nextPos;

            transform.SetPositionAndRotation(_currentPos.WorldPosition + 2 * GlobalParameters.NodeRadius * Vector3.up, Quaternion.Euler(0, transform.rotation.eulerAngles.y + rotation, 0));
        }

        internal virtual void DropPheromone()
        {
            _dropPheroTimer -= Time.deltaTime;
            if (_dropPheroTimer > 0)
                return;

            _dropPheroTimer += _dropPheroInterval;
            // Spawn pheromone
            var scriptablePheromone = ResourceSystem.Instance.PheromoneOfTypeGet(GetPheroType());
            var newPheromone = Instantiate(scriptablePheromone.PheromonePrefab, _currentPos.WorldPosition + GlobalParameters.NodeRadius * Vector3.up, Quaternion.identity, _pheromoneContainer);
            var scalingFactor = GlobalParameters.NodeRadius / 10f;
            //newPheromone.transform.localScale = new Vector3(scalingFactor, newPheromone.transform.localScale.y, scalingFactor);
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
