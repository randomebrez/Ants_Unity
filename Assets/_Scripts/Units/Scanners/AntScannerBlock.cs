using mew;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Scripts.Units.Ants.AntScanners
{
    public class AntScannerBlock : AntScannerBase
    {
        protected override bool _checkObtruction => false;
        protected override bool _checkVisionField => false;

        private Dictionary<int, bool> _visionFieldPortion = new Dictionary<int, bool>();

        public override void Initialyze(BaseAnt ant, int scannerSubdivision)
        {
            base.Initialyze(ant, scannerSubdivision);
            for (int i = 0; i < scannerSubdivision; i++)
            {
                if (Mathf.Abs(_subdivisions[i] + 60) < _scannerAngle / 2f)
                    _visionFieldPortion.Add(i, true);
                else if (Mathf.Abs(_subdivisions[i]) < _scannerAngle / 2f)
                    _visionFieldPortion.Add(i, true);
                else
                    _visionFieldPortion.Add(i, false);
            }
        }

        public override void Scan()
        {
            _positionAtScanTime = _ant.CurrentPos.Block.WorldPosition;
            _directionAtScanTime = _ant.BodyHeadAxis;
            var count = Physics.OverlapSphereNonAlloc(_positionAtScanTime, _scannerRadius, _colliders, ScanningMainLayer, QueryTriggerInteraction.Collide);

            _objects.Clear();
            for (int i = 0; i < count; i++)
            {
                var obj = _colliders[i].transform.parent.GetComponent<GroundBlock>();
                if (obj != null && IsInSight(obj.gameObject))
                    _objects.Add(obj.gameObject);
            }
        }

        public (int number, float averageDensity) GetPheromonesOfType(int index, ScriptablePheromoneBase.PheromoneTypeEnum type)
        {
            try
            {
                var sum = 0f;
                var blocks = Objects[index].Select(t => t.GetComponent<GroundBlock>()).Where(t => t != null).ToList();
                var pheroDensities = blocks.Select(t => t.GetPheromoneDensity(type)).Where(t => t > 0);
                var number = pheroDensities.Count();
                if (number == 0)
                    return (0, 0);

                foreach (var density in pheroDensities)
                    sum += density;

                return (number, sum / blocks.Count);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return (0, 0);
            }
        }

        public bool IsThereFood(int index)
        {
            var blocks = Objects[index].Select(t => t.GetComponent<GroundBlock>()).Where(t => t != null).ToList();
            return blocks.Any(t => t.HasAnyFood);
        }

        public bool IsNestInSight(int index)
        {
            var blocks = Objects[index].Select(t => t.GetComponent<GroundBlock>()).Where(t => t != null).ToList();
            return blocks.Any(t => t.IsUnderNest);
        }


        public bool IsPortionInVisionField(int portionIndex)
        {
            return _visionFieldPortion[portionIndex];
        }

        public override float GetPortionValue(int index)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<int, List<GameObject>> ToDictionnary(List<GameObject> objects)
        {
            var result = new Dictionary<int, List<GameObject>>();

            for (int i = 0; i < _scannerSubdivision; i++)
                result.Add(i, new List<GameObject>());

            var delta = 360f / _scannerSubdivision;
            foreach (var obj in objects)
            {
                var block = obj.GetComponent<GroundBlock>();
                var objDirection = block.Block.WorldPosition - _positionAtScanTime;
                var angle = Vector3.SignedAngle(_directionAtScanTime, objDirection, Vector3.up);
                if (_subdivisions.Any(t => t > angle))
                    result[_subdivisions.FindIndex(t => t > angle) - 1].Add(obj);
                else if (360 + _subdivisions[0] > angle)
                    result[_scannerSubdivision - 1].Add(obj);
                else
                    result[0].Add(obj);
            }

            return result;
        }

        protected override bool IsInSight(GameObject obj)
        {
            var objPosition = obj.GetComponent<GroundBlock>().Block.WorldPosition;
            var direction = objPosition - _positionAtScanTime;
            var floatDist = direction.x * direction.x + direction.y * direction.y + direction.z * direction.z;

            // Check on distance
            if (floatDist <= _apothem * _apothem || floatDist > _scannerRadius * _scannerRadius)
                return false;
        
            // Check on angle
            if (_checkVisionField && Mathf.Abs(Vector3.SignedAngle(_directionAtScanTime, direction, Vector3.up)) > _scannerAngle / 2f)
                return false;
        
            if (_checkObtruction == false)
                return true;

            // Check if no obstruction
            var fakePos = new Vector3(_positionAtScanTime.x, objPosition.y, _positionAtScanTime.z);
            direction.y = 0;
            if (Physics.Linecast(fakePos, direction, OcclusionLayer))
                return false;

            return true;
        }

        //private void OnDrawGizmos()
        //{
        //    if (_ant.name != "Worker_0")
        //        return;
        //
        //    // Draw scanner with portions
        //    Gizmos.color = Color.black;
        //    Gizmos.DrawWireSphere(_positionAtScanTime, _scannerRadius);
        //    float deltaTheta = 360 / _scannerSubdivision;
        //    // start at the back of the ant for the 'ToDictionary' method that uses the fact that indexes are increasing
        //    var current = -180 - deltaTheta / 2f;
        //    for (int i = 0; i < _scannerSubdivision; i++)
        //    {
        //        Gizmos.DrawLine(_positionAtScanTime, _positionAtScanTime + Quaternion.Euler(0, current, 0) * _directionAtScanTime * _scannerRadius);
        //        current += deltaTheta;
        //    }
        //
        //    // Print scanned obj
        //    var portionColors = new Color[] { Color.yellow, Color.red, Color.blue, Color.green, Color.cyan,  Color.white };
        //    for (int i = 0; i < _scannerSubdivision; i++)
        //    {
        //        Gizmos.color = portionColors[i];
        //        foreach (var obstacle in Objects[i])
        //        {
        //            Gizmos.DrawWireCube(obstacle.transform.position, Vector3.one);
        //        }
        //    }
        //
        //    // Print ground detected
        //    //var colliders = new Collider[70];
        //    //Physics.OverlapSphereNonAlloc(_positionAtScanTime, _scannerRadius, colliders, LayerMask.GetMask(Layer.Walkable.ToString()));
        //    //Gizmos.color = Color.magenta;
        //    //foreach (var collider in colliders)
        //    //    Gizmos.DrawWireCube(collider.transform.position, Vector3.one);
        //
        //    // Vision Field
        //    //Gizmos.color = Color.red;
        //    //Gizmos.DrawLine(_positionAtScanTime, _positionAtScanTime + Quaternion.Euler(0, -_scannerAngle / 2f, 0) * _directionAtScanTime * _scannerRadius);
        //    //Gizmos.DrawLine(_positionAtScanTime, _positionAtScanTime + Quaternion.Euler(0, _scannerAngle / 2f, 0) * _directionAtScanTime * _scannerRadius);
        //}
    }
}
