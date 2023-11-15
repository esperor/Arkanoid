using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using Arkanoid;

namespace tests
{
    [TestClass]
    public class GameTests
    {

        // Тестовый метод проверяет работу функции нахождения точки пересечения
        [TestMethod]
        public void TestBlockIntersects()
        {
            Block block = new Block();
            block.Width = 100;
            block.Height = 100;
            block.Location = new Point(0, 0);
            Point? intersection = block.IntersectsWith(new Point(120, 100), 20);
            Assert.IsTrue(intersection.HasValue);
            Assert.AreEqual(new Point(100, 100), intersection.Value);

            block.Width = 200;
            block.Height = 200;
            intersection = block.IntersectsWith(new Point(220, 220), 30);
            Assert.IsTrue(intersection.HasValue);
            Assert.AreEqual(new Point(200, 200), intersection.Value);

            intersection = block.IntersectsWith(new Point(220, 100), 40);
            Assert.IsTrue(intersection.HasValue);
            Assert.AreEqual(new Point(200, 100), intersection.Value);

            intersection = block.IntersectsWith(new Point(400, 100), 5);
            Assert.IsFalse(intersection.HasValue);

            block.Width = 200;
            block.Height = 200;
            block.Location = new Point(250, 100);
            intersection = block.IntersectsWith(new Point(50, 50), 30);
            Assert.IsFalse(intersection.HasValue);

            block.Width = 200;
            block.Height = 100;
            block.Location = new Point(200, 100);
            intersection = block.IntersectsWith(new Point(300, 70), 30);
            Assert.IsTrue(intersection.HasValue);
            Assert.AreEqual(new Point(300, 100), intersection.Value);
        }

        [TestMethod]
        public void TestRotateVector()
        {
            PointF vector = new PointF(0, 5);
            PointF rotated = Utility.RotateVector(vector, (float)Math.PI / 2f);
            Assert.AreEqual(new Point(-5, 0), new Point((int)rotated.X, (int)rotated.Y));

            vector = new PointF(0, 5);
            rotated = Utility.RotateVector(vector, (float)Math.PI);
            Assert.AreEqual(new Point(0, -5), new Point((int)rotated.X, (int)rotated.Y));

            vector = new PointF(5, 7);
            rotated = Utility.RotateVector(vector, (float)Math.PI / 18);
            Assert.AreEqual(new Point(3, 7), new Point((int)rotated.X, (int)rotated.Y));
        }
    }
}
