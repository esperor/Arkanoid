using System.Drawing;
using System.Windows.Forms;

namespace Arkanoid
{
    // Мяч является UserControl
    // Мяч имеет скорость, координаты, размер
    public class Ball : UserControl
    {
        // Скорость и координаты определим как отдельные значения
        // чтобы избежать ошибки marshal-by-reference
        public float speedX = 0; 
        public float speedY = 0;
        public float xf = 0;
        public float yf = 0;

        // Объявим статическую перменную для скорости мяча
        public const int Speed = 8;

        // Size по умолчанию int, но будет меняться в зависимости от размера окна
        public new int Size = 30; 
        
        // Radius даже не должен быть отдельной переменной, это просто Size / 2
        public int Radius
        { get { return Size / 2; } }

        // Конструктор можно оставить пустым
        public Ball()
        {}

        // По событию Paint рисуем круг
        protected override void OnPaint(PaintEventArgs e)
        {
            var brush = new SolidBrush(ForeColor);
            e.Graphics.FillEllipse(brush, new Rectangle(0, 0, Size, Size));
        }
    }
}