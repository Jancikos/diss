using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRI.DISS.Libs.Generators
{
    public class UniformGenerator : AbstractGenerator
    {
        private Random _random;

        /// <summary>
        /// Minimalna hodnota (obsiahnuta v generovanych hodnotach)
        /// </summary>
        /// <value></value>
        public double Min { get; init; } = 0;

        /// <summary>
        /// Maximalna hodnota (neoobsiahnuta v generovanych hodnotach)
        /// </summary>
        /// <value></value>
        public double Max { get; init; } = 1;

        public UniformGenerator(GenerationMode mode, SeedGenerator seedGenerator) : base(mode)
        {
            _random = seedGenerator.GetRandom();
        }

        protected override int _GetSampleInt()
        {
            return _random.Next((int)Min, (int)Max);
        }

        protected override double _GetSampleDouble()
        {
            return Min + _random.NextDouble() * (Max - Min);
        }
    }
}