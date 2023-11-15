using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid
{
    public class Utility
    {
        // Длина двумерного вектора (координаты типа float)
        public static double Length(PointF vector)
        {
            return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        }

        // Длина двумерного вектора (координаты типа int)
        public static double Length(Point vector) 
        {
            return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        }

        // Повернуть вектор на угол (в радианах)
        public static PointF RotateVector(PointF vector, float angle)
        {
            float x = vector.X;
            float y = vector.Y;
            var rotatedX = x * Math.Cos(angle) - y * Math.Sin(angle);
            var rotatedY = x * Math.Sin(angle) + y * Math.Cos(angle);

            return new PointF((float)rotatedX, (float)rotatedY);
        }

        // Функция "отзеркаливания" вектора для отскока мяча
        public static PointF MirrorVector(PointF vector, Point mirrorVector)
        {
            var x = vector.X; var y = vector.Y;
            if (Math.Abs(mirrorVector.X) < Math.Abs(mirrorVector.Y))
                return new PointF(x, -y);
            else if (Math.Abs(mirrorVector.Y) < Math.Abs(mirrorVector.X))
                return new PointF(-x, y);
            else if (Math.Abs(mirrorVector.X) == Math.Abs(mirrorVector.Y))
                return new PointF(-x, -y);
            else
                throw new NotImplementedException();
        }

        // Функция нахождения угла коллизии
        public static double CalcAngle(Ball ball, Point intersection)
        {
            float ax = intersection.X - (ball.xf + ball.Radius);
            float ay = intersection.Y - (ball.yf + ball.Radius);

            float bx = ball.speedX;
            float by = ball.speedY;

            double cos =
                (ax * bx + ay * by) /
                (
                Utility.Length(new PointF(ax, ay)) *
                Utility.Length(new PointF(bx, by))
                );

            cos = Math.Min(1, cos);
            cos = Math.Max(-1, cos);

            return Math.Acos(cos);
        }
    }
}
