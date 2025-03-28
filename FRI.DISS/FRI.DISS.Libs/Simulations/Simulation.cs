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
        Starting,
        Resuming,
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

        protected virtual void _beforeSimulation() { }
        protected virtual void _afterSimulation() { }

        protected virtual void _beforeExperiment() { }
        protected virtual void _afterExperiment(int replication, double result) { }

        protected abstract void RunSimulation();

        public void StartSimulation()
        {
            if (State != SimulationState.Created && State != SimulationState.Done)
            {
                throw new InvalidOperationException("Simulation is not is state created or done");
            }

            State = SimulationState.Starting;
            RunSimulation();
        }

        public void StopSimulation()
        {
            if (State != SimulationState.Running)
            {
                throw new InvalidOperationException("Simulation is not is state running");
            }

            State = SimulationState.Stopping;
        }

        public void PauseSimulation()
        {
            if (State != SimulationState.Running)
            {
                throw new InvalidOperationException("Simulation is not is state running");
            }

            State = SimulationState.Pausing;
        }
        
        public void ResumeSimulation()
        {
            if (State != SimulationState.Paused)
            {
                throw new InvalidOperationException("Simulation is not is state paused");
            }

            State = SimulationState.Resuming;
            RunSimulation();
        }
    }
}
