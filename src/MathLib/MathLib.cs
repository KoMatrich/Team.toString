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
            return 0;
        }

        public static decimal Sub(decimal number1, decimal number2)
        {
            return 0;
        }

        public static decimal Mult(decimal number1, decimal number2)
        {
            return 0;
        }

        public static decimal Div(decimal number1, decimal number2)
        {
            return 0;
        }

        public static decimal Mod(decimal number1, decimal number2)
        {
            return 0;
        }

        public static decimal Fact(decimal number)
        {
            return 0;
        }

        public static decimal Pow(decimal number, decimal n)
        {
            return 0;
        }

        public static decimal Root(decimal number, decimal n)
        {
            return 0;
        }

        public static decimal Sqrt(decimal number)
        {
            return 0;
        }

        public static decimal Abs(decimal number)
        {
            return 0;
        }

        public static decimal Rand()
        {
            return 0;
        }
    }
}
