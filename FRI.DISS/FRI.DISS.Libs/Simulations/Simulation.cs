using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FRI.DISS.Libs.Generators;

namespace FRI.DISS.Libs.Simulations
{
    public enum SimulationState
    {
        Created,
        Running,
        Stopping,
        Pausing,
        Paused,
        Done
    }

    public abstract class Simulation
    {
        public SimulationState State { get; protected set; } = SimulationState.Created;

        public int ReplicationsCount { get; set; } = 1000;
        public SeedGenerator SeedGenerator { get; set; } = SeedGenerator.Global;

        public abstract int ReplicationsDone { get; }


        protected abstract void _initialize();
        protected abstract double _doExperiment();

        protected virtual void _beforeSimulation() { }
        protected virtual void _afterSimulation() { }

        protected virtual void _beforeExperiment() { }
        protected virtual void _afterExperiment(int replication, double result) { }

        public abstract void RunSimulation();

        public void StopSimulation()
        {
            if (State != SimulationState.Running)
            {
                throw new InvalidOperationException("Simulation is not running");
            }

            State = SimulationState.Stopping;
        }

        public void PauseSimulation()
        {
            if (State != SimulationState.Running)
            {
                throw new InvalidOperationException("Simulation is not running");
            }

            State = SimulationState.Paused;
        }
    }
}
