

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
                // vedla ostatnych pracovisk
                int skladPosX = (basePositionX * 3) + ((MyAnimator.PracoviskoWidth + spacing) * MyAnimator.PracoviskaCount);
                int skladPosY = basePositionY;

                _animShapeItem = new AnimShapeItem(AnimShape.RECTANGLE_EMPTY, MyAnimator.PracoviskoWidth * 2, MyAnimator.PracoviskoHeight);
                _animShapeItem.SetPosition(skladPosX, skladPosY);
                animator.Register(_animShapeItem);

                _animTextItem = new AnimTextItem($"Sklad");
                _animTextItem.SetPosition(skladPosX + gap, skladPosY + gap); // under the shape
                animator.Register(_animTextItem);

                return;
            }

            int posX = basePositionX + ((MyAnimator.PracoviskoWidth + spacing) * ((Id - 1) % MyAnimator.PracoviskaCount));
            int posY = basePositionY + ((MyAnimator.PracoviskoHeight + spacing * 2) * ((Id - 1) / MyAnimator.PracoviskaCount));

            _animShapeItem = new AnimShapeItem(AnimShape.RECTANGLE_EMPTY, MyAnimator.PracoviskoWidth, MyAnimator.PracoviskoHeight);
            _animShapeItem.SetPosition(posX, posY);
            _animShapeItem.SetZIndex(1);
            animator.Register(_animShapeItem);

            // text nabytku
            _animTextItem = new AnimTextItem($"#{Id}");
            // _animTextItem.SetFontSize(20); // default size
            _animTextItem.SetPosition(posX, posY - fontSize - gap); // above the shape
            animator.Register(_animTextItem);

            // obrazok nabytku
            _animImageItem = new AnimImageItem(MyAnimator.Image_Free, MyAnimator.PracoviskoWidth - (gap * 2), MyAnimator.PracoviskoHeight - (gap * 2));
            _animImageItem.SetZIndex(10); // aby bol nad pracoviskom
            _animImageItem.SetPosition(posX + gap, posY + gap);
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

            animator.Remove(_animTextItem);
            _animTextItem = null;

            if (!IsWarehouse)
            {
                animator.Remove(_animImageItem);
                _animImageItem = null;
            }
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
                _animTextItem!.Text = $"#{Id}-free";
                return;
            }

            _animImageItem!.SetImage(MyAnimator.Image_Stolicka);
            var text = $"#{Id}-{CurrentNabytok!.State}";
            _animTextItem!.Text = text.Length <= 16 ? text : text.Substring(0, 16);    
        }

        private void _rerenderSklad(IAnimator animator)
        {
            
        }
    }
}