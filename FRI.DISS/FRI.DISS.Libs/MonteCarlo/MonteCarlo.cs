using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FRI.DISS.Libs.Generators;

namespace FRI.DISS.Libs.MonteCarlo
{
    public enum MonteCarloState
    {
        Created,
        Running,
        Stopping,
        Done
    }

    public abstract class MonteCarlo
    {
        public MonteCarloState State { get; protected set; } = MonteCarloState.Created;

        public int ReplicationsCount { get; set; } = 1000;
        public int UpdateStatsInterval { get; set; } = 1000;
        public Action<MonteCarlo, int, double>? UpdateStatsCallback { get; set; }
        public SeedGenerator SeedGenerator { get; set; } = SeedGenerator.Global;

        
        public Statistics? ResultRaw { get; protected set; }
        public int ReplicationsDone => ResultRaw?.Count ?? 0;



        protected abstract void _initialize();
        protected abstract double _doExperiment();
        public virtual double ProcessExperimentResult() { return ResultRaw?.Mean ?? throw new InvalidOperationException("Simulation not run yet"); }

        protected virtual void _beforeSimulation() { }
        protected virtual void _afterSimulation() { }

        protected virtual void _beforeExperiment() { }
        protected virtual void _afterExperiment(int replication, double result) { }

        public void RunSimulation()
        {
            if (State == MonteCarloState.Running)
            {
                throw new InvalidOperationException("Simulation already running");
            }
            State = MonteCarloState.Running;

            _initialize();

            _beforeSimulation();

            ResultRaw = new Statistics();
            for (int repDone = 0; repDone < ReplicationsCount; repDone++)
            {
                if (State == MonteCarloState.Stopping)
                {
                    break;
                }

                _beforeExperiment();
                var expRawResult = _doExperiment();
                ResultRaw.AddSample(expRawResult);
                _afterExperiment(repDone, expRawResult);

                if (repDone % UpdateStatsInterval == 0 || repDone == ReplicationsCount - 1)
                {
                    UpdateStatsCallback?.Invoke(this, repDone, expRawResult);
                }
            }

            _afterSimulation();

            State = MonteCarloState.Done;
        }

        public void StopSimulation()
        {
            if (State != MonteCarloState.Running)
            {
                throw new InvalidOperationException("Simulation is not running");
            }

            State = MonteCarloState.Stopping;
        }
    }
}
