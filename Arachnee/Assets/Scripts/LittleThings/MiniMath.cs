﻿using System;
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


    
}
