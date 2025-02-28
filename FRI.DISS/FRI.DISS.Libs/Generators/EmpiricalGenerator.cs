using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRI.DISS.Libs.Generators
{
    public class EmpiricalGenerator : AbstractGenerator
    {
        private UniformGenerator _probRandom;

        private UniformGenerator[] _intervalsRandoms;

        /// <summary>
        /// Minimalna hodnota (obsiahnuta v generovanych hodnotach)
        /// </summary>
        /// <value></value>
        public double Min => Borders[0];

        public double[] Borders { get; init; }

        public double[] Probabilities { get; init; }

        /// <summary>
        /// Maximalna hodnota (neoobsiahnuta v generovanych hodnotach)
        /// </summary>
        /// <value></value>
        public double Max => Borders[^1]; 



        public EmpiricalGenerator(GenerationMode mode, double[] borders, double[] probabilities, SeedGenerator seedGenerator) : base(mode)
        {
            if (borders.Length - 1 != probabilities.Length)
            {
                throw new ArgumentException("Probabilities must have one element less than borders");
            }

            if (probabilities.Sum() != 1)
            {
                throw new ArgumentException("Probabilities must sum to 1");
            }

            for (int i = 0; i < borders.Length - 1; i++)
            {
                if (borders[i] >= borders[i + 1])
                {
                    throw new ArgumentException("Borders must be in ascending order");
                }
            }

            Borders = borders;
            Probabilities = probabilities;

            _probRandom = new UniformGenerator(GenerationMode.Continuous, seedGenerator);
            
            double previous = Min;
            _intervalsRandoms = new UniformGenerator[borders.Length - 1];
            for (int i = 1; i < borders.Length; i++)
            {
                var current = borders[i];

                _intervalsRandoms[i - 1] = new UniformGenerator(mode, seedGenerator)
                {
                    Min = previous,
                    Max = current
                };

                previous = current;
            }
        }

        protected int _getIntervalIndex()
        {
            double prob = _probRandom.GetSampleDouble();

            double cumulative = 0;
            for (int i = 0; i < Probabilities.Length; i++)
            {
                cumulative += Probabilities[i];
                if (prob <= cumulative)
                {
                    return i;
                }
            }

            // should never happen
            throw new InvalidOperationException("No interval found for the given probability.");
        }

        protected override int _GetSampleInt()
        {
            return _intervalsRandoms[_getIntervalIndex()].GetSampleInt();
        }

        protected override double _GetSampleDouble()
        {
            return _intervalsRandoms[_getIntervalIndex()].GetSampleDouble();
        }
    }
}