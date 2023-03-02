using Assets.Dtos;
using System.Linq;
using UnityEngine;

namespace mew
{
    public class WorkerAnt : BaseAnt
    {
        public bool _carryingFood;
        public override void Move()
        {
            if (HasTarget || ScanForNest() || ScanForFood())
                MoveTowardTarget();
            else
                RandomWalk();

            base.Move();
        }


        public override void OnTargetReach()
        {
            if (_target == null)
                return;

            switch (_target.Type)
            {
                case TargetTypeEnum.Food:
                    _carryingFood = true;
                    transform.Rotate(0, 180, 0);
                    Destroy(_target.Transform.gameObject);
                    break;
                case TargetTypeEnum.Nest:
                    _carryingFood = false;
                    break;
            }

            _target.Active = false;
        }

        private bool ScanForNest()
        {
            if (!_carryingFood)
                return false;

            if (_scannerManager.IsNestInSight(_nest.name).answer == false)
                return false;

            _target = new Target { Type = TargetTypeEnum.Nest, Transform = _nest, Active = true };
            return true;
        }

        private bool ScanForFood()
        {
            if (_carryingFood)
                return false;

            var foodObjects = _scannerManager.CollectablesListByTag("Food");
            if (foodObjects.Count <= 0)
                return false;

            _target = new Target { Type = TargetTypeEnum.Food, Transform = foodObjects.First().transform, Active = true };
            return true;
        }

        protected override ScriptablePheromoneBase.PheromoneTypeEnum GetPheroType()
        {
            if (_carryingFood)
                return ScriptablePheromoneBase.PheromoneTypeEnum.CarryFood;

            return ScriptablePheromoneBase.PheromoneTypeEnum.Wander;
        }

        private void MoveTowardTarget()
        {
            if (!HasTarget)
                return;

            _desiredDirection = _target.Transform.position - _position;
            _desiredDirection.y = 0;

            // Normalize if above one
            // Otherwise it will slow down the ant until it reaches the target
            if (Mathf.Sqrt(_desiredDirection.x * _desiredDirection.x + _desiredDirection.y * _desiredDirection.y + _desiredDirection.z * _desiredDirection.z) > 1)
                _desiredDirection = _desiredDirection.normalized;
        }

        private void RandomWalk()
        {
            var random = GetRandomDirection(_carryingFood);

            _desiredDirection = (_desiredDirection + new Vector3(random.x, 0, random.z) * Stats.WanderStrength).normalized;
            _desiredDirection.y = 0;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + Vector3.up, transform.position + 100 * Vector3.down);
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
