using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FRI.DISS.Libs.Generators;
using FRI.DISS.Libs.Helpers;

namespace FRI.DISS.Libs.Simulations.EventDriven
{
    public class NabytokSimulation : EventSimulation
    {
        // pracovna doba je denne od 6:00 do 14:00
        public override string CurrentTimeFormatted => TimeSpan.FromSeconds(TimeHelper.HoursToSeconds(6) + (CurrentTime % TimeHelper.HoursToSeconds(8))).ToString(@"hh\:mm\:ss");
        public string CurrentTimeDayFormatted => ((int)CurrentTime / TimeHelper.HoursToSeconds(8)).ToString();

        protected NabytokGenerators? _generators;
        protected NabytokGenerators Generators => _generators ?? throw new InvalidOperationException("Generators are not initialized");

        protected NabytokExperimentStatistics? _experimentStatistics;
        public NabytokExperimentStatistics ExperimentStatistics => _experimentStatistics ?? throw new InvalidOperationException("Experiment statistics are not initialized");

        public Dictionary<StolarType, int> StolariCount { get; } = new()
        {
            { StolarType.A, 2 },
            { StolarType.B, 2 },
            { StolarType.C, 2 }
        };
        protected NabytokExperimentData? _experimentData;
        public NabytokExperimentData ExperimentData => _experimentData ?? throw new InvalidOperationException("Experiment data are not initialized");

        protected NabytokReplicationsStatistics? _replicationsStatistics;
        public NabytokReplicationsStatistics ReplicationsStatistics => _replicationsStatistics ?? throw new InvalidOperationException("Replications statistics are not initialized");

        protected override void _beforeSimulation()
        {
            base._beforeSimulation();

            _generators = new NabytokGenerators(SeedGenerator);
            _replicationsStatistics = new NabytokReplicationsStatistics();

            _endTime = TimeHelper.HoursToSeconds(8) * 249; // 6:00 az 14:00 * 249 dni
            // _endTime = TimeHelper.HoursToSeconds(8) * 24; // ONLY FOR TESTING
        }

        protected override void _beforeExperiment()
        {
            base._beforeExperiment();

            _experimentStatistics = new NabytokExperimentStatistics();

            _experimentData = new NabytokExperimentData();
            Stolar.ResetCounter();
            foreach (var stolarType in StolariCount.Keys)
            {
                _experimentData.SetStolariCount(stolarType, StolariCount[stolarType]);
            }

            PlanEvent<ObjednavkaRecievedEvent>(Generators.ObjednavkyInputIntesity.GetSampleDouble());
        }

        protected override void _afterExperiment(int replication, double result)
        {
            base._afterExperiment(replication, result);

            ReplicationsStatistics.ObjednavkaTime.AddSample(ExperimentStatistics.ObjednavkaTotalTime.Mean);

            ReplicationsStatistics.ObjednavkyRecieved.AddSample(ExperimentData.ObjednavkyRecieved);
            ReplicationsStatistics.ObjednavkyNotWorkingOn.AddSample(ExperimentData.Workplaces.Count(o => o?.WorkStarted == false));

            // stolari work time ratio
            foreach (var stolarType in Enum.GetValues<StolarType>())
            {
                var stolari = ExperimentData.Stolari[stolarType];
                var totalWorkTime = EndTime!.Value;
                var groupWorkTime = stolari.Sum(s => s.TimeInWork);

                ReplicationsStatistics.StolariWorkTimeRatio[stolarType].AddSample((double)groupWorkTime / (double)(totalWorkTime * stolari.Count));

                for (int i = 0; i < stolari.Count; i++)
                {
                    if (ReplicationsStatistics.StolarWorkTimeRatio[stolarType].Count <= i)
                        ReplicationsStatistics.StolarWorkTimeRatio[stolarType].Add(new Statistics());

                    ReplicationsStatistics.StolarWorkTimeRatio[stolarType][i].AddSample(stolari[i].TimeInWork / totalWorkTime);
                }
            }
        }
        #region ReplicationsStatistics

        public class NabytokReplicationsStatistics
        {
            public Statistics ObjednavkaTime { get; set; } = new Statistics();
            public Statistics ObjednavkyRecieved { get; } = new Statistics();
            public Statistics ObjednavkyNotWorkingOn { get; } = new Statistics();

