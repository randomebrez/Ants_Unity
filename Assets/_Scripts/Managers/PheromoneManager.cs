using Assets._Scripts.Utilities;
using Assets.Dtos;
using mew;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PheromoneManager : MonoBehaviour
{
    public int Count;
    void Update()
    {
        Count = transform.childCount;
    }

    public void CleanAllPheromones()
    {
        for(int i = transform.childCount; i > 0; i--)
            Destroy(transform.GetChild(i - 1).gameObject);
    }
}
