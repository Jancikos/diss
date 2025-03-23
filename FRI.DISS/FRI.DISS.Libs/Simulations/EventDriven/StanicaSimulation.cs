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

        protected StanicaSimulationExperimentStatistics? _experimentStatistics;
        public StanicaSimulationExperimentStatistics ExperimentStatistics => _experimentStatistics ?? throw new InvalidOperationException("Experiment statistics not initialized");

        protected StanicaSimulationReplicationsStatistics? _replicationsStatistics;
        public StanicaSimulationReplicationsStatistics ReplicationsStatistics => _replicationsStatistics ?? throw new InvalidOperationException("Replications statistics not initialized");

        public override string CurrentTimeFormatted => TimeSpan.FromMinutes(8 * 60 + CurrentTime).ToString(@"hh\:mm");

        protected override void _beforeSimulation()
        {
            base._beforeSimulation();

            _replicationsStatistics = new StanicaSimulationReplicationsStatistics();
            _generators = new StanicaSimulationGenerators();


            // 8 hours
            _endTime = 8 * 60;

            _replicationsDone = 0;
        }

        protected override void _beforeExperiment()
        {
            base._beforeExperiment();

            _experimentData = new StanicaSimulationExperimentData();
            _experimentStatistics = new StanicaSimulationExperimentStatistics();

            // plan first events
            PlanEvent<PrichodZakaznikaEvent>(Generators.PrichodZakaznika.GetSampleDouble());
        }

        protected override void _afterExperiment(int rep, double d)
        {
            base._afterExperiment(rep, d);

            ReplicationsStatistics.ServedCustomersCount.AddSample(ExperimentData.ServicedCustomers);
            ReplicationsStatistics.CustomerWaitingTime.AddSample(ExperimentStatistics.CustomerWaitingTime.Mean);
            ReplicationsStatistics.CustomersInQueueCount.AddSample(ExperimentStatistics.CustomersInQueueCount.Mean);
            ReplicationsStatistics.CustomersServiceTime.AddSample(ExperimentStatistics.CustomersServiceTime.Mean);
            ReplicationsStatistics.CustomersInSystemTime.AddSample(ExperimentStatistics.CustomersInSystemTime.Mean);
        }

        #region Generators
        public class StanicaSimulationGenerators
        {
            // toto je spravne (sedi pocet zakaznikov za 8 hodin prevazdky)
            public AbstractGenerator PrichodZakaznika { get; init; } = new ExponentialGenerator(1 / 5.0, SeedGenerator.Global);
            
            // toto sa mi nezda - malo by byt exponencialne so strednou dobou 4 minuty 
            public AbstractGenerator ObsluhaZakaznika { get; init; } = new ExponentialGenerator(1 / 4.0, SeedGenerator.Global);
        }
        #endregion

        #region ExperimentData
        public class StanicaSimulationExperimentData
        {
            public int VisitedCustomers { get; set; } = 0;
            public int ServicedCustomers { get; set; } = 0;

            public Queue<StanicaSimulationCustomer> CustomersQueue { get; set; } = new();

            public bool IsCustomerBeingServed { get; set; } = false;
        }

        public class StanicaSimulationCustomer
        {
            public int Id { get; init; }

            public double ArrivalTime { get; init; }
            public double? EndTime { get; set; }
            public double? TotalTime => EndTime - ArrivalTime;

            public double? ServiceStartTime { get; set; }
            public double? ServiceTime => ServiceStartTime.HasValue ? EndTime - ServiceStartTime : null;

            public bool QueueEntered { get; set; } = false;
            public double QueueTime => (QueueEntered && ServiceStartTime.HasValue) ? ServiceStartTime.Value - ArrivalTime : 0.0;
        }

        #endregion

        #region ExperimentStatistics
        public class StanicaSimulationExperimentStatistics
        {
            public Statistics CustomerWaitingTime { get; set; } = new Statistics();
            public Statistics CustomersInQueueCount { get; set; } = new Statistics();
            public Statistics CustomersServiceTime { get; set; } = new Statistics();
            public Statistics CustomersInSystemTime { get; set; } = new Statistics();
        }
        #endregion
        #region ReplicationsStatistics
        public class StanicaSimulationReplicationsStatistics
        {
            public Statistics ServedCustomersCount { get; set; } = new Statistics();
            public Statistics CustomerWaitingTime { get; set; } = new Statistics();
            public Statistics CustomersInQueueCount { get; set; } = new Statistics();
            public Statistics CustomersServiceTime { get; set; } = new Statistics();
            public Statistics CustomersInSystemTime { get; set; } = new Statistics();
        }
        #endregion

        #region Events
        public abstract class StanicaSimulationEvent : EventSimulataionEvent
        {
            public StanicaSimulation Simulation { get; init; }

            public StanicaSimulationCustomer? Customer { get; set; }

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

                Customer = new StanicaSimulationCustomer
                {
                    Id = Simulation.ExperimentData.VisitedCustomers,
                    ArrivalTime = Simulation.CurrentTime
                };

                if (Simulation.ExperimentData.IsCustomerBeingServed)
                {
                    Customer.QueueEntered = true;
                    Simulation.ExperimentData.CustomersQueue.Enqueue(Customer);
                    Simulation.ExperimentStatistics.CustomersInQueueCount.AddSample(Simulation.ExperimentData.CustomersQueue.Count);
                }
            }

            public override void PlanNextEvents()
            {
                if (!Simulation.ExperimentData.IsCustomerBeingServed)
                {
                    Simulation.PlanEvent(new ZaciatokObsluhyEvent(Simulation) { Customer = Customer });
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
                    throw new InvalidOperationException("Some customer is already being served");
                }

                if (Customer is null)
                {
                    throw new InvalidOperationException("No customer to serve");
                }
                Customer.ServiceStartTime = Simulation.CurrentTime;

                Simulation.ExperimentStatistics.CustomerWaitingTime.AddSample(Customer.QueueTime);

                Simulation.ExperimentData.IsCustomerBeingServed = true;
            }

            public override void PlanNextEvents()
            {
                Simulation.PlanEvent(new KoniecObsluhyEvent(Simulation) { Customer = Customer, Priority = EventSimulationEventPriority.High }, Simulation.Generators.ObsluhaZakaznika.GetSampleDouble());
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

                if (Customer is null)
                {
                    throw new InvalidOperationException("No customer to end serve");
                }
                Customer.EndTime = Simulation.CurrentTime;

                Simulation.ExperimentData.ServicedCustomers++;
                Simulation.ExperimentStatistics.CustomersServiceTime.AddSample(Customer.ServiceTime!.Value);
                Simulation.ExperimentStatistics.CustomersInSystemTime.AddSample(Customer.TotalTime!.Value);

                Simulation.ExperimentData.IsCustomerBeingServed = false;
            }

            public override void PlanNextEvents()
            {
                if (Simulation.ExperimentData.CustomersQueue.Count > 0)
                {
                    var nextCustomer = Simulation.ExperimentData.CustomersQueue.Dequeue();
                    Simulation.PlanEvent(new ZaciatokObsluhyEvent(Simulation) { Customer = nextCustomer });
                    Simulation.ExperimentStatistics.CustomersInQueueCount.AddSample(Simulation.ExperimentData.CustomersQueue.Count);
                }
            }
        }
        #endregion
    }
}
