using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Falloff
{
    public static float EvaluateWorldFalloffMap(int x, int y, int worldSize, float mainlandSize, float falloffTransitionWidth)
    {
        float centerX = worldSize / 2f;
        float centerY = worldSize / 2f;

        float distanceX = (x - centerX) / (worldSize * mainlandSize / 2f);
        float distanceY = (y - centerY) / (worldSize * mainlandSize / 2f);

        float distance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);

        float falloffValue = Mathf.Pow(distance, falloffTransitionWidth);

        return Mathf.Clamp01(falloffValue);
    }
}
