using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRI.DISS.Libs.Simulations.EventDriven
{
    public enum EventDrivenSimulationEventArgsType
    {
        SimulationEventDone,
        SimulationExperimentDone,
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
    
    public enum EventDrivenSimulationRealTimeRatios
    {   
        Slower10x = -10,
        Slower5x = -5,
        Slower2x = -2,
        Regular = 1,
        Fatster2x = 2,
        Faster5x = 5,
        Faster10x = 10,
        Faster100x = 100,
        Faster500x = 500,
        Faster1000x = 1000,
        Faster5000x = 5000,
        Faster10000x = 10000,
        Faster50000x = 50000,
        Faster100000x = 100000,
        Faster250000x = 250000,
        Faster500000x = 500000,
        Faster1000000x = 1000000
    }

    public abstract class EventSimulation : Simulation
    {
        /// <summary>
        /// for notifying GUI clients
        /// </summary>
        public event EventHandler<EventDrivenSimulationGUIEventArgs>? GUIEventHappened;

        protected int _replicationsDone = 0;
        public override int ReplicationsDone => _replicationsDone;

        public EventDrivenSimulationTimeMode TimeMode { get; set; } = EventDrivenSimulationTimeMode.RealTime;

        private double _currentTime;
        public double CurrentTime => _currentTime;
        public abstract string CurrentTimeFormatted { get; }
        protected double? _endTime = null;
        public double? EndTime => _endTime;
        

        public EventSimulationEventsCalendar? _eventsStack; // public only for debugging purposes
        public EventSimulationEventsCalendar EventsCalendar => _eventsStack ?? throw new InvalidOperationException("Events stack not initialized");
        protected SystemEvent _systemEvent;
        public EventDrivenSimulationRealTimeRatios RealTimeRatio 
        {
            get => _systemEvent.Ratio;
            set => _systemEvent.Ratio = value;
        }

        protected EventSimulation()
        {
            _systemEvent = new SystemEvent(this);
        }
        protected override void _beforeSimulation() 
        {
            _replicationsDone = 0;
         }

        protected override void _beforeExperiment() 
        {
            _eventsStack = new EventSimulationEventsCalendar();
            _currentTime = 0;
         }

        protected override void RunSimulation()
        {
            var initialState = State;
            if (State == SimulationState.Running)
                throw new InvalidOperationException("Simulation already running");

            if (State == SimulationState.Starting)
                _beforeSimulation();

            State = SimulationState.Running;
            var firstRun = true;
            for (int repDone = _replicationsDone; repDone < ReplicationsCount && State == SimulationState.Running; repDone++)
            {
                if (!firstRun || initialState == SimulationState.Starting)
                {
                    _beforeExperiment();
                
                    if (TimeMode == EventDrivenSimulationTimeMode.RealTime)
                    {
                        _planSystemEvent();
                    }
                }

                while (!_eventsStack!.IsEmpty && !_checkStopCondition())
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

                    // zmen systemovy cas
                    _currentTime = currentEvent.StartTime;

                    // vykonaj event
                    currentEvent.Execute();

                    // naplanuj dalsie eventy
                    currentEvent.PlanNextEvents();

                    // notifikuj GUI
                    if (TimeMode == EventDrivenSimulationTimeMode.RealTime)
                    {
                        OnGUIEventHappened(EventDrivenSimulationEventArgsType.SimulationEventDone);
                    }
                }

                if (State == SimulationState.Running)
                {
                    _afterExperiment(repDone, 0);
                    
                    _replicationsDone++;

                    // notifikuj GUI
                    OnGUIEventHappened(EventDrivenSimulationEventArgsType.SimulationExperimentDone);
                }

                firstRun = false;
            }
            
            if (State == SimulationState.Pausing)
            {
                State = SimulationState.Paused;
            } else {
                _afterSimulation();
                State = SimulationState.Done;
            }

            OnGUIEventHappened(EventDrivenSimulationEventArgsType.RefreshGUI);

        }

        private void _planSystemEvent()
        {
            PlanEvent(_systemEvent);
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
            Debug.WriteLine("EventSimGUI evetn: " + eventArgs.Type);
            GUIEventHappened?.Invoke(this, eventArgs);
        }
    }
}
