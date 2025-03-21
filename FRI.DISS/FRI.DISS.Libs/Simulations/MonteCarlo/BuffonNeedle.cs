using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FRI.DISS.Libs.Generators;

namespace FRI.DISS.Libs.Simulations.MonteCarlo
{
    public class BuffonNeedle : MonteCarlo
    {
        public double D;
        public double L;

        private UniformGenerator? _randY;
        private UniformGenerator? _randAngle;

        protected override double _doExperiment()
        {
            double y = _randY!.GetSampleDouble();

            double angleInDegrees = _randAngle!.GetSampleDouble();
            double angleInRadians = angleInDegrees * Math.PI / 180;

            double a = L * Math.Sin(angleInRadians);

            return y + a >= D ? 1 : 0;
        }

        public override double ProcessExperimentResult()
        {
            var p = base.ProcessExperimentResult();
            return 2 * L / (p * D);
        }

        protected override void _initialize()
        {
            _randY = new UniformGenerator(GenerationMode.Continuous, SeedGenerator) { Min = 0, Max = D };
            _randAngle = new UniformGenerator(GenerationMode.Continuous, SeedGenerator) { Min = 0, Max = 180 };
        }

        protected override void _afterSimulation()
        {
            Debug.WriteLine($"Pi estimation: {ProcessExperimentResult()}");
        }
    }
}
