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

        protected AbstractGenerator? _rndSupplyProbability;
        protected AbstractGenerator[]? _rndSupplier1Reliability;
        protected AbstractGenerator[]? _rndSupplier2Reliability;

        protected AbstractGenerator? _rndBuyerDampers;
        protected AbstractGenerator? _rndBuyerBrakes;
        protected AbstractGenerator? _rndBuyerLights;

        protected override double _doExperiment()
        {
            var warehouse = new Warehouse();
            var totalCost = 0.0;

            for (int w = 0; w < Weeks; w++)
            {
                for (int d = 0; d < DaysOfWeek; d++)
                {
                    if (d == DayOfBuy)
                    {
                        // pravdepodobnost dodania tovaru od dodavatela
                        var supplierReliability = _getSupplierReliabilityGenerator(w).GetSampleDouble();
                        var supplyProbability = _rndSupplyProbability!.GetSampleDouble();

                        // ak true, tak dodavatel dodal tovar
                        if (supplierReliability < supplyProbability)
                        {
                            warehouse.Supply();
                        }
                    }

                    if (d == DayOfSell)
                    {
                        var missingDemand = warehouse.Sell(
                            _rndBuyerDampers!.GetSampleInt(),
                            _rndBuyerBrakes!.GetSampleInt(),
                            _rndBuyerLights!.GetSampleInt()
                        );

                        totalCost += missingDemand * MissingDemandPenalty;
                    }

                    totalCost += warehouse.GetDailyCost();
                }
            }

            return totalCost;
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
                    if (w < 11)
                        return _rndSupplier1Reliability![0];

                    return _rndSupplier1Reliability![1];
                // dodavatel 2
                case 1:
                    if (w < 16)
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

        protected override void _initialize()
        {
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

            _rndBuyerDampers = new UniformGenerator(GenerationMode.Discrete, SeedGenerator) { Min = 50, Max = 100 };
            _rndBuyerBrakes = new UniformGenerator(GenerationMode.Discrete, SeedGenerator) { Min = 60, Max = 250 };
            _rndBuyerLights = new EmpiricalGenerator(GenerationMode.Discrete,
                [30, 60, 10, 140, 160],
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

            public void Supply()
            {
                _dampers += 100;
                _brakes += 200;
                _lights += 150;
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
}
