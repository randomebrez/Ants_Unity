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
            var frontRandomDirection = Quaternion.Euler(0, Vector3.Angle(Vector3.forward, BodyHeadAxe()), 0) * new Vector2(random.x, Mathf.Abs(random.y));

            _desiredDirection = (_desiredDirection + new Vector3(frontRandomDirection.x, 0, frontRandomDirection.y) * WanderStrength).normalized;

            var desiredVelocity = _desiredDirection * Stats.MaxSpeed;
            var desiredSteeringForce = (desiredVelocity - _velocity) * SteerStrength;
            var acceleration = Vector3.ClampMagnitude(desiredSteeringForce, SteerStrength);

            _velocity = Vector3.ClampMagnitude(_velocity + acceleration * Time.deltaTime, Stats.MaxSpeed);
            _position += _velocity * Time.deltaTime;
            var angle = Vector3.Angle(BodyHeadAxe(), _velocity);
            //var angle = Mathf.Atan2(_velocity.z, _velocity.x) * Mathf.Rad2Deg;
            transform.SetPositionAndRotation(_position, Quaternion.Euler(0, angle, 0));
        }
    }
}
