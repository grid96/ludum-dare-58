using System;

public static class RandomDateUtil
{
    public static DateTime RandomDateTimeExp(Random random, double rangeYears = 100.0, double biasFactor = 100.0, bool useUtc = true)
    {
        if (random == null)
            throw new ArgumentNullException(nameof(random));
        if (rangeYears <= 0)
            throw new ArgumentOutOfRangeException(nameof(rangeYears));
        if (biasFactor <= 1.0)
            throw new ArgumentOutOfRangeException(nameof(biasFactor));
        double lambda = Math.Log(biasFactor) / rangeYears;
        double u = random.NextDouble();
        double cut = Math.Exp(-lambda * rangeYears);
        double tYears = -Math.Log(1.0 - u * (1.0 - cut)) / lambda;
        const double daysPerYear = 365.2425;
        var now = useUtc ? DateTime.UtcNow : DateTime.Now;
        return now.AddDays(-tYears * daysPerYear);
    }
}
