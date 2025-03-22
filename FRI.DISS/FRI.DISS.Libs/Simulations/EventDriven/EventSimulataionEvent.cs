using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRI.DISS.Libs.Simulations.EventDriven
{
    public enum EventSimulationEventPriority
    {
        Highest = 1,
        High = 2,
        Medium = 10,
        Low = 100,
        Lowest = 1000
    }
    public abstract class EventSimulataionEvent<TSimulation> :  IComparable<EventSimulataionEvent<TSimulation>> where TSimulation : EventSimulation
    {
        public TSimulation Simulation { get; init; }
        public double StartTime { get; init; }
        public EventSimulationEventPriority Priority { get; init; } = EventSimulationEventPriority.Low;

        public EventSimulataionEvent(TSimulation simulation, double startTime)
        {
            Simulation = simulation;
            StartTime = startTime;
        }

        public abstract void Execute();
        public abstract void PlanNextEvents();

        public int CompareTo(EventSimulataionEvent<TSimulation>? other)
        {
            if (other is null)
            {
                return 1;
            }

            if (StartTime < other.StartTime)
            {
                return -1;
            }

            if (StartTime > other.StartTime)
            {
                return 1;
            }

            return Priority.CompareTo(other.Priority);
        }
    }

    public class SystemEvent : EventSimulataionEvent<EventSimulation>
    {
        /// <summary>
        /// in miliseconds
        /// </summary>
        /// <value></value>
        public int SleepTime { get; set; } = 497;

        /// <summary>
        /// in Simulation time units
        /// </summary>
        /// <value></value>
        public double Gap { get; set; } = 100.0;

        public SystemEvent(EventSimulation simulation, double startTime) : base(simulation, startTime)
        {
        }

        public override void Execute()
        {
            Thread.Sleep(SleepTime);

            Simulation.OnGUIEventHappened(EventDrivenSimulationEventArgsType.RefreshTime);
        }

        public override void PlanNextEvents()
        {
            if (Simulation.TimeMode == EventDrivenSimulationTimeMode.RealTime)
            {
                // Simulation.PlanEvent(new SystemEvent(Simulation, StartTime + Gap) {SleepTime = SleepTime, Gap = Gap});
                Simulation.PlanEvent<SystemEvent>(Gap);
            }
        }
    }
}
