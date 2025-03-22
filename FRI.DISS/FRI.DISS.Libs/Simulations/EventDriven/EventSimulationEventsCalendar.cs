using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRI.DISS.Libs.Simulations.EventDriven
{
    public class EventSimulationEventsCalendar
    {
        protected PriorityQueue<EventSimulataionEvent<EventSimulation>, EventSimulataionEvent<EventSimulation>> _events = new();
        public bool IsEmpty => _events.Count == 0;

        public void PlanEvent(EventSimulataionEvent<EventSimulation> newEvent)
        {
            _events.Enqueue(newEvent, newEvent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <throws>InvalidOperationException`</throws>
        /// <returns></returns>
        public EventSimulataionEvent<EventSimulation> PopEvent()
        {
            return _events.Dequeue();
        }
    }
}
