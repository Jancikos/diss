using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FRI.DISS.Libs.Generators;

namespace FRI.DISS.Libs.MonteCarlo
{
    // jan prevadzkuje sklad
    // 3 typy tovaru, kt. skladuje - tlmice, brzdy, svetla
    // // tovar je vzdy dovezeny v pondelok
    // // // tovar objednava od 2 dodavatelov (lisia sa len v spolahlivosti dodania)
    // // // vzdy v rovnakom pocte: tlmice 100, brzdy 200, svetla 150
    // // // dodavatel moze stornovat objednavku s nahodnou pravdepodobnostou (vid. tabulka 2)
    // // denne naklady na skladovanie: tlmice 0.2, brzdy 0.3, svetla 0.25
    // tovar odchadza vzdy v piatok
    // // cena za predaj je fixna
    // // pocet je kazdy tyzden nahodny (vid. tabuka 1)
    // // !!! za kazdu nedodanu suciastku sa plati pokuta 0.3

    // jan chcem minimalizovat naklady za 30 tyzdnov pomocou 4och strategii
    // // A - objednavam vzdy od dodovatela 1
    // // B - objednavam vzdy od dodavatela 2
    // // C - objednavam od dodavatela 1 kazdy neparny tyzden, inak (parny) od dodavatela 2
    // // D - objednavam od dodavatela 1 kazdy parny tyzden, inak (neparny) od dodavatela 2

    // vysledok 1
    // // ktoru strategiu by mal jan zvolit (ak vzdy zacina s prazdnym skladom)

    public abstract class BusinessmanJan : MonteCarlo
    {
        protected const int Weeks = 30;
        protected const int DayOfBuy = 0;
        protected const int DayOfSell = 4;
        protected const int DaysOfWeek = 7;

        protected const double MissingDemandPenalty = 0.3;
        protected const double DailyStorageCostDampers = 0.2;
        protected const double DailyStorageCostBrakes = 0.3;
        protected const double DailyStorageCostLights = 0.25;

        protected Warehouse? _warehouse;

        protected AbstractGenerator? _rndSupplyProbability;
        protected AbstractGenerator[]? _rndSupplier1Reliability;
        protected AbstractGenerator[]? _rndSupplier2Reliability;

        protected AbstractGenerator? _rndBuyerDampers;
        protected AbstractGenerator? _rndBuyerBrakes;
        protected AbstractGenerator? _rndBuyerLights;

        public Statistics? ResultSuplliersReliability { get; protected set; }
        public Statistics? ResultMissingDemandPenalty { get; protected set; }
        public Statistics? ResultWarehouseCosts { get; protected set; }

        /// <summary>
        /// week, day, totalCost
        /// 
        /// called only in first replication
        /// </summary>
        public Action<int, int, double>? UpdateStatsDailyCallback { get; set; }

        protected override void _beforeSimulation()
        {
            ResultSuplliersReliability = new Statistics();
            ResultMissingDemandPenalty = new Statistics();
            ResultWarehouseCosts = new Statistics();
        }

        protected override double _doExperiment()
        {
            var totalPenalty = 0.0;
            var totalWarehouseCost = 0.0;

            for (int w = 0; w < Weeks; w++)
            {
                for (int d = 0; d < DaysOfWeek; d++)
                {
                    if (d == DayOfBuy)
                    {
                        // pravdepodobnost dodania tovaru od dodavatela
                        var supplierReliability = _getSupplierReliabilityGenerator(w).GetSampleDouble();
                        var supplyProbability = _rndSupplyProbability!.GetSampleDouble();

                        if (supplyProbability < supplierReliability)
                        {
                            _warehouse!.Supply(
                                _getSupplyDampers(w),
                                _getSupplyBrakes(w),
                                _getSupplyLights(w)
                            );
                            ResultSuplliersReliability!.AddSample(1);
                        } else {
                            ResultSuplliersReliability!.AddSample(0);
                        }
                    }

                    if (d == DayOfSell)
                    {
                        var missingDemand = _warehouse!.Sell(
                            _rndBuyerDampers!.GetSampleInt(),
                            _rndBuyerBrakes!.GetSampleInt(),
                            _rndBuyerLights!.GetSampleInt()
                        );


                        totalPenalty += missingDemand * MissingDemandPenalty;
                    }

                    totalWarehouseCost += _warehouse!.GetDailyCost();

                    if (ReplicationsDone == 0) 
                    {
                        UpdateStatsDailyCallback?.Invoke(w, d, totalPenalty + totalWarehouseCost);
                    }
                }

            }

            ResultMissingDemandPenalty!.AddSample(totalPenalty);
            ResultWarehouseCosts!.AddSample(totalWarehouseCost);

            return totalPenalty + totalWarehouseCost;
        }

