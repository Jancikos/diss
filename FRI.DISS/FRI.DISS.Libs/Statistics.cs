using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRI.DISS.Libs.Generators
{
    public class Statistics
    {
        public int Count { get; protected set; }
        public double Sum { get; protected set; }
        public double SumOfSquares { get; protected set; }

        public double Min { get; protected set; }
        public double Max { get; protected set; }

        public double Mean => (double)Sum / Count;
        public double Variance => (double)SumOfSquares / Count - Mean * Mean;
        public double StandardDeviation => Math.Sqrt(Variance);

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
    }
}