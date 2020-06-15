using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Coname
{
	class MainClass
	{
        static public int BulletCounter = -1;
        public class GameObject
        {
            public char Symbol { get; set; }
            public void Print()
            {
                Console.Write(Symbol);
            }
            public int ID { get; set; }
        }
        public class MovingGameObject : GameObject
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int SpawnX { get; set; }
            public int SpawnY { get; set; }
            public string Direction { get; set; }

        }

        public class Player : MovingGameObject
		{
            ConsoleKeyInfo LastTouchedKey { get; set; }
            public static Bullet b = new Bullet('*', 30);
            List<ConsoleKey> Controls;
            public bool CoolDown = false;
            public int DieCounter { get; set; }
            public string Name { get; set; }
            public Player(int x, int y, char f, int id, List<ConsoleKey> contr, string n)
			{
                SpawnX = x;
                X = SpawnX;
				SpawnY = y;
                Y = SpawnY;
				Symbol = f;
                ID = id;
                Controls = contr;
                Name = n;
			}
			
			public void Move ()
			{
                Gameplay.Objects[Y, X] = ID;
            }
            public void Act(ConsoleKeyInfo ltk)
            {
                //LastTouchedKey = Console.ReadKey(true);

                LastTouchedKey = ltk;

                int buffer = Gameplay.BF.Walls[Y, X];

                if (LastTouchedKey.Key == Controls[0])
                {
                    Direction = "u";
                    if (Gameplay.Objects[Y - 1, X] == 1 || Y - 1 == -1 || Gameplay.Objects[Y - 1, X] == 3 || Gameplay.Objects[Y - 1, X] == 2 || Gameplay.Objects[Y - 1, X] == 666)
                        return;
                    Y--;
                    Move();
                    Gameplay.Objects[Y + 1, X] = buffer;
                }
                if (LastTouchedKey.Key == Controls[1])
                {
                    Direction = "l";
                    if (Gameplay.Objects[Y, X - 1] == 1 || X - 1 == -1 || Gameplay.Objects[Y, X - 1] == 3 || Gameplay.Objects[Y, X - 1] == 2 || Gameplay.Objects[Y, X - 1] == 666)
                        return;
                    X--;              
                    Move();
                    Gameplay.Objects[Y, X + 1] = buffer;
                }
                if (LastTouchedKey.Key == Controls[2])
                {
                    Direction = "d";
                    if (Gameplay.Objects[Y + 1, X] == 1 || Gameplay.Objects[Y + 1, X] == 3 || Gameplay.Objects[Y + 1, X] == 2 || Gameplay.Objects[Y + 1, X] == 666)
                        return;
                    Y++; 
                    Move();
                    Gameplay.Objects[Y - 1, X] = buffer;
                }
                if (LastTouchedKey.Key == Controls[3])
                {
                    Direction = "r";
                    if (Gameplay.Objects[Y, X + 1] == 1 || Gameplay.Objects[Y, X + 1] == 3 || Gameplay.Objects[Y, X + 1] == 2 || Gameplay.Objects[Y, X + 1] == 666)
                        return;
                    X++;   
                    Move();
                    Gameplay.Objects[Y, X - 1] = buffer;
                }
                if (LastTouchedKey.Key == Controls[4])
                {
                    Shoot();
                }
            }

            public void Die(int y, int x)
            {
                DieCounter++;
                int buffer;
                CoolDown = true;
                buffer = Gameplay.BF.Walls[y, x];
                Gameplay.Objects[y, x] = 666;
                Thread.Sleep(1500);
                Gameplay.Objects[y, x] = buffer;
                X = SpawnX;
                Y = SpawnY;
                Gameplay.Objects[Y, X] = ID;
                CoolDown = false;
            }
            public async void Shoot()
            {
                b = new Bullet('*', 30);
                await Task.Run(() => b.Act(X, Y, Direction));
            }

            internal static void PrintGravestone()
            {
                Console.Write('X');
            }
        }
        public class Bullet : MovingGameObject
        {
            public int BulletSpeed { get; set; }

            public Bullet(char s, int bs)
            {
                Interlocked.Increment(ref BulletCounter);
                Symbol = s;
                BulletSpeed = bs;
                ID = 8;
            }
            ~Bullet()
            {
                Interlocked.Decrement(ref BulletCounter);
            }

            public void Act(int x, int y, string d)
            {
                X = x;
                Y = y;
                Direction = d;

                int buffer = Gameplay.BF.Walls[Y, X];

                try
                {
                    switch (Direction)
                    {
                        case "l":
                            while (Gameplay.Objects[Y, X - 1] != 1 && Gameplay.Objects[Y, X - 1] != 666)
                            {
                                if (Gameplay.Objects[Y, X - 1] == 2) 
                                {
                                    Gameplay.p1.Die(Y, X - 1);
                                    return;
                                }
                                if (Gameplay.Objects[Y, X - 1] == 3) 
                                {
                                    Gameplay.p2.Die(Y, X - 1);
                                    return;
                                }
                                Gameplay.Objects[Y, X - 1] = ID;
                                Thread.Sleep(BulletSpeed);
                                Gameplay.Objects[Y, X - 1] = buffer;
                                X--;
                            }
                            break;
                        case "r":
                            while (Gameplay.Objects[Y, X + 1] != 1 && Gameplay.Objects[Y, X + 1] != 666)
                            {
                                if (Gameplay.Objects[Y, X + 1] == 2)
                                {
                                    Gameplay.p1.Die(Y, X + 1);
                                    return;
                                }
                                if (Gameplay.Objects[Y, X + 1] == 3)
                                {
                                    Gameplay.p2.Die(Y, X + 1);
                                    return;
                                }
                                Gameplay.Objects[Y, X + 1] = ID;
                                Thread.Sleep(BulletSpeed);
                                Gameplay.Objects[Y, X + 1] = buffer;
                                X++;
                            }
                            break;
                        case "u":
                            while (Gameplay.Objects[Y - 1, X] != 1 && Gameplay.Objects[Y - 1, X] != 666)
                            {
                                if (Gameplay.Objects[Y - 1, X] == 2)
                                {
                                    Gameplay.p1.Die(Y - 1, X);
                                    return;
                                }
                                if (Gameplay.Objects[Y - 1, X] == 3)
                                {
                                    Gameplay.p2.Die(Y - 1, X);
                                    return;
                                }
                                Gameplay.Objects[Y - 1, X] = ID;
                                Thread.Sleep(BulletSpeed);
                                Gameplay.Objects[Y - 1, X] = buffer;
                                Y--;
                            }
                            break;
                        case "d":
                            while (Gameplay.Objects[Y + 1, X] != 1 && Gameplay.Objects[Y + 1, X] != 666)
                            {
                                if (Gameplay.Objects[Y + 1, X] == 2)
                                {
                                    Gameplay.p1.Die(Y + 1, X);
                                    return;
                                }
                                if (Gameplay.Objects[Y + 1, X] == 3)
                                {
                                    Gameplay.p2.Die(Y + 1, X);
                                    return;
                                }
                                Gameplay.Objects[Y + 1, X] = ID;
                                Thread.Sleep(BulletSpeed);
                                Gameplay.Objects[Y + 1, X] = buffer;
                                Y++;
                            }
                            break;
                    }
                }
                catch { }
                
            }
        }
        public class Wall : GameObject
        {

        }
        public class Floor : GameObject
        {

        }
        public class BattleField
		{
            public int[,] Walls { get; set; }
            public Wall wall = new Wall();
			public Floor floor = new Floor();

			public BattleField (char w, char f)
			{
				wall.Symbol = w;
				floor.Symbol = f;
                Test();
			}

            public BattleField()
            {
                Test();
            }
            public void Test()
			{
				Walls = new int[,] { {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
					  				 {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1},
                                     {1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1},
                                     {1, 0, 0, 0, 0, 1, 0, 1, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 1},
                                     {1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1},
                                     {1, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 1},
                                     {1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1},
                                     {1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1},
                                     {1, 0, 1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1},
                                     {1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1},
                                     {1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1},
                                     {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1},
                                     {1, 0, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1},
                                     {1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                     {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1} };
			}
            public void GenerateRandom()
            {

            }

        }		

		public class Gameplay 
		{
            public static int FPS;

            public static Player p1;
            public static Player p2;

            static List<ConsoleKey> controls1 = new List<ConsoleKey> { ConsoleKey.W, ConsoleKey.A, ConsoleKey.S, ConsoleKey.D, ConsoleKey.Spacebar };
            static List<ConsoleKey> controls2 = new List<ConsoleKey> { ConsoleKey.UpArrow, ConsoleKey.LeftArrow, ConsoleKey.DownArrow, ConsoleKey.RightArrow, ConsoleKey.NumPad0 };

            public static BattleField BF = new BattleField('█', ' ');

            public static int[,] Objects { get; set; }

            public static void Initialize(int fps)
            {
                p1 = new Player(1, 1, '@', 2, controls1, "Володя");
                p2 = new Player(BF.Walls.GetLength(1) - 2, BF.Walls.GetLength(0) - 2, '$', 3, controls2, "Влад");
                FPS = fps;
                Objects = (int[,])BF.Walls.Clone();
                Objects[p1.Y, p1.X] = p1.ID;
                Objects[p2.Y, p2.X] = p2.ID;
                Print(null);

                TimerCallback tcbPrint = new TimerCallback(Print);
                Timer timer = new Timer(tcbPrint, null, 0, FPS);

            }

            public static void PlayMultiplayer()
			{
                while (true)
                {
                    ConsoleKeyInfo LastTouchedKey = Console.ReadKey(true);
                    if (controls1.Contains(LastTouchedKey.Key) && !p1.CoolDown)
                        p1.Act(LastTouchedKey);
                    if (controls2.Contains(LastTouchedKey.Key) && !p2.CoolDown) p2.Act(LastTouchedKey);
                }
            }
            public static void Print(object obj)
			{
                Console.SetCursorPosition(0, 0);
                Console.ForegroundColor = ConsoleColor.Cyan;

                for (int i = 0; i < Objects.GetLength(0); i++) 
				{
					for (int j = 0; j < Objects.GetLength(1); j++) 
					{
						if (Objects [i, j] == 1)
							BF.wall.Print();
                        if (Objects[i, j] == 2)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            p1.Print();
                            Console.ForegroundColor = ConsoleColor.Cyan;
                        }
                        if (Objects[i, j] == 3)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            p2.Print();
                            Console.ForegroundColor = ConsoleColor.Cyan;
                        }
                        if (Objects[i, j] == 8)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Player.b.Print();
                            Console.ForegroundColor = ConsoleColor.Cyan;
                        }
                        if (Objects[i, j] == 666)
                            Player.PrintGravestone();
                        if (Objects[i, j] == 0)
                            BF.floor.Print();
                    }
					Console.Write('\n');
				}
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\n\n\nУправление {p1.Name}({p1.Symbol}): WASD, выстрел - Space");
                Console.WriteLine($"Управление {p2.Name}({p2.Symbol}): Стрелки, выстрел - Num0");
                Console.WriteLine("\n\n\nКоличество выстреленных пуль: " + BulletCounter);
                Console.WriteLine($"{p1.Name}({p1.Symbol}) умер {p1.DieCounter} раз");
                Console.WriteLine($"{p2.Name}({p2.Symbol}) умер {p2.DieCounter} раз");
            }
		}

		public static void Main (string[] args)
		{
			Console.CursorVisible = false;
            Console.Title = "Coname";
            
            Gameplay.Initialize(35);
            Gameplay.PlayMultiplayer();
        }
	}
}
