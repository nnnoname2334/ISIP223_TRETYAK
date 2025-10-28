using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Xml.Linq;
var rand = new Random();
List<Enemy> enemyList = new List<Enemy>();
List<Enemy> bossList = new List<Enemy>();
enemyList.Add(new Enemy("Гоблин", 10, 3, 4, crit_chance: 0.2));
enemyList.Add(new Enemy("Скелет", 20, 2, 2, ignore_defence: true));
enemyList.Add(new Enemy("Маг", 15, 3, 5, freeze_chance: 0.15));
bossList.Add(new Enemy("ВВГ", 10 * 2, 3 * 1.2, 4 * 1.5, crit_chance: 0.2 + 0.1));
bossList.Add(new Enemy("Ковальский", 20 * 2.5, 2 * 1.4, 3 * 1.3, ignore_defence: true));
bossList.Add(new Enemy("Архимаг C++", 15 * 1.8, 3 * 1.1, 5 * 1.6, freeze_chance: 0.15));
bossList.Add(new Enemy("Пестов С--", 20 * 1.3, 2 * 0.6, 3 * 1.8, freeze_chance: 0.15, crit_chance: 0.2));
Player player = new Player(35, 3, 6, 0.2);




void end(string difficult)
{
    Thread.Sleep(1500);
    Console.Clear();
    if (player.hp <= 0) { Console.WriteLine("Иди поиграй лучше в свой аркадный Dark Souls"); return; }
    if (difficult != "2" && difficult != "1")
    {
        Console.WriteLine("Как ты победил? Я специально сделал игру так, что это невозможно...");
    }
    else { Console.WriteLine("А теперь на нормальной сложности"); }
}
void get_tools()
{
    Thread.Sleep(1000);
    Random r = new Random();
    int a = r.Next(3);
    Console.Clear();
    switch (a)
    {
        case 0:
            Console.WriteLine("Вам выпало зелье лечения от поноса");
            player.Regeneration();
            Thread.Sleep(1500);
            break;
        case 1:
            Console.WriteLine("Вам выпал новый меч, желаете его взять? (1-да, 0-нет)");
            double n_damage = r.NextDouble() * 5 + r.NextDouble() * 4 + r.NextDouble() * 3 + r.NextDouble() * 3;
            player.Stats_a(n_damage);
            string s = Console.ReadLine();
            switch (s)
            {
                case "1":
                    player.damage = n_damage;
                    break;
            }
            break;
        case 2:
            Console.WriteLine("Вам выпал новая броня, хотите ее экипировать? (1-да, 0-нет)");
            double n_def = r.NextDouble() * 5 + r.NextDouble() * 2 + r.NextDouble() + r.NextDouble();
            player.Stats_d(n_def);
            string s_ = Console.ReadLine();
            switch (s_)
            {
                case "1":
                    player.defence = n_def;
                    break;
            }
            break;
    }
}


