using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FRI.DISS.Libs.Generators;

namespace FRI.DISS.Libs.Simulations.EventDriven
{
    public class NabytokSimulation : EventSimulation
    {
        public override string CurrentTimeFormatted => throw new NotImplementedException();

        #region ExperimentData

        protected class ExperimentData
        {
            public int ObjednavkyRecieved { get; set; } = 0;
            public int ObjednavkyDone { get; set; } = 0;

            public List<bool> Workplaces { get; set; } = new List<bool>();

            public List<Stolar> StolariA { get; set; } = new List<Stolar>();
            public Queue<Objednavka> StolariAQueue { get; set; } = new Queue<Objednavka>();

            public List<Stolar> StolariB { get; set; } = new List<Stolar>();
            public Queue<Objednavka> StolariBQueue { get; set; } = new Queue<Objednavka>();

            public List<Stolar> StolariC { get; set; } = new List<Stolar>();
            public Queue<Objednavka> StolariCQueue { get; set; } = new Queue<Objednavka>();
        }

        protected class Objednavka
        {
            public int Id { get; set; }
            public int Workplace { get; init; }
            public Nabytok Nabytok { get; init; }

            public ObjednavkaStatus Status { get; set; } = ObjednavkaStatus.Vytvorena;

            public double CreationTime { get; init; }
        }

        protected enum ObjednavkaStatus
        {
            Vytvorena,
            Narezana,
            Namorena,
            Poskladana,
            Ukoncena
        }

        protected enum Nabytok
        {
            Stol,
            Stolicka,
            Skrina
        }

        protected class Stolar
        {
            public int Id { get; init; }
            public StolarType Type { get; init; }

            public int CurrentPlace { get; set; }= 0;
            public bool IsWorking { get; set; } = false;
        }

        protected enum StolarType
        {
            A, 
            B,
            C
        }

        #endregion

        #region Events
        protected abstract class NabytokSimulationEvent : EventSimulataionEvent
        {
            public NabytokSimulation Simulation { get; init; }

            public NabytokSimulationEvent(NabytokSimulation simulation) : base()
            {
                Simulation = simulation;
            }
        }
        #endregion
    }
}