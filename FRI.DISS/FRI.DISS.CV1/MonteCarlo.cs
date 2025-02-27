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

        public bool IsRunning { get; protected set; }
        protected abstract void _initialize(int repCount);
        protected abstract double _doExperiment();
        protected virtual double _processExperimentResults(int repCount, double results) { return results / repCount;}

        protected virtual void _beforeReplications(int repCount) { }
        protected virtual void _afterReplications(int repCount, double result) { }

        public double RunExperiment(int repCount)
        {
            IsRunning = true;
            _initialize(repCount);

            _beforeReplications(repCount);

            double results = 0.0;
            int repDone;
            for (repDone = 0; repDone < repCount; repDone++)
            {
                if (!IsRunning)
                {
                    break;
                }

                results += _doExperiment();

                if (repDone % UpdateStatsInterval == 0)
                {
                    UpdateStatsCallback?.Invoke(repDone, _processExperimentResults(repDone, results));
                }
            }

            _afterReplications(repDone, results);

            var result = _processExperimentResults(repDone, results);
            UpdateStatsCallback?.Invoke(repDone, result);
            IsRunning = false;
            return result;
        }

        public void StopExperiment()
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException("Experiment is not running");
            }

            IsRunning = false;
        }
    }
}
