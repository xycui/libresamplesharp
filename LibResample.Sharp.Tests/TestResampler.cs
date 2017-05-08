using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibResample.Sharp.Tests
{
    [TestClass]
    public class TestResampler
    {
        [TestMethod]
        public void TestResample()
        {
            int i, srclen, dstlen, ifreq;
            double factor;

            Console.WriteLine("\n*** Vary source block size*** \n\n");
            srclen = 10000;
            ifreq = 100;
            for (i = 0; i < 20; i++)
            {
                factor = ((new Random().Next() % 16) + 1) / 4.0;
                dstlen = (int)(srclen * factor + 10);
                runtest(srclen, (double)ifreq, factor, 64, dstlen);
                runtest(srclen, (double)ifreq, factor, 32, dstlen);
                runtest(srclen, (double)ifreq, factor, 8, dstlen);
                runtest(srclen, (double)ifreq, factor, 2, dstlen);
                runtest(srclen, (double)ifreq, factor, srclen, dstlen);
            }
            Assert.IsTrue(true);
        }

        private static void runtest(int srclen, double freq, double factor, int srcblocksize, int dstblocksize)
        {
            int expectedlen = (int)(srclen * factor);
            int dstlen = expectedlen + 1000;
            float[] src = new float[srclen + 100];
            float[] dst = new float[dstlen + 100];

            ReSampler resampler;
            double sum, sumsq, err, rmserr;
            int i, outi, o, srcused, errcount, rangecount;
            int statlen, srcpos, lendiff;
            int fwidth;

            for (i = 0; i < srclen; i++)
                src[i] = (float)Math.Sin(i / freq);
            for (i = srclen; i < srclen + 100; i++)
                src[i] = -99.0f;

            for (i = 0; i < dstlen + 100; i++)
                dst[i] = -99.0f;

            resampler = new ReSampler(true, factor, factor);
            fwidth = resampler.GetfilterWidth();
            outi = 0;
            srcpos = 0;
            for (; ; )
            {
                int srcBlock = Math.Min(srclen - srcpos, srcblocksize);
                bool lastFlag = (srcBlock == srclen - srcpos);

                var result = resampler.Process(factor, src, srcpos, srcBlock,
                    lastFlag,
                    dst, outi, Math.Min(dstlen - outi, dstblocksize));

                o = result.OutputSamplesgenerated;
                srcused = result.InputSamplesConsumed;

                //o = resampler.process(factor,
                //                     &src[srcpos], srcBlock,
                //                     lastFlag, &srcused,
                //                     &dst[out], min(dstlen-out, dstblocksize));

                srcpos += srcused;
                if (o >= 0)
                    outi += o;
                if (o < 0 || (o == 0 && srcpos == srclen))
                    break;
            }

            if (o < 0)
            {
                Console.WriteLine("Error: resample_process returned an error: {0}", o);
            }

            if (outi <= 0)
            {
                Console.WriteLine("Error: resample_process returned {0} samples\n", outi);
                return;
            }

            lendiff = Math.Abs(outi - expectedlen);
            if (lendiff > (int)(2 * factor + 1.0))
            {
                Console.WriteLine("Expected ~{0}, got {1} samples out\n",
                    expectedlen, outi);
            }

            sum = 0.0;
            sumsq = 0.0;
            errcount = 0;

            /* Don't compute statistics on all output values; the last few
     are guaranteed to be off because it's based on far less
     interpolation. */
            statlen = outi - fwidth;

            for (i = 0; i < statlen; i++)
            {
                double diff = Math.Sin((i / freq) / factor) - dst[i];
                if (Math.Abs(diff) > 0.05)
                {
                    if (errcount == 0)
                        Console.WriteLine("   First error at i={0}: expected {1}, got {2}",
                            i, Math.Sin((i / freq) / factor), dst[i]);
                    errcount++;
                }
                sum += Math.Abs(diff);
                sumsq += diff * diff;
            }

            rangecount = 0;
            for (i = 0; i < statlen; i++)
            {
                if (dst[i] < -1.01 || dst[i] > 1.01)
                {
                    if (rangecount == 0)
                        Console.WriteLine("   Error at i={0}: value is {1}", i, dst[i]);
                    rangecount++;
                }
            }
            if (rangecount > 1)
                Console.WriteLine("   At least {0} samples were out of range", rangecount);

            if (errcount > 0)
            {
                i = outi - 1;
                Console.WriteLine("   i={0}:  expected {1}, got {2}\n",
                    i, Math.Sin((i / freq) / factor), dst[i]);
                Console.WriteLine("   At least {0} samples had significant error.", errcount);
            }
            err = sum / statlen;
            rmserr = Math.Sqrt(sumsq / statlen);
            Console.WriteLine("   Out: {0} samples  Avg err: {1} RMS err: {2}", outi, err, rmserr);
        }
    }
}
