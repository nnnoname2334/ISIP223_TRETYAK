using System;
using System.Collections.Generic;
using System.Threading;

var rand = R.rand;

List<Enemy> enemyList = new List<Enemy>();
List<Enemy> bossList = new List<Enemy>();

// Базовые враги
enemyList.Add(new Enemy("Гоблин", 10, 3, 4, crit_chance: 0.2));
enemyList.Add(new Enemy("Скелет", 20, 2, 2, ignore_defence: true));
enemyList.Add(new Enemy("Маг", 15, 3, 5, freeze_chance: 0.15));

// Боссы (характеристики — модификаторы от базовых, но здесь указаны напрямую согласно ТЗ)
bossList.Add(new Enemy("ВВГ", 10 * 2.0, 3 * 1.2, 4 * 1.5, crit_chance: 0.2 + 0.1)); // гоблин-босс
bossList.Add(new Enemy("Ковальский", 20 * 2.5, 2 * 1.4, 3 * 1.3, ignore_defence: true)); // скелет-босс
bossList.Add(new Enemy("Архимаг C++", 15 * 1.8, 3 * 1.1, 5 * 1.6, freeze_chance: 0.15 + 0.10)); // маг-босс
bossList.Add(new Enemy("Пестов С--", 20 * 1.3, 2 * 0.6, 3 * 1.8, freeze_chance: 0.15 + 0.15, ignore_defence: true)); // смешанный/скелет с шансом заморозки

Player player = new Player(35, 3, 6, 0.2);

void End(string difficult)
{
    Thread.Sleep(800);
    Console.Clear();
    if (player.hp <= 0)
    {
        Console.WriteLine("Иди поиграй лучше в свой аркадный Dark Souls");
        return;
    }
    if (difficult != "2" && difficult != "1")
    {
        Console.WriteLine("Как ты победил? Я специально сделал игру так, что это невозможно...");
    }
    else
    {
        Console.WriteLine("А теперь на нормальной сложности");
    }
}

void GetTools()
{
    Thread.Sleep(600);
    int a = rand.Next(3);
    Console.Clear();
    switch (a)
    {
        case 0:
            Console.WriteLine("Вам выпало лечебное зелье!");
            player.Regeneration();
            Thread.Sleep(1000);
            break;
        case 1:
            Console.WriteLine("Вам выпал новый меч, желаете его взять? (1-да, 0-нет)");
            double n_damage = rand.NextDouble() * 5 + rand.NextDouble() * 4 + rand.NextDouble() * 3 + rand.NextDouble() * 3;
            player.StatsA(n_damage);
            string s = Console.ReadLine();
            if (s == "1")
            {
                player.damage = n_damage;
                Console.WriteLine("Меч экипирован.");
            }
            else
            {
                Console.WriteLine("Меч выброшен.");
            }
            Thread.Sleep(800);
            break;
        case 2:
            Console.WriteLine("Вам выпала новая броня, хотите ее экипировать? (1-да, 0-нет)");
            double n_def = rand.NextDouble() * 5 + rand.NextDouble() * 2 + rand.NextDouble() + rand.NextDouble();
            player.StatsD(n_def);
            string s_ = Console.ReadLine();
            if (s_ == "1")
            {
                player.defence = n_def;
                Console.WriteLine("Броня экипирована.");
            }
            else
            {
                Console.WriteLine("Броня выброшена.");
            }
            Thread.Sleep(800);
            break;
    }
}

