using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRI.DISS.Libs.Generators
{
    public class TriangularGenerator : AbstractGenerator
    {
        private Random _random;

        public double Min { get; init; }
        public double Max { get; init; }

        /// <summary>
        /// Stredna hodnota (modus) rozdelenia
        /// </summary>
        /// <value></value>
        public double ModeT { get; init; }

        public double Fc => (ModeT - Min) / (Max - Min);

        public TriangularGenerator(double min, double max, double mode, SeedGenerator seedGenerator) : base(GenerationMode.Continuous)
        {
            if (min >= mode || mode >= max)
                throw new ArgumentOutOfRangeException("Min, Mode and Max must be in ascending order.");

            Min = min;
            Max = max;
            ModeT = mode;

            _random = seedGenerator.GetRandom();
        }

        protected override int _GetSampleInt()
        {
            throw new NotImplementedException("TriangularGenerator does not support integer generation");
        }
        

        protected override double _GetSampleDouble()
        {
            double u = _random.NextDouble();
            
            // https://en.wikipedia.org/wiki/Triangular_distribution
            if (u < Fc)
            {
                return Min + Math.Sqrt(u * (Max - Min) * (ModeT - Min));
            }

            return Max - Math.Sqrt((1 - u) * (Max - Min) * (Max - ModeT));
        }
    }
}