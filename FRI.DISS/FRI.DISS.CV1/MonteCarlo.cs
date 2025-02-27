using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRI.DISS.CV1
{
    abstract class MonteCarlo
    {
        public int UpdateStatsInterval { get; set; } = 1000;
        public Action<int, double>? UpdateStatsCallback { get; set; }

        protected abstract void _initialize(int repCount);
        protected abstract double _doExperiment();
        protected virtual double _processExperimentResults(int repCount, double results) { return results / repCount;}

        protected virtual void _beforeReplications(int repCount) { }
        protected virtual void _afterReplications(int repCount, double result) { }

        public double RunExperiment(int repCount)
        {
            _initialize(repCount);

            _beforeReplications(repCount);

            double results = 0.0;
            for (int i = 0; i < repCount; i++)
            {
                results += _doExperiment();

                if (i % UpdateStatsInterval == 0)
                {
                    UpdateStatsCallback?.Invoke(i, _processExperimentResults(i, results));
                }
            }

            _afterReplications(repCount, results);

            var result = _processExperimentResults(repCount, results);
            UpdateStatsCallback?.Invoke(repCount, result);
            return result;
        }
    }
}
