using Microsoft.VisualStudio.TestTools.UnitTesting;
using static MathLib.MyMath;

/**
 * Testy matematické knihovny.
 *
 * Testuje jednotlivé matematické funkce předdefinovanými konstantami.
 * výsledky matematických funkcí se nesmí lišit od axiomu o více jak Epsilon.
 * @see MathLib
 * @see MathLib.MyMath.Epsilon
 */

namespace Tests
{
    [TestClass()]
    public class MyMath
    {
        /**
        * Test na porovnávání čísel
        *
        * @warning tento test musí projít, aby mohly ostatní testy spolehlivě testovat funkce
        * @see MathLib.MyMath.Equal
        */

        [TestMethod()]
        public void EqualTest()
        {
            // input, input, result
            decimal[,] val_t =
            {
                { 0, 0 }, {Epsilon/10,0}, { 0.5m, 0.5m}, { -0.5m, -0.5m}
            };

            for (int i = 0; i < val_t.GetLength(0); i++) {
                decimal x = val_t[i, 0];
                decimal y = val_t[i, 1];

                //test fail MSG
                string msg = $"{x} != {y}";
                Assert.IsTrue(Equal(x, y), msg);

                //commutation test fail MSG
                msg = $"{y} != {x}";
                Assert.IsTrue(Equal(y, x), msg);
            }

            decimal[,] val_f =
            {
                { 1, 0 }, {Epsilon,0}, { 0.5m, -0.5m}, { -0.5m, Epsilon-0.5m}
            };

            for (int i = 0; i < val_f.GetLength(0); i++) {
                decimal x = val_f[i, 0];
                decimal y = val_f[i, 1];

                //test fail MSG
                string msg = $"{x} = {y}";
                Assert.IsFalse(Equal(x, y), msg);

                //commutation test fail MSG
                msg = $"{y} = {x}";
                Assert.IsFalse(Equal(y, x), msg);
            }
        }

        /**
        * Test na sčítání čísel
        *
        * @see MathLib.MyMath.Add
        */

        [TestMethod()]
        public void AddTest()
        {
            decimal[,] val =
            {
                { 0, 0, 0 }, { 0, 0.5m, 0.5m }, { 0.5m, 0.5m, 1}, {-1, 0.5m, -0.5m}
            };

            for (int i = 0; i < val.GetLength(0); i++) {
                decimal x = val[i, 0];
                decimal y = val[i, 1];
                decimal z = val[i, 2];

                string msg = $"({x}) + ({y}) != {z}";
                Assert.IsTrue(Equal(Add(x, y), z), msg);

                msg = $"({x}) + ({y}) != ({y}) + ({x})";
                Assert.IsTrue(Equal(Add(x, y), Add(y, x)), msg);
            }
        }

        /**
        * Test na odečítání čísel
        *
        * @see MathLib.MyMath.Sub
        */

        [TestMethod()]
        public void SubTest()
        {
            decimal[,] val =
            {
                { 0, 0, 0 }, { 0, 0.5m, -0.5m }, { 0.5m, 0.5m, 0}, {-1, 0.5m, -1.5m}
            };

            for (int i = 0; i < val.GetLength(0); i++) {
                decimal x = val[i, 0];
                decimal y = val[i, 1];
                decimal z = val[i, 2];

                string msg = $"({x}) - ({y}) != {z}";
                Assert.IsTrue(Equal(Sub(x, y), z), msg);

                msg = $"({x}) - ({y}) != -[({y}) - ({x})]";
                Assert.IsTrue(Equal(Sub(x, y), -Sub(y, x)), msg);
            }
        }

        /**
        * Test na násobení čísel
        *
        * @see MathLib.MyMath.Mult
        */

        [TestMethod()]
        public void MultTest()
        {
            decimal[,] val =
            {
                { 0, 0, 0 }, { 0, 0.5m, 0}, { 0.5m, 0.2m, 0.1m}, {-1, 0.5m, -0.5m}
            };

            for (int i = 0; i < val.GetLength(0); i++) {
                decimal x = val[i, 0];
                decimal y = val[i, 1];
                decimal z = val[i, 2];

                string msg = $"({x}) * ({y}) != {z}";
                Assert.IsTrue(Equal(Mult(x, y), z), msg);

                msg = $"({x}) * ({y}) != ({y}) * ({x})";
                Assert.IsTrue(Equal(Mult(x, y), Mult(y, x)), msg);
            }
        }

        /**
        * Test na dělení čísel
        *
        * @see MathLib.MyMath.Div
        */

        [TestMethod()]
        public void DivTest()
        {
            decimal[,] val =
            {
                { 0, 1, 0 }, { 0.5m, 0.1m, 5}, {-1, 5, -0.2m}
            };

            for (int i = 0; i < val.GetLength(0); i++) {
                decimal x = val[i, 0];
                decimal y = val[i, 1];
                decimal z = val[i, 2];

                string msg = $"({x}) / ({y}) != {z}";
                Assert.IsTrue(Equal(Div(x, y), z), msg);
            }
        }

