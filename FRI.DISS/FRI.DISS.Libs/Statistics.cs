using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRI.DISS.Libs.Generators
{
    public enum IntervalT_alfa
    {
        t_alfa_90,  // 90% interval
        t_alfa_95,  // 95% interval
        t_alfa_99   // 99% interval
    }

    public class Statistics
    {
        public int Count { get; protected set; }
        public double Sum { get; protected set; }
        public double SumOfSquares { get; protected set; }

        public double Min { get; protected set; }
        public double Max { get; protected set; }

        public double Mean => Count == 0
        ? 0
        : (double)Sum / Count;

        public double StandardDeviation => Math.Sqrt(SumOfSquares / Count - Math.Pow(Sum / Count, 2));
        public double SampleStandardDeviation => Math.Sqrt((SumOfSquares - (Math.Pow(Sum, 2) / Count)) / (Count - 1));

        // interval spolahlivosti
        public IntervalT_alfa CurrentIntervalT_alfa { get; set; } = IntervalT_alfa.t_alfa_95;
        public double IntervalT_alfaValue => CurrentIntervalT_alfa switch
        {
            IntervalT_alfa.t_alfa_90 => 1.645,
            IntervalT_alfa.t_alfa_95 => 1.96,
            IntervalT_alfa.t_alfa_99 => 2.576,
            _ => throw new ArgumentOutOfRangeException(nameof(CurrentIntervalT_alfa))
        };
        public bool CanCalculateInterval => Count >= 30;
        public double IntervalHalfWidth => CanCalculateInterval
            ? (IntervalT_alfaValue * SampleStandardDeviation) / Math.Sqrt(Count)
            : throw new InvalidOperationException("Interval calculation is not valid for sample sizes less than 30.");  
        public double IntervalLowerBound => Mean - IntervalHalfWidth;
        public double IntervalUpperBound => Mean + IntervalHalfWidth;
        public double IntervalWidth => IntervalHalfWidth * 2;

        public Statistics()
        {
            Count = 0;

            Min = 0;
            Max = 0;

            Sum = 0;
            SumOfSquares = 0;
        }

        public void AddSample(double sample)
        {
            Count++;
            Sum += sample;
            SumOfSquares += sample * sample;

            _updateMinMax(sample);
        }

        private void _updateMinMax(double sample)
        {
            if (Count == 1)
            {
                Min = sample;
                Max = sample;
                return;
            }

            if (sample < Min)
            {
                Min = sample;
            }

            if (sample > Max)
            {
                Max = sample;
            }
        }

        public void Reset()
        {
            Count = 0;
            Sum = 0;
            SumOfSquares = 0;
        }

        public override string ToString()
        {
            return $"Count: {Count}, Mean: {Mean}, Max: {Max}, Min: {Min}";
        }

        public string MeanToString(bool withMinMax = false)
        {
            return withMinMax
            ? $"{Mean:0.##} [{Min:0.##}; {Max:0.##}]"
            : Mean.ToString("0.####");
        }
    }
}