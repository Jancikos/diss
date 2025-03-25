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

        public Dictionary<StolarType, int> StolariCount {get;} = new()
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
        }

        protected override void _beforeExperiment()
        {
            base._beforeExperiment();

            _experimentStatistics = new NabytokExperimentStatistics();

            _experimentData = new NabytokExperimentData();
            Stolar.ResetCounter();
            _experimentData.SetStolariCount(StolarType.A, 2);
            _experimentData.SetStolariCount(StolarType.B, 2);
            _experimentData.SetStolariCount(StolarType.C, 2);

            PlanEvent<ObjednavkaRecievedEvent>(Generators.ObjednavkyInputIntesity.GetSampleDouble());
        }

        protected override void _afterExperiment(int replication, double result)
        {
            base._afterExperiment(replication, result);

            ReplicationsStatistics.ObjednavkaTime.AddSample(ExperimentStatistics.ObjednavkaTime.Mean);

            // stolari work time ratio
            foreach (var stolarType in Enum.GetValues<StolarType>())
            {
                var stolari = ExperimentData.Stolari[stolarType];
                var totalWorkTime = EndTime!.Value;
                var groupWorkTime = stolari.Sum(s => s.TimeInWork);

                ExperimentStatistics.StolariWorkTimeRatio[stolarType].AddSample(groupWorkTime / totalWorkTime);

                for (int i = 0; i < stolari.Count; i++)
                {
                    if (ExperimentStatistics.StolarWorkTimeRatio[stolarType].Count <= i)
                        ExperimentStatistics.StolarWorkTimeRatio[stolarType].Add(new Statistics());

                    ExperimentStatistics.StolarWorkTimeRatio[stolarType][i].AddSample(stolari[i].TimeInWork / totalWorkTime);
                }
            }
        }
        #region ReplicationsStatistics

        public class NabytokReplicationsStatistics
        {
            public Statistics ObjednavkaTime { get; set; } = new Statistics();
        }

        #endregion
        #region ReplicationsStatistics

        public class NabytokExperimentStatistics
        {
            public Statistics ObjednavkaTime { get; } = new Statistics();

            public Statistics ObjednavkyRecieved { get; } = new Statistics();
            public Statistics ObjednavkyDone { get; } = new Statistics();
            public Statistics ObjednavkyNotDone { get; } = new Statistics();

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

        #region Generators
        public class NabytokGenerators(SeedGenerator seedGenerator)
        {
            // objednavky
            public AbstractGenerator ObjednavkyInputIntesity = new ExponentialGenerator(1.0 / 
        (30.0 * 60.0), seedGenerator);
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
        public class NabytokExperimentData
        {
            public int ObjednavkyRecieved { get; set; } = 0;
            public int ObjednavkyDone { get; set; } = 0;

            public List<Objednavka?> Workplaces { get; set; } = new();

            public Dictionary<StolarType, List<Stolar>> Stolari = new()
            {
                { StolarType.A, new() },
                { StolarType.B, new() },
                { StolarType.C, new() }
            };

            public Dictionary<StolarType, Queue<Objednavka>> StolariQueues = new()
            {
                { StolarType.A, new() },
                { StolarType.B, new() },
                { StolarType.C, new() }
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
                var freeWorkplaceIndex = Workplaces.IndexOf(null);

                if (freeWorkplaceIndex == -1)
                {
                    Workplaces.Add(objednavka);
                    freeWorkplaceIndex = Workplaces.Count - 1;
                }

                objednavka.Workplace = freeWorkplaceIndex;
            }

            public Stolar? GetFreeStolar(StolarType type) => GetFreeStolar(Stolari[type]);
            public Stolar? GetFreeStolar(List<Stolar> stolari)
            {
                return stolari.FirstOrDefault(s => !s!.IsWorking, null);
            }

            public Objednavka? GetWaitingObjednavka(StolarType type) => GetWaitingObjednavka(StolariQueues[type]);
            public Objednavka? GetWaitingObjednavka(Queue<Objednavka> queue)
            {
                if (queue.Count == 0)
                    return null;

                return queue.Dequeue();
            }
            public void EnqueueWaitingObjednavka(StolarType type, Objednavka objednavka) => EnqueueWaitingObjednavka(StolariQueues[type], objednavka);
            public void EnqueueWaitingObjednavka(Queue<Objednavka> queue, Objednavka objednavka)
            {
                queue.Enqueue(objednavka);
            }

            public (Stolar stolar, Objednavka objednavka)? GetFreeStolarAndWaitingObjednavka(StolarType type)
            {
                var objednavka = GetWaitingObjednavka(type);
                if (objednavka is null)
                    return null;

                var stolar = GetFreeStolar(type);
                if (stolar is null)
                    return null;

                return (stolar, objednavka);
            }
        }

        public class Objednavka : IComparable<Objednavka>
        {
            public int Id { get; init; }
            public int Workplace { get; set; } // should be init 
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

            public int CompareTo(Objednavka? other)
            {
                if (other is null)
                    return 1;

                if (Status == ObjednavkaStatus.Poskladana && Nabytok == Nabytok.Skrina)
                {
                    if (other.Nabytok != Nabytok.Skrina || other.Status != ObjednavkaStatus.Poskladana)
                        return -1;

                    return Id.CompareTo(other.Id);
                }

                return Id.CompareTo(other.Id);
            }

            public double CreationTime { get; init; }
            public double? EndTime { get; set; }

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
                Simulation.PlanEvent<ObjednavkaRecievedEvent>(Simulation.Generators.ObjednavkyInputIntesity.GetSampleDouble());

                // plan RezanieZaciatok
                var stolarA = Simulation.ExperimentData.GetFreeStolar(StolarType.A);
                if (stolarA is null)
                {
                    Simulation.ExperimentData.EnqueueWaitingObjednavka(StolarType.A, Objednavka!);
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
                Stolar!.CurrentPlace = Objednavka.Workplace;
                Stolar!.StopWork(Simulation.CurrentTime);
            }

            public override void PlanNextEvents()
            {
                // try plan RezanieZaciatok for next objednavka
                var nextRezanie = Simulation.ExperimentData.GetFreeStolarAndWaitingObjednavka(StolarType.A);
                if (nextRezanie is not null)
                {
                    Simulation.PlanEvent(new RezanieZaciatokEvent(Simulation)
                    {
                        Objednavka = nextRezanie.Value.objednavka,
                        Stolar = nextRezanie.Value.stolar
                    });
                }

                // try plan MorenieZaciatok for this objednavka
                var freeStolarC = Simulation.ExperimentData.GetFreeStolar(StolarType.C);
                if (freeStolarC is null)
                {
                    Simulation.ExperimentData.EnqueueWaitingObjednavka(StolarType.C, Objednavka!);
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
                if (Stolar!.CurrentPlace != Objednavka!.Workplace)
                {
                    totalDuration += Simulation.Generators.StolarMoveBetweenWorkplaces.GetSampleDouble();
                }

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
                Stolar!.CurrentPlace = Objednavka.Workplace;
                Stolar!.StopWork(Simulation.CurrentTime);
            }

            public override void PlanNextEvents()
            {
                // try plan next objednavka for Stolar C
                var nextObjednavka = Simulation.ExperimentData.GetFreeStolarAndWaitingObjednavka(StolarType.C);
                if (nextObjednavka is not null)
                {
                    if (nextObjednavka.Value.objednavka.Status == ObjednavkaStatus.Narezana)
                    {
                        Simulation.PlanEvent(new MorenieZaciatok(Simulation)
                        {
                            Objednavka = nextObjednavka.Value.objednavka,
                            Stolar = nextObjednavka.Value.stolar
                        });
                    }

                    if (nextObjednavka.Value.objednavka.Status == ObjednavkaStatus.Poskladana)
                    {
                        Simulation.PlanEvent(new MontazKovaniZaciatok(Simulation)
                        {
                            Objednavka = nextObjednavka.Value.objednavka,
                            Stolar = nextObjednavka.Value.stolar
                        });
                    }
                }

                // try plan SkladanieZaciatok for this objednavka
                var freeStolarB = Simulation.ExperimentData.GetFreeStolar(StolarType.B);
                if (freeStolarB is null)
                {
                    Simulation.ExperimentData.EnqueueWaitingObjednavka(StolarType.B, Objednavka!);
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
                if (Stolar!.CurrentPlace != Objednavka!.Workplace)
                {
                    totalDuration += Simulation.Generators.StolarMoveBetweenWorkplaces.GetSampleDouble();
                }

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
                Stolar!.CurrentPlace = Objednavka.Workplace;
                Stolar!.StopWork(Simulation.CurrentTime);
            }

            public override void PlanNextEvents()
            {
                // try plan next objednavka for Stolar B
                var nextObjednavka = Simulation.ExperimentData.GetFreeStolarAndWaitingObjednavka(StolarType.B);
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
                    var freeStolarC = Simulation.ExperimentData.GetFreeStolar(StolarType.C);
                    if (freeStolarC is null)
                    {
                        Simulation.ExperimentData.EnqueueWaitingObjednavka(StolarType.C, Objednavka!);
                        return;
                    }
                    Simulation.PlanEvent(new MontazKovaniZaciatok(Simulation) { Objednavka = Objednavka, Stolar = freeStolarC });
                }
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
                if (Stolar!.CurrentPlace != Objednavka!.Workplace)
                {
                    totalDuration += Simulation.Generators.StolarMoveBetweenWorkplaces.GetSampleDouble();
                }

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
                Stolar!.CurrentPlace = Objednavka.Workplace;
                Stolar!.StopWork(Simulation.CurrentTime);
            }

            public override void PlanNextEvents()
            {
                // try plan next objednavka for Stolar C
                var nextObjednavka = Simulation.ExperimentData.GetFreeStolarAndWaitingObjednavka(StolarType.C);
                if (nextObjednavka is not null)
                {
                    if (nextObjednavka.Value.objednavka.Status == ObjednavkaStatus.Narezana)
                    {
                        Simulation.PlanEvent(new MorenieZaciatok(Simulation)
                        {
                            Objednavka = nextObjednavka.Value.objednavka,
                            Stolar = nextObjednavka.Value.stolar
                        });
                    }

                    if (nextObjednavka.Value.objednavka.Status == ObjednavkaStatus.Poskladana)
                    {
                        Simulation.PlanEvent(new MontazKovaniZaciatok(Simulation)
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

                // uvolni workplace
                Simulation.ExperimentData.Workplaces[Objednavka.Workplace] = null;
            }

            public override void PlanNextEvents()
            {
            }
        }
        #endregion
    }
}