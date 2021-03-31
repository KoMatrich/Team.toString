using Microsoft.VisualStudio.TestTools.UnitTesting;
using static MathLib.MyMath;

namespace MathLib.Tests
{
    [TestClass()]
    public class MathTests
    {
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

        [TestMethod()]
        public void MultTest()
        {
            decimal[,] val =
            {
                { 0, 0, 0 }, { 0, 0.5m, 0}, { 0.5m, 0.2m, 0.01m}, {-1, 0.5m, -0.5m}
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

        [TestMethod()]
        public void ModTest()
        {
            decimal[,] val =
            {
                { 0, 1, 0 }, { 0.5m, 0.1m, 0}, { 0.5m, 0.4m, 0.1m}, {14, 5, 4}, {-7, 5, -2} , {7,-5,-3}
            };

            for (int i = 0; i < val.GetLength(0); i++) {
                decimal x = val[i, 0];
                decimal y = val[i, 1];
                decimal z = val[i, 2];

                string msg = $"({x}) % ({y}) != {z}";
                Assert.IsTrue(Equal(Mod(x, y), z), msg);
            }
        }

        [TestMethod()]
        public void FactTest()
        {
            decimal[,] val =
            {
                { 0, 0 }, { 1, 1}, {5, 120},{0.5m, 0.88622692545275801364908374167057m}
            };

            for (int i = 0; i < val.GetLength(0); i++) {
                decimal x = val[i, 0];
                decimal y = val[i, 1];

                string msg = $"({x})! != {y}";
                Assert.IsTrue(Equal(Fact(x), y), msg);
            }
        }

        [TestMethod()]
        public void PowTest()
        {
            decimal[,] val =
            {
                { 0, 1, 0 }, { 99, 0, 1}, { 10, -2, 0.01m}, {14, 5, 4}, {-7, 5, -2} , {7,-5,-3}
            };

            for (int i = 0; i < val.GetLength(0); i++) {
                decimal x = val[i, 0];
                decimal y = val[i, 1];
                decimal z = val[i, 2];

                string msg = $"({x})^({y}) != {z}";
                Assert.IsTrue(Equal(Pow(x, y), z), msg);
            }
        }

        [TestMethod()]
        public void RootTest()
        {
            decimal[,] val =
            {
                { 0, 2, 0 }, { 99, 2, 9.9498743710661995473447982100121m},
                { 25, 2, 5}, {1/4, 2, 1/2}, {125, 3, 5}, {7,1/4,2401},{2, -2, 0.70710678118654752440084436210485m}
            };

            for (int i = 0; i < val.GetLength(0); i++) {
                decimal x = val[i, 0];
                decimal y = val[i, 1];
                decimal z = val[i, 2];

                string msg = $"({x})^[1/({y})] != {z}";
                Assert.IsTrue(Equal(Root(x, y), z), msg);
            }
        }

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
            //chance that this test will randomly fail is 10^28^1000
            //from that we can say that this test is always testing correctly
        }
    }
}
