using UnityEngine;

/// <summary>
/// Utility methods to generate Gaussian (normal) distributed random values
/// and helper to generate XZ positions following a Gaussian distribution.
/// </summary>
public static class Gauss
{
    /// <summary>
    /// Returns a single-sample Gaussian (normal) random value using the Box-Muller transform.
    /// </summary>
    public static float NextGaussian(float mean = 0f, float stdDev = 1f)
    {
        // Use two uniform (0,1] values
        float u1 = 1f - Random.value; // avoid 0
        float u2 = 1f - Random.value;
        float randStdNormal = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(2f * Mathf.PI * u2);
        return mean + stdDev * randStdNormal;
    }

    /// <summary>
    /// Samples a Gaussian and attempts to return a value inside [min,max]. If it fails after maxAttempts,
    /// the sampled value is clamped to the range as a fallback.
    /// </summary>
    public static float NextGaussianClamped(float mean, float stdDev, float min, float max, int maxAttempts = 10)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            float v = NextGaussian(mean, stdDev);
            if (v >= min && v <= max) return v;
        }
        // fallback
        return Mathf.Clamp(NextGaussian(mean, stdDev), min, max);
    }

    /// <summary>
    /// Generates a Vector3 on the XZ plane using independent Gaussian distributions for X and Z.
    /// Values are clamped/truncated to the supplied min/max ranges.
    /// </summary>
    public static Vector3 NextGaussianXZ(Vector2 center, float stdX, float stdZ, float minX, float maxX, float minZ, float maxZ, float y = 0f, int attempts = 10)
    {
        float x = NextGaussianClamped(center.x, stdX, minX, maxX, attempts);
        float z = NextGaussianClamped(center.y, stdZ, minZ, maxZ, attempts);
        return new Vector3(x, y, z);
    }
}
