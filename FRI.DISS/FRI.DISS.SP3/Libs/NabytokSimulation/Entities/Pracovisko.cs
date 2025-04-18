

namespace FRI.DISS.SP3.Libs.NabytokSimulation.Entities
{
    public class Pracovisko
    {
        public static int IdCounter { get; private set; } = 0;
        public static int GetNextId() => IdCounter++;
        public static void ResetIdCounter() => IdCounter = 0;
        public int Id { get; init; } = GetNextId();

        public Nabytok? CurrentNabytok { get; set; } = null;
        public bool IsFree => CurrentNabytok is null;

        public Dictionary<int, Stolar> Stolari { get; init; } = new();

    }
}