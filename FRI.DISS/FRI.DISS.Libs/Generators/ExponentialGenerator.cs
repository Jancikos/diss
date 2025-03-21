using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRI.DISS.Libs.Generators
{
    public class ExponentialGenerator : AbstractGenerator
    {
        protected Random _random;

        /// <summary>
        /// Lambda parameter
        /// </summary>
        /// <value></value>
        public double Lambda { get; init; }

        public ExponentialGenerator(double lambda, SeedGenerator seedGenerator) : base(GenerationMode.Continuous)
        {
            if (lambda <= 0)
                throw new ArgumentOutOfRangeException(nameof(lambda), "Lambda must be greater than 0.");

            Lambda = lambda;
            _random = seedGenerator.GetRandom();
        }

        protected override int _GetSampleInt()
        {
            throw new NotImplementedException("ExponentialGenerator does not support integer generation");
        }

        protected override double _GetSampleDouble()
        {
            // https://en.wikipedia.org/wiki/Exponential_distribution
            return -Math.Log(1 - _random.NextDouble()) / Lambda;
        }
    }
}