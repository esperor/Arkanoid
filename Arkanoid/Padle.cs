namespace Arkanoid
{
    // Каретка по факту является просто блоком, который может двигаться
    // и не может быть уничтожен
    public class Padle : Block
    {
        // Зададим скорость относительно оси X
        public int Speed = 0;
        // Статичская константа класса, отражающая максимальную скорость каретки
        public const int MaxSpeed = 15;
        // Определим логические переменные для понимания направления движения каретки
        public bool movingLeft = false;
        public bool movingRight = false;

        // Конструктор можно оставить пустым
        public Padle()
        {}
    }
}