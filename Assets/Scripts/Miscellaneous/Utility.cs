using System.Collections.Generic;
using UnityEngine;

public class Utility {

    // Get a number with a Normal distribution
    public static int NextGaussian(float mean, float standard_deviation, float min, float max, System.Random rng) {
        float x;
        do {
            x = NextGaussian(mean, standard_deviation, rng);
        } while (x < min || x > max);
        return Mathf.RoundToInt(x);
    }

    private static float NextGaussian(float mean, float standard_deviation, System.Random rng) {
        return mean + NextGaussian(rng) * standard_deviation;
    }

    private static float NextGaussian(System.Random rng) {
        float v1, v2, s;
        do {
            v1 = 2.0f * rng.Next(0, 100)/100f - 1.0f;
            v2 = 2.0f * rng.Next(0, 100)/100f - 1.0f;
            s = v1 * v1 + v2 * v2;
        } while (s >= 1.0f || s == 0f);

        s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);

        return v1 * s;
    }

    public static int GetRandomIntWithExclusion(int start, int end, System.Random rng, List<int> exclude) {
        int random = start + rng.Next(0, (end - start + 1 - exclude.Count));
        foreach (int ex in exclude) {
            if (random < ex) {
                break;
            }
            random++;
        }
        return random;
    }
}
