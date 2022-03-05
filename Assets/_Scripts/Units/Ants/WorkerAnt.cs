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
            if (!_carryingFood || _visionField.Collectables.Any(t => t.transform.name == _nest.name) == false)
                return false;

            _target = new Target { Type = TargetTypeEnum.Nest, Transform = _nest, Active = true };
            return true;
        }

        private bool ScanForFood()
        {
            if (_carryingFood)
                return false;

            var food = _visionField.Collectables.FirstOrDefault(t => t.tag == "Food");
            if (food == null)
                return false;

            _target = new Target { Type = TargetTypeEnum.Food, Transform = food.transform, Active = true };
            return true;
        }

        internal override void DropPheromone()
        {
            if (_carryingFood)
                base.DropPheromone();
        }

        private void MoveTowardTarget()
        {
            if (!HasTarget)
                return;

            _desiredDirection = _target.Transform.position - _position;

            // Normalize if above one
            // Otherwise it will slow down the ant until it reaches the target
            if (Mathf.Sqrt(_desiredDirection.x * _desiredDirection.x + _desiredDirection.y * _desiredDirection.y + _desiredDirection.z * _desiredDirection.z) > 1)
                _desiredDirection = _desiredDirection.normalized;
        }

        private void RandomWalk()
        {
            var random = GetRandomDirection();

            // Random direction shouldn't be in the back circular arc of the ant. (4 degrees not allowed)
            var randomDirectionAngle = Vector3.SignedAngle(BodyHeadAxis, new Vector3(random.x, 0, random.z), Vector3.up);
            if (Mathf.Abs(randomDirectionAngle) < 178)
            {
                _desiredDirection = (_desiredDirection + new Vector3(random.x, 0, random.z) * WanderStrength).normalized;
                _desiredDirection.y = 0;
            }
        }

        private void OnDrawGizmos()
        {
        }
    }
}
