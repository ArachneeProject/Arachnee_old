using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class MiniMath
{
    /// <summary>
    /// Return the squared distance between two positions
    /// </summary>
    /// <param name="positionA"></param>
    /// <param name="positionB"></param>
    /// <returns></returns>
    public static float getSquaredDistance(Vector3 positionA, Vector3 positionB)
    {
        float X = positionA.x - positionB.x;
        float Y = positionA.y - positionB.y;
        float Z = positionA.z - positionB.z;
        return X*X + Y*Y + Z*Z;
    }

    /// <summary>
    /// Return the squared magnitude of the given vector
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static float getSquaredMagnitude(Vector3 vector)
    {
        return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
    }

    public static bool CanSee(Transform watcher, Transform target)
    {
        return Vector3.Angle(watcher.position - target.position, watcher.forward) > 90;
    }

    public static string GetHash(string text)
    {
        using (var sha = new System.Security.Cryptography.SHA256Managed())
        {
            byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hash = sha.ComputeHash(textData);
            return System.BitConverter.ToString(hash).Replace("-", String.Empty);
        }
    }
}
