using FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentOkolia;
using FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentPresunuStolarov;
using FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolarov;
using OSPABA;
using FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentModelu;
using FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolariC;
using FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolariB;
using FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolariA;
using FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentPracovisk;
using FRI.DISS.SP3.Libs.NabytokSimulation.Entities;
using FRI.DISS.Libs.Generators;
using FRI.DISS.Libs.Helpers;
using System.Drawing;
using OSPAnimator;
using System.Windows.Media;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Simulation
{
    public class MySimulation : OSPABA.Simulation
    {
        public int PracoviskaCount { get; set; } = 20;
        public Dictionary<StolarType, int> StolariCount { get; set; } = new()
        {
            { StolarType.A, 2 },
            { StolarType.B, 2 },
            { StolarType.C, 18 }
        };

        public SeedGenerator SeedGenerator { get; set; } = SeedGenerator.Global;

        // pracovna doba je denne od 6:00 do 14:00
        public string CurrentTimeFormatted => TimeSpan.FromSeconds(TimeHelper.HoursToSeconds(6) + (CurrentTime % TimeHelper.HoursToSeconds(8))).ToString(@"hh\:mm\:ss");
        public string CurrentTimeDayFormatted => ((int)CurrentTime / TimeHelper.HoursToSeconds(8)).ToString();

        public int Endtime => TimeHelper.HoursToSeconds(8) * 249; // 6:00 az 14:00 * 249 dni
        public int RepliactionsRefreshStatisticsInterval { get; set; } = 100; // kazdych 10 replikacii sa refreshne statistika

        public NabytokReplicationsStatistics ReplicationsStatistics { get; set; } = new NabytokReplicationsStatistics();

        public MySimulation()
        {
            Init();
        }

        override public void PrepareSimulation()
        {
            base.PrepareSimulation();

            // Create global statistcis
            ReplicationsStatistics = new NabytokReplicationsStatistics();
        }

        override public void PrepareReplication()
        {
            // Reset entities, queues, local statistics, etc...
            Pracovisko.ResetSklad();
            Pracovisko.ResetIdCounter();
            Stolar.ResetIdCounter();
            Nabytok.ResetIdCounter();
            Objednavka.ResetIdCounter();

            // musi sa volat az po resetovani idcok
            base.PrepareReplication();
        }

        override public void ReplicationFinished()
        {
            // Collect local statistics into global, update UI, etc...

            // Collect local statistics into global
            ReplicationsStatistics.ObjednavkaTime.AddSample(AgentModelu.ObjednavkaTotalTime.Mean);


            ReplicationsStatistics.ObjednavkyRecieved.AddSample(AgentModelu.ObjednavkyCount);
            ReplicationsStatistics.ObjednavkyNotWorkingOn.AddSample(AgentModelu.ObjednavkyNotWorkingOnCount);

            // stolari work time ratio
            foreach (var stolarType in Enum.GetValues<StolarType>())
            {
                var stolari = ((IAgentStolari)FindAgent(Mc.GetAgentByStolarType(stolarType))).Stolari.Values.ToList();
                var totalWorkTime = Endtime;
                var groupWorkTime = stolari.Sum(s => s.TimeInWork);

                ReplicationsStatistics.StolariWorkTimeRatio[stolarType].AddSample((double)groupWorkTime / (double)(totalWorkTime * stolari.Count));

                for (int i = 0; i < stolari.Count; i++)
                {
                    if (ReplicationsStatistics.StolarWorkTimeRatio[stolarType].Count <= i)
                        ReplicationsStatistics.StolarWorkTimeRatio[stolarType].Add(new Statistics());

                    ReplicationsStatistics.StolarWorkTimeRatio[stolarType][i].AddSample(stolari[i].TimeInWork / totalWorkTime);
                }
            }

            base.ReplicationFinished();
        }

        override public void SimulationFinished()
        {
            // Display simulation results
            base.SimulationFinished();
        }

        //meta! userInfo="Generated code: do not modify", tag="begin"
        private void Init()
        {
            AgentModelu = new AgentModelu(SimId.AgentModelu, this, null);
            AgentOkolia = new AgentOkolia(SimId.AgentOkolia, this, AgentModelu);
            AgentStolarov = new AgentStolarov(SimId.AgentStolarov, this, AgentModelu);
            AgentStolariA = new AgentStolariA(SimId.AgentStolariA, this, AgentStolarov);
            AgentStolariB = new AgentStolariB(SimId.AgentStolariB, this, AgentStolarov);
            AgentStolariC = new AgentStolariC(SimId.AgentStolariC, this, AgentStolarov);
            AgentPracovisk = new AgentPracovisk(SimId.AgentPracovisk, this, AgentModelu);
            AgentPresunuStolarov = new AgentPresunuStolarov(SimId.AgentPresunuStolarov, this, AgentStolarov);
        }
        public AgentModelu AgentModelu
        { get; set; }
        public AgentOkolia AgentOkolia
        { get; set; }
        public AgentStolarov AgentStolarov
        { get; set; }
        public AgentStolariA AgentStolariA
        { get; set; }
        public AgentStolariB AgentStolariB
        { get; set; }
        public AgentStolariC AgentStolariC
        { get; set; }
        public AgentPracovisk AgentPracovisk
        { get; set; }
        public AgentPresunuStolarov AgentPresunuStolarov
        { get; set; }
        //meta! tag="end"
    }

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

    public class NabytokReplicationsStatisticsCsvWriter : CsvWriter
    {
        public static NabytokReplicationsStatisticsCsvWriter Instance { get; } = new NabytokReplicationsStatisticsCsvWriter();

        public NabytokReplicationsStatisticsCsvWriter()
        {
            Delimiter = ";";
            IncludeHeaderToEmptyFile = true;

            Columns.Add(new CsvColumn()
            {
                Title = "Timestamp",
                Converter = (x) => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });

            // parametre
            Columns.Add(new CsvColumn()
            {
                Title = "ReplicationsCount",
                Converter = (x) => (x as MySimulation)?.CurrentReplication.ToString() ?? "0"
            });
            foreach (var stolarType in Enum.GetValues<StolarType>())
            {
                Columns.Add(new CsvColumn()
                {
                    Title = $"Stolari{stolarType} count",
                    Converter = (x) => (x as MySimulation)?.StolariCount[stolarType].ToString() ?? "0"
                });
            }
            Columns.Add(new CsvColumn()
            {
                Title = "Pracoviska count",
                Converter = (x) => (x as MySimulation)?.PracoviskaCount.ToString() ?? "0"
            });

            // statistiky
            Columns.Add(new CsvColumn()
            {
                Title = "ObjednavkaTime_IS_lower",
                Converter = (x) => ((x as MySimulation)?.ReplicationsStatistics.ObjednavkaTime.IntervalLowerBound ?? 0).ToString()
            });
            Columns.Add(new CsvColumn()
            {
                Title = "ObjednavkaTime_mean",
                Converter = (x) => ((x as MySimulation)?.ReplicationsStatistics.ObjednavkaTime.Mean ?? 0).ToString()
            }); 
            Columns.Add(new CsvColumn()
            {
                Title = "ObjednavkaTime_IS_upper",
                Converter = (x) => ((x as MySimulation)?.ReplicationsStatistics.ObjednavkaTime.IntervalUpperBound ?? 0).ToString()
            });

            Columns.Add(new CsvColumn()
            {
                Title = "ObjednavkyRecieved_IS_lower",
                Converter = (x) => ((x as MySimulation)?.ReplicationsStatistics.ObjednavkyRecieved.IntervalLowerBound ?? 0).ToString()
            });
            Columns.Add(new CsvColumn()
            {
                Title = "ObjednavkyRecieved_mean",
                Converter = (x) => ((x as MySimulation)?.ReplicationsStatistics.ObjednavkyRecieved.Mean ?? 0).ToString()
            });
            Columns.Add(new CsvColumn()
            {
                Title = "ObjednavkyRecieved_IS_upper",
                Converter = (x) => ((x as MySimulation)?.ReplicationsStatistics.ObjednavkyRecieved.IntervalUpperBound ?? 0).ToString()
            });

            Columns.Add(new CsvColumn()
            {
                Title = "ObjednavkyNotWorkingOn_IS_lower",
                Converter = (x) => ((x as MySimulation)?.ReplicationsStatistics.ObjednavkyNotWorkingOn.IntervalLowerBound ?? 0).ToString()
            });
            Columns.Add(new CsvColumn()
            {
                Title = "ObjednavkyNotWorkingOn_mean",
                Converter = (x) => ((x as MySimulation)?.ReplicationsStatistics.ObjednavkyNotWorkingOn.Mean ?? 0).ToString()
            });
            Columns.Add(new CsvColumn()
            {
                Title = "ObjednavkyNotWorkingOn_IS_upper",
                Converter = (x) => ((x as MySimulation)?.ReplicationsStatistics.ObjednavkyNotWorkingOn.IntervalUpperBound ?? 0).ToString()
            });

            foreach (var stolarType in Enum.GetValues<StolarType>())
            {
                Columns.Add(new CsvColumn()
                {
                    Title = $"Stolari{stolarType}WorkTimeRatio_IS_lower",
                    Converter = (x) => ((x as MySimulation)?.ReplicationsStatistics.StolariWorkTimeRatio[stolarType].IntervalLowerBound ?? 0).ToString()
                });
                Columns.Add(new CsvColumn()
                {
                    Title = $"Stolari{stolarType}WorkTimeRatio_mean",
                    Converter = (x) => ((x as MySimulation)?.ReplicationsStatistics.StolariWorkTimeRatio[stolarType].Mean ?? 0).ToString()
                });
                Columns.Add(new CsvColumn()
                {
                    Title = $"Stolari{stolarType}WorkTimeRatio_IS_upper",
                    Converter = (x) => ((x as MySimulation)?.ReplicationsStatistics.StolariWorkTimeRatio[stolarType].IntervalUpperBound ?? 0).ToString()
                });
            }
        }
    }

    public static class MyAnimator
    {
        public static readonly int Width = 1000 - 0;
        public static readonly int Height = 800 - 0;
        public static readonly int Offset = 50;

        public static readonly int FontSize = 10;
        public static readonly int Gap = 5;

        public static readonly int PracoviskoWidth = 100;
        public static readonly int PracoviskoHeight = 100;
        public static readonly int PracoviskaCount = 5;
        public static readonly int PracoviskoSpacing = 20;
        public static (int x, int y) GetPracoviskoPosition(Pracovisko pracovisko)
        {
            int baseX = Offset;
            int baseY = PracoviskoSpacing;

            if (pracovisko.IsWarehouse)
            {
                // vedla ostatnych pracovisk
                int skladPosX = (baseX * 3) + ((PracoviskoWidth + PracoviskoSpacing) * PracoviskaCount);
                int skladPosY = baseY;

                return (x: skladPosX, y: skladPosY);
            }

            int posX = baseX + ((PracoviskoWidth + PracoviskoSpacing) * ((pracovisko.Id - 1) % PracoviskaCount));
            int posY = baseY + ((PracoviskoHeight + PracoviskoSpacing * 2) * ((pracovisko.Id - 1) / PracoviskaCount));

            return (x: posX, y: posY);
        }

        public static (int x, int y) GetStolarPosition(Stolar stolar)
        {
            if (stolar.IsOnTravel)
                return (x: 0, y: 0);

            var wpPos = GetPracoviskoPosition(stolar.CurrentPracovisko!);
            int posX = wpPos.x + GetStolarXGapByType(stolar.Type); 
            int posY = wpPos.y + PracoviskoHeight + Gap + StolarRadius;

            return (x: posX, y: posY);
        }
        
        public static int GetStolarXGapByType(StolarType type)
        {
            int gap = Gap;

            // zoradeny budu A, B, C
            int stolarsCountOffset = 0;
            switch (type)
            {
                case StolarType.A:
                    stolarsCountOffset = 0;
                    break;
                case StolarType.B:
                    stolarsCountOffset = 1;
                    break;
                case StolarType.C:
                    stolarsCountOffset = 2;
                    break;
            }

            for (int i = 0; i < stolarsCountOffset; i++)
            {
                gap += Gap + Gap + StolarRadius;
            }

            return gap;
        }

        public static readonly int StolarRadius = 20;

        public static readonly int SkladWidth = 200;
        public static readonly int SkladHeight = 200;

        public static readonly string Image_Free = "./Images/free.png";
        public static readonly string Image_Stolicka = "./Images/stolicka.jpg";

        public static readonly PointF LeftTop = new PointF(0 + Offset, 0 + Offset);
        public static readonly PointF LeftBottom = new PointF(0 + Offset, Height - Offset);
        public static readonly PointF RightTop = new PointF(Width - Offset, 0 + Offset);
        public static readonly PointF RightBottom = new PointF(Width - Offset, Height - Offset);

        public static System.Windows.Media.Color GetStolarColor(StolarType type)
        {
            return type switch
            {
                StolarType.A => Colors.Red,
                StolarType.B => Colors.Green,
                StolarType.C => Colors.Blue,
                _ => Colors.Black
            };
        }
    }

    public interface IAnimatoredAgent
    {
        public void InitializeSimulationAnimator(IAnimator oldAnimator, IAnimator newAnimator);
        public void DestroySimulationAnimator(IAnimator oldAnimator);
    }
    
    public interface IAnimatoredEntity
    {
        public void Initialize(IAnimator animator);
        public void Rerender(IAnimator animator);
        public void Destroy(IAnimator animator);
    }
}
