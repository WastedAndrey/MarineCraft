using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс для запаковки статов в Json
/// </summary>
public struct StatsContainer
{
    [SerializeField]
    public List<UnitStats> list;

    public StatsContainer(List<UnitStats> list)
    {
        this.list = list;
    }
}