void battle(Enemy enemy)
{
    Console.WriteLine($"Ваш противник {enemy.name}");
    while (true)
    {
        if (!player.is_freeze)
        {
            Console.WriteLine("Выберите дейтсвие\n1-Атака, 2-Защита");
            string s = Console.ReadLine();
            switch (s)
            {
                case "1":
                    enemy.Get_damage(player.damage);
                    Thread.Sleep(500);
                    if (enemy.hp <= 0)
                    {
                        Console.WriteLine("Этому бро надо было тренироваться, враг убит");
                        break;
                    }
                    player.Get_damage_in_attack(enemy.Attack());
                    if (enemy.freeze())
                    {
                        Console.WriteLine("Враг заморозил тебя");
                        player.is_freeze = true;
                    }
                    break;
                case "2":
                    Console.WriteLine("Игрок приготовился к удару");
                    Thread.Sleep(500);
                    bool contrattack = player.Get_damage_in_def(enemy.Attack(), enemy.ignore_defence);
                    if (contrattack)
                    {
                        Console.WriteLine("Персонаж успешно контратаковал");
                        enemy.Get_damage(player.damage);
                    }
                    if (enemy.hp <= 0)
                    {
                        Console.WriteLine("Этому бро надо было тренироваться, враг убит");
                        break;
                    }
                    break;
            }
        }
        if (player.hp <= 0)
        {
            Console.WriteLine("Враг убил тебя");
            break;
        }

        if (player.is_freeze)
        {
            player.is_freeze = false;
            player.Get_damage_in_attack(enemy.Attack());
            Console.WriteLine("Ожидайте хода");
        }
        Thread.Sleep(2000);
        Console.Clear();
        player.info();
        enemy.info();
        Console.WriteLine();
    }
    enemy.Regeneration();
}
void main()
{
    Console.WriteLine("'''Герой КИПФИН'''");
    Console.WriteLine("'''Версия 0.2'''");
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
    }
    for (int i = 0; i < 9; i++)
    {
        int deistv = rand.Next(2);
        switch (deistv)
        {
            case 0:
                get_tools();
                break;
            case 1:
                var r = new Random();
                Console.WriteLine("Сейчас махыч будет");
                Thread.Sleep(1000);
                int ind = r.Next(enemyList.Count);
                Enemy enemy = enemyList[ind];
                battle(enemy);
                if (player.hp <= 0) { end(difficult); return; }
                break;
        }
    }
    Console.WriteLine("\n\n\nОНО ПРИБЛЕЖАЕТЬСЯ");
    Thread.Sleep(1000);
    Console.WriteLine("Ты чувствуешь как холод пробигает по спине");
    Thread.Sleep(1000);
    var r_ = new Random();
    int ind_ = r_.Next(bossList.Count);
    Enemy enemy_ = bossList[ind_];
    battle(enemy_);
    end(difficult);
}
main();


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
        hp = max_hp;
        this.max_hp = max_hp;
        this.defence = defence;
        this.damage = damage;
        this.contr_chance = contr_chance;
    }
    public void Regeneration()
    {
        hp = max_hp;
        Console.WriteLine("Здоровье восстановлено");
        Console.WriteLine($"Здоровье персонажа {Double.Round(hp, 2)}");
    }
    public bool Get_damage_in_def(double dmg, bool ignore)
    {
        Random rand = new Random();
        if (rand.NextDouble() > 0.40)
        {
            double def;
            if (!ignore)
            {
                def = defence * (0.7 + rand.NextDouble() * 0.3);
            }
            else
            {
                def = 0;
            }
            var result = dmg - def;
            if (result > 0)
            {
                hp -= Double.Round(result, 3);
                Console.WriteLine($"Персонаж получил {Double.Round(result, 2)} урона");
            }
            else
            {
                Console.WriteLine("Эта броня просто имба");
            }
        }
        else
        {
            Console.WriteLine("Игрок уклонился");
        }
        Thread.Sleep(1000);
        return (contr_chance >= R.rand.NextDouble());
    }
    public void Get_damage_in_attack(double dmg)
    {
        hp -= dmg;
        Console.WriteLine($"Персонаж получил {Double.Round(dmg, 2)} урона");
    }
    public void Stats_a(double n_dmg)
    {
        Console.WriteLine($"Ваша атака {Double.Round(damage, 3)}\nАтака нового меча {Double.Round(n_dmg, 3)}");
    }
    public void Stats_d(double n_def)
    {
        Console.WriteLine($"Ваша защита {Double.Round(defence, 3)}\nЗащита новой брони {Double.Round(n_def, 3)}");
    }
    public void info()
    {
        Console.WriteLine($"Здоровье персонажа {Double.Round(hp, 2)}");
    }
}

class Enemy : Player
{
    public string name;
    public double crit_chance;
    public double freeze_chance;
    public bool ignore_defence;

    public Enemy(string name, double max_hp, double defence, double damage, double crit_chance = 0, double freeze_chance = 0, bool ignore_defence = false)
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

    public double Attack()
    {
        Random r = new Random();
        if (r.NextDouble() <= crit_chance)
        {
            Console.WriteLine("Враг замахнулся посильнее");
            return damage * 2;
        }
        return damage;
    }

    public void Get_damage(double dmg)
    {
        var result = dmg - defence;
        if (result > 0)
        {
            hp -= Double.Round(result, 3);
            Console.WriteLine($"Враг получил {Double.Round(result, 2)} урона");
        }
        else
        {
            Console.WriteLine("Врагу пофиг, он бронированный");
        }
        Thread.Sleep(1000);
    }
    public new void info()
    {
        Console.WriteLine($"Здоровье {name} = {Double.Round(hp, 2)}");
    }
    public bool freeze()
    {
        Random r = new Random();
        double ra = r.NextDouble();
        return (ra <= freeze_chance);
    }
}