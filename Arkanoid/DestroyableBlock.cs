using System.Drawing;
using System.Windows.Forms;

namespace Arkanoid
{
    public class DestroyableBlock : Block
    {
        public int _health; // у блоков есть хит-поинты
        public Point _coords; // и координаты в матрице блоков

        public DestroyableBlock(int health)
        {
            this._health = health;
        }
    }
}