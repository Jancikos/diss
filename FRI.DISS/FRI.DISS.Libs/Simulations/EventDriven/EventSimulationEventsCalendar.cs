using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRI.DISS.Libs.Simulations.EventDriven
{
    public class EventSimulationEventsCalendar
    {
        protected PriorityQueue<EventSimulataionEvent, EventSimulataionEvent> _events = new();
        public bool IsEmpty => _events.Count == 0;

        public void PlanEvent(EventSimulataionEvent newEvent)
        {
            _events.Enqueue(newEvent, newEvent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <throws>InvalidOperationException`</throws>
        /// <returns></returns>
        public EventSimulataionEvent PopEvent()
        {
            return _events.Dequeue();
        }

        public IEnumerable<EventSimulataionEvent> GetOrderedEvents()
        {
            return _events.UnorderedItems
                            .Select(item => item.Element)
                            .OrderBy(e => e)  // Adjust this if EventSimulataionEvent has a custom ordering logic
            ;
        }
    }
}
