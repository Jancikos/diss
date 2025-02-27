using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRI.DISS.Libs.MonteCarlo
{
    public class BuffonNeedle : MonteCarlo
    {
        public double D;
        public double L;

        private Random? _randY;
        private Random? _randAngle;

        protected override double _doExperiment()
        {
            double y = _randY!.NextDouble() * D;

            double angleInDegrees = _randAngle!.NextDouble() * 180;
            double angleInRadians = angleInDegrees * Math.PI / 180;

            double a = L * Math.Sin(angleInRadians);

            return y + a >= D ? 1 : 0;
        }

        protected override double _processExperimentResults(int repCount, double results)
        {
            var p = results / repCount;
            return (2 * L) / (p * D);
        }

        protected override void _initialize(int repCount)
        {
            _randY = new Random();
            _randAngle = new Random();
        }

        protected override void _afterReplications(int repCount, double result)
        {
            var piEst = (2 * L) / (result * D);
            Debug.WriteLine($"Pi estimation: {piEst}");
        }
    }
}
