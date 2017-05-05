/**************************************************************
 * License detail will be added later
 * 
 **************************************************************/

using System.Linq;

namespace LibResample.Sharp
{
    public class ReSampler
    {
        public class Result
        {
            public int InputSamplesConsumed { get; private set; }
            public int OutputSamplesgenerated { get; private set; }

            public Result(int inputSamplesconsumed, int outputSamplesGenerated)
            {
                InputSamplesConsumed = inputSamplesconsumed;
                OutputSamplesgenerated = outputSamplesGenerated;
            }
        }

        /// <summary>
        /// Number of values per 1/delta in impulse response
        /// </summary>
        protected const int Npc = 4096;

        private readonly float[] _imp;
        private readonly float[] _impD;
        private readonly float _lpScl;
        private readonly int _nmult;
        private readonly int _nwing;
        private readonly double _minFactor;
        private readonly double _maxFactor;
        private readonly int _xSize;
        private readonly float[] _x;
        private int _xp; // current "now"-sample pointer for input
        private int _xread; // position to put new samples
        private readonly int _xoff;
        private readonly float[] _y;
        private int _yp;
        private double _time;

        /// <summary>
        /// Clone an existing resampling session. Faster than creating one from scratch.
        /// </summary>
        /// <param name="other">another instance of resampler</param>
        public ReSampler(ReSampler other)
        {
            _imp = other._imp.ToArray();
            _impD = other._impD.ToArray();
            _lpScl = other._lpScl;
            _nmult = other._nmult;
            _nwing = other._nwing;
            _minFactor = other._minFactor;
            _maxFactor = other._maxFactor;
            _xSize = other._xSize;
            _x = other._x.ToArray();
            _xp = other._xp;
            _xread = other._xread;
            _xoff = other._xoff;
            _y = other._y;
            _yp = other._yp;
            _time = other._time;
        }

        public ReSampler(bool highQuality, double minFactor,double maxFactor)
        {
            if(minFactor<=0.0||maxFactor <=0.0)
        }
    }
}
