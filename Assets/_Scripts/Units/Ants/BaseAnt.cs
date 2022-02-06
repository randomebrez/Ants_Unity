using System.Collections;
using System.Collections.Generic;
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

        public float SteerStrength = 3;
        public float WanderStrength = 1;

        public abstract void Move();

        void Start()
        {
            _position = transform.position;
            _position.y = 1.25f;
            _body = transform.GetChild(0);
            _head = transform.GetChild(1);

            _desiredDirection = BodyHeadAxis.normalized;
        }

        void Update()
        {
            Move();
        }

        // Get axis from body to head.
        // In order to have that vector following the ant, one need to add ant position (so that position will always be the origin of the vector)
        protected Vector3 BodyHeadAxis => (_head.position - _body.position).normalized;
    }
}
