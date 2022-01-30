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

        public float SteerStrength = 1;
        public float WanderStrength = 1;

        public abstract void Move();

        void Start()
        {
            _position.y = 1.25f;
            _body = transform.GetChild(0);
            _head = transform.GetChild(1);

            _desiredDirection = BodyHeadAxe();
        }

        void Update()
        {
            Move();
        }

        protected Vector3 BodyHeadAxe() => _head.position - _body.position;
    }
}