        /**
        * Test na modulo operaci
        *
        * @see MathLib.MyMath.Mod
        */

        [TestMethod()]
        public void ModTest()
        {
            decimal[,] val =
            {
                { 0, 1, 0 }, { 0.5m, 0.1m, 0}, { 0.5m, 0.4m, 0.1m}, {14, 5, 4}, {-7, 5, -2} , {7,-5,2}
            };

            for (int i = 0; i < val.GetLength(0); i++) {
                decimal x = val[i, 0];
                decimal y = val[i, 1];
                decimal z = val[i, 2];

                string msg = $"({x}) % ({y}) != {z}";
                Assert.IsTrue(Equal(Mod(x, y), z), msg);
            }
        }

        /**
        * Test na faktorial
        *
        * @see MathLib.MyMath.Fact
        */

        [TestMethod()]
        public void FactTest()
        {
            decimal[,] val =
            {
                { 0, 1 }, { 1, 1}, {5, 120},{0.5m, 0.886226923674294m}
            };

            for (int i = 0; i < val.GetLength(0); i++) {
                decimal x = val[i, 0];
                decimal y = val[i, 1];

                string msg = $"({x})! != {y}";
                Assert.IsTrue(Equal(Fact(x), y), msg);
            }
        }

        /**
        * Test na funkci mocniny
        *
        * @see MathLib.MyMath.Pow
        */

        [TestMethod()]
        public void PowTest()
        {
            decimal[,] val =
            {
                { 0, 1, 0 }, { 99, 0, 1}, { 10, -2, 0.01m}, {14, 5, 537824}, {-7, 5, -16807} //, {7,-5,0.000059499m}
            };

            for (int i = 0; i < val.GetLength(0); i++) {
                decimal x = val[i, 0];
                decimal y = val[i, 1];
                decimal z = val[i, 2];

                string msg = $"({x})^({y}) != {z}";
                Assert.IsTrue(Equal(Pow(x, y), z), msg);
            }
        }

        /**
        * Test na funkci odmocniny
        *
        * @see MathLib.MyMath.Root
        */

        [TestMethod()]
        public void RootTest()
        {
            decimal[,] val =
            {
                { 0, 2, 0 }, { 99, 2, 9.9498743710661995473447982100121m},
                { 25, 2, 5}, {1/4, 2, 1/2}, {125, 3, 5}, {7,0.25m,2401},{2, -2, 0.70710678118654752440084436210485m}
            };

            for (int i = 0; i < val.GetLength(0); i++) {
                decimal x = val[i, 0];
                decimal y = val[i, 1];
                decimal z = val[i, 2];

                string msg = $"({x})^[1/({y})] != {z}";
                Assert.IsTrue(Equal(Root(x, y), z), msg);
            }
        }

        /**
        * Test na funkci 2 odmocniny
        *
        * @see MathLib.MyMath.Sqrt
        */

        [TestMethod()]
        public void SqrtTest()
        {
            decimal[,] val =
            {
                { 0, 0 }, { 99, 9.9498743710661995473447982100121m},
                { 25, 5}, {1/4, 1/2}
            };

            for (int i = 0; i < val.GetLength(0); i++) {
                decimal x = val[i, 0];
                decimal y = val[i, 1];

                string msg = $"({x})^(1/2) != {y}";
                Assert.IsTrue(Equal(Sqrt(x), y), msg);
            }
        }

        /**
         * Test na absolutní hodnoutu
         *
         * @see MathLib.MyMath.Abs
         */

        [TestMethod()]
        public void AbsTest()
        {
            {
                decimal[,] val =
                {
                { 0, 0 }, { -9.9498743710661995473447982100121m, 9.9498743710661995473447982100121m},
                { 9.9498743710661995473447982100121m, 9.9498743710661995473447982100121m}
            };

                for (int i = 0; i < val.GetLength(0); i++) {
                    decimal x = val[i, 0];
                    decimal y = val[i, 1];

                    string msg = $"|{x}| != {y}";
                    Assert.IsTrue(Equal(Abs(x), y), msg);
                }
            }
        }

        /**
         * Test na generování náhodných čísel
         *
         * @note U tohoto testu je šasnce, že neprojde. Tato šance je 1 : (10^6^1000), kvůli samotné implementace funkce Rand
         * @see MathLib.MyMath.Rand
         */

        [TestMethod()]
        public void RandTest()
        {
            decimal val = Rand();
            bool found_dif = false;
            const int count = 1000;

            for (int i = 0; i < count; i++) {
                if (!Equal(val, Rand())) {
                    found_dif = true;
                    break;
                }
            }
            string msg = $"Random function generated {count} same numbers.";
            Assert.IsTrue(found_dif, msg);
        }
    }
}
