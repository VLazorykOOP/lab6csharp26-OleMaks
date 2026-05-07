using System;
using System.Collections;

namespace Lab6
{

    public interface IEngine
    {
        double Power { get; set; }
        void ShowInfo();
    }

    public interface ICombustionEngine : IEngine
    {
        int Cylinders { get; set; }
        void Refuel();
    }

    public interface IJetEngine : IEngine
    {
        double Thrust { get; set; }
        void EnableAfterburner();
    }

    // Класи, що реалізують інтерфейси
    public class DieselEngine : ICombustionEngine
    {
        public double Power { get; set; }
        public int Cylinders { get; set; }
        public bool HasTurbo { get; set; }

        public DieselEngine(double power, int cylinders, bool hasTurbo)
        {
            Power = power;
            Cylinders = cylinders;
            HasTurbo = hasTurbo;
        }

        public void ShowInfo() => Console.WriteLine($"[Дизель] Потужність: {Power}, Циліндри: {Cylinders}, Турбіна: {HasTurbo}");
        public void Refuel() => Console.WriteLine(" -> Заправляємо дизельне паливо...");
        public void CleanSootFilter() => Console.WriteLine(" -> Очищення сажового фільтра виконано (особистий метод дизеля).");
    }

    public class JetEngine : IJetEngine
    {
        public double Power { get; set; }
        public double Thrust { get; set; }

        public JetEngine(double power, double thrust)
        {
            Power = power;
            Thrust = thrust;
        }

        public void ShowInfo() => Console.WriteLine($"[Реактивний] Потужність: {Power}, Тяга: {Thrust} кН");
        public void EnableAfterburner() => Console.WriteLine(" -> Форсаж увімкнено! (особистий метод реактивного двигуна).");
    }


    public class FunctionCalculationException : Exception
    {
        public FunctionCalculationException(string message) : base(message) { }
    }

    public interface IFunction : ICloneable, IComparable<IFunction>
    {
        double Calculate(double x);
        void PrintInfo();
    }

    public class Line : IFunction
    {
        public double A { get; set; }
        public double B { get; set; }

        public Line(double a, double b) { A = a; B = b; }

        public double Calculate(double x) => A * x + B;
        public void PrintInfo() => Console.WriteLine($"Пряма: y = {A}x + {B}");

        public object Clone() => new Line(A, B);
        public int CompareTo(IFunction? other) => this.Calculate(1).CompareTo(other?.Calculate(1) ?? 0);
    }

    public class Kub : IFunction 
    {
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }

        public Kub(double a, double b, double c) { A = a; B = b; C = c; }

        public double Calculate(double x) => A * x * x + B * x + C;
        public void PrintInfo() => Console.WriteLine($"Парабола: y = {A}x^2 + {B}x + {C}");

        public object Clone() => new Kub(A, B, C);
        public int CompareTo(IFunction? other) => this.Calculate(1).CompareTo(other?.Calculate(1) ?? 0);
    }

    public class Hyperbola : IFunction
    {
        public double K { get; set; }

        public Hyperbola(double k) { K = k; }

        public double Calculate(double x)
        {
            try
            {
                if (x == 0)
                {
                    throw new DivideByZeroException("Спроба ділення на нуль при обчисленні гіперболи!");
                }
                if (K == 0)
                {
                    throw new FunctionCalculationException("Коефіцієнт K не може бути нулем для гіперболи.");
                }
                
                return K / x;
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine($"[ПОМИЛКА .NET]: {ex.Message}");
                return double.NaN;
            }
            catch (FunctionCalculationException ex)
            {
                Console.WriteLine($"[КАСТОМНА ПОМИЛКА]: {ex.Message}");
                return 0;
            }
        }

        public void PrintInfo() => Console.WriteLine($"Гіпербола: y = {K}/x");

        public object Clone() => new Hyperbola(K);
        public int CompareTo(IFunction? other) => this.Calculate(1).CompareTo(other?.Calculate(1) ?? 0);
    }
    
    public class EngineCollection : IEnumerable
    {
        private IEngine[] engines;

        public EngineCollection(IEngine[] engArray)
        {
            engines = engArray;
        }
        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < engines.Length; i++)
            {
                yield return engines[i]; 
            }
        }
    }

    
    class Program
    {
        static void Main()
        {
            Console.WriteLine("ЗАВДАННЯ 1: Масив інтерфейсів та Паттерни Типу");
            
            IEngine[] myEngines = new IEngine[]
            {
                new DieselEngine(150, 4, true),
                new JetEngine(50000, 120),
                new DieselEngine(300, 8, false)
            };

            foreach (var engine in myEngines)
            {
                engine.ShowInfo();
                AnalyzeEngine(engine);
                Console.WriteLine();
            }


            Console.WriteLine("ЗАВДАННЯ 2: .NET Інтерфейси (IComparable, ICloneable)");
            
            IFunction[] functions = new IFunction[]
            {
                new Kub(1, -3, 2),
                new Line(2, 5),
                new Hyperbola(10)
            };

            IFunction clonedLine = (IFunction)functions[1].Clone();
            Console.WriteLine("Оригінал:"); functions[1].PrintInfo();
            Console.WriteLine("Клон:"); clonedLine.PrintInfo();

            Console.WriteLine("\nСортування функцій (за значенням y(1)):");
            Array.Sort(functions);
            foreach (var f in functions)
            {
                f.PrintInfo();
                Console.WriteLine($"  Значення y(1) = {f.Calculate(1)}");
            }


            Console.WriteLine("\nЗАВДАННЯ 3: Обробка винятків");
            
            Hyperbola hyp1 = new Hyperbola(10);
            Console.WriteLine("Обчислення Hyperbola(10) в x = 2:");
            Console.WriteLine($"Результат: {hyp1.Calculate(2)}\n");

            Console.WriteLine("Обчислення Hyperbola(10) в x = 0 (викличе DivideByZeroException):");
            double res1 = hyp1.Calculate(0);
            
            Console.WriteLine("\nОбчислення Hyperbola(0) в x = 5 (викличе власний FunctionCalculationException):");
            Hyperbola hyp2 = new Hyperbola(0);
            double res2 = hyp2.Calculate(5);


            Console.WriteLine("\nЗАВДАННЯ 4: Інтерфейс IEnumerable (цикл foreach для класу)");
            
            EngineCollection collection = new EngineCollection(myEngines);
            
            foreach (IEngine eng in collection)
            {
                Console.Write("З колекції: ");
                eng.ShowInfo();
            }
        }

        static void AnalyzeEngine(IEngine engine)
        {
            switch (engine)
            {
                case DieselEngine diesel:
                    Console.WriteLine(" -> Це дизельний двигун!");
                    diesel.Refuel();
                    diesel.CleanSootFilter();
                    break;
                
                case JetEngine jet:
                    Console.WriteLine(" -> Це реактивний двигун!");
                    jet.EnableAfterburner();
                    break;
                
                default:
                    Console.WriteLine(" -> Невідомий тип двигуна.");
                    break;
            }
        }
    }
}