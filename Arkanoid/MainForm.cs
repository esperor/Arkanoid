using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Arkanoid
{
    // Основная форма приложения
    public partial class MainForm : Form
    {
        // Определим массив блоков, изначально пустой
        // Определим каретку и мяч
        // Определим константы для количества строк и столбцов
        // Определим логические переменные, отслиживающие состояние игры
        List<DestroyableBlock> blocks = new List<DestroyableBlock>();
        Padle padle;
        Ball ball;
        const int rows = 3, columns = 10;
        bool initialized = false;
        Random random = new Random();

        // В конструкторе будем вызывать стандарртную инициализацию
        // Также определим стандартные размеры окна
        public MainForm()
        {
            InitializeComponent();
            Height = 1000;
            Width = 1000;
        }

        // Функция Init - реальное начало игры
        // Вызывается после нажатия на кнопку старта
        private void Init()
        {
            // Остановим отрисовку
            SuspendLayout();

            blocks.Clear();

            // Создадим все блоки с учетом заданного количества строк и столбцов
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    blocks.Add(new DestroyableBlock(i == rows - 1 ? 2 : 1));
                    var last = blocks.Last();
                    last._coords = new Point(j, i);
                    last.Parent = this;
                    last.Show();
                    // Цвет блока зависит от количества хит-поинтов
                    last.ForeColor = last._health > 1 ? Color.DarkGreen : Color.Green;
                }
            }

            // Создадим каретку внизу примерно посередине экрана
            padle = new Padle
            {
                Parent = this,
                ForeColor = Color.DarkBlue,
                Location = new Point((Bounds.Width) / 2, (int)(Bounds.Height * 0.85)),
            };
            padle.Show();

            PointF ballSpeedV = new PointF(0, Ball.Speed);
            // Создадим случайный угол для поворота мяча
            // Угол будет не больше pi / 5 + 0.2 и не меньше 0.2 (рад)
            // Также угол может быть отриацтельным или положительным
            float angle = (float)(random.NextDouble() * Math.PI / 5 + 0.2) * 
                (random.Next() % 2 == 1 ? 1 : -1); // Эта строка случайно умножает угол на 1 либо -1

            ballSpeedV = Utility.RotateVector(ballSpeedV, angle);

            // Создадим мяч
            ball = new Ball()
            {
                Parent = this,
                ForeColor = Color.LightBlue,
                speedX = ballSpeedV.X,
                speedY = ballSpeedV.Y,
                // Сделаем положение мяча случайным в пределах допустимой зоны
                xf = (float)Math.Min(Bounds.Width * random.NextDouble(), Bounds.Width - 50), 
                yf = Bounds.Height * (float)(random.NextDouble() / 5 + 0.3),
            };
            ball.Show();

            initialized = true;

            // Включим таймер для игрового цикла
            mainTimer.Enabled = true;
            // Запустим определение размеров для элементов
            MainForm_Resize(this, null);
            // Восстановим отрисовку
            ResumeLayout();
        }

        // Обработки нажатия на кнопку старта
        private void btn_start_Click(object sender, EventArgs e)
        {
            btn_start.Enabled = false;
            btn_start.Hide();
            Init();
        }

        // Обработка победы
        private void Lost()
        {
            StopGame();
            btn_start.Text = "Вы проиграли :(\r\nНачать с начала?";
        }

        // Обработки поражения
        private void Win()
        {
            StopGame();
            btn_start.Text = "Вы выиграли! :)\r\nНачать с начала?";
        }

        // Завершение игры
        private void StopGame()
        {
            mainTimer.Stop();
            padle.Hide();
            padle = null;
            ball.Hide();
            ball = null;
            foreach (var block in blocks)
                block.Hide();

            blocks.Clear();
            btn_start.Enabled = true;
            btn_start.Show();
            initialized = false;
        }

        // Процедура игрового тика
        // Запускается раз в 20 мс
        private void mainTimer_Tick(object sender, EventArgs e)
        {
            // Остановим отрисовку
            SuspendLayout();
            // Переместим мяч 
            ball.xf += ball.speedX;
            ball.yf += ball.speedY;

            if (ball.yf > Bounds.Height + 50)
            {
                Lost();
                return;
            }

            // Если мяч косается стен, кроме нижней, отзеркалим его
            if (ball.xf + ball.Size * 1.5 >= Bounds.Width || ball.xf <= 0)
                ball.speedX = -ball.speedX;
            if (ball.yf <= 0)
                ball.speedY = -ball.speedY;

            // ------------ КАРЕТКА
            // Обработаем перемещение каретки
            if (padle.Speed != 0)
                padle.Location = new Point(padle.Location.X + padle.Speed, padle.Location.Y);

            // Остановим каретку у края
            if (padle.Location.X <= 0)
                padle.Location = new Point(0, padle.Location.Y);
            else if (padle.Location.X + padle.Width >= Bounds.Width)
                padle.Location = new Point(Bounds.Width - padle.Width, padle.Location.Y);

            // Обработаем возможное перемечение каретки и мяча
            if (CheckIntersectionWithBlock(padle))
            { // Подкорректируем вектор движения мяча в зависимости от движения каретки
                PointF? newSpeed = null;
                if (padle.Speed > 0)
                    newSpeed = Utility.RotateVector(new PointF(ball.speedX, ball.speedY), (float)(Math.PI / 18));
                else if (padle.Speed < 0)
                    newSpeed = Utility.RotateVector(new PointF(ball.speedX, ball.speedY), (float)(-Math.PI / 18));
                if (newSpeed != null)
                {
                    ball.speedX = newSpeed.Value.X;
                    ball.speedY = newSpeed.Value.Y;
                }
            }

            // ------------ БЛОКИ
            // Так как операция достаточно тяжелая, будем проверять коллизию
            // мяча с блоками только если он в определенной зоне экрана
            if (ball.yf < (int)(Bounds.Height * 0.3f) + ball.Size) 
            {
                foreach(var block in blocks)
                {
                    if (CheckIntersectionWithBlock(block))
                    {
                        block._health--; // Уменьшим хп

                        switch (block._health)
                        { // Обновим цвет блока в завимисимости от оставшихся хит-поинтов
                            case 0:
                                block.Hide();
                                blocks.Remove(block); // Уничтожим блок с 0 хп
                                break;
                            case 1: block.ForeColor = Color.Green; break;
                            case 2: block.ForeColor = Color.DarkGreen; break;
                        }
                        break;
                    }     
                }
            }

            if (blocks.Count == 0)
            {
                Win();
                return;
            }

            // Обновим мяч
            ball.Location = new Point((int)ball.xf, (int)ball.yf);

            // Восстановим отрисовку
            ResumeLayout();
        }

        // Функция проверки пересечения мяча с блоком (любым, в том числе с кареткой)
        // Функция автоматически изменит траекторию движения мяча при коллизии и вернет
        // true, если коллизия была
        private bool CheckIntersectionWithBlock(Block block)
        {
            var intersection = block.IntersectsWith(new Point((int)ball.xf + ball.Radius, (int)ball.yf + ball.Radius), ball.Radius);
            if (intersection.HasValue)
            {
                PointF newSpeedVector;
                Point collisionVector =
                    new Point(
                        (int)(intersection.Value.X - (ball.xf + ball.Radius)),
                        (int)(intersection.Value.Y - (ball.yf + ball.Radius))
                    );

                // Подсчитаем угол коллизии
                double angle = Utility.CalcAngle(ball, intersection.Value);
                if (angle == 0) // Если угол = 0, отразим вектор скорости полностью
                {
                    PointF speedV = new PointF(ball.speedX, ball.speedY);
                    PointF colV = collisionVector;
                    double k = Utility.Length(speedV) / Utility.Length(colV);
                    newSpeedVector = new Point((int)(-colV.X * k), (int)(-colV.Y * k));
                }
                else
                {
                    // В других случаях отразим вектор скорости с учетом вектора коллизии
                    newSpeedVector = Utility.MirrorVector(
                        new PointF(ball.speedX, ball.speedY), collisionVector
                    );
                }
                // Обновим скорость
                ball.speedY = newSpeedVector.Y;
                ball.speedX = newSpeedVector.X;

                return true;
            }
            return false;
        }


        // Функции KeyUp и KeyDown отвечают за перемещение каретки

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (!initialized) return;

            if (e.KeyCode == Keys.A && padle.movingLeft)
            {
                padle.Speed += Padle.MaxSpeed;
                padle.movingLeft = false;
            }
            if (e.KeyCode == Keys.D && padle.movingRight)
            {
                padle.Speed -= Padle.MaxSpeed;
                padle.movingRight = false;
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (!initialized) return;

            if (e.KeyCode == Keys.A && !padle.movingLeft)
            {
                padle.Speed -= Padle.MaxSpeed;
                padle.movingLeft = true;
            }
                
            if (e.KeyCode == Keys.D && !padle.movingRight)
            {
                padle.Speed += Padle.MaxSpeed;
                padle.movingRight = true;
            }
                
        }


        // По событию Resize обновим размеры элементов, положение блоков
        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (initialized)
            {
                SuspendLayout();

                var width = (int)Math.Floor(((double)Bounds.Width * 0.98f) / ((double)columns));
                var height = (int)(Bounds.Height * 0.1f);

                foreach(var block in blocks)
                {
                    block.Width = width;
                    block.Height = height;
                    block.Location = new Point(block._coords.X * block.Width, block._coords.Y * block.Height);
                }

                padle.Width = width;
                padle.Height = width / 4;

                ball.Size = width / 5;
                ball.Width = ball.Size;
                ball.Height = ball.Size;

                ResumeLayout();
            }
        }
    }
}
