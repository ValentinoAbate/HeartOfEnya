using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SerializableCollections.Utilities
{
    /// <summary> A class containing enum extension methods and static methods pertaining to enum types </summary>
    public static class EnumUtils
    {
        /// <summary> Gets and IEnumerable containing all values in this enum (for iteration/foreach loops) </summary>
        public static IEnumerable<T> GetValues<T>() where T : Enum
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        /// <summary> Quickly returns the number of items defined in this enum </summary>
        public static int Count<T>() where T : Enum
        {
            return Enum.GetNames(typeof(T)).Length;
        }

        /// <summary> Extension method to all enums to get the next value
        /// Wraps around to the beginning if this is the last element </summary>
        public static T Next<T>(this T e) where T : Enum
        {
            var Arr = (T[])Enum.GetValues(typeof(T));
            int i = Array.IndexOf(Arr, e) + 1;
            return (Arr.Length == i) ? Arr[0] : Arr[i];
        }

        /// <summary> Extension method to all enums to get the previous value 
        /// Wraps around to the end if this is the first element </summary>
        public static T Previous<T>(this T e) where T : Enum
        {
            var Arr = (T[])Enum.GetValues(typeof(T));
            int i = Array.IndexOf(Arr, e) - 1;
            return (i == -1) ? Arr[Arr.Length - 1] : Arr[i];
        }

        /// <summary> Extension method to all enums. 
        /// gets the value ahead/behind of the given value by offset places
        /// Wraps around to the end if this is the first element </summary>
        public static T Offset<T>(this T e, int offset) where T : Enum
        {
            var Arr = (T[])Enum.GetValues(typeof(T));
            int i = (Array.IndexOf(Arr, e) + offset);
            if (i >= Arr.Length)
                return Arr[i % Arr.Length];
            if (i < 0)
                return Arr[Arr.Length + (i % Arr.Length)];
            return Arr[i];
        }
    }

}