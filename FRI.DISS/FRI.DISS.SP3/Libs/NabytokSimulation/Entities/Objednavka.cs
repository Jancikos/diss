

namespace FRI.DISS.SP3.Libs.NabytokSimulation.Entities
{
    public class Objednavka
    {
        public static int IdCounter { get; private set; } = 0;
        public static int GetNextId() => ++IdCounter;
        public static void ResetIdCounter() => IdCounter = 0;
        public int Id { get; init; } = GetNextId();

        public List<Nabytok> Nabytky { get; init; } = new();
        public int NabytokCount => Nabytky.Count;
        public int NabytokDoneCount {get; set; } = 0;
        public bool IsDone => NabytokCount == NabytokDoneCount;
        public double CreationTime { get; init; }
        public double? EndTime { get; set; }
        public double TimeInSystem => EndTime is not null
            ? EndTime.Value - CreationTime
            : throw new InvalidOperationException("End time is not set");
    }
}