        /// <summary>
        /// vrati generator pravdepodobnosti dodania tovaru od dodavatela podla strategie
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        private AbstractGenerator _getSupplierReliabilityGenerator(int w)
        {
            int supplierIndex = _getSupplierIndex(w);

            switch (supplierIndex)
            {
                // dodavatel 1
                case 0:
                    if (w < 10)
                        return _rndSupplier1Reliability![0];

                    return _rndSupplier1Reliability![1];
                // dodavatel 2
                case 1:
                    if (w < 15)
                        return _rndSupplier2Reliability![0];

                    return _rndSupplier2Reliability![1];
                default:
                    throw new ArgumentOutOfRangeException("supplierIndex out of range");
            }
        }

        /// <summary>
        /// vrati index dodavatela ktory sa ma pouzit v dany tyzden podla strategie
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        protected abstract int _getSupplierIndex(int w);

        protected virtual int _getSupplyDampers(int w) => 100;
        protected virtual int _getSupplyBrakes(int w) => 200;
        protected virtual int _getSupplyLights(int w) => 150;

        protected override void _initialize()
        {
            _warehouse = new Warehouse();

            _rndSupplyProbability = new UniformGenerator(GenerationMode.Continuous, SeedGenerator) { Min = 0, Max = 100};
            _rndSupplier1Reliability =
            [
                new UniformGenerator(GenerationMode.Continuous, SeedGenerator) {Min = 10, Max = 70},
                new UniformGenerator(GenerationMode.Continuous, SeedGenerator) {Min = 30, Max = 95}
            ];
            _rndSupplier2Reliability =
            [
                new EmpiricalGenerator(GenerationMode.Continuous,
                    [5, 10, 50, 70, 80, 95],
                    [0.4, 0.3, 0.2, 0.06, 0.04],
                    SeedGenerator
                ),
                new EmpiricalGenerator(GenerationMode.Continuous,
                    [5, 10, 50, 70, 80, 95],
                    [0.2, 0.4, 0.3, 0.06, 0.04],
                    SeedGenerator
                )
            ];

            _rndBuyerDampers = new UniformGenerator(GenerationMode.Discrete, SeedGenerator) { Min = 50, Max = 100 + 1 };
            _rndBuyerBrakes = new UniformGenerator(GenerationMode.Discrete, SeedGenerator) { Min = 60, Max = 250 + 1 };
            _rndBuyerLights = new EmpiricalGenerator(GenerationMode.Discrete,
                [30, 60, 100, 140, 160],
                [0.2, 0.4, 0.3, 0.1],
                SeedGenerator
            );
        }

        protected class Warehouse
        {
            private int _dampers = 0;
            public int Dampers => _dampers;
            private int _brakes = 0;
            public int Brakes => _brakes;
            private int _lights = 0;
            public int Lights => _lights;

            public int TotalItemsCount => Dampers + Brakes + Lights;

            public void Supply(int dampers, int brakes, int lights)
            {
                _dampers += dampers;
                _brakes += brakes;
                _lights += lights;
            }

            public int Sell(int dampers, int brakes, int lights)
            {
                return ProcessSale(ref _dampers, dampers) +
                       ProcessSale(ref _brakes, brakes) +
                       ProcessSale(ref _lights, lights);

                static int ProcessSale(ref int stock, int demand)
                {
                    if (stock < demand)
                    {
                        var missing = demand - stock;
                        stock = 0;
                        return missing;
                    }

                    stock -= demand;
                    return 0;
                }
            }

            public double GetDailyCost()
            {
                return Dampers * DailyStorageCostDampers +
                       Brakes * DailyStorageCostBrakes +
                       Lights * DailyStorageCostLights;
            }
        }
    }

    public class BusinessmanJanStrategyA : BusinessmanJan
    {
        protected override int _getSupplierIndex(int w) => 0;
    }

    public class BusinessmanJanStrategyB : BusinessmanJan
    {
        protected override int _getSupplierIndex(int w) => 1;
    }

    public class BusinessmanJanStrategyC : BusinessmanJan
    {
        protected override int _getSupplierIndex(int w) => w % 2;
    }

    public class BusinessmanJanStrategyD : BusinessmanJan
    {
        protected override int _getSupplierIndex(int w) => (w + 1) % 2;
    }

