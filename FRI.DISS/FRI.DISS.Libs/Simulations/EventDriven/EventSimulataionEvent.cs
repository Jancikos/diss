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
    public abstract class EventSimulataionEvent : IComparable<EventSimulataionEvent>
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

        public override string ToString()
        {
            return $"{GetType().Name} at {StartTime:F2}";
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
        public EventDrivenSimulationRealTimeRatios Ratio { get; set; } = EventDrivenSimulationRealTimeRatios.Regular;

        /// <summary>
        /// in miliseconds
        /// </summary>
        /// <value></value>
        public int SleepTime
        {
            get
            {
                int multiplier = 1;

                return DefaultSleepTime * multiplier;
            }
        }
        protected int DefaultSleepTime => 1000 / FPS;

        /// <summary>
        /// in Simulation time units (seconds)
        /// </summary>
        /// <value></value>
        public double Gap
        {
            get
            {
                int multiplier = (int)Ratio;

                if (multiplier < 0)
                {
                    return DefaultGap / Math.Abs(multiplier);
                }

                return DefaultGap * multiplier;
            }
        }
        public double DefaultGap => 1 / (double)FPS;

        /// <summary>
        /// how many times per second the event should be executed (screen refresh rate)
        /// </summary>
        public int FPS => 4;

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
                Simulation.PlanEvent(this, Gap);
            }
        }
    }
}