            public Dictionary<StolarType, Statistics> StolariWorkTimeRatio { get; } = new()
            {
                { StolarType.A, new() },
                { StolarType.B, new() },
                { StolarType.C, new() }
            };
            public Dictionary<StolarType, List<Statistics>> StolarWorkTimeRatio { get; } = new()
            {
                { StolarType.A, new() },
                { StolarType.B, new() },
                { StolarType.C, new() }
            };
        }

        #endregion
        #region ExperimentStatistics

        public class NabytokExperimentStatistics
        {
            public Statistics ObjednavkaTotalTime { get; } = new Statistics();
        }
        #endregion

        #region Generators
        public class NabytokGenerators(SeedGenerator seedGenerator)
        {
            // objednavky
            public AbstractGenerator ObjednavkyInputIntesity = new ExponentialGenerator(1.0 /
        (double)TimeHelper.M2S(30), seedGenerator);
            protected AbstractGenerator ObjednavkyNabytokType = new UniformGenerator(GenerationMode.Continuous, seedGenerator);
            public Nabytok GenerateObjednavkaNabytokType()
            {
                var p = ObjednavkyNabytokType.GetSampleDouble();

                if (p <= 0.50)
                    return Nabytok.Stol;

                if (p <= 0.65)
                    return Nabytok.Stolicka;

                return Nabytok.Skrina;
            }

            // presuny
            public AbstractGenerator StolarMoveToWarehouse = new TriangularGenerator(60, 480, 120, seedGenerator);
            public AbstractGenerator StolarMoveBetweenWorkplaces = new TriangularGenerator(120, 500, 150, seedGenerator);

            // technologicke procesy
            public AbstractGenerator SkladPripravaMaterialu = new TriangularGenerator(300, 900, 500, seedGenerator);

