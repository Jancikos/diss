

using FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;

namespace FRI.DISS.SP3.Libs.NabytokSimulation.Entities
{
    public enum StolarType
    {
        A,
        B,
        C
    }

    public class Stolar
    {
        public static int IdCounter { get; private set; } = 0;
        public static int GetNextId() => IdCounter++;
        public static void ResetIdCounter() => IdCounter = 0;
        public int Id { get; init; } = GetNextId();

        public StolarType Type { get; init; }

        public Pracovisko? CurrentPracovisko { get; set; } = null;
        public bool IsInWarehouse => CurrentPracovisko is null;

        public double TimeInWork { get; protected set; } = 0;
        public bool IsWorking => _lastWorkStartTime is not null;
        protected double? _lastWorkStartTime = null;

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
}