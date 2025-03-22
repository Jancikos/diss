using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FRI.DISS.Libs.Generators;

namespace FRI.DISS.Libs.Simulations.EventDriven
{
    public class StanicaSimulation : EventSimulation
    {

        protected StanicaSimulationGenerators? _generators;
        protected StanicaSimulationGenerators Generators => _generators ?? throw new InvalidOperationException("Generators not initialized");

        protected StanicaSimulationExperimentData? _experimentData;
        public StanicaSimulationExperimentData ExperimentData => _experimentData ?? throw new InvalidOperationException("Experiment data not initialized");

        protected override void _beforeSimulation()
        {
            base._beforeSimulation();

            _generators = new StanicaSimulationGenerators();

            // 8 hours
            _endTime = 8 * 60;
        }

        protected override void _beforeExperiment()
        {
            base._beforeExperiment();

            _experimentData = new StanicaSimulationExperimentData();

            // plan first events
            PlanEvent<PrichodZakaznikaEvent>(Generators.PrichodZakaznika.GetSampleDouble());

            Debug.WriteLine("event planned");
        }

        #region Generators
        public class StanicaSimulationGenerators
        {
            public ExponentialGenerator PrichodZakaznika { get; set; }
            public TriangularGenerator ObsluhaZakaznika { get; set; }

            public StanicaSimulationGenerators()
            {
                PrichodZakaznika = new ExponentialGenerator(10.0 / 60.0, SeedGenerator.Global);
                ObsluhaZakaznika = new TriangularGenerator(1, 5, 3, SeedGenerator.Global);
            }
        }
        #endregion

        #region ExperimentData
        public class StanicaSimulationExperimentData
        {
            public int VisitedCustomers { get; set; } = 0;

            public Queue<int> CustomersQueue { get; set; } = new Queue<int>();

            public bool IsCustomerBeingServed { get; set; } = false;
        }
        #endregion

        #region Events
        public abstract class StanicaSimulationEvent : EventSimulataionEvent
        {
            public StanicaSimulation Simulation { get; init; }

            public StanicaSimulationEvent(StanicaSimulation simulation) : base()
            { 
                Simulation = simulation;
            }
        }

        public class PrichodZakaznikaEvent : StanicaSimulationEvent
        {
            public PrichodZakaznikaEvent(StanicaSimulation simulation) : base(simulation)
            { }

            public override void Execute()
            {
                Simulation.ExperimentData.VisitedCustomers++;

                if (Simulation.ExperimentData.IsCustomerBeingServed)
                {
                    Simulation.ExperimentData.CustomersQueue.Enqueue(Simulation.ExperimentData.VisitedCustomers);
                }
            }

            public override void PlanNextEvents()
            {
                if (!Simulation.ExperimentData.IsCustomerBeingServed)
                {
                    Simulation.PlanEvent<ZaciatokObsluhyEvent>();
                }

                Simulation.PlanEvent<PrichodZakaznikaEvent>(Simulation.Generators.PrichodZakaznika.GetSampleDouble());
            }
        }

        public class ZaciatokObsluhyEvent : StanicaSimulationEvent
        {
            public ZaciatokObsluhyEvent(StanicaSimulation simulation) : base(simulation)
            { }

            public override void Execute()
            {
                if (Simulation.ExperimentData.IsCustomerBeingServed)
                {
                    throw new InvalidOperationException("Customer is already being served");
                }

                Simulation.ExperimentData.IsCustomerBeingServed = true;
            }

            public override void PlanNextEvents()
            {
                Simulation.PlanEvent<KoniecObsluhyEvent>(Simulation.Generators.ObsluhaZakaznika.GetSampleDouble());
            }
        }

        public class KoniecObsluhyEvent : StanicaSimulationEvent
        {
            public KoniecObsluhyEvent(StanicaSimulation simulation) : base(simulation)
            { }

            public override void Execute()
            {
                if (!Simulation.ExperimentData.IsCustomerBeingServed)
                {
                    throw new InvalidOperationException("No customer is being served");
                }

                Simulation.ExperimentData.IsCustomerBeingServed = false;
            }

            public override void PlanNextEvents()
            {
                if (Simulation.ExperimentData.CustomersQueue.Count > 0)
                {
                    Simulation.PlanEvent<ZaciatokObsluhyEvent>();
                    Simulation.ExperimentData.CustomersQueue.Dequeue();
                }
            }
        }
        #endregion
    }
}