            public Dictionary<Nabytok, Dictionary<NabytokOperation, AbstractGenerator>> NabytokOperations = new Dictionary<Nabytok, Dictionary<NabytokOperation, AbstractGenerator>>
            {
                { Nabytok.Stol, new Dictionary<NabytokOperation, AbstractGenerator>
                    {
                        { NabytokOperation.Rezanie, new EmpiricalGenerator(GenerationMode.Continuous, [TimeHelper.M2S(10), TimeHelper.M2S(25), TimeHelper.M2S(50)], [0.6, 0.4], seedGenerator) },
                        { NabytokOperation.Morenie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = TimeHelper.M2S(200), Max = TimeHelper.M2S(610)} },
                        { NabytokOperation.Skladanie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = TimeHelper.M2S(30), Max = TimeHelper.M2S(60)} }
                    }
                },
                { Nabytok.Stolicka, new Dictionary<NabytokOperation, AbstractGenerator>
                    {
                        { NabytokOperation.Rezanie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = TimeHelper.M2S(12), Max = TimeHelper.M2S(16)} },
                        { NabytokOperation.Morenie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = TimeHelper.M2S(210), Max = TimeHelper.M2S(540)} },
                        { NabytokOperation.Skladanie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = TimeHelper.M2S(14), Max = TimeHelper.M2S(24)} }
                    }
                },
                { Nabytok.Skrina, new Dictionary<NabytokOperation, AbstractGenerator>
                    {
                        { NabytokOperation.Rezanie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = TimeHelper.M2S(15), Max = TimeHelper.M2S(80)} },
                        { NabytokOperation.Morenie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = TimeHelper.M2S(600), Max = TimeHelper.M2S(700)} },
                        { NabytokOperation.Skladanie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = TimeHelper.M2S(35), Max = TimeHelper.M2S(75)} },
                        { NabytokOperation.MontazKovani, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = TimeHelper.M2S(15), Max = TimeHelper.M2S(25)} }
                    }
                }
            };
            public double GetNabytokOperationTime(Objednavka objednavka)
            {
                return NabytokOperations[objednavka.Nabytok][objednavka.MapStatusToNextOperation()].GetSampleDouble();
            }
        }
        #endregion

        #region ExperimentData
        public class NabytokExperimentData
        {
            public int ObjednavkyRecieved { get; set; } = 0;
            public int ObjednavkyDone { get; set; } = 0;
            public int ObjednavkyInSystem => ObjednavkyRecieved - ObjednavkyDone;

            public List<Objednavka?> Workplaces { get; set; } = new();
            public PriorityQueue<int, int> WorkplacesAvailable = new();

            public Dictionary<StolarType, List<Stolar>> Stolari = new()
            {
                { StolarType.A, new() },
                { StolarType.B, new() },
                { StolarType.C, new() }
            };

            public Dictionary<NabytokOperation, Queue<Objednavka>> OperationsQueues = new()
            {
                { NabytokOperation.Rezanie, new() },
                { NabytokOperation.Morenie, new() },
                { NabytokOperation.Skladanie, new() },
                { NabytokOperation.MontazKovani, new() }
            };

            public void SetStolariCount(StolarType stolarType, int newCount)
            {
                if (newCount < 0)
                    throw new InvalidOperationException("Count of stolari should be greater than 0");

                var actualCount = Stolari[stolarType].Count;
                if (newCount == actualCount)
                    return;

                if (newCount < actualCount)
                {
                    Stolari[stolarType].RemoveRange(newCount, actualCount - newCount);
                    return;
                }

                for (int i = actualCount; i < newCount; i++)
                {
                    Stolari[stolarType].Add(new Stolar { Type = stolarType });
                }
            }


            public void AssignWorkplace(Objednavka objednavka)
            {
                if (!WorkplacesAvailable.TryDequeue(out var freeWorkplaceIndex, out var _))
                {
                    Workplaces.Add(objednavka);
                    freeWorkplaceIndex = Workplaces.Count - 1;
                }

                Workplaces[freeWorkplaceIndex] = objednavka;
                objednavka.WorkplaceIndex = freeWorkplaceIndex;
            }

            internal void FreeWorkplace(Objednavka objednavka)
            {
                WorkplacesAvailable.Enqueue(objednavka.WorkplaceIndex, objednavka.WorkplaceIndex);
                Workplaces[objednavka.WorkplaceIndex] = null;
            }

            public List<Objednavka> GetWaitingObjednavky(StolarType stolarType)
            {
                switch (stolarType)
                {
                    case StolarType.A:
                        return OperationsQueues[NabytokOperation.Rezanie].ToList();
                    case StolarType.B:
                        return OperationsQueues[NabytokOperation.Skladanie].ToList();
                    case StolarType.C:
                        return OperationsQueues[NabytokOperation.MontazKovani].ToList().Concat(OperationsQueues[NabytokOperation.Morenie]).ToList();
                    default:
                        throw new NotImplementedException($"Stolar type {stolarType} is not implemented");
                }
            }

            public Stolar? GetFreeStolar(NabytokOperation operation) => GetFreeStolar(Objednavka.MapOperationToStolarType(operation));
            public Stolar? GetFreeStolar(StolarType type) => GetFreeStolar(Stolari[type]);
            public Stolar? GetFreeStolar(List<Stolar> stolari)
            {
                return stolari.FirstOrDefault(s => !s!.IsWorking, null);
            }

            public Objednavka? GetWaitingObjednavka(NabytokOperation type) => GetWaitingObjednavka(OperationsQueues[type]);
            public Objednavka? GetWaitingObjednavka(Queue<Objednavka> queue)
            {
                if (queue.Count == 0)
                    return null;

                return queue.Dequeue();
            }
            public void EnqueueWaitingObjednavka(NabytokOperation type, Objednavka objednavka) => EnqueueWaitingObjednavka(OperationsQueues[type], objednavka);
            public void EnqueueWaitingObjednavka(Queue<Objednavka> queue, Objednavka objednavka)
            {
                queue.Enqueue(objednavka);
            }

            public (Stolar stolar, Objednavka objednavka)? GetFreeStolarAndWaitingObjednavka(NabytokOperation operation)
            {
                var objednavka = GetWaitingObjednavka(operation);
                if (objednavka is null)
                    return null;

                var stolar = GetFreeStolar(Objednavka.MapOperationToStolarType(operation));
                if (stolar is null)
                    return null;

                return (stolar, objednavka);
            }
        }

        public class Objednavka
        {
            public int Id { get; init; }
            public int WorkplaceIndex { get; set; } // should be init 
            public Nabytok Nabytok { get; init; }

            public bool WorkStarted { get; set; } = false;
            public ObjednavkaStatus Status { get; set; } = ObjednavkaStatus.Vytvorena;

            public NabytokOperation MapStatusToNextOperation()
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

            public static StolarType MapOperationToStolarType(NabytokOperation operation)
            {
                return operation switch
                {
                    NabytokOperation.Rezanie => StolarType.A,
                    NabytokOperation.Morenie => StolarType.C,
                    NabytokOperation.Skladanie => StolarType.B,
                    NabytokOperation.MontazKovani => StolarType.C,
                    _ => throw new NotImplementedException()
                };
            }

            public double CreationTime { get; init; }
            public double? EndTime { get; set; }
            public double TimeInSystem => EndTime is not null
                ? EndTime.Value - CreationTime
                : throw new InvalidOperationException("End time is not set");

            public override string ToString() => $"{Id} - {Nabytok} [{Status}]";
        }

        public enum ObjednavkaStatus
        {
            Vytvorena,
            Narezana,
            Namorena,
            Poskladana,
            Ukoncena
        }

        public enum Nabytok
        {
            Stol,
            Stolicka,
            Skrina
        }

        public enum NabytokOperation
        {
            Rezanie,
            Morenie,
            Skladanie,
            MontazKovani
        }

        public class Stolar
        {
            public static int StolarCounter = 0;
            public int Id { get; init; } = ++StolarCounter;
            public StolarType Type { get; init; }

            public const int WarehousePlaceIndex = 0;
            public int CurrentPlace { get; set; } = WarehousePlaceIndex;

            public double TimeInWork { get; protected set; } = 0;
            public bool IsWorking => _lastWorkStartTime is not null;
            protected double? _lastWorkStartTime = null;

            public static void ResetCounter() => StolarCounter = 0;

            public void StartWork(double time)
            {
                if (IsWorking)
                    throw new InvalidOperationException("Stolar is already working");

                _lastWorkStartTime = time;
            }
            public void StopWork(double time)
            {
                if (!IsWorking || _lastWorkStartTime is null)
                    throw new InvalidOperationException("Stolar is not working");

                if (time < _lastWorkStartTime.Value)
                    throw new InvalidOperationException("Time is less than last work start time");

                TimeInWork += time - _lastWorkStartTime.Value;
                _lastWorkStartTime = null;
            }

            public double GetMoveTime(NabytokSimulation simulation, int toWorkplace)
            {
                if (CurrentPlace == toWorkplace)
                    return 0;

                if (CurrentPlace == WarehousePlaceIndex || toWorkplace == WarehousePlaceIndex)
                    return simulation.Generators.StolarMoveToWarehouse.GetSampleDouble();

                return simulation.Generators.StolarMoveBetweenWorkplaces.GetSampleDouble();
            }

            public override string ToString() => $"{Id} - Stolar{Type}";
        }

        public enum StolarType
        {
            A,
            B,
            C
        }

        #endregion

        #region Events
        public abstract class NabytokSimulationEvent : EventSimulataionEvent
        {
            public NabytokSimulation Simulation { get; init; }

            public Objednavka? Objednavka { get; set; }
            public Stolar? Stolar { get; set; }

            public NabytokSimulationEvent(NabytokSimulation simulation) : base()
            {
                Simulation = simulation;
            }

            public void Validate(StolarType? stolarType = null, bool? stolarIsWorking = null)
            {
                if (Simulation is null)
                    throw new InvalidOperationException("Simulation is not set");

                if (Objednavka is null)
                    throw new InvalidOperationException("Objednavka is not set");

                if (Stolar is null)
                    throw new InvalidOperationException("Stolar is not set");

                if (stolarType is not null && Stolar.Type != stolarType)
                    throw new InvalidOperationException($"Stolar type should be {stolarType}. Current type is {Stolar.Type}");

                if (stolarIsWorking is not null && Stolar.IsWorking != stolarIsWorking)
                    throw new InvalidOperationException($"Stolar should be {(stolarIsWorking.Value ? "working" : "not working")}. Current state is {(Stolar.IsWorking ? "working" : "not working")}");
            }
        }

        public class ObjednavkaRecievedEvent : NabytokSimulationEvent
        {
            public ObjednavkaRecievedEvent(NabytokSimulation simulation) : base(simulation)
            {
            }

            public override void Execute()
            {
                Objednavka = new Objednavka
                {
                    Id = ++Simulation.ExperimentData.ObjednavkyRecieved,
                    Nabytok = Simulation.Generators.GenerateObjednavkaNabytokType(),
                    CreationTime = Simulation.CurrentTime
                };
                Simulation.ExperimentData.AssignWorkplace(Objednavka);
            }

            public override void PlanNextEvents()
            {
                // plan next objednavka recieved
                Simulation.PlanEvent(this, Simulation.Generators.ObjednavkyInputIntesity.GetSampleDouble());

                // plan RezanieZaciatok
                var stolarA = Simulation.ExperimentData.GetFreeStolar(NabytokOperation.Rezanie);
                if (stolarA is null)
                {
                    Simulation.ExperimentData.EnqueueWaitingObjednavka(NabytokOperation.Rezanie, Objednavka!);
                    return;
                }
                Simulation.PlanEvent(new RezanieZaciatokEvent(Simulation) { Objednavka = Objednavka, Stolar = stolarA });
            }
        }

        public class RezanieZaciatokEvent : NabytokSimulationEvent
        {
            public RezanieZaciatokEvent(NabytokSimulation simulation) : base(simulation)
            {
            }

            public override void Execute()
            {
                Validate(StolarType.A, false);

                Objednavka!.WorkStarted = true;
                Stolar!.StartWork(Simulation.CurrentTime);
            }

            public override void PlanNextEvents()
            {
                var totalDuration = 0.0;

                // presun do skladu
                if (Stolar.WarehousePlaceIndex != Stolar!.CurrentPlace)
                {
                    totalDuration += Simulation.Generators.StolarMoveToWarehouse.GetSampleDouble();
                }

                // priprava materialu v sklade
                totalDuration += Simulation.Generators.SkladPripravaMaterialu.GetSampleDouble();

                // presun na pracovisko
                totalDuration += Simulation.Generators.StolarMoveToWarehouse.GetSampleDouble();

                // doba rezania
                totalDuration += Simulation.Generators.GetNabytokOperationTime(Objednavka!);

                Simulation.PlanEvent(new RezanieKoniecEvent(Simulation) { Objednavka = Objednavka, Stolar = Stolar }, totalDuration);
            }

        }

        public class RezanieKoniecEvent : NabytokSimulationEvent
        {
            public RezanieKoniecEvent(NabytokSimulation simulation) : base(simulation)
            {
            }

            public override void Execute()
            {
                Validate(StolarType.A, true);

                // zmen stav Objednavka
                Objednavka!.Status = ObjednavkaStatus.Narezana;

                // zmen stav Stolar
                Stolar!.CurrentPlace = Objednavka.WorkplaceIndex;
                Stolar!.StopWork(Simulation.CurrentTime);
            }

            public override void PlanNextEvents()
            {
                // try plan RezanieZaciatok for next objednavka
                var nextRezanie = Simulation.ExperimentData.GetFreeStolarAndWaitingObjednavka(NabytokOperation.Rezanie);
                if (nextRezanie is not null)
                {
                    Simulation.PlanEvent(new RezanieZaciatokEvent(Simulation)
                    {
                        Objednavka = nextRezanie.Value.objednavka,
                        Stolar = nextRezanie.Value.stolar
                    });
                }

                // try plan MorenieZaciatok for this objednavka
                var freeStolarC = Simulation.ExperimentData.GetFreeStolar(NabytokOperation.Morenie);
                if (freeStolarC is null)
                {
                    Simulation.ExperimentData.EnqueueWaitingObjednavka(NabytokOperation.Morenie, Objednavka!);
                    return;
                }
                Simulation.PlanEvent(new MorenieZaciatok(Simulation) { Objednavka = Objednavka, Stolar = freeStolarC });
            }
        }

        public class MorenieZaciatok : NabytokSimulationEvent
        {
            public MorenieZaciatok(NabytokSimulation simulation) : base(simulation)
            {
            }

            public override void Execute()
            {
                Validate(StolarType.C, false);

                // zmen stav Stolar
                Stolar!.StartWork(Simulation.CurrentTime);
            }

            public override void PlanNextEvents()
            {
                var totalDuration = 0.0;

                // presun na pracovisko
                totalDuration += Stolar!.GetMoveTime(Simulation, Objednavka!.WorkplaceIndex);

                // doba morenia
                totalDuration += Simulation.Generators.GetNabytokOperationTime(Objednavka);

                Simulation.PlanEvent(new MorenieKoniec(Simulation) { Objednavka = Objednavka, Stolar = Stolar }, totalDuration);
            }
        }

        public class MorenieKoniec : NabytokSimulationEvent
        {
            public MorenieKoniec(NabytokSimulation simulation) : base(simulation)
            {
            }

            public override void Execute()
            {
                Validate(StolarType.C, true);

                // zmen stav Objednavka
                Objednavka!.Status = ObjednavkaStatus.Namorena;

                // zmen stav Stolar
                Stolar!.CurrentPlace = Objednavka.WorkplaceIndex;
                Stolar!.StopWork(Simulation.CurrentTime);
            }

            public override void PlanNextEvents()
            {
                // try plan next objednavka for Stolar C
                var nextObjednavka = Simulation.ExperimentData.GetFreeStolarAndWaitingObjednavka(NabytokOperation.MontazKovani);
                if (nextObjednavka is not null)
                {
                    Simulation.PlanEvent(new MontazKovaniZaciatok(Simulation)
                    {
                        Objednavka = nextObjednavka.Value.objednavka,
                        Stolar = nextObjednavka.Value.stolar
                    });
                }
                else
                {
                    nextObjednavka = Simulation.ExperimentData.GetFreeStolarAndWaitingObjednavka(NabytokOperation.Morenie);
                    if (nextObjednavka is not null)
                    {
                        Simulation.PlanEvent(new MorenieZaciatok(Simulation)
                        {
                            Objednavka = nextObjednavka.Value.objednavka,
                            Stolar = nextObjednavka.Value.stolar
                        });
                    }
                }

                // try plan SkladanieZaciatok for this objednavka
                var freeStolarB = Simulation.ExperimentData.GetFreeStolar(NabytokOperation.Skladanie);
                if (freeStolarB is null)
                {
                    Simulation.ExperimentData.EnqueueWaitingObjednavka(NabytokOperation.Skladanie, Objednavka!);
                    return;
                }
                Simulation.PlanEvent(new SkladanieZaciatok(Simulation) { Objednavka = Objednavka, Stolar = freeStolarB });
            }
        }

        public class SkladanieZaciatok : NabytokSimulationEvent
        {
            public SkladanieZaciatok(NabytokSimulation simulation) : base(simulation)
            {
            }

            public override void Execute()
            {
                Validate(StolarType.B, false);

                // zmen stav Stolar
                Stolar!.StartWork(Simulation.CurrentTime);
            }

            public override void PlanNextEvents()
            {
                var totalDuration = 0.0;

                // presun na pracovisko
                totalDuration += Stolar!.GetMoveTime(Simulation, Objednavka!.WorkplaceIndex);

                // doba skladania
                totalDuration += Simulation.Generators.GetNabytokOperationTime(Objednavka);

                Simulation.PlanEvent(new SkladanieKoniec(Simulation) { Objednavka = Objednavka, Stolar = Stolar }, totalDuration);
            }
        }

        public class SkladanieKoniec : NabytokSimulationEvent
        {
            public SkladanieKoniec(NabytokSimulation simulation) : base(simulation)
            {
            }

            public override void Execute()
            {
                Validate(StolarType.B, true);

                // zmen stav Objednavka
                Objednavka!.Status = ObjednavkaStatus.Poskladana;

                // zmen stav Stolar
                Stolar!.CurrentPlace = Objednavka.WorkplaceIndex;
                Stolar!.StopWork(Simulation.CurrentTime);
            }

            public override void PlanNextEvents()
            {
                // try plan next objednavka for Stolar B
                var nextObjednavka = Simulation.ExperimentData.GetFreeStolarAndWaitingObjednavka(NabytokOperation.Skladanie);
                if (nextObjednavka is not null)
                {
                    Simulation.PlanEvent(new SkladanieZaciatok(Simulation)
                    {
                        Objednavka = nextObjednavka.Value.objednavka,
                        Stolar = nextObjednavka.Value.stolar
                    });
                }

                // try plan MontazKovaniZaciatok for this objednavka
                if (Objednavka!.Nabytok == Nabytok.Skrina)
                {
                    var freeStolarC = Simulation.ExperimentData.GetFreeStolar(NabytokOperation.MontazKovani);
                    if (freeStolarC is null)
                    {
                        Simulation.ExperimentData.EnqueueWaitingObjednavka(NabytokOperation.MontazKovani, Objednavka!);
                        return;
                    }

                    Simulation.PlanEvent(new MontazKovaniZaciatok(Simulation) { Objednavka = Objednavka, Stolar = freeStolarC });
                    return;
                }

                // plan end of objednavka
                Simulation.PlanEvent(new ObjednavkaFinishedEvent(Simulation) { Objednavka = Objednavka });
            }
        }

        public class MontazKovaniZaciatok : NabytokSimulationEvent
        {
            public MontazKovaniZaciatok(NabytokSimulation simulation) : base(simulation)
            {
            }

            public override void Execute()
            {
                Validate(StolarType.C, false);

                // zmen stav Stolar
                Stolar!.StartWork(Simulation.CurrentTime);
            }

            public override void PlanNextEvents()
            {
                var totalDuration = 0.0;

                // presun na pracovisko
                totalDuration += Stolar!.GetMoveTime(Simulation, Objednavka!.WorkplaceIndex);

                // doba montaze kovani
                totalDuration += Simulation.Generators.GetNabytokOperationTime(Objednavka);

                Simulation.PlanEvent(new MontazKovaniKoniec(Simulation) { Objednavka = Objednavka, Stolar = Stolar }, totalDuration);
            }
        }

        public class MontazKovaniKoniec : NabytokSimulationEvent
        {
            public MontazKovaniKoniec(NabytokSimulation simulation) : base(simulation)
            {
            }

            public override void Execute()
            {
                Validate(StolarType.C, true);

                // zmen stav Objednavka
                Objednavka!.Status = ObjednavkaStatus.Ukoncena;

                // zmen stav Stolar
                Stolar!.CurrentPlace = Objednavka.WorkplaceIndex;
                Stolar!.StopWork(Simulation.CurrentTime);
            }

            public override void PlanNextEvents()
            {
                // try plan next objednavka for Stolar C
                var nextObjednavka = Simulation.ExperimentData.GetFreeStolarAndWaitingObjednavka(NabytokOperation.MontazKovani);
                if (nextObjednavka is not null)
                {
                    if (nextObjednavka.Value.objednavka.Status == ObjednavkaStatus.Poskladana)
                    {
                        Simulation.PlanEvent(new MontazKovaniZaciatok(Simulation)
                        {
                            Objednavka = nextObjednavka.Value.objednavka,
                            Stolar = nextObjednavka.Value.stolar
                        });
                    }
                }
                else
                {
                    nextObjednavka = Simulation.ExperimentData.GetFreeStolarAndWaitingObjednavka(NabytokOperation.Morenie);
                    if (nextObjednavka is not null)
                    {
                        Simulation.PlanEvent(new MorenieZaciatok(Simulation)
                        {
                            Objednavka = nextObjednavka.Value.objednavka,
                            Stolar = nextObjednavka.Value.stolar
                        });
                    }
                }

                // plan end of objednavka
                Simulation.PlanEvent(new ObjednavkaFinishedEvent(Simulation) { Objednavka = Objednavka });
            }
        }

        public class ObjednavkaFinishedEvent : NabytokSimulationEvent
        {
            public ObjednavkaFinishedEvent(NabytokSimulation simulation) : base(simulation)
            {
            }

            public override void Execute()
            {
                if (Objednavka is null)
                    throw new InvalidOperationException("Objednavka is not set");

                Objednavka.EndTime = Simulation.CurrentTime;
                Simulation.ExperimentData.ObjednavkyDone++;

                Simulation.ExperimentStatistics.ObjednavkaTotalTime.AddSample(Objednavka.TimeInSystem);

                // uvolni workplace
                Simulation.ExperimentData.FreeWorkplace(Objednavka);
            }

            public override void PlanNextEvents()
            {
            }
        }
        #endregion
    }
}