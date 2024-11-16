using System;
using UnityEngine;

public class LevelSelector
{
    public Level[] levels = new Level[3];

    public LevelSelector(){
        levels[1] = new Level(1, 4, 4, 
            new Tuple<int, int>[] { 
                new(0, 3),
                new(1, 0),
                new(1, 3),
                new(3, 0),
                new(3, 1)
            }, 
            new Tuple<Tuple<int, int>, Tuple<int, int> >[] {
                new(new(0, 1), new(1, 0))
            },
            new Tuple<int, int>[] { 
                new(0, 0)
            }
        );
    }
}
