﻿using System;

namespace Assets.Dtos
{
    public class UnitPortionCaracteristics
    {

        private readonly Guid _uniqueName;

        public UnitPortionCaracteristics()
        {
            _uniqueName= Guid.NewGuid();
        }

        public string UniqueName => _uniqueName.ToString();

        public string PrettyName { get; set; }

        public int Id { get; set; }

        public float MinAngle { get; set; }

        public float MaxAngle { get; set; }

        public bool IntersectVisionRange { get; set; }
    }
}
