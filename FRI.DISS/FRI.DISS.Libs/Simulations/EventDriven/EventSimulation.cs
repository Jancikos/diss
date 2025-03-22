using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRI.DISS.Libs.Simulations.EventDriven
{
    public enum EventDrivenSimulationEventArgsType
    {
        SimulationEventDone,
        RefreshGUI,
        RefreshTime
    }
    public class EventDrivenSimulationGUIEventArgs : EventArgs
    {
        public EventDrivenSimulationEventArgsType Type { get; set; }
        public EventDrivenSimulationGUIEventArgs(EventDrivenSimulationEventArgsType type)
        {
            Type = type;
        }
    }

    public enum EventDrivenSimulationTimeMode
    {
        RealTime,
        FastForward
    }

    public abstract class EventSimulation : Simulation
    {
        /// <summary>
        /// for notifying GUI clients
        /// </summary>
        public event EventHandler<EventDrivenSimulationGUIEventArgs>? GUIEventHappened;

        public override int ReplicationsDone => throw new NotImplementedException();

        public EventDrivenSimulationTimeMode TimeMode { get; set; } = EventDrivenSimulationTimeMode.RealTime;

        private double _currentTime;
        public double CurrentTime => _currentTime;
        protected double? _endTime = null;
        public double? EndTime => _endTime;
        

        protected EventSimulationEventsCalendar? _eventsStack;
        public EventSimulationEventsCalendar EventsCalendar => _eventsStack ?? throw new InvalidOperationException("Events stack not initialized");

        public override void RunSimulation()
        {
            if (State == SimulationState.Running)
            {
                throw new InvalidOperationException("Simulation already running");
            }
            State = SimulationState.Running;

            _beforeSimulation();

            for (int repDone = 0; repDone < ReplicationsCount; repDone++)
            {

                _eventsStack = new EventSimulationEventsCalendar();
                _currentTime = 0;

                _beforeExperiment();
                
                if (TimeMode == EventDrivenSimulationTimeMode.RealTime)
                {
                    _planSystemEvent();
                }

                while (!_eventsStack.IsEmpty && !_checkStopCondition())
                {
                    if (State == SimulationState.Stopping || State == SimulationState.Pausing)
                    {
                        break;
                    }

                    var currentEvent = _eventsStack.PopEvent();
                    if (_currentTime > currentEvent.StartTime)
                    {
                        throw new InvalidOperationException("Trying to execute event in the past!!!");
                    }

                    // vykonaj event
                    currentEvent.Execute();

                    // zmen systemovy cas
                    _currentTime = currentEvent.StartTime;

                    // naplanuj dalsie eventy
                    currentEvent.PlanNextEvents();

                    // notifikuj GUI
                    OnGUIEventHappened(EventDrivenSimulationEventArgsType.SimulationEventDone);
                }

                if (State != SimulationState.Pausing)
                {
                    _afterExperiment(repDone, 0);
                }
            }
            
            if (State != SimulationState.Pausing)
            {
                _afterSimulation();
            }
        }

        private void _planSystemEvent()
        {
            PlanEvent<SystemEvent>();
        }

        public void PlanEvent<TEvent>(double inTime = 0) where TEvent : EventSimulataionEvent
        {
            var eveObject = Activator.CreateInstance(typeof(TEvent), this);

            if (eveObject is not EventSimulataionEvent simulationEvent)
            {
                throw new InvalidOperationException("Event must be of type EventSimulataionEvent");
            }

            PlanEvent(simulationEvent, inTime);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inTime">o kolko posunut zaciatok planovanej udalosti</param>
        /// <typeparam name="SimulationEvent"></typeparam>
        public void PlanEvent(EventSimulataionEvent eve, double inTime = 0)
        {
            if (inTime < 0)
            {
                throw new InvalidOperationException("Cannot plan event in the past");
            }

            eve.StartTime = CurrentTime + inTime;

            // Add the event to the events stack
            EventsCalendar.PlanEvent(eve);
        }

        /// <summary>
        /// true ak sa ma simulacia ukoncit
        /// </summary>
        /// <returns></returns>
        protected virtual bool _checkStopCondition()
        {
            if (EndTime is not null)
            {
                return CurrentTime >= EndTime;
            }

            return false;
        }

        public void OnGUIEventHappened(EventDrivenSimulationEventArgsType eventType)
        {
            OnGUIEventHappened(new EventDrivenSimulationGUIEventArgs(eventType));
        }
        public void OnGUIEventHappened(EventDrivenSimulationGUIEventArgs eventArgs)
        {
            GUIEventHappened?.Invoke(this, eventArgs);
        }

    }
}
