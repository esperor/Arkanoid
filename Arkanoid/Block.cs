using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Arkanoid
{

    public class Block: UserControl
    {
        // Конструктор можно оставить пустым
        public Block()
        {}

        // Функция, находящая точку пересечения между этим блоком и мячом
        // если такая существует. В случае наличия двух точек пересечения, 
        // функция вернет среднюю между ними двумя.
        public Point? IntersectsWith(Point ballCenter, int ballRadius)
        {
            // Определим массив отрезков - сторон
            Tuple<Point, Point>[] sides = new Tuple<Point, Point>[4];
            sides[0] = new Tuple<Point, Point>(Location, new Point(Location.X + Width, Location.Y + 0));
            sides[1] = new Tuple<Point, Point>(Location, new Point(Location.X + 0, Location.Y + Height));
            sides[2] = new Tuple<Point, Point>(new Point(Location.X + Width, Location.Y + 0), new Point(Location.X + Width, Location.Y + Height));
            sides[3] = new Tuple<Point, Point>(new Point(Location.X + 0, Location.Y + Height), new Point(Location.X + Width, Location.Y + Height));

            int r = ballRadius;

            PointF? firstInter = null; // первое найденное пересечение
            PointF? secondInter = null; // второе найденной пересечение

            // переменная будет отслеживать количество найденных пересечений
            bool firstFilled = false; 

            for (int i = 0; i < 4; i++)
            {
                double x1 = sides[i].Item1.X - ballCenter.X;
                double x2 = sides[i].Item2.X - ballCenter.X;
                double y1 = sides[i].Item1.Y - ballCenter.Y;
                double y2 = sides[i].Item2.Y - ballCenter.Y;

                // Найдем ближайшую к центру мяча точку, лежащую на стороне
                // Для этого возьмем x1;y1 за начало координат 
                //
                // 'a' здесь будет вектором стороны относительно (x1;y1)
                // 'b' будет вектором центра мяча относительно (x1;y1)
                // 'c' будет проекцией вектора b на вектор a относительно (x1;y1)

                x2 -= x1;
                y2 -= y1;

                double bx = -x1;
                double by = -y1;

                double ax = x2;
                double ay = y2;

                // Найдем длину a
                double aLen = Utility.Length(new PointF((float)ax, (float)ay)); 

                double cLen = (ax * bx + ay * by) / aLen; // Найдем длину вектора c

                // По формуле найдем вектор c
                PointF c = new PointF((int)(ax * cLen / aLen), (int)(ay * cLen / aLen)); 

                // Убедимся, что c находится на отрезке, а не просто на прямой
                c.X = c.X > ax ? (int)ax : c.X;
                c.X = c.X < 0 ? 0 : c.X;
                c.Y = c.Y > ay ? (int)ay : c.Y;
                c.Y = c.Y < 0 ? 0 : c.Y;

                // Вернемся к исходной системе координат
                PointF closest = new PointF(ballCenter.X + (int)x1 + c.X, ballCenter.Y + (int)y1 + c.Y);

                // Еще проверим валидность найденной точки: проверим, что
                // расстояние между центром мяча и найденной точкой не 
                // превышает радиус мяча
                // 'd' здесь является вектором ближайшей точки относительно центра мяча
                PointF d = new PointF(closest.X - ballCenter.X, closest.Y - ballCenter.Y);
                double dLen = Utility.Length(d);

                if (dLen > r) continue;

                if (firstFilled) 
                    secondInter = closest;
                else
                    firstInter = closest;
            }

            // Вернем точку либо среднюю точку между двумя найденными
            if (secondInter.HasValue && firstInter.HasValue)
                return new Point((int)((firstInter.Value.X + secondInter.Value.X) / 2),
                                 (int)((firstInter.Value.Y + secondInter.Value.Y) / 2));
            else if (firstInter.HasValue) return new Point((int)firstInter.Value.X, (int)firstInter.Value.Y);

            return null;
        }

        // По событию Paint будет рисовать прямоугольник с закругленными углами
        protected override void OnPaint(PaintEventArgs e)
        {
            var brush = new SolidBrush(ForeColor);
            var path = Path.RoundedRect(new Rectangle(new Point(0, 0), new Size(Width, Height)), 5);
            e.Graphics.FillPath(brush, path);
        }
    }
}
