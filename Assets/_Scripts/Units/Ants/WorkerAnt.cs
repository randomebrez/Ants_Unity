using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mew
{
    public class WorkerAnt : BaseAnt
    {
        public override void Move()
        {
            var random = Random.insideUnitCircle;

            // Random direction shouldn't be in the back circular arc of the ant.
            // Accepted direction are the 85% of the unit circle in front of the ant
            var randomDirectionAngle = Vector3.SignedAngle(BodyHeadAxis, new Vector3(random.x, 0, random.y), Vector3.up);
            if (Mathf.Abs(randomDirectionAngle) < 178)
            {
                _desiredDirection = (_desiredDirection + new Vector3(random.x, 0, random.y) * WanderStrength).normalized;
                _desiredDirection.y = 0;
            }

            var desiredVelocity = _desiredDirection * Stats.MaxSpeed;
            var desiredSteeringForce = (desiredVelocity - _velocity) * SteerStrength;

            // get acceleration
            var acceleration = Vector3.ClampMagnitude(desiredSteeringForce, SteerStrength);

            // set new velocity
            _velocity = Vector3.ClampMagnitude(_velocity + acceleration * Time.deltaTime, Stats.MaxSpeed);

            // set new position
            _position += _velocity * Time.deltaTime;

            // calculate the rotation to go in the new direction
            var angle = Vector3.SignedAngle(BodyHeadAxis, _velocity, Vector3.up);

            // Apply it to the ant game object
            transform.SetPositionAndRotation(_position, Quaternion.Euler(0,transform.rotation.eulerAngles.y + angle, 0));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_position, _position + BodyHeadAxis * 5);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_position, _position + _velocity * 5);
        }
    }
}
