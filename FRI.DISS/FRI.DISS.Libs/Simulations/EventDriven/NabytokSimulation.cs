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
        // pracovna doba je denne od 6:00 do 14:00
        public override string CurrentTimeFormatted => throw new NotImplementedException();

        protected override void _beforeSimulation()
        {
            base._beforeSimulation();
        }

        #region Generators
        protected class Generators(SeedGenerator seedGenerator)
        {
            // objednavky
            public AbstractGenerator ObjednavkyInputIntesity = new ExponentialGenerator((1 / 30.0)
             * 60, seedGenerator);
            protected AbstractGenerator ObjednavkyNabytokType = new UniformGenerator(GenerationMode.Continuous, seedGenerator);
            public Nabytok GenerateObjednavkaNabytokType()
            {
                var p = ObjednavkyNabytokType.GetSampleDouble();

                if (p <= 50)
                    return Nabytok.Stol;

                if (p <= 65)
                    return Nabytok.Stolicka;

                return Nabytok.Skrina;
            }

            // presuny
            public AbstractGenerator StolarMoveToWarehouse = new TriangularGenerator(60 * 60, 480 * 60, 120 * 60, seedGenerator);
            public AbstractGenerator StolarMoveBetweenWorkplaces = new TriangularGenerator(120 * 60, 500 * 60, 150 * 60, seedGenerator);

            // technologicke procesy
            public AbstractGenerator SkladPripravaMaterialu = new TriangularGenerator(300 * 60, 900 * 60, 500 * 60, seedGenerator);

            public Dictionary<Nabytok, Dictionary<NabytokOperation, AbstractGenerator>> NabytokOperations = new Dictionary<Nabytok, Dictionary<NabytokOperation, AbstractGenerator>>
            {
                { Nabytok.Stol, new Dictionary<NabytokOperation, AbstractGenerator>
                    {
                        { NabytokOperation.Rezanie, new EmpiricalGenerator(GenerationMode.Continuous, [10 * 60, 25 * 60, 50 * 60], [0.6, 0.4], seedGenerator) },
                        { NabytokOperation.Morenie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = 200 * 60, Max = 610 * 60} },
                        { NabytokOperation.Skladanie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = 30 * 60, Max = 60 * 60} }
                    }
                },
                { Nabytok.Stolicka, new Dictionary<NabytokOperation, AbstractGenerator>
                    {
                        { NabytokOperation.Rezanie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = 12 * 60, Max = 16 * 60} },
                        { NabytokOperation.Morenie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = 210 * 60, Max = 540 * 60} },
                        { NabytokOperation.Skladanie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = 14 * 60, Max = 24 * 60} }
                    }
                },
                { Nabytok.Skrina, new Dictionary<NabytokOperation, AbstractGenerator>
                    {
                        { NabytokOperation.Rezanie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = 15 * 60, Max = 80 * 60} },
                        { NabytokOperation.Morenie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = 600 * 60, Max = 700 * 60} },
                        { NabytokOperation.Skladanie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = 35 * 60, Max = 75 * 60} },
                        { NabytokOperation.MontazKovani, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = 15 * 60, Max = 25 * 60} }
                    }
                }
            };
            public double GetNabytokOperationTime(Objednavka objednavka)
            {
                return NabytokOperations[objednavka.Nabytok][objednavka.MapStatusToNectOperation()].GetSampleDouble();
            }
        }

        #endregion
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

            public NabytokOperation MapStatusToNectOperation()
            {
                switch (Status)
                {
                    case ObjednavkaStatus.Vytvorena:
                        return NabytokOperation.Rezanie;
                    case ObjednavkaStatus.Narezana:
                        return NabytokOperation.Morenie;
                    case ObjednavkaStatus.Namorena:
                        return NabytokOperation.Skladanie;
                    case ObjednavkaStatus.Poskladana:
                        if (Nabytok == Nabytok.Skrina)
                            return NabytokOperation.MontazKovani;
                        break;
                }
                throw new NotImplementedException();
            }

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

        protected enum NabytokOperation
        {
            Rezanie,
            Morenie,
            Skladanie,
            MontazKovani
        }

        protected class Stolar
        {
            public int Id { get; init; }
            public StolarType Type { get; init; }

            public int CurrentPlace { get; set; } = 0;
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

            public Objednavka? Objednavka { get; set; }
            public Stolar? Stolar { get; set; }

            public NabytokSimulationEvent(NabytokSimulation simulation) : base()
            {
                Simulation = simulation;
            }
        }

        protected class ObjednavkaRecievedEvent : NabytokSimulationEvent
        {
            public ObjednavkaRecievedEvent(NabytokSimulation simulation) : base(simulation)
            {
            }

            public override void Execute()
            {
                throw new NotImplementedException();
            }

            public override void PlanNextEvents()
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}