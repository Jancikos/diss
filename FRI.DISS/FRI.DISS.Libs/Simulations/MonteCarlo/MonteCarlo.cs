using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FRI.DISS.Libs.Generators;

namespace FRI.DISS.Libs.Simulations.MonteCarlo
{
    public abstract class MonteCarlo : Simulation
    {
        public int UpdateStatsInterval { get; set; } = 100;
        public Action<MonteCarlo, int, double>? UpdateStatsCallback { get; set; }

        public Statistics? ResultRaw { get; protected set; }
        public override int ReplicationsDone => ResultRaw?.Count ?? 0;

        public virtual double ProcessExperimentResult() { return ResultRaw?.Mean ?? throw new InvalidOperationException("Simulation not run yet"); }

        protected abstract double _doExperiment();

        protected override void RunSimulation()
        {
            if (State == SimulationState.Running)
            {
                throw new InvalidOperationException("Simulation already running");
            }
            State = SimulationState.Running;

            _beforeSimulation();

            ResultRaw = new Statistics();
            for (int repDone = 0; repDone < ReplicationsCount; repDone++)
            {
                if (State == SimulationState.Stopping)
                {
                    break;
                }

                _beforeExperiment();
                var expRawResult = _doExperiment();
                ResultRaw.AddSample(expRawResult);
                _afterExperiment(repDone, expRawResult);

                if (repDone % UpdateStatsInterval == 0 || repDone == ReplicationsCount - 1)
                {
                    UpdateStatsCallback?.Invoke(this, repDone + 1, expRawResult);
                }
            }

            _afterSimulation();

            State = SimulationState.Done;
        }
    }
}