void Battle(Enemy enemy)
{
    Console.Clear();
    Console.WriteLine($"Ваш противник: {enemy.name}");
    enemy.info();
    player.info();
    Thread.Sleep(800);

    bool battleOver = false;

    while (!battleOver)
    {
        // Если игрок заморожен — пропускает ход, враг атакует
        if (player.is_freeze)
        {
            Console.WriteLine("Вы заморожены и пропускаете ход!");
            player.is_freeze = false; // заморозка действует только один ход (по ТЗ пропуск следующего хода)
            double dmgFromEnemy = enemy.Attack();
            player.GetDamageInAttack(dmgFromEnemy);
            Thread.Sleep(800);
        }
        else
        {
            Console.WriteLine("Выберите действие: 1 - Атака, 2 - Защита");
            string s = Console.ReadLine();
            switch (s)
            {
                case "1": // Атака
                    enemy.GetDamage(player.damage);
                    Thread.Sleep(500);
                    if (enemy.hp <= 0)
                    {
                        Console.WriteLine("Враг повержен!");
                        battleOver = true;
                        break;
                    }

                    // если враг выжил — он атакует
                    double dmg = enemy.Attack();
                    player.GetDamageInAttack(dmg);

                    // проверка на заморозку после атаки (если враг применяет эффект)
                    if (enemy.freeze())
                    {
                        Console.WriteLine("Враг наложил заморозку! Вы пропустите следующий ход.");
                        player.is_freeze = true;
                    }
                    break;

                case "2": // Защита
                    Console.WriteLine("Вы приняли защитную стойку.");
                    Thread.Sleep(300);
                    bool contrAttack = player.GetDamageInDef(enemy.Attack(), enemy.ignore_defence);
                    if (contrAttack)
                    {
                        enemy.GetDamage(player.damage);
                        if (enemy.hp <= 0)
                        {
                            Console.WriteLine("Враг повержен контратакой!");
                            battleOver = true;
                            break;
                        }
                    }

                    // Если после блока/контры враг жив — возможна заморозка от врага (некоторые враги накладывают)
                    if (!battleOver && enemy.freeze())
                    {
                        Console.WriteLine("Враг наложил заморозку! Вы пропустите следующий ход.");
                        player.is_freeze = true;
                    }
                    break;

                default:
                    Console.WriteLine("Неверный ввод, пропускаете ход.");
                    Thread.Sleep(500);
                    break;
            }
        }

        if (player.hp <= 0)
        {
            Console.WriteLine("Враг убил вас...");
            battleOver = true;
        }

        // Информирование
        Thread.Sleep(500);
        Console.Clear();
        player.info();
        enemy.info();
        Thread.Sleep(500);
    }

    // Восстановим здоровье врага (чтобы при повторном взятии того же объекта он был "свежим")
    enemy.Regeneration();
}

void MainLoop()
{
    Console.WriteLine("'''Герой КИПФИН'''");
    Console.WriteLine("'''Версия 0.3 (исправленная)'''");
    Console.WriteLine("Приготовьтесь к игре");
    Console.WriteLine("Выберите уровень сложности 1-слизняк 2-младенец 3-нормальная");
    string difficult = Console.ReadLine();
    switch (difficult)
    {
        case "1":
            player.max_hp = 100;
            player.defence += 2;
            player.damage += 2;
            player.Regeneration();
            Console.Clear();
            break;
        case "2":
            player.max_hp = 60;
            player.Regeneration();
            Console.Clear();
            break;
        default:
            // нормальная — оставляем как есть
            break;
    }

    int turn = 0;
    while (player.hp > 0)
    {
        turn++;
        Console.WriteLine($"\n--- Ход {turn} ---");
        Thread.Sleep(400);

        // Каждые 10 ходов — босс
        if (turn % 10 == 0)
        {
            Console.WriteLine("ОНО ПРИБЛИЖАЕТСЯ... Это босс!");
            Thread.Sleep(800);
            int indBoss = rand.Next(bossList.Count);
            Enemy boss = bossList[indBoss];
            Battle(boss);
            if (player.hp <= 0) { End(difficult); return; }
            continue;
        }

        // 50 на 50: сундук или враг
        int eventType = rand.Next(2); // 0 - сундук, 1 - враг
        if (eventType == 0)
        {
            Console.WriteLine("Вам попался сундук!");
            Thread.Sleep(600);
            GetTools();
        }
        else
        {
            Console.WriteLine("Появился враг!");
            Thread.Sleep(600);
            int ind = rand.Next(enemyList.Count);
            Enemy enemy = enemyList[ind];
            Battle(enemy);
            if (player.hp <= 0) { End(difficult); return; }
        }
    }

    End(difficult);
}

MainLoop();

static class R
{
    public static Random rand = new Random();
}

