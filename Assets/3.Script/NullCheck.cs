using UnityEngine;

public static class NullCheck
{
public static bool IsNull<T>(T obj, string message = "Null reference detected!")
    {
        // Use 'object.Equals' or 'EqualityComparer' to catch Unity's "fake" nulls in generics
        bool isActuallyNull = System.Collections.Generic.EqualityComparer<T>.Default.Equals(obj, default(T));
        
        // Additional check for Unity Objects (which might be destroyed but not null)
        if (!isActuallyNull && obj is UnityEngine.Object unityObj)
        {
            isActuallyNull = unityObj == null;
        }

        if (isActuallyNull)
        {
            // This format allows the Unity Console to track the call stack
            Debug.LogWarning($"<b>[NullCheck]</b> {message}\n" + 
                             $"Context: {StackTraceUtility.ExtractStackTrace()}");
            return true;
        }
        return false;
    }
}
