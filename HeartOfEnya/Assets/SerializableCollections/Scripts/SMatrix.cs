using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SerializableCollections
{
    [System.Serializable]
    public class IntRange : IEnumerable<int>
    {
        public int Count { get { return max - min + 1; } }
        public int min;
        public int max;

        public IntRange(int min, int max)
        {
            this.min = min;
            this.max = max;
            if (!IsValid)
                throw new System.ArgumentException("Min: " + min + " and " + "Max: " + max + " is an invalid range");
        }
        public IntRange(int minMax)
        {
            min = minMax;
            max = minMax;
        }

        public bool IsValid => min <= max;
        public bool Contains(int item)
        {
            return (item >= min) && (item <= max);
        }
        /// <summary> Constrain the input such that it is inside the range </summary>
        public int Clamp(int value)
        {
            if (value <= max)
                return value >= min ? value : min;
            return max;
        }
        /// <summary> Constrain the input such that it is inside the range 
        /// and provide the overflow outside of the range as a signed scalar </summary>
        public int Clamp(int value, out int overflow)
        {
            if (value > max)
            {
                overflow = value - max;
                return max;
            }
            else if (value < min)
            {
                overflow = value - min;
                return min;
            }
            else
            {
                overflow = 0;
                return value;
            }
        }
        public void Shift(int shiftBy)
        {
            min += shiftBy;
            max += shiftBy;
        }
        /// <summary> Constrain the range such that its min is not less than the lower limit and its max is not greater than the upper limit</summary>
        public void Limit(int lowerLimit, int upperLimit)
        {
            min = Mathf.Max(min, lowerLimit);
            max = Mathf.Min(max, upperLimit);
        }

        public override string ToString() => "(" + min + " - " + max + ")";

        public IEnumerator<int> GetEnumerator()
        {
            int curr = min - 1;
            while (curr < max)
                yield return ++curr;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [System.Serializable]
    public class SMatrix2D<T> : IEnumerable<T>
    {
        [SerializeField] private T[] _data;
        [SerializeField] private int _rows;
        public int Rows { get => _rows; }
        [SerializeField] private int _cols;
        public int Columns { get => _cols; }

        public SMatrix2D(int rows, int columns)
        {
#if DEBUG
            if (rows <= 0 || columns <= 0)
                throw new System.ArgumentOutOfRangeException("Matrix dimensions must be >= 0");
#endif
            _rows = rows;
            _cols = columns;
            _data = new T[rows * columns];
        }
        private SMatrix2D(T[] data, int rows, int columns)
        {
            _rows = rows;
            _cols = columns;
            _data = data;
        }

        public bool Contains(int row, int col)
        {
            return row >= 0 && col >= 0 && row < Rows && col < Columns;
        }
        // Warning: shallow copy if reference type!
        public SMatrix2D<T> Rotated90()
        {
            T[] rot = new T[_data.Length];
            _data.CopyTo(rot, 0);
            for (IntRange range = new IntRange(0, Columns - 1); range.max < rot.Length; range.Shift(Rows))
                System.Array.Reverse(rot, range.min, Columns);
            return new SMatrix2D<T>(rot, Columns, Rows);
        }
        // Warning: shallow clone if reference type!
        public SMatrix2D<T> Rotated180()
        {
            T[] rot = new T[_data.Length];
            _data.CopyTo(rot, 0);
            System.Array.Reverse(rot);
            return new SMatrix2D<T>(rot, Rows, Columns);
        }
        // Warning: shallow clone if reference type!
        public SMatrix2D<T> RowsFlipped()
        {
            T[] rot = new T[_data.Length];
            int j = _data.Length - Columns;
            for (int i = 0; i < _data.Length; i += Columns, j -= Columns)
                System.Array.ConstrainedCopy(_data, j, rot, i, Columns);
            return new SMatrix2D<T>(rot, Rows, Columns);
        }

        public T this[int row, int col]
        {
            get
            {
                return _data[(row * Columns) + col];
            }
            set
            {
                _data[(row * Columns) + col] = value;
            }
        }
        public T[] this[int row]
        {
            get
            {
                T[] _row = new T[Rows];
                System.Array.ConstrainedCopy(_data, row * Columns, _row, 0, Rows);
                return _row;
            }
        }

        #region IEnumarable Implementation
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (int i = 0; i < _data.Length; ++i)
                yield return _data[i];
        }
        #endregion
    }

    [System.Serializable] public class BoolMatrix2D : SMatrix2D<bool> { public BoolMatrix2D(int rows, int columns) : base(rows, columns) { } }
    [System.Serializable] public class IntMatrix2D : SMatrix2D<int> { public IntMatrix2D(int rows, int columns) : base(rows, columns) { } }
    [System.Serializable] public class FloatMatrix2D : SMatrix2D<float> { public FloatMatrix2D(int rows, int columns) : base(rows, columns) { } }
    [System.Serializable] public class GOMatrix2D : SMatrix2D<GameObject> { public GOMatrix2D(int rows, int columns) : base(rows, columns) { } } 
}
