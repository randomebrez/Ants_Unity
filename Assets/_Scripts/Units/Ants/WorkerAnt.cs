using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mew
{
    public class WorkerAnt : BaseAnt
    {
        public override void Move()
        {
            if (HasTarget)
                MoveTowardTarget();

            else if (_visionField.Objects.Any())
            {
                _target = _visionField.Objects.First().transform;
                MoveTowardTarget();
            }
            else
                RandomWalk();

            base.Move();
        }

        private void MoveTowardTarget()
        {
            _desiredDirection = (_target.position - _position).normalized;
        }

        private void RandomWalk()
        {
            var random = Random.insideUnitCircle;

            // Random direction shouldn't be in the back circular arc of the ant. (4 degrees not allowed)
            var randomDirectionAngle = Vector3.SignedAngle(BodyHeadAxis, new Vector3(random.x, 0, random.y), Vector3.up);
            if (Mathf.Abs(randomDirectionAngle) < 178)
            {
                _desiredDirection = (_desiredDirection + new Vector3(random.x, 0, random.y) * WanderStrength).normalized;
                _desiredDirection.y = 0;
            }
        }

        private void OnDrawGizmos()
        {
        }
    }
}
