/****************************************************
 * How to write license ? any one could provide help?
 * 
 ***************************************************/

using System;
using System.ComponentModel;

namespace LibResample.Sharp
{
    /// <summary>
    /// reference: "Digital Filters, 2nd edition"
    /// This file provides Kaiser-windowed low-pass filter support,
    /// including a function to create the filter coefficients, and
    /// two functions to apply the filter at a particular point.
    /// 
    /// reference: "Digital Filters, 2nd edition"
    ///            R.W. Hamming, pp. 178-179
    ///
    /// Izero() computes the 0th order modified bessel function of the first kind.
    ///    (Needed to compute Kaiser window).
    ///
    /// LpFilter() computes the coeffs of a Kaiser-windowed low pass filter with
    ///    the following characteristics:
    ///
    ///       c[]  = array in which to store computed coeffs
    ///       frq  = roll-off frequency of filter
    ///       N    = Half the window length in number of coeffs
    ///       Beta = parameter of Kaiser window
    ///       Num  = number of coeffs before 1/frq
    ///
    /// Beta trades the rejection of the lowpass filter against the transition
    ///    width from passband to stopband.  Larger Beta means a slower
    ///    transition and greater stopband rejection.  See Rabiner and Gold
    ///    (Theory and Application of DSP) under Kaiser windows for more about
    ///    Beta.  The following table from Rabiner and Gold gives some feel
    ///    for the effect of Beta:
    ///
    /// All ripples in dB, width of transition band = D*N where N = window length
    ///
    ///               BETA    D       PB RIP   SB RIP
    ///               2.120   1.50  +-0.27      -30
    ///               3.384   2.23    0.0864    -40
    ///               4.538   2.93    0.0274    -50
    ///               5.658   3.62    0.00868   -60
    ///               6.764   4.32    0.00275   -70
    ///               7.865   5.0     0.000868  -80
    ///               8.960   5.7     0.000275  -90
    ///               10.056  6.4     0.000087  -100
    /// </summary>
    public class FilterKit
    {
        private const double ZeroEpsilon = 1e-21;

        private static double GetIdealZero(double x)
        {
            double u;
            int n;

            var sum = u = n = 1;
            var halfx = x / 2.0;
            do
            {
                var temp = halfx / n;
                n += 1;
                temp *= temp;
                u *= temp;
                sum += u;
            } while (u >= ZeroEpsilon * sum);

            return sum;
        }

        public static void LrsLpFilter(double[] c, int n, double frq, double beta, int num)
        {
            double ibeta, temp, temp1, inm1;
            int i;

            // Calculate ideal lowpass filter impulse response :
            c[0] = 2.0 * frq;
            for (i = 0; i < n; i++)
            {
                temp = Math.PI * (double)i / (double)num;
                c[i] = Math.Sin(2.0 * temp * frq) / temp; // Analog sinc function
            }

            /*
             * Calculate and Apply Kaiser window to ideal lowpass filter. Note: last
             * window value is IBeta which is NOT zero. You're supposed to really
             * truncate the window here, not ramp it to zero. This helps reduce the
             * first sidelobe.
             */
            ibeta = 1.0 / GetIdealZero(beta);
        }

    }
}
