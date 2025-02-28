using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRI.DISS.Libs.Generators
{
    public enum GenerationMode
    {
        /// <summary>
        /// Cele cisla (int)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        Discrete,

        /// <summary>
        /// Realne cisla (double)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        Continuous
    }

    public abstract class AbstractGenerator
    {
        public GenerationMode Mode { get; private set; }

        public AbstractGenerator(GenerationMode mode)
        {
            Mode = mode;
        }

        protected abstract int _GetSampleInt();
        public int GetSampleInt()
        {
            if (Mode != GenerationMode.Discrete)
            {
                throw new InvalidOperationException("Generator is not in discrete mode");
            }

            return _GetSampleInt();
        }

        protected abstract double _GetSampleDouble();
        public double GetSampleDouble()
        {
            if (Mode != GenerationMode.Continuous)
            {
                throw new InvalidOperationException("Generator is not in continuous mode");
            }

            return _GetSampleDouble();
        }
    }
}