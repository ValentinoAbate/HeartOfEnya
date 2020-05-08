using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MathUtils
{
    [System.Serializable]
    public class FloatRange
    {
        public float min;
        public float max;

        public FloatRange(float min, float max)
        {
            this.min = min;
            this.max = max;
            if (!IsValid())
                throw new System.ArgumentException("Min: " + min + " and " + "Max: " + max + " is an invalid range");
        }
        public FloatRange(float minMax)
        {
            min = minMax;
            max = minMax;
        }

        public bool IsValid()
        {
            return min <= max;
        }
        public bool Contains(float item)
        {
            return (item >= min) && (item <= max);
        }
        public float Clamp(float value)
        {
            if (value <= max)
                return value >= min ? value : min;
            return max;
        }
        public void Shift(float shiftBy)
        {
            min += shiftBy;
            max += shiftBy;
        }
        public FloatRange Shifted(float shiftBy)
        {
            return new FloatRange(min + shiftBy, max + shiftBy);
        }
        public float Lerp(float t)
        {
            return Mathf.Lerp(min, max, t);
        }
    }

    [System.Serializable]
    public class IntRange
    {
        public int Count { get { return max - min + 1; } }
        public int min;
        public int max;

        public IntRange(int min, int max)
        {
            this.min = min;
            this.max = max;
            if (!isValid())
                throw new System.ArgumentException("Min: " + min + " and " + "Max: " + max + " is an invalid range");
        }
        public IntRange(int minMax)
        {
            min = minMax;
            max = minMax;
        }

        public bool isValid()
        {
            return min <= max;
        }
        public bool contains(int item)
        {
            return (item >= min) && (item <= max);
        }
        public int clamp(int value)
        {
            if (value <= max)
                return value >= min ? value : min;
            return max;
        }
        public void shift(int shiftBy)
        {
            min += shiftBy;
            max += shiftBy;
        }
    }

    [System.Serializable]
    public class PointVector
    {
        /// <summary>
        /// change point rotated around pivot point by angle
        /// </summary>
        /// <param name="pivotPoint">anchor position</param>
        /// <param name="angle">angle</param>
        /// <param name="changePoint">old destination</param>
        /// <returns>Vector2 new position</returns>
        public Vector2 RotatePoint(Vector2 pivotPoint, float angle, Vector2 changePoint)
        {
            // sin and cos
            float sin = Mathf.Sin(Mathf.Deg2Rad * angle);
            float cos = Mathf.Cos(Mathf.Deg2Rad * angle);

            // translate point back to origin
            changePoint.x -= pivotPoint.x;
            changePoint.y -= pivotPoint.y;

            // rotate point
            float xnew = changePoint.x * cos - changePoint.y * sin;
            float ynew = changePoint.x * sin + changePoint.y * cos;

            // return new vector
            // after readjusting from pivot
            return new Vector2(xnew + pivotPoint.x, ynew + pivotPoint.y);
        }

        /// <summary>
        /// vector to angle in degreses
        /// </summary>
        /// <param name="angle">as a normal vector</param>
        /// <returns>Float</returns>
        public float Vec2Ang(Vector2 angle)
        {
            return Mathf.Atan2(angle.y, angle.x) * Mathf.Rad2Deg;
        }
        /// <summary>
        /// angle to vector
        /// </summary>
        /// <param name="angle">angle in degrees</param>
        /// <returns></returns>
        public Vector2 Ang2Vec(float angle)
        {
            return new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
        }
    }
}


