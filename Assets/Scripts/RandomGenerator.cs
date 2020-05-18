using UnityEngine;

public class RandomGenerator
{
    public static bool GenerateRandomDoNothing() {
        float rand = Random.Range(0f, 10f);
        if (rand > 0.3)
            return false;
        else
            return true;
    }

    public static bool GenerateRandomNegative() {
        float rand = Random.Range(0f, 10f);

        if (rand > 0.3)
            return false;
        else
            return true;
    }

    public static int GenerateRandomTimes() {
        float rand = Random.Range(0f, 10f);
        if (rand >= 1)
            return 1;
        else if (rand >= 0.3)
            return 2;
        else
            return 3;
    }

    public static int GenerateRandom(int maxNums) {
        int rand = Random.Range(0, maxNums);
        return rand;
    }

    public static int GenerateRandomCommand() {
        float rand = Random.Range(0f, 10f);

        if (rand > 0.3)
            return 1;
        else
            return 2;
    }

    public static int GenerateFinishLine() {
        int rand = Random.Range(50, 70);
        return rand;
    }

    public static bool GenerateRandomCrocodile() {
        float rand = Random.Range(0f, 10f);
        if (rand > 0.1f)
            return false;
        else
            return true;
    }
}
