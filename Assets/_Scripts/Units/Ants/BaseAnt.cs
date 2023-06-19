using System;
using UnityEngine;
using Assets._Scripts.Utilities;
using System.Collections.Generic;
using Assets.Dtos;
using NeuralNetwork.Implementations;
using NeuralNetwork.Abstraction;
using NeuralNetwork.Abstraction.Model;

namespace mew
{
    public abstract class BaseAnt : UnitBase
    {
        protected List<Color> _colors = new List<Color> { Color.black, Color.red, Color.yellow, Color.blue, Color.magenta, Color.cyan, Color.green, Color.gray, Color.white };
        // Events
        public Action<BaseAnt> Clicked;

        public UnitWrapper Unit { get; private set; }

        // Positions
        public GroundBlock CurrentPos { get; protected set; }
        protected GroundBlock _nextPos;

        //Managers
        protected IBrainCalculator _brainComputer;
        protected UnitScannerManager _scannerManager;

        // Private fields
        private Transform _body;
        private Transform _head;

        protected bool _initialyzed = false;
        protected int _age = 0;

        // Public methods
        public Vector3 BodyHeadAxis => (_head.position - _body.position).normalized;
        public float PhysicalLength => Vector3.Distance(_body.position, _head.position);
        public Unit GetNugetUnit => Unit.NugetUnit;


        // Unity methods
        private void Awake()
        {
            _body = transform.GetChild(0);
            _head = transform.GetChild(1);

            _scannerManager = GetComponentInChildren<UnitScannerManager>();
        }


        // Abstract Methods
        public abstract void CheckCollectableCollisions();

        public abstract float GetUnitScore();

        public abstract Dictionary<UnitStatististicsEnum, float> GetStatistics();

        protected abstract HashSet<UnitStatististicsEnum> RequiredStatistics();

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
        public void Initialyze(ScriptableUnitBase.Stats stats, UnitWrapper unit, GroundBlock initalPosition)
        {
            Unit = unit;
            Stats = stats;
            CurrentPos = initalPosition;
            _brainComputer = new BrainCalculator();
            _scannerManager.InitialyzeScanners();
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

            var rotation = Vector3.SignedAngle(BodyHeadAxis, _nextPos.Block.WorldPosition - CurrentPos.Block.WorldPosition, Vector3.up);

            CurrentPos = _nextPos;
            transform.SetPositionAndRotation(CurrentPos.Block.WorldPosition + 2 * GlobalParameters.NodeRadius * Vector3.up, Quaternion.Euler(0, transform.rotation.eulerAngles.y + rotation, 0));

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
