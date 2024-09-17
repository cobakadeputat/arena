using System;
using System.Collections.Generic;

namespace аренабой
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Arena arena = new Arena();

            arena.Work();
        }
    }

    class Arena
    {
        private List<Fighter> _fighters = new List<Fighter>();

        public Arena()
        {
            _fighters.Add(new Spy());
            _fighters.Add(new Barbarian());
            _fighters.Add(new Wizard());
            _fighters.Add(new Tank());
        }

        public void Work()
        {
            Fighter firstFighter = null;
            Fighter secondFighter = null;

            Console.WriteLine("Это Арена где сражаются два героя не на жизнь а на смерть");

            UserUtils.SetTextColor(ConsoleColor.Green);
            ShowAllFighters();
            UserUtils.SetTextColor(ConsoleColor.White);

            Console.Write("\nДля начала боя вам нужно выбрать двух героев.\n");

            PickFighters(ref firstFighter, ref secondFighter);

            UserUtils.SetTextColor(ConsoleColor.Red);
            Console.WriteLine("\n Бойцы выбраны, можно начинать");
            UserUtils.SetTextColor(ConsoleColor.White);

            Console.WriteLine("\nДля начала боя нажмите любую клавишу...");
            Console.ReadKey();
            Console.Clear();

            Fight(firstFighter, secondFighter);

            Summarize(firstFighter, secondFighter);

            Console.ReadKey();
        }

        private void PickFighters(ref Fighter firstFighter, ref Fighter secondFighter)
        {
            Console.Write("\nДля выбора первого бойца введите его ID: ");
            PickFighter(out firstFighter);
            firstFighter.ShowInfo();
            _fighters.Remove(firstFighter);

            Console.Write("\nДля выбора второго Бойца введите его ID: ");
            PickFighter(out secondFighter);
            secondFighter.ShowInfo();
            _fighters.Remove(secondFighter);
        }

        private void Fight(Fighter firstFighter, Fighter secondFighter)
        {
            while (firstFighter.CurrentHealth > 0 && secondFighter.CurrentHealth > 0)
            {
                Attack(firstFighter, secondFighter);
                Attack(secondFighter, firstFighter);

                Console.WriteLine();

                firstFighter.ShowCurrentHealth();
                secondFighter.ShowCurrentHealth();

                Console.WriteLine();
                Console.ReadKey();
            }
        }

        private void Summarize(Fighter firstFighter, Fighter secondFighter)
        {
            UserUtils.SetTextColor(ConsoleColor.Green);

            if (firstFighter.CurrentHealth <= 0 && secondFighter.CurrentHealth <= 0)
                Console.WriteLine("ничья");
            else if (firstFighter.CurrentHealth <= 0)
                Console.WriteLine($"{secondFighter.Name} выигрывает бой ");
            else if (secondFighter.CurrentHealth <= 0)
                Console.WriteLine($"{firstFighter.Name} выигрывает бой ");
        }

        private void Attack(Fighter firstFighter, Fighter secondFighter)
        {
            if (firstFighter.TryUseAbility())
            {
                secondFighter.TakeDamage(firstFighter.AbilityDamage);
            }
            else
            {
                secondFighter.TakeDamage(firstFighter.DefaultDamage);

                Console.WriteLine($"{firstFighter.Name} бьет противника " +
                    $"и наносит ему {firstFighter.DefaultDamage} урона.");
            }
        }

        private void ShowAllFighters()
        {
            Console.WriteLine("Все бойцы");

            for (int i = 0; i < _fighters.Count; i++)
            {
                int index = i + 1;

                Console.Write($"{index}. ");
                _fighters[i].ShowStats();
            }
        }

        private int ReadNumber()
        {
            int number;

            while (int.TryParse(Console.ReadLine(), out number) == false)
            {
                UserUtils.SetTextColor(ConsoleColor.Red);
                Console.WriteLine("Ошибка: введено не число. Попробуйте еще раз.");
                UserUtils.SetTextColor(ConsoleColor.White);
            }

            return number;
        }

        private bool TryGetFighter(out Fighter fighter, int id)
        {
            for (int i = 0; i < _fighters.Count; i++)
            {
                if (_fighters[i].Id == id)
                {
                    fighter = _fighters[i];
                    return true;
                }
            }

            fighter = null;
            return false;
        }

        private void PickFighter(out Fighter fighter)
        {
            fighter = null;

            while (fighter == null)
            {
                int fighterId = ReadNumber();

                if (TryGetFighter(out fighter, fighterId))
                {
                    UserUtils.SetTextColor(ConsoleColor.Green);
                    Console.WriteLine($"\nБоец выбран и это - {fighter.Name}.\n");
                    UserUtils.SetTextColor(ConsoleColor.White);
                }
                else
                {
                    UserUtils.SetTextColor(ConsoleColor.Red);
                    Console.WriteLine("Бойца с этим ID не найдено!");
                    UserUtils.SetTextColor(ConsoleColor.White);
                }
            }
        }
    }

    abstract class Fighter
    {
        private static int s_counter = 0;

        public Fighter(string name, int health, int damage)
        {
            Name = name;
            MaxHealth = health;
            CurrentHealth = MaxHealth;
            DefaultDamage = damage;
            Id = ++s_counter;
        }

        public string Name { get; protected set; }
        public int MaxHealth { get; protected set; }
        public int CurrentHealth { get; protected set; }
        public int DefaultDamage { get; protected set; }
        public int AbilityDamage { get; protected set; }

        public bool IsAlive => MaxHealth > 0;

        public int Id { get; protected set; }

        public abstract void ShowInfo();

        public abstract void Atack(Fighter fighter);

        public virtual void TakeDamage(int damage)
        {
            if (IsAlive)
                CurrentHealth -= damage;
        }

        public void ShowStats()
        {
            Console.WriteLine($"{Name}, здоровье - {MaxHealth}, урон {DefaultDamage}, Id-{Id}");
        }

        public abstract bool TryUseAbility();

        public void ShowCurrentHealth()
        {
            Console.WriteLine($"{Name}, осталось {CurrentHealth} здоровья.");
        }
    }


    class Spy : Fighter
    {
        private int _agilityNumber;
        private int _counter;
        public Spy() : base("Шпион", 120, 25)
        {
            _agilityNumber = 3;
            _counter = 0;
        }
        public override void Atack(Fighter fighter)
        {
            if (TryUseAbility())
            {
                fighter.TakeDamage(DefaultDamage * 2);
            }
            else
            {
                fighter.TakeDamage(DefaultDamage);
                ShowStats();
            }
        }
        public override void ShowInfo()
        {
            Console.WriteLine("Быстрые руки позволяют нанести атаку дважды {_agilityNumber}");
        }
        //public override void showstats()
        //{
        //    console.writeline($"{name}, здоровье - {maxhealth}, урон - {defaultdamage}, id - {id}.");
        //}

        public override bool TryUseAbility()
        {
            _counter++;
            int multiplier = 2;

            if (_counter % _agilityNumber == 0)
            {
                AbilityDamage = DefaultDamage * multiplier;

                UserUtils.SetTextColor(ConsoleColor.Green);
                Console.WriteLine($"{Name} дважды наносит урон врагу! " +
                    $"Итоговый урон - {AbilityDamage}.\n");
                UserUtils.SetTextColor(ConsoleColor.White);

                return true;
            }

            return false;
        }

    }

    class Barbarian : Fighter
    {
        private int _healAmount;
        private int _maxAnger;
        private int _currentAnger;
        private int _angerStep;

        public Barbarian() : base("Варвар ", 100, 30)
        {
            int minRandomValue = 30;
            int maxRandomValue = 50;

            _healAmount = UserUtils.GenerateRandomNumber(minRandomValue, maxRandomValue);
            _maxAnger = 60;
            _currentAnger = 0;
            _angerStep = 20;
        }

        public override void Atack(Fighter fighter)
        {
            if (TryUseAbility())
            {
                fighter.TakeDamage(DefaultDamage + _angerStep);
            }
            else
            {
                fighter.TakeDamage(DefaultDamage);
                ShowStats();
            }
        }
        public override void ShowInfo()
        {
            Console.WriteLine("Копит ярость а затем лечится");
        }
        //public override void ShowStats()
        //{
        //    Console.WriteLine($"{Name}, ХП - {MaxHealth}, ДМГ - {DefaultDamage}, ID - {Id}.");
        //}

        public override bool TryUseAbility()
        {
            _currentAnger += _angerStep;
            int difference = MaxHealth - CurrentHealth;

            if (_currentAnger >= _maxAnger)
            {
                if (difference > _healAmount)
                {
                    CurrentHealth += _healAmount;

                    UserUtils.SetTextColor(ConsoleColor.Green);
                    Console.WriteLine($"{Name} накопил достаточно ярости, поэтому он применяет свою способность " +
                        $"и восстанавливает {_healAmount} здоровья.\n");
                    UserUtils.SetTextColor(ConsoleColor.White);
                }
                else
                {
                    CurrentHealth += difference;

                    UserUtils.SetTextColor(ConsoleColor.Green);
                    Console.WriteLine($"\n{Name} накопил достаточно ярости, поэтому он применяет свою способность " +
                        $"и восстанавливает {difference} здоровья.\n");
                    UserUtils.SetTextColor(ConsoleColor.White);
                }

                AbilityDamage = DefaultDamage;
                _currentAnger = 0;

                return true;
            }

            return false;
        }
    }
    class Wizard : Fighter
    {
        private int _maxMana;
        private int _currentMana;
        private int _fireBallDamage;
        private int _fireBallManaCost;

        public Wizard() : base("Волшебник", 130, 25)
        {
            int minManaCostRandomValue = 30;
            int maxManaCostRandomValue = 60;
            int minDamageRandomValue = 15;
            int maxDamageRandomValue = 25;

            _maxMana = 100;
            _currentMana = _maxMana;
            _fireBallManaCost = UserUtils.GenerateRandomNumber(minManaCostRandomValue, maxManaCostRandomValue);
            _fireBallDamage = UserUtils.GenerateRandomNumber(minDamageRandomValue, maxDamageRandomValue);
        }
        public override void Atack(Fighter fighter)
        {
            if (TryUseAbility())
            {
                fighter.TakeDamage(DefaultDamage + _fireBallDamage);
            }
            else
            {
                fighter.TakeDamage(DefaultDamage);
                ShowStats();
            }
        }


        public override void ShowInfo()
        {
            Console.WriteLine("Волшебник способный накладывать огненный шар" +
                $"{_fireBallDamage} урона в дополнение к основному. Стоит {_fireBallManaCost} маны.");
        }

        //public override void ShowStats()
        //{
        //    Console.WriteLine($"{Name}, ХП - {MaxHealth}, ДМГ - {DefaultDamage}, мана - {_maxMana}, ID - {Id}.");
        //}



        public override bool TryUseAbility()
        {
            if (_currentMana >= _fireBallManaCost)
            {
                _currentMana -= _fireBallManaCost;
                AbilityDamage = DefaultDamage + _fireBallDamage;

                UserUtils.SetTextColor(ConsoleColor.Green);
                Console.WriteLine($"{Name} концентрируется и выпускает во врага огненный шар, " +
                    $"который наносит ему {AbilityDamage} урона!\n");
                UserUtils.SetTextColor(ConsoleColor.White);

                return true;
            }

            return false;
        }
    }


    class Tank : Fighter
    {
        private int _block;

        public Tank() : base("Танк", 200, 15)
        {
            _block = 50;
        }

        public override void Atack(Fighter fighter)
        {
            if (TryUseAbility())
            {
                fighter.TakeDamage(DefaultDamage + _block);
            }
            else
            {
                fighter.TakeDamage(DefaultDamage);
                ShowStats();
            }
        }

        public override void ShowInfo()
        {
            Console.WriteLine("Танк" + "с крепким телом" +
                $"способноть: с вероятность{_block} % поглощает дамаг в 2 раза");
        }

        //public override void ShowStats()
        //{

        //    Console.WriteLine($"{Name}, ХП - {MaxHealth}, ДМГ - {DefaultDamage}, ID - {Id}.");
        //}
        public override void TakeDamage(int damage)
        {
            if (TryUseAbility())
            {

                UserUtils.SetTextColor(ConsoleColor.Green);
                Console.WriteLine($"{Name} блокирует  атаки противника " +
                    $"поглощает дамаг!\n");
                UserUtils.SetTextColor(ConsoleColor.White);
            }
            else
                CurrentHealth -= damage;

        }

        public override bool TryUseAbility()
        {
            int minRandomValue = 0;
            int maxRandomValue = 2;
            if (UserUtils.GenerateRandomNumber(minRandomValue, maxRandomValue) == 0)
            {

                AbilityDamage = DefaultDamage;

                return true;
            }
            return false;
        }
    }

    static class UserUtils
    {
        private static Random s_random = new Random();

        public static int GenerateRandomNumber(int minRandomValue, int maxRandomValue)
        {
            return s_random.Next(minRandomValue, maxRandomValue);
        }

        public static void SetTextColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }
    }



}