

namespace FRI.DISS.SP3.Libs.NabytokSimulation.Entities
{
    public class Pracovisko
    {
        public static Pracovisko Sklad = new() { Id = 0 };

        public static int IdCounter { get; private set; } = 0;
        public static int GetNextId() => ++IdCounter;
        public static void ResetIdCounter() => IdCounter = 0;
        public int Id { get; init; } = GetNextId();

        public Nabytok? CurrentNabytok { get; set; } = null;
        public bool IsFree => CurrentNabytok is null;

        public bool IsWarehouse => this == Sklad; // or Id == 0

        public Dictionary<int, Stolar> Stolari { get; init; } = new();

        public Dictionary<StolarType, int> StolarTypesCount => Stolari
            .GroupBy(x => x.Value.Type)
            .ToDictionary(x => x.Key, x => x.Count());

        public override string ToString()
        {
            return $"#{Id} [{(IsFree ? "free" : CurrentNabytok!.ToString())}]";
        }
    }
}