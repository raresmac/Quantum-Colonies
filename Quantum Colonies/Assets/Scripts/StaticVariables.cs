using System;
using System.Collections;
using UnityEngine;

public static class StaticVariables
{
    public static int currentLevel = 1;
    public static LevelSelector levelSelector = new();
    public static Tuple<float, float> beginningArea = new(-100, 500);
    public static Tuple<float, float> endingArea = new(900, -500);
    public static int turn = 0;
    public static int casualties = 0;
    public static Stack moves = new();
}
