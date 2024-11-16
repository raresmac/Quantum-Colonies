using System;
using UnityEngine;

public class Level
{
    public int levelNumber = 1;
    public int rows = 5;
    public int columns = 5;
    public Tuple<int, int>[] bombs = null;
    public Tuple<Tuple<int, int>, Tuple<int, int> >[] entanglements = null;
    public Tuple<int, int>[] initialCells = null;

    public Level(int levelNumber, int rows, int columns, Tuple<int, int>[] bombs, Tuple<Tuple<int, int>, Tuple<int, int> >[] entanglements, Tuple<int, int>[] initialCells){
        this.levelNumber = levelNumber;
        this.rows = rows;
        this.columns = columns;
        this.bombs = new Tuple<int, int>[bombs.Length];
        this.entanglements = new Tuple<Tuple<int, int>, Tuple<int, int> >[entanglements.Length];
        this.initialCells = new Tuple<int, int>[initialCells.Length];
        Array.Copy(bombs, this.bombs, bombs.Length);
        Array.Copy(entanglements, this.entanglements, entanglements.Length);
        Array.Copy(initialCells, this.initialCells, initialCells.Length);
    }
}
