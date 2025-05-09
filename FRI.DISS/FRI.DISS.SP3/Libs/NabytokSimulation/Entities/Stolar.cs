

using FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
using OSPAnimator;

namespace FRI.DISS.SP3.Libs.NabytokSimulation.Entities
{
    public enum StolarType
    {
        A,
        B,
        C
    }

    public class Stolar : IAnimatoredEntity
    {
        public static int IdCounter { get; private set; } = 0;
        public static int GetNextId() => ++IdCounter;
        public static void ResetIdCounter() => IdCounter = 0;
        public int Id { get; init; } = GetNextId();

        public StolarType Type { get; init; }

        public Pracovisko? CurrentPracovisko { get; set; }
        public bool IsInWarehouse => CurrentPracovisko?.IsWarehouse ?? false;
        public bool IsOnTravel => CurrentPracovisko is null;


        public double TimeInWork { get; protected set; } = 0;
        public bool IsWorking => _lastWorkStartTime is not null;
        protected double? _lastWorkStartTime = null;

        public Stolar()
        {
            CurrentPracovisko = Pracovisko.Sklad;
            Pracovisko.Sklad.Stolari.Add(Id, this);
        }

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

        public AnimShapeItem? AnimShapeItem = null;

        public void Initialize(IAnimator animator)
        {
            AnimShapeItem = new AnimShapeItem(AnimShape.CIRCLE, MyAnimator.StolarRadius, MyAnimator.StolarRadius);
            AnimShapeItem.Color = MyAnimator.GetStolarColor(Type);
            AnimShapeItem.SetZIndex(1000); // above the shape
            animator.Register(AnimShapeItem);

            Rerender(animator);
        }

        public void Rerender(IAnimator animator)
        {
            if (AnimShapeItem is null)
                throw new InvalidOperationException("Stolar animator is not initialized");

            var pos = MyAnimator.GetStolarPosition(this);
            AnimShapeItem.SetPosition(pos.x, pos.y);
        }

        public void Destroy(IAnimator animator)
        {
            if (AnimShapeItem is not null)
            {
                animator.Remove(AnimShapeItem);
                AnimShapeItem = null;
            }
        }
    }
}