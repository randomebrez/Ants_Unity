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
            var outputIndex = ComputeAndGetOutput();

            // Select next pos : Interpret output
            InterpretOutput(outputIndex);

            base.Move();
        }

        private int ComputeAndGetOutput()
        {
            var inputs = GetBrainInputs();
            _brainComputer.BrainGraphCompute(GetNugetUnit.BrainGraph, inputs);
            return GetBestOutput();
        }

        private int GetBestOutput()
        {
            var decisionBrain = GetNugetUnit.BrainGraph.DecisionBrain;
            var outputs = decisionBrain.Neurons.OutputLayer.Neurons.ToList();
            (int bestOutputIndex, float bestOutputValue) = (-1, 0);
            for(int i = 0; i < outputs.Count; i++)
            {
                if (outputs[i].Value > bestOutputValue)
                    (bestOutputIndex, bestOutputValue) = (outputs[i].Id, outputs[i].Value);
            }
            return bestOutputIndex;
        }

        private Dictionary<string, List<float>> GetBrainInputs()
        {
            var result = new Dictionary<string, List<float>>();

            // Get brut object NeuralNetworkInputs (bool carryFood + List<Valeurs sur chaque portion>)
            var allInputs = _scannerManager.GetInputs;

            // Pour chaque template
            foreach (var template in Unit.InstanceGraph.UnityInputsUsingTemplates)
            {
                // On filtre les inputs pour garder que ceux dont a besoin le tp pour chaque portion

                // ToDo : améliorer ça
                var requiredInputs = new List<UnityInputTypeEnum>();
                var portions = template.Value.InputsTypes.Where(t => t.InputType == InputTypeEnum.Portion).Select(t => (t as InputTypePortion).UnityInputTypes);
                foreach (var portion in portions)
                    requiredInputs.AddRange(portion);

                var tpInputs = allInputs.RestrictedInputListGet(requiredInputs.ToHashSet());

                // On récupère toutes les instances utilisant ce tp
                var instanceBrains = Unit.InstanceGraph.InstanceByTemplate[template.Key];
                foreach (var brain in instanceBrains)
                {
                    // pour chaque on construit sa liste d'input
                    var brainInputs = new List<float>();
                    foreach (var portionIndex in brain.PortionIndexes)
                        brainInputs.AddRange(tpInputs[portionIndex]);

                    if (template.Value.InputsTypes.Any(t => t.InputType == InputTypeEnum.CarryFood))
                        brainInputs.Add(allInputs.CarryFood ? 1 : 0);

                    result.Add(brain.UniqueName, brainInputs);
                }
            }

            return result;
        }

        // Abstract method implementations

        protected override HashSet<UnitStatististicsEnum> RequiredStatistics() => new HashSet<UnitStatististicsEnum>
        { 
            UnitStatististicsEnum.Score,
            UnitStatististicsEnum.Age,
            UnitStatististicsEnum.ComeBackMean,
            UnitStatististicsEnum.FoodCollected,
            UnitStatististicsEnum.FoodGrabbed
        };

        public override Dictionary<UnitStatististicsEnum, float> GetStatistics()
        {
            var stats = new Dictionary<UnitStatististicsEnum, float>();
            foreach (var statType in RequiredStatistics())
            {
                switch(statType)
                {
                    case UnitStatististicsEnum.Score:
                        stats.Add(statType, GetUnitScore());
                        break;
                    case UnitStatististicsEnum.BestFoodReachStepNumber:
                        stats.Add(statType, _bestFindFoodStepNumber);
                        break;
                    case UnitStatististicsEnum.ComeBackMean:
                        stats.Add(statType, _bestComeBackStepNumber);
                        break;
                    case UnitStatististicsEnum.FoodCollected:
                        stats.Add(statType, FoodCollected);
                        break;
                    case UnitStatististicsEnum.FoodGrabbed:
                        stats.Add(statType, FoodGrabbed);
                        break;
                    case UnitStatististicsEnum.Age:
                        stats.Add(statType, _age);
                        break;
                }
            }

            return stats;
        }

        public override void CheckCollectableCollisions()
        {
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
                if (CurrentPos.HasAnyFood)
                    WrongFoodCollision++;

                if (CurrentPos.IsUnderNest)
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

            var result = _score + bonusGrab + bonusCollected  - outputOverloadMalus - randomMoveMalus - wrongFoodMalus;
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
                    if (Physics.Raycast(CurrentPos.Block.WorldPosition, direction, out hit, 2 * GlobalParameters.NodeRadius, LayerMask.GetMask(UnityLayerEnum.Walkable.ToString())))
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
    }   
}
