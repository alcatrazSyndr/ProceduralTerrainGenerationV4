using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HydraulicErosion
{
    private static int[][] erosionBrushIndices;
    private static float[][] erosionBrushWeights;

    public static float[,] HydraulicErosionSimulation(float[,] map, int mapSize, int numIterations)
    {
        System.Random prng = new System.Random();
        int erosionRadius = 3;

        InitializeBrushIndices(mapSize, erosionRadius);

        float inertia = .05f;
        float sedimentCapacityFactor = 4;
        float minSedimentCapacity = .01f;
        float erodeSpeed = .3f;
        float depositSpeed = .3f;
        float evaporateSpeed = .01f;
        float gravity = 4;
        int maxDropletLifetime = 30;

        float initialWaterVolume = 1;
        float initialSpeed = 1;

        for (int iteration = 0; iteration < numIterations; iteration++)
        {
            float posX = prng.Next(0, mapSize - 1);
            float posY = prng.Next(0, mapSize - 1);
            float dirX = 0;
            float dirY = 0;
            float speed = initialSpeed;
            float water = initialWaterVolume;
            float sediment = 0;

            for (int lifetime = 0; lifetime < maxDropletLifetime; lifetime++)
            {
                int nodeX = (int)posX;
                int nodeY = (int)posY;

                float cellOffsetX = posX - nodeX;
                float cellOffsetY = posY - nodeY;

                HeightAndGradient heightAndGradient = CalculateHeightAndGradient(map, mapSize, posX, posY);

                dirX = (dirX * inertia - heightAndGradient.gradientX * (1 - inertia));
                dirY = (dirY * inertia - heightAndGradient.gradientY * (1 - inertia));

                float len = Mathf.Sqrt(dirX * dirX + dirY * dirY);
                if (len != 0)
                {
                    dirX /= len;
                    dirY /= len;
                }

                posX += dirX;
                posY += dirY;

                if ((dirX == 0 && dirY == 0) || posX < 0 || posX >= mapSize - 1 || posY < 0 || posY >= mapSize - 1)
                {
                    break;
                }

                float newHeight = CalculateHeightAndGradient(map, mapSize, posX, posY).height;
                float deltaHeight = newHeight - heightAndGradient.height;

                float sedimentCapacity = Mathf.Max(-deltaHeight * speed * water * sedimentCapacityFactor, minSedimentCapacity);

                if (sediment > sedimentCapacity || deltaHeight > 0)
                {
                    float amountToDeposit = (deltaHeight > 0) ? Mathf.Min(deltaHeight, sediment) : (sediment - sedimentCapacity) * depositSpeed;
                    sediment -= amountToDeposit;

                    DepositSediment(map, mapSize, nodeX, nodeY, cellOffsetX, cellOffsetY, amountToDeposit);

                }
                else
                {
                    float amountToErode = Mathf.Min((sedimentCapacity - sediment) * erodeSpeed, -deltaHeight);
                    Erode(map, mapSize, nodeX, nodeY, amountToErode);
                    sediment += amountToErode;
                }

                speed = Mathf.Sqrt(speed * speed + deltaHeight * gravity);
                water *= (1 - evaporateSpeed);
            }
        }

        return map;
    }

    private static void DepositSediment(float[,] map, int mapSize, int nodeX, int nodeY, float cellOffsetX, float cellOffsetY, float amountToDeposit)
    {
        map[nodeY, nodeX] += amountToDeposit * (1 - cellOffsetX) * (1 - cellOffsetY);
        map[nodeY, nodeX + 1] += amountToDeposit * cellOffsetX * (1 - cellOffsetY);
        map[nodeY + 1, nodeX] += amountToDeposit * (1 - cellOffsetX) * cellOffsetY;
        map[nodeY + 1, nodeX + 1] += amountToDeposit * cellOffsetX * cellOffsetY;
    }

    private static void Erode(float[,] map, int mapSize, int nodeX, int nodeY, float amountToErode)
    {
        for (int brushPointIndex = 0; brushPointIndex < erosionBrushIndices[nodeY * mapSize + nodeX].Length; brushPointIndex++)
        {
            int nodeIndex = erosionBrushIndices[nodeY * mapSize + nodeX][brushPointIndex];
            int targetX = nodeIndex % mapSize;
            int targetY = nodeIndex / mapSize;
            float weighedErodeAmount = amountToErode * erosionBrushWeights[nodeY * mapSize + nodeX][brushPointIndex];
            float deltaSediment = (map[targetY, targetX] < weighedErodeAmount) ? map[targetY, targetX] : weighedErodeAmount;
            map[targetY, targetX] -= deltaSediment;
        }
    }

    private static HeightAndGradient CalculateHeightAndGradient(float[,] map, int mapSize, float posX, float posY)
    {
        int coordX = (int)posX;
        int coordY = (int)posY;

        float x = posX - coordX;
        float y = posY - coordY;

        float heightNW = map[coordY, coordX];
        float heightNE = map[coordY, coordX + 1];
        float heightSW = map[coordY + 1, coordX];
        float heightSE = map[coordY + 1, coordX + 1];

        float gradientX = (heightNE - heightNW) * (1 - y) + (heightSE - heightSW) * y;
        float gradientY = (heightSW - heightNW) * (1 - x) + (heightSE - heightNE) * x;

        float height = heightNW * (1 - x) * (1 - y) + heightNE * x * (1 - y) + heightSW * (1 - x) * y + heightSE * x * y;

        return new HeightAndGradient() { height = height, gradientX = gradientX, gradientY = gradientY };
    }

    private static void InitializeBrushIndices(int mapSize, int radius)
    {
        erosionBrushIndices = new int[mapSize * mapSize][];
        erosionBrushWeights = new float[mapSize * mapSize][];

        int[] xOffsets = new int[radius * radius * 4];
        int[] yOffsets = new int[radius * radius * 4];
        float[] weights = new float[radius * radius * 4];
        float weightSum = 0;
        int addIndex = 0;

        for (int i = 0; i < erosionBrushIndices.GetLength(0); i++)
        {
            int centreX = i % mapSize;
            int centreY = i / mapSize;

            if (centreY <= radius || centreY >= mapSize - radius || centreX <= radius + 1 || centreX >= mapSize - radius)
            {
                weightSum = 0;
                addIndex = 0;
                for (int y = -radius; y <= radius; y++)
                {
                    for (int x = -radius; x <= radius; x++)
                    {
                        float sqrDst = x * x + y * y;
                        if (sqrDst < radius * radius)
                        {
                            int coordX = centreX + x;
                            int coordY = centreY + y;

                            if (coordX >= 0 && coordX < mapSize && coordY >= 0 && coordY < mapSize)
                            {
                                float weight = 1 - Mathf.Sqrt(sqrDst) / radius;
                                weightSum += weight;
                                weights[addIndex] = weight;
                                xOffsets[addIndex] = x;
                                yOffsets[addIndex] = y;
                                addIndex++;
                            }
                        }
                    }
                }
            }

            int numEntries = addIndex;
            erosionBrushIndices[i] = new int[numEntries];
            erosionBrushWeights[i] = new float[numEntries];

            for (int j = 0; j < numEntries; j++)
            {
                erosionBrushIndices[i][j] = (yOffsets[j] + centreY) * mapSize + xOffsets[j] + centreX;
                erosionBrushWeights[i][j] = weights[j] / weightSum;
            }
        }
    }

    struct HeightAndGradient
    {
        public float height;
        public float gradientX;
        public float gradientY;
    }
}
