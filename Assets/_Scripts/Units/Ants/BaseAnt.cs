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
        public Block CurrentPos { get; protected set; }
        protected Block _nextPos;

        //Managers
        protected BrainManager _brainManager;
        protected AntScannerManager _scannerManager;

        // Private fields
        private Transform _body;
        private Transform _head;
        private Transform _nest;

        protected bool _initialyzed = false;
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
            CurrentPos = EnvironmentManager.Instance.BlockFromWorldPoint(transform.position);
        }


        // Abstract Methods
        public abstract void CheckCollectableCollisions();

        public abstract float GetUnitScore();

        public abstract Dictionary<StatisticEnum, float> GetStatistics();

        protected abstract HashSet<StatisticEnum> RequiredStatistics();

        public abstract ScriptablePheromoneBase.PheromoneTypeEnum GetPheroType();


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
        public void Initialyze(ScriptableUnitBase.Stats stats, AntBrains brains)
        {
            Stats = stats;
            _brainManager = new BrainManager(brains.MainBrain);
            _scannerManager.InitialyzeScanners(brains.ScannerBrains);
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

            var rotation = Vector3.SignedAngle(BodyHeadAxis, _nextPos.WorldPosition - CurrentPos.WorldPosition, Vector3.up);
            _totalRotation += rotation;
            if (Mathf.Abs(_totalRotation) >= 360)
            {
                _totalRotation = 0f;
                _roundNumber++;
            }

            CurrentPos = _nextPos;
            transform.SetPositionAndRotation(CurrentPos.WorldPosition + 2 * GlobalParameters.NodeRadius * Vector3.up, Quaternion.Euler(0, transform.rotation.eulerAngles.y + rotation, 0));

            CheckCollectableCollisions();
        }


        // Events
        private void OnMouseDown()
        {
            Clicked?.Invoke(this);
            Debug.Log($"Mouse was clicked on {name}");
        }
    }
}
