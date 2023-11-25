using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    //recipes 는 일단 있는데 사용처는 없음 그냥 필요하면 쓸듯 (어느 조건에 해당한 조합법을 모아놓고 보여준다던가)
    List<Recipe> recipes = new List<Recipe>();
    List<Recipe> recipes3X3 = new List<Recipe>();
    List<Recipe> recipes3X2 = new List<Recipe>();
    List<Recipe> recipes2X3 = new List<Recipe>();
    private void Start()
    {
        Recipe r = new Plank_Recipe();

        if (r.recipe is Table3X3)
            recipes3X3.Add(r);
        else if(r.recipe is Table3X2)
            recipes3X2.Add(r);
        else if(r.recipe is Table2X3)
            recipes2X3.Add(r);

        Debug.Log($"{recipes3X3.Count} {recipes3X2.Count} {recipes2X3.Count}");
        //recipes.Add(new Plank_Recipe());
    }

    public void Combination(Table3X3 x3, out int id, out int result)
    {
        id = -1;
        result = 0;
        for(int i = 0; i < recipes.Count; i++)
        {
            if(recipes[i].Comparison(x3))
            {
                id = recipes[i].result;
                result = recipes[i].resultCount;
                return;
            }
        }
    }
}

public class Recipe
{
    public Table recipe;
    public int result;
    public int resultCount;

    public bool Comparison(Table3X3 x3)
    {
        return recipe._Equals(x3);
    }
}

public class Plank_Recipe : Recipe
{
    public Plank_Recipe()
    {
        result = 1;
        resultCount = 4;

        recipe = new Table3X3();
        recipe.codes = new int[3,3]
        {
            { 1, 1, -1 },
            { 1, 1, -1 },
            { -1,-1,-1 }
        };
        recipe.Slice(recipe, out recipe);
    }
}

public class Table
{
    public int[,] codes;

    public bool _Equals(Table table)
    {
        if (table.codes.GetLength(0) != codes.GetLength(0) || table.codes.GetLength(1) != codes.GetLength(1))
            return false;

        for (int i = 0; i < table.codes.GetLength(0); i++)
        {
            for (int j = 0; j < table.codes.GetLength(1); j++)
            {
                if (table.codes[i, j] != codes[i, j])
                    return false;
            }
        }

        return true;
    }

    public Table NewTable(int x, int y)
    {
        if (x == 3 && y == 3) return new Table3X3();
        else if (x == 3 && y == 2) return new Table3X2();
        else if (x == 2 && y == 3) return new Table2X3();
        else if(x == 2 && y == 2) return new Table2X2();

        return new Table();
    }

    public void Slice(Table table, out Table slice)
    {
        int rowfront = 0;
        int rowback = table.codes.GetLength(0);
        int linefront = 0;
        int lineback = table.codes.GetLength(1);

        bool forward = false;
        bool reverse = false;

        for (int i = 0; i < table.codes.GetLength(0); i++)
        {
            rowfront = i;
            for (int j = 0; j < table.codes.GetLength(1); j++)
            {
                if (table.codes[i, j] != -1)
                {
                    forward = true;
                    break;
                }
            }
            if (forward)
                break;
        }

        //모든칸이 -1인경우
        if(rowfront == table.codes.GetLength(0) - 1 && forward == false)
        {
            slice = new Table();
            return;
        }

        for (int i = table.codes.GetLength(0) - 1; i >= 0; i--)
        {
            rowback = i;
            for (int j = 0; j < table.codes.GetLength(1); j++)
            {
                if (table.codes[i, j] != -1)
                {
                    reverse = true;
                    break;
                }
            }
            if (reverse)
                break;
        }

        //rowfront 가 시작줄 rowback 이 마지막 줄 rowfront ~ rowback 가 행의 크기

        forward = false; reverse = false;


        for (int i = 0; i < table.codes.GetLength(1); i++)
        {
            linefront = i;
            for (int j = 0; j < table.codes.GetLength(0); j++)
            {
                if (table.codes[j, i] != -1)
                {
                    forward = true;
                    break;
                }
            }
            if (forward)
                break;
        }

        for (int i = table.codes.GetLength(1) - 1; i >= 0; i--)
        {
            lineback = i;
            for (int j = 0; j < table.codes.GetLength(0); j++)
            {
                if (table.codes[j, i] != -1)
                {
                    reverse = true;
                    break;
                }
            }
            if (reverse)
                break;
        }

        int line = Mathf.Abs(linefront - lineback) + 1;
        int row  = Mathf.Abs(rowfront - rowback) + 1;
        slice = NewTable(row, line);
        for(int i = rowfront; i <= rowback; i++)
        {
            for(int j = linefront; j <= lineback; j++)
            {
                slice.codes[i - rowfront, j - linefront] = table.codes[i, j];
            }
        }
    }
}

public class Table3X3 : Table
{
    public Table3X3()
    {
        codes = new int[3, 3]
        {
            { -1, -1, -1},
            { -1, -1, -1},
            { -1, -1, -1}
        };
    }
}

public class Table2X3 : Table
{
    public Table2X3()
    {
        codes = new int[2, 3]
        {
            { -1, -1, -1},
            { -1, -1, -1}
        };
    }
}

public class Table3X2 : Table
{
    public Table3X2()
    {
        codes = new int[3, 2]
        {
            { -1, -1},
            { -1, -1},
            { -1, -1}
        };
    }
}

public class Table2X2 : Table
{
    public Table2X2()
    {
        codes = new int[2, 2]
        {
            { -1, -1},
            { -1, -1},
        };
    }
}