class Player
{
    public double hp;
    public double max_hp;
    public double defence;
    public double damage;
    public bool is_freeze = false;
    public double contr_chance;

    public Player(double max_hp, double defence, double damage, double contr_chance)
    {
        this.max_hp = max_hp;
        hp = max_hp;
        this.defence = defence;
        this.damage = damage;
        this.contr_chance = contr_chance;
    }

    public void Regeneration()
    {
        hp = max_hp;
        Console.WriteLine("Здоровье полностью восстановлено.");
        Console.WriteLine($"Здоровье персонажа: {Math.Round(hp, 2)}");
    }

    // Возвращает true — если сработал контрудар
    public bool GetDamageInDef(double incomingDmg, bool ignoreDefence)
    {
        // 40% шанс полностью уклониться
        double roll = R.rand.NextDouble();
        if (roll <= 0.40)
        {
            Console.WriteLine("Игрок уклонился от удара!");
            Thread.Sleep(400);
            return (contr_chance >= R.rand.NextDouble());
        }
        else
        {
            double defValue = 0;
            if (!ignoreDefence)
            {
                // блок уменьшает получаемый урон на величину равную 70% - 100% от характеристики защиты
                defValue = defence * (0.7 + R.rand.NextDouble() * 0.3);
            }
            // итоговый урон
            double result = incomingDmg - defValue;
            if (result > 0)
            {
                hp -= Math.Round(result, 3);
                Console.WriteLine($"Персонаж получил {Math.Round(result, 2)} урона (после блока).");
            }
            else
            {
                Console.WriteLine("Блок полностью поглотил урон!");
            }
            Thread.Sleep(400);
            return (contr_chance >= R.rand.NextDouble());
        }
    }

    // Прямой урон от атаки врага (когда защита не применялась)
    public void GetDamageInAttack(double dmg)
    {
        hp -= Math.Round(dmg, 3);
        Console.WriteLine($"Персонаж получил {Math.Round(dmg, 2)} урона.");
        Thread.Sleep(300);
    }

    public void StatsA(double n_dmg)
    {
        Console.WriteLine($"Ваша атака: {Math.Round(damage, 3)}\nАтака нового меча: {Math.Round(n_dmg, 3)}");
    }

    public void StatsD(double n_def)
    {
        Console.WriteLine($"Ваша защита: {Math.Round(defence, 3)}\nЗащита новой брони: {Math.Round(n_def, 3)}");
    }

    public void info()
    {
        Console.WriteLine($"Здоровье персонажа: {Math.Round(hp, 2)} / {Math.Round(max_hp, 2)}");
    }
}

class Enemy : Player
{
    public string name;
    public double crit_chance;
    public double freeze_chance;
    public bool ignore_defence;

    public Enemy(
        string name,
        double max_hp,
        double defence,
        double damage,
        double crit_chance = 0,
        double freeze_chance = 0,
        bool ignore_defence = false)
        : base(max_hp, defence, damage, 0)
    {
        this.name = name;
        this.crit_chance = crit_chance;
        this.freeze_chance = freeze_chance;
        this.ignore_defence = ignore_defence;
    }

    public new void Regeneration()
    {
        hp = max_hp;
    }

    // Атака врага: учитывает шанс крита
    public double Attack()
    {
        if (R.rand.NextDouble() <= crit_chance)
        {
            Console.WriteLine("Враг наносит критический удар!");
            return damage * 2;
        }
        return damage;
    }

    // Получение урона от игрока (учитываем броню врага)
    public void GetDamage(double dmg)
    {
        double result = dmg - defence;
        if (result > 0)
        {
            hp -= Math.Round(result, 3);
            Console.WriteLine($"Враг получил {Math.Round(result, 2)} урона.");
        }
        else
        {
            Console.WriteLine("Урон не прошёл сквозь броню врага.");
        }
        Thread.Sleep(300);
    }

    public new void info()
    {
        Console.WriteLine($"[{name}] Здоровье: {Math.Round(hp, 2)} / {Math.Round(max_hp, 2)}");
    }

    // Шанс заморозить игрока
    public bool freeze()
    {
        return (R.rand.NextDouble() <= freeze_chance);
    }
}
