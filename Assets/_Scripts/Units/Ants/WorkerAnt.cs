using Assets._Scripts.Units.Ants;
using Assets._Scripts.Utilities;
using Assets.Dtos;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mew
{
    public class WorkerAnt : BaseAnt
    {
        public bool _carryingFood = false;
        public int FoodCollected = 0;
        public int FoodGrabbed = 0;
        private int _comeBackStepNumber = 0;
        private int _bestComeBackStepNumber = int.MaxValue;
        private int _findFoodStepNumber = 0;
        private int _bestFindFoodStepNumber = int.MaxValue;
        private float RandomMoveCount = 0f;
        private float WrongFoodCollision = 0f;

        private float _score = 0f;
        private int[] _outputs = new int[7];


        // Override Methods
        public override void Move()
        {
            if (_initialyzed == false)
                return;

            // Scan environment
            _scannerManager.ScanAll(_carryingFood);

            // Update Statistics
            UpdateStatistics();

            // Compute output using brain
            var scanneroutputs = _scannerManager.GetInputs();
            var output = _brainManager.ComputeOutput(scanneroutputs);

            // Select next pos : Interpret output
            InterpretOutput(output.ouputId);

            base.Move();
        }


        // Abstract method implementations

        protected override HashSet<StatisticEnum> RequiredStatistics() => new HashSet<StatisticEnum>
        { 
            StatisticEnum.Score,
            StatisticEnum.Age,
            StatisticEnum.ComeBackMean,
            StatisticEnum.FoodCollected,
            StatisticEnum.FoodGrabbed
        };

        public override Dictionary<StatisticEnum, float> GetStatistics()
        {
            var stats = new Dictionary<StatisticEnum, float>();
            foreach (var statType in RequiredStatistics())
            {
                switch(statType)
                {
                    case StatisticEnum.Score:
                        stats.Add(statType, GetUnitScore());
                        break;
                    case StatisticEnum.BestFoodReachStepNumber:
                        stats.Add(statType, _bestFindFoodStepNumber);
                        break;
                    case StatisticEnum.ComeBackMean:
                        stats.Add(statType, _bestComeBackStepNumber);
                        break;
                    case StatisticEnum.FoodCollected:
                        stats.Add(statType, FoodCollected);
                        break;
                    case StatisticEnum.FoodGrabbed:
                        stats.Add(statType, FoodGrabbed);
                        break;
                    case StatisticEnum.Age:
                        stats.Add(statType, _age);
                        break;
                }
            }

            return stats;
        }

        public override void CheckCollectableCollisions()
        {
            Collider[] colliders = new Collider[5];
            Physics.OverlapSphereNonAlloc(transform.position, GlobalParameters.NodeRadius / 2f, colliders, LayerMask.GetMask(Layer.Trigger.ToString()));
            var foodtoken = CurrentPos.HasAnyFood;
            if (!_carryingFood)
            {
                if (CurrentPos.HasAnyFood)
                {
                    CurrentPos.RemoveFoodToken();
                    FoodGrabbed++;

                    if (_findFoodStepNumber < _bestFindFoodStepNumber)
                        _bestFindFoodStepNumber = _findFoodStepNumber;

                    if (FoodGrabbed > 1)
                        _score += Mathf.Pow(1f / _findFoodStepNumber, 1f / FoodGrabbed);

                    _findFoodStepNumber = 0;
                    _carryingFood = true;

                    if (FoodGrabbed < _colors.Count)
                        SetHeadColor(_colors[FoodGrabbed]);
                }
            }
            else
            {
                if (foodtoken)
                    WrongFoodCollision++;

                var nest = colliders.Where(t => t != null).FirstOrDefault(t => t.transform.parent.parent.name == NestName);
                if (nest != null)
                {
                    FoodCollected++;

                    if (_comeBackStepNumber < _bestComeBackStepNumber)
                        _bestComeBackStepNumber = _comeBackStepNumber;

                    _score += Mathf.Pow(1f / _comeBackStepNumber, 1f / (2 * FoodCollected));
                    _comeBackStepNumber = 0;
                    _carryingFood = false;

                    if (FoodCollected < _colors.Count)
                        SetBodyColor(_colors[FoodCollected]);
                }
            }
        }

        public override float GetUnitScore()
        {
            var maxRound = _age / 3;

            var bonusGrab = Mathf.Min(1, FoodGrabbed);
            var bonusCollected = Mathf.Min(0, FoodCollected);
            var roundMalus = _roundNumber / maxRound;
            var randomMoveMalus = RandomMoveCount / _age;
            var wrongFoodMalus = Mathf.Pow(WrongFoodCollision / _age, 2);
            var overloadedOutputs = _outputs.Where(t => t > 0.4 * _age).ToList();
            var outputOverloadMalus = 0f;

            //for (int i = 0; i < overloadedOutputs.Count(); i++)
            //{
            //    outputOverloadMalus += (float)overloadedOutputs[i] / _age;
            //}
            //
            //if (overloadedOutputs.Count > 0)
            //    outputOverloadMalus /= overloadedOutputs.Count;

            var result = _score + bonusGrab + bonusCollected  - outputOverloadMalus - randomMoveMalus;
            return result;
        }

        public override ScriptablePheromoneBase.PheromoneTypeEnum GetPheroType()
        {
            if (_carryingFood)
                return ScriptablePheromoneBase.PheromoneTypeEnum.CarryFood;

            return ScriptablePheromoneBase.PheromoneTypeEnum.Wander;
        }


        // Private methods
        private void UpdateStatistics()
        {
            if (_carryingFood)
                _comeBackStepNumber++;
            else
                _findFoodStepNumber++;
        }

        private void InterpretOutput(int outputValue)
        {
            _outputs[outputValue + 1]++;

            var deltaTheta = 360f / Stats.ScannerSubdivisions;
            Vector3 direction;
            RaycastHit hit;
            switch (outputValue)
            {
                case -1:
                    RandomMove();
                    break;
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    direction = Quaternion.Euler(0, outputValue * deltaTheta, 0) * BodyHeadAxis;
                    if (Physics.Raycast(CurrentPos.Block.WorldPosition, direction, out hit, 2 * GlobalParameters.NodeRadius, LayerMask.GetMask(Layer.Walkable.ToString())))
                        _nextPos = hit.collider.GetComponentInParent<GroundBlock>();
                    else
                    {
                        RandomMove();
                        RandomMoveCount++;
                    }
                    break;
            }
        }

        private void RandomMove()
        {
            var maxNeighbourIndex = CurrentPos.Block.Neighbours.Count();
            var randomIndex = Random.Range(0, maxNeighbourIndex);
            _nextPos = EnvironmentManager.Instance.GroundBlockFromBlock(CurrentPos.Block.Neighbours[randomIndex]);
        }


         /*
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(_position, _position + BodyHeadAxis);
            if (Probabilities.Length == 0)
                return;
        
            Gizmos.color = Color.red;
            var stratingAngle = -180f;
            var deltaAngle = 360f / 20;
            for (int i = 0; i < Probabilities.Length; i++)
            {
                Gizmos.DrawLine(_position, _position + 100 * Probabilities[i] * (Quaternion.Euler(0, stratingAngle + i * deltaAngle, 0) * BodyHeadAxis));
            }
        }*/
    }   
}
