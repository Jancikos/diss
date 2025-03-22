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
    public abstract class EventSimulataionEvent :  IComparable<EventSimulataionEvent>
    {
        public double StartTime { get; set; }
        public EventSimulationEventPriority Priority { get; set; } = EventSimulationEventPriority.Low;

        public abstract void Execute();
        public abstract void PlanNextEvents();

        public int CompareTo(EventSimulataionEvent? other)
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

    public abstract class GenericEventSimulataionEvent : EventSimulataionEvent
    {
        public EventSimulation Simulation { get; set; }

        public GenericEventSimulataionEvent(EventSimulation simulation)
        {
            Simulation = simulation;
        }
    }

    public class SystemEvent : GenericEventSimulataionEvent
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

        public SystemEvent(EventSimulation simulation) : base(simulation)
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
                Simulation.PlanEvent(new SystemEvent(Simulation) {SleepTime = SleepTime, Gap = Gap}, Gap);
                // Simulation.PlanEvent<SystemEvent>(Gap);
            }
        }
    }
}
