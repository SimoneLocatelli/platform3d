using UnityEngine;

public static class FloatRounding
{
    public static float Round(float value, int digits)
    {
        CustomAssert.IsNotNegative(digits, nameof(digits));

        switch (digits)
        {
            case 0: return Mathf.Round(value);
            case 1: return Mathf.Round(value * 10) * 0.1f;
            case 2: return Mathf.Round(value * 100) * 0.01f;

            default:
                var factor = Mathf.Pow(10, digits);
                return Mathf.Round(value * factor) / factor;
        }
    }
}