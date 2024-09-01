using System;
using System.Collections.Generic;

namespace Dassie.Preprocessor.Parser.Helpers;

// Taken from https://github.com/io-monad/line-column and translated to C#
public class CodePositionHelper
{
    private readonly string _str;
    private readonly int[] _lineToIndex;
    private readonly int _origin;

    public CodePositionHelper(string str, int origin = 1)
    {
        _str = str ?? string.Empty;
        _lineToIndex = BuildLineToIndex(_str);
        _origin = origin;
    }

    public (int line, int col) FromIndex(int index)
    {
        if (index < 0 || index >= _str.Length)
        {
            return default;
        }

        int line = FindLowerIndexInRangeArray(index, _lineToIndex);
        return (line + _origin, index - _lineToIndex[line] + _origin);
    }

    public int ToIndex(int line, int? column = null)
    {
        if (column == null)
        {
            throw new ArgumentException("Column must be provided when line is an integer.");
        }

        line -= _origin;
        column -= _origin;

        if (line >= 0 && column >= 0 && line < _lineToIndex.Length)
        {
            int lineIndex = _lineToIndex[line];
            int nextIndex = (line == _lineToIndex.Length - 1)
                ? _str.Length
                : _lineToIndex[line + 1];

            if (column < nextIndex - lineIndex)
            {
                return lineIndex + column.Value;
            }
        }

        return -1;
    }

    public int ToIndex((int line, int col) lineCol)
    {
        return ToIndex(lineCol.line, lineCol.col);
    }

    public int ToIndex(object lineColObj)
    {
        switch (lineColObj)
        {
            case (int line, int col):
                return ToIndex(line, col);

            case Dictionary<string, int> dict when dict.ContainsKey("line") && (dict.ContainsKey("col") || dict.ContainsKey("column")):
                return ToIndex(dict["line"], dict.ContainsKey("col") ? dict["col"] : dict["column"]);

            default:
                return -1;
        }
    }

    private static int[] BuildLineToIndex(string str)
    {
        var lines = str.Split('\n');
        var lineToIndex = new int[lines.Length];
        int index = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            lineToIndex[i] = index;
            index += lines[i].Length + 1; // Adding 1 for "\n"
        }

        return lineToIndex;
    }

    private static int FindLowerIndexInRangeArray(int value, int[] arr)
    {
        if (value >= arr[^1])
        {
            return arr.Length - 1;
        }

        int min = 0, max = arr.Length - 2;

        while (min < max)
        {
            int mid = min + ((max - min) >> 1);

            if (value < arr[mid])
            {
                max = mid - 1;
            }
            else if (value >= arr[mid + 1])
            {
                min = mid + 1;
            }
            else
            {
                min = mid;
                break;
            }
        }

        return min;
    }
}