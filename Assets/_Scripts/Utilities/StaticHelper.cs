using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Utilities
{
    public static class StaticHelper
    {
        public static void SetRecursiveLayer(List<GameObject> objects, int layerId)
        {
            if (objects.Count == 0)
                return;

            var newList = new List<GameObject>();

            foreach (var obj in objects)
            {
                obj.layer = layerId;

                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    newList.Add(obj.transform.GetChild(i).gameObject);
                }
            }

            SetRecursiveLayer(newList, layerId);
        }
    }
}