    /// <summary>
    /// Strategy Kostor A - objednavanie len ak kapaicta skladu je pod priemernou velkostou objednavok, dodavatel 0
    /// </summary>
    public class BusinessmanJanStrategyKostorA : BusinessmanJan
    {
        protected override int _getSupplierIndex(int w) => 0;

        protected override int _getSupplyDampers(int w) => _warehouse!.Dampers > 75 ? 0 : base._getSupplyDampers(w);
        protected override int _getSupplyBrakes(int w) => _warehouse!.Brakes > 155 ? 0 : base._getSupplyBrakes(w);
        protected override int _getSupplyLights(int w) => _warehouse!.Lights > 95 ? 0 : base._getSupplyLights(w);
    }

    /// <summary>
    /// Strategy Kostor B - objednavanie len ak kapaicta skladu je pod priemernou velkostou objednavok, dodavatel 1
    /// </summary>
    public class BusinessmanJanStrategyKostorB : BusinessmanJanStrategyKostorA
    {
        protected override int _getSupplierIndex(int w) => 1;
    } 

    /// <summary>
    /// Strategy Kostor C - objednavanie podla priemernej velkosti objednavok, dodavatel 0
    /// </summary>
    public class BusinessmanJanStrategyKostorC : BusinessmanJan
    {
        protected override int _getSupplierIndex(int w) => 0;

        protected override int _getSupplyDampers(int w) => 75; // 100 + 50 / 2
        protected override int _getSupplyBrakes(int w) => 155; //  60 + 250 / 2
        protected override int _getSupplyLights(int w) => 92; // 45*0.2 + 80*0.4 + 120*0.3 + 150*0.1 = 92
    }

    /// <summary>
    /// Strategy Kostor D - objednavanie podla priemernej velkosti objednavok, dodavatel 1
    /// </summary>
    public class BusinessmanJanStrategyKostorD : BusinessmanJanStrategyKostorC 
    {
        protected override int _getSupplierIndex(int w) => 1;
    }

    public class BusinessmanJanCustomStrategy : BusinessmanJan
    {
        /// <summary>
        /// csv subor s konfiguraciou strategie 
        /// 
        /// obsahuje strukturu: Week, SupplierIndex, DamperSupply, BrakeSupply, LightSupply
        /// </summary>
        /// <value></value>
        public FileInfo? SuppliersStrategyConfig { get; set; }

        protected int[]? _supplierIndexes { get; set; }
        protected int[]? _dampersSupplies { get; set; }
        protected int[]? _brakesSupplies { get; set; }
        protected int[]? _lightsSupplies { get; set; }

        protected override void _initialize()
        {
            base._initialize();

            if (SuppliersStrategyConfig == null)
            {
                throw new InvalidOperationException("SuppliersStrategyConfig not set");
            }

            var lines = File.ReadAllLines(SuppliersStrategyConfig.FullName);
            if (lines.Length != Weeks)
            {
                throw new InvalidOperationException($"Invalid SuppliersStrategyConfig file. Must have {Weeks} lines (weeks).");
            }

            _supplierIndexes = new int[lines.Length];
            _dampersSupplies = new int[lines.Length];
            _brakesSupplies = new int[lines.Length];
            _lightsSupplies = new int[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                var parts = lines[i].Split(';');
                if (parts.Length != 5)
                {
                    throw new InvalidOperationException($"Invalid SuppliersStrategyConfig file. Line {i + 1} must have 5 parts.");
                }

                if (!int.TryParse(parts[1], out _supplierIndexes[i]))
                {
                    throw new InvalidOperationException($"Invalid SuppliersStrategyConfig file. Line {i + 1} must have integer as first part.");
                }

                if (!int.TryParse(parts[2], out _dampersSupplies[i]))
                {
                    throw new InvalidOperationException($"Invalid SuppliersStrategyConfig file. Line {i + 1} must have integer as second part.");
                }

                if (!int.TryParse(parts[3], out _brakesSupplies[i]))
                {
                    throw new InvalidOperationException($"Invalid SuppliersStrategyConfig file. Line {i + 1} must have integer as third part.");
                }

                if (!int.TryParse(parts[4], out _lightsSupplies[i]))
                {
                    throw new InvalidOperationException($"Invalid SuppliersStrategyConfig file. Line {i + 1} must have integer as fourth part.");
                }
            }

        }

        protected override int _getSupplierIndex(int w) => _supplierIndexes![w];
        protected override int _getSupplyDampers(int w) => _dampersSupplies![w];
        protected override int _getSupplyBrakes(int w) => _brakesSupplies![w];
        protected override int _getSupplyLights(int w) => _lightsSupplies![w];
    }
}
