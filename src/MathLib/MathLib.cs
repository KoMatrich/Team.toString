namespace MathLib
{
    public static class MyMath
    {
        //  1/(10^28) is smallest decimal number
        public static readonly decimal Epsilon = (decimal)(1 / System.Math.Pow(10, 20));

        public static bool Equal(decimal number1, decimal number2)
        {
            return System.Math.Abs(number1 - number2) < Epsilon;
        }

        public static decimal Add(decimal number1, decimal number2)
        {
            return ((number1)+(number2));
        }

        public static decimal Sub(decimal number1, decimal number2)
        {
            return ((number1) - (number2));
        }

        public static decimal Mult(decimal number1, decimal number2)
        {
            return ((number1) * (number2));
        }

        public static decimal Div(decimal number1, decimal number2)
        {
            return ((number1) / (number2));
        }

        public static decimal Mod(decimal number1, decimal number2)
        {
            return ((number1) % (number2));
        }

        public static decimal Fact(decimal number)
        {
            if (Mod(number, 1) == 0) {
                if (number == 0) {
                    return 1;
                }
                int result = 1;
                for (int i = 1; i <= number; i++) {
                    result *= i;
                }
                return result;
            }
            else {
                return System.Convert.ToDecimal(Gamma(decimal.ToDouble(number)));
            }            
        }

        private static double Gamma(double n)
        {
            int g = 7;
            double[] p = {0.99999999999980993, 676.5203681218851, -1259.1392167224028,
                                 771.32342877765313, -176.61502916214059, 12.507343278686905,
                                 -0.13857109526572012, 9.9843695780195716e-6, 1.5056327351493116e-7};
            if (n < 0.5) {
                return System.Math.PI / (System.Math.Sin(System.Math.PI * n) * Gamma(1 - n));
            }
            n -= 1;
            double x = p[0];
            for (var i = 1; i < g ;i++) {
                x += p[i] / (n + i);
            }
            double t = n + g + 0.5;
            return (System.Math.Sqrt(2 * System.Math.PI) * (System.Math.Pow(t, n + 0.5)) * System.Math.Exp(-t) * x)/2;
               
        }


        public static decimal Pow(decimal number, decimal n)
        {
            if(number == 0) {
                return 0;
            }
            if (n == 0) {
                return 1;
            }
            if(n < 0) {
                number = 1 / number;
                n *= -1;
            }
            decimal result = number;
            for(decimal i = 1; i < n ; i++) {
                result *= number;
            }
            return result;
        }

        public static decimal Root(decimal number, decimal n)
        {


            if (n < 1 && n > 0)
                return (Pow(number, 1 / n));

            decimal _n = Abs(n);
            decimal[] x = new decimal[2];
            x[0] = number;
            x[1] = number / _n;
            while (Abs(x[0] - x[1]) > Epsilon) {
                x[1] = x[0];
                x[0] = (1 / _n) * (((_n - 1) * x[1]) + (number / Pow(x[1], _n - 1)));

            }
            if (n > 0) {
                return x[0];
            }
            else {
                return (1 / x[0]);
            }
        }


        public static decimal Sqrt(decimal number)
        {
            return Root(number, 2);
        }

        public static decimal Abs(decimal number)
        {
            if (number < 0)
                return (number*(-1));
            return number;
        }

        public static decimal Rand()
        {
            System.Random rand = new System.Random();
            return rand.Next(0, 1000);
        }
    }
}
