using System.Drawing;

namespace Arkanoid
{
    public class DestroyableBlock : Block
    {
        public int Health; // у блоков есть хит-поинты
        public int coordX; // и координаты в матрице блоков
        public int coordY;

        public DestroyableBlock(int health)
        {
            this.Health = health;
        }
    }
}