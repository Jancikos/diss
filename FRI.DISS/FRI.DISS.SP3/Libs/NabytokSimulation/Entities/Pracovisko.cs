

using System.IO;
using FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
using OSPAnimator;

namespace FRI.DISS.SP3.Libs.NabytokSimulation.Entities
{
    public class Pracovisko : IAnimatoredEntity
    {
        public static Pracovisko Sklad = new() { Id = 0 };

        public static int IdCounter { get; private set; } = 0;
        public static int GetNextId() => ++IdCounter;
        public static void ResetIdCounter() => IdCounter = 0;
        public static void ResetSklad() => Sklad = new() { Id = 0 };
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


        protected AnimShapeItem? _animShapeItem = null;
        protected AnimImageItem? _animImageItem = null;
        protected AnimTextItem? _animTextItem = null;

        public void Initialize(IAnimator animator)
        {
            int fontSize = 10;
            var gap = 5;
            int spacing = 20;
            int basePositionX = MyAnimator.Offset;
            int basePositionY = spacing;

            if (IsWarehouse)
            {
                _animShapeItem = new AnimShapeItem(AnimShape.RECTANGLE_EMPTY, MyAnimator.PracoviskoWidth * 2, MyAnimator.PracoviskoHeight); 
                
                // vedla ostatnych pracovisk
                _animShapeItem.SetPosition((basePositionX * 3) + ((MyAnimator.PracoviskoWidth + spacing) * MyAnimator.PracoviskaCount), basePositionY);

                animator.Register(_animShapeItem);
                return;
            }

            int positionX = basePositionX + ((MyAnimator.PracoviskoWidth + spacing) * ((Id - 1) % MyAnimator.PracoviskaCount));
            int positionY = basePositionY + ((MyAnimator.PracoviskoHeight + spacing * 2) * ((Id - 1) / MyAnimator.PracoviskaCount));

            _animShapeItem = new AnimShapeItem(AnimShape.RECTANGLE_EMPTY, MyAnimator.PracoviskoWidth, MyAnimator.PracoviskoHeight);
            _animShapeItem.SetPosition(positionX, positionY);
            _animShapeItem.SetZIndex(1);
            animator.Register(_animShapeItem);

            // text nabytku
            _animTextItem = new AnimTextItem($"#{Id}");
            // _animTextItem.SetFontSize(20); // default size
            _animTextItem.SetPosition(positionX, positionY - fontSize - gap); // above the shape
            animator.Register(_animTextItem);

            // obrazok nabytku
            _animImageItem = new AnimImageItem(MyAnimator.Image_Free, MyAnimator.PracoviskoWidth - (gap * 2), MyAnimator.PracoviskoHeight - (gap * 2));
            _animImageItem.SetZIndex(10); // aby bol nad pracoviskom
            _animImageItem.SetPosition(positionX + gap, positionY + gap);
            animator.Register(_animImageItem);

            // to set correct imamge
            Rerender(animator);
        }

        public void Destroy(IAnimator animator)
        {
            if (_animShapeItem is null)
                throw new InvalidOperationException("Pracovisko animator is not initialized.");

            animator.Remove(_animShapeItem);
            _animShapeItem = null;
        }

        public void Rerender(IAnimator animator)
        {
            if (_animShapeItem is null)
                throw new InvalidOperationException("Pracovisko animator is not initialized.");

            if (IsWarehouse)
            {
                _rerenderSklad(animator);
                return;
            }

            if (IsFree)
            {
                _animImageItem!.SetImage(MyAnimator.Image_Free);
                _animTextItem!.Text = $"#{Id} - free";
                return;
            }

            _animImageItem!.SetImage(MyAnimator.Image_Stolicka);
            _animTextItem!.Text = $"#{Id}-{CurrentNabytok!.State}".Substring(0, 16);
        }

        private void _rerenderSklad(IAnimator animator)
        {
            
        }
    }
}