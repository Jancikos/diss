using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRI.DISS.Libs.Generators
{
    public class SeedGenerator : AbstractGenerator
    {
        public static SeedGenerator Global { get; private set; } = new SeedGenerator();
        public static void SetGlobalSeed(int seed)
        {
            Global = new SeedGenerator(seed);
        }

        private Random _random;

        private int _seedGeneratedCount = 0;
        public int SeedGeneratedCount => _seedGeneratedCount;

        public SeedGenerator(int seed = 0) : base(GenerationMode.Discrete)
        {
            _random = new Random(seed);
        }

        /// <summary>
        /// </summary>
        /// <returns>correctly initialized Random object</returns>
        public Random GetRandom()
        {
            return new Random(GetSampleInt());
        }

        protected override int _GetSampleInt()
        {
            _seedGeneratedCount++;
            return _random.Next();
        }

        protected override double _GetSampleDouble() => throw new NotImplementedException("SeedGenerator does not support continuous generation");
    }
}