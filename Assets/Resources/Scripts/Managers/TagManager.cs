using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Менеджер тегов. Теги в основном используются у компонентов интерфейса. Что бы их можно было куда-то повесить и они сами нашли, с чем работать
/// </summary>
public class TagManager
{
    public static readonly string Unit = "Unit";
    public static readonly string Missile = "Missile";
    public static readonly string Effect = "Effect";
    public static readonly string Level = "Level";
    public static readonly string Message = "Message";
}
