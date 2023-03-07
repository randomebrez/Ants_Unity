using Assets.Dtos;
using System.Linq;
using UnityEngine;

namespace mew
{
    public class WorkerAnt : BaseAnt
    {
        public bool _carryingFood = false;
        public int FoodCounter = 0;

        // Override Methods
        public override void Move()
        {
            // Update Inputs (CarryFood/NestInSight)
            UpdateInputs();

            // Compute output using brain
            var output = BrainManager.ComputeOutput(_scannerManager.GetInputs());

            // Select next pos : Interpret output
            InterpretOutput(output.ouputId);

            base.Move();
        }

        public override void CheckCollectableCollisions()
        {
            Collider[] colliders = new Collider[5];
            Physics.OverlapSphereNonAlloc(transform.position, EnvironmentManager.Instance.NodeRadius / 2f, colliders, LayerMask.GetMask("Trigger"));
            if (!_carryingFood)
            {
                var foodtoken = colliders.Where(t => t != null).FirstOrDefault(t => t.tag == "Food");
                if (foodtoken != null)
                {
                    Destroy(foodtoken.transform.parent.gameObject);
                    _carryingFood = true;
                }
            }
            else
            {
                var nest = colliders.Where(t => t != null).FirstOrDefault(t => t.transform.parent.parent.name == _nest.name);
                if (nest != null)
                {
                    _carryingFood = false;
                    FoodCounter++;
                }
            }
        }

        public override float GetUnitScore()
        {
            return FoodCounter;
        }


        private void UpdateInputs()
        {
            _scannerManager.UpdateAntInputs(_carryingFood);
        }

        private void InterpretOutput(int outputValue)
        {
            var deltaTheta = 360f / Stats.ScannerSubdivisions;
            switch (outputValue)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    var direction = Quaternion.Euler(0, outputValue * deltaTheta, 0) * BodyHeadAxis;
                    if (Physics.Raycast(_currentPos.WorldPosition, direction, out var hit, 2 * EnvironmentManager.Instance.NodeRadius, LayerMask.GetMask(Layer.Walkable.ToString())))
                        _nextPos = hit.collider.GetComponentInParent<GroundBlock>().Block;
                    else
                    {
                        //Debug.Log($"Can't reach direction ({direction.x}, {direction.z}) from ({_currentPos.XCoordinate}, {_currentPos.ZCoordinate})");
                        RandomMove();
                    }
                    break;
                case 6:
                    RandomMove();
                    break;
            }
        }

        private void RandomMove()
        {
            var maxNeighbourIndex = _currentPos.Neighbours.Count();
            var randomIndex = Random.Range(0, maxNeighbourIndex);
            _nextPos = _currentPos.Neighbours[randomIndex];
        }

        protected override ScriptablePheromoneBase.PheromoneTypeEnum GetPheroType()
        {
            if (_carryingFood)
                return ScriptablePheromoneBase.PheromoneTypeEnum.CarryFood;

            return ScriptablePheromoneBase.PheromoneTypeEnum.Wander;
        }

        private void OnDrawGizmos()
        {
            /*Gizmos.color = Color.black;
            Gizmos.DrawLine(_position, _position + BodyHeadAxis);
            if (Probabilities.Length == 0)
                return;

            Gizmos.color = Color.red;
            var stratingAngle = -180f;
            var deltaAngle = 360f / 20;
            for (int i = 0; i < Probabilities.Length; i++)
            {
                Gizmos.DrawLine(_position, _position + 100 * Probabilities[i] * (Quaternion.Euler(0, stratingAngle + i * deltaAngle, 0) * BodyHeadAxis));
            }*/
        }
    }
}
