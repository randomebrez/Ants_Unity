using UnityEngine;

namespace mew
{
    public abstract class BaseAnt : UnitBase
    {
        protected Vector3 _position;
        protected Vector3 _velocity;
        protected Vector3 _desiredDirection;

        private Transform _body;
        private Transform _head;
        protected AntVision _visionField;

        protected bool HasTarget => _target != null;
        protected Transform _target = null;
        protected float _targetTreshold = 0.3f;

        public float SteerStrength = 3;
        public float WanderStrength = 1;

        void Start()
        {
            _position = transform.position;
            _position.y = 1.25f;
            _body = transform.GetChild(0);
            _head = transform.GetChild(1);

            _visionField = _head.GetComponent<AntVision>();

            _desiredDirection = BodyHeadAxis.normalized;
        }

        void Update()
        {
            if (TargetDistance() < Stats.TargetDestroyTreshold)
                DestroyTarget();

            Move();
        }

        // Get axis from body to head.
        // In order to have that vector following the ant, one need to add ant position (so that position will always be the origin of the vector)
        public Vector3 BodyHeadAxis => (_head.position - _body.position).normalized;


        // Set _desiredDirection in non abstract classes
        // Once _desiredDirection is set ant movement always end like that
        public virtual void Move()
        {
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
            transform.SetPositionAndRotation(_position, Quaternion.Euler(0, transform.rotation.eulerAngles.y + angle, 0));
        }

        public virtual void DestroyTarget()
        {
            if (HasTarget)
                Destroy(_target.gameObject);
        }

        public float TargetDistance()
        {
            if (!HasTarget)
                return float.MaxValue;

            var distance = _target.position - _position;
            return Mathf.Sqrt(distance.x * distance.x + distance.y * distance.y + distance.z * distance.z);
        }
    }
}
