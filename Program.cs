using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class Program
{
    static int maxHp = 100;
    static int hp = maxHp;
    static int dmg = 10;
    static int maxMana = 100;
    static int mana = maxMana;
    static int prachy = 10;
    static int xp = 0;

    static Random rnd = new Random();

    static List<Item> shopItems = new List<Item>
    {
        new Item("Lektvar života", 15, () => {
            hp = maxHp;
            PrintLine("Vyléčil jsi se na maximum!");
        }),
        new Item("Sekera", 20, () => {
            dmg += 4;
            PrintLine("Získal jsi sekeru! dmg +4");
        }),
        new Item("Magický svitek", 10, () => {
            mana += 30;
            if (mana > maxMana) mana = maxMana;
            PrintLine("Doplnil sis 30 many!");
        }),
        new Item("Brnění", 25, () => {
            maxHp += 10;
            PrintLine("Získal jsi brnění! maximální hp +10");
        }),
        new Item("Prsten Všehosi", 30, () => {
            maxHp += 10;
            maxMana += 10;
            dmg += 5;
            PrintLine("Získal jsi Prsten Všehosi! maximální hp a mana +10, dmg +5");
        })
    };

    static List<Enemy> enemies = new List<Enemy>
    {
        new Enemy("Vlk", 30, 5, 0),
        new Enemy("Goblin", 50, 8, 0),
        new Enemy("Skřet", 40, 6, 0),
        new Enemy("Had", 20, 4, 0),
        new Enemy("Kostlivec", 30, 5, 0)
    };

    static List<Enemy> strongEnemies = new List<Enemy>
    {
        new Enemy("Obří pavouk", 80, 15, 0),
        new Enemy("Kamenný golem", 100, 20, 0)
    };

    static Enemy boss = new Enemy("Temný rytíř", 150, 30, 0);

    public static void Main()
    {
        PrintLine("Vítej v hře!");
        while (true)
        {
            PrintLine("Vydej se na adventuru!");
            PrintLine("1. Jít do hradu");
            PrintLine("2. Ukončit hru");

            string volba = Console.ReadLine();
            if (volba == "1")
            {
                EnterCastle();
            }
            else if (volba == "2")
            {
                PrintLine("Díky za hraní!");
                break;
            }
            else
            {
                PrintLine("Neplatná volba.");
            }
        }
    }

    static void EnterCastle()
    {
        PrintLine("Vstupuješ do hradu.");
        for (int room = 1; room <= 20; room++)
        {
            PrintLine($"\nMístnost č. {room}");

            if (room == 10)
            {
                PrintLine("Tohle je zvláštní místnost… cítíš nebezpečí!");
                Spust4Event(forceFight: true); // forced strong enemy fight
                CheckDeath();
            }
            else if (room == 20)
            {
                PrintLine($"Narazil jsi na Bossa: {boss.EName}!");
                bool vyhral = Fight(boss, isBoss: true);
                CheckDeath();
                if (vyhral)
                {
                    PrintLine("Gratulujeme! Porazil jsi Temného rytíře!");
                    SpustDialog();﻿Console.WriteLine("Napis 3 cisla");
int cislo1 = int.Parse(Console.ReadLine());
int cislo2 = int.Parse(Console.ReadLine());
int cislo3 = int.Parse(Console.ReadLine());
Console.WriteLine(cislo1 + " " + cislo2 + " " + cislo3);
                    PrintLine("Zatím jsi prošel hrou až sem. Tady může pokračovat další příběh...");
                }
                else
                {
                    EndGame("Zemřel jsi u Temného rytíře. Konec hry.");
                }
                Environment.Exit(0);
            }
            else
            {
                RunEvent();
                CheckDeath();
            }
        }
    }

    static void RunEvent()
    {
        int eventNum = rnd.Next(1, 5);
        switch (eventNum)
        {
            case 1: Spust1Event(); break;
            case 2: Spust2Event(); break;
            case 3: Spust3Event(); break;
            case 4: Spust4Event(forceFight: false); break;
        }
    }

    static void showStats()
    {
        PrintLine($"HP: {hp}/{maxHp} | DMG: {dmg} | Mana: {mana}/{maxMana} | Peníze: {prachy}");
    }

    static void Spust1Event()
    {
        int typ = rnd.Next(1, 4);
        if (typ == 1)
        {
            prachy += 10;
            PrintLine("Našel jsi truhlu. Získal jsi 10 peněz.");
        }
        else if (typ == 2)
        {
            dmg += 2;
            PrintLine("Potkal jsi hodného poutníka. Naučil tě silnějšímu úderu (+2 dmg).");
        }
        else if (typ == 3)
        {
            maxHp += 5;
            hp = maxHp;
            PrintLine("Potkal jsi léčitele co ti zahojil rány (+5 maximální hp, plné životy).");
        }
        showStats();
        WaitForSpace();
    }

    static void Spust2Event()
    {
        PrintLine("Narazil jsi na obchodníka!");

        List<Item> vybrane = shopItems.OrderBy(x => rnd.Next()).Take(3).ToList();

        bool nakup = true;
        while (nakup)
        {
            PrintLine("\nNabídka v obchodě:");
            for (int i = 0; i < vybrane.Count; i++)
            {
                PrintLine($"{i + 1}. {vybrane[i].Name} - Cena: {vybrane[i].Price}");
            }
            PrintLine("4. Nekoupit nic");

            PrintLine($"Máš {prachy} peněz. Co si koupíš?");
            string volba = Console.ReadLine();

            int cislo;
            if (int.TryParse(volba, out cislo))
            {
                if (cislo >= 1 && cislo <= 3)
                {
                    Item vybrany = vybrane[cislo - 1];
                    if (prachy >= vybrany.Price)
                    {
                        prachy -= vybrany.Price;
                        vybrany.Effect();
                        showStats();
                        nakup = false;
                    }
                    else
                    {
                        PrintLine("Nemáš dost peněz!");
                    }
                }
                else if (cislo == 4)
                {
                    PrintLine("Odcházíš z obchodu bez nákupu.");
                    nakup = false;
                }
                else
                {
                    PrintLine("Neplatná volba. Zkus to znovu.");
                }
            }
            else
            {
                PrintLine("Neplatná volba. Zkus to znovu.");
            }
        }
        WaitForSpace();
    }

    static void Spust3Event()
    {
        Enemy nepritel = CloneEnemy(enemies[rnd.Next(enemies.Count)]);
        PrintLine($"Narazil jsi na nepřítele {nepritel.EName}!");
        Fight(nepritel, isBoss: false);
        CheckDeath();
        WaitForSpace();
    }

    static void Spust4Event(bool forceFight)
    {
        Enemy strongEnemy = CloneEnemy(strongEnemies[rnd.Next(strongEnemies.Count)]);
        PrintLine($"Narazil jsi na silného nepřítele {strongEnemy.EName}!");

        bool vyhral = Fight(strongEnemy, isBoss: forceFight);
        CheckDeath();

        if (vyhral)
        {
            Item item = shopItems[rnd.Next(shopItems.Count)];
            PrintLine($"Získal jsi za vítězství předmět: {item.Name}!");
            item.Effect();
        }
        WaitForSpace();
    }

    static bool Fight(Enemy enemy, bool isBoss)
    {
        PrintLine($"Bojuješ proti: {enemy.EName}");

        while (enemy.EHealth > 0 && hp > 0)
        {
            showStats();
            PrintLine($"Nepřítel: {enemy.EName} | HP: {enemy.EHealth}");
            PrintLine("\nCo chceš udělat?");
            PrintLine("1. Útočit");
            PrintLine("2. Magie (stojí 10 many, dvojnásobný útok)");

            if (!isBoss)
            {
                PrintLine("3. Utéct");
            }

            string volba = Console.ReadLine();

            if (volba == "1")
            {
                enemy.EHealth -= dmg;
                PrintLine($"Zasáhl jsi nepřítele za {dmg} poškození.");
            }
            else if (volba == "2")
            {
                if (mana >= 10)
                {
                    int magicDamage = dmg * 2;
                    enemy.EHealth -= magicDamage;
                    mana -= 10;
                    PrintLine($"Použil jsi magii a zasáhl nepřítele za {magicDamage} poškození.");
                }
                else
                {
                    PrintLine("Nemáš dost many!");
                    continue;
                }
            }
            else if (volba == "3" && !isBoss)
            {
                PrintLine("Utíkáš z boje!");
                return false;
            }
            else
            {
                PrintLine("Neplatná volba.");
                continue;
            }

            if (enemy.EHealth > 0)
            {
                hp -= enemy.EAttack;
                PrintLine($"Nepřítel tě zasáhl za {enemy.EAttack} poškození.");
                CheckDeath();
            }
            else
            {
                PrintLine($"Porazil jsi nepřítele {enemy.EName}!");
                prachy += 20;
                PrintLine("Získal jsi 20 peněz.");
                showStats();
                return true;
            }
        }

        return false;
    }

    static Enemy CloneEnemy(Enemy enemy)
    {
        return new Enemy(enemy.EName, enemy.EHealth, enemy.EAttack, 0);
    }

    static void CheckDeath()
    {
        if (hp <= 0)
        {
            EndGame("Zemřel jsi. Konec hry.");
        }
    }

    static void EndGame(string message)
    {
        PrintLine(message);
        Environment.Exit(0);
    }

    static void PrintLine(string text)
    {
        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(30);
        }
        Console.WriteLine();
        Thread.Sleep(200);
    }

    static void WaitForSpace()
    {
        PrintLine("\nStiskni mezerník pro pokračování...");
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);
        } while (key.Key != ConsoleKey.Spacebar);
    }

    static void SpustDialog()
    {
        PrintLine("\nPotkáváš v tajné místnosti postavu jménem Fandys.");
        PrintLine("Fandys: „daš si pivko?“");
        PrintLine("Ja: jop");
        hp = maxHp;
        mana = maxMana;
        PrintLine("Plně sis doplnil manu a životy");
        PrintLine("Fandys: „Chceš zajit se mnou do bordelu za prsatejma elfkama? “");

        PrintLine("1. Ne, Mám Svojí Adventuru");
        PrintLine("2. Klidně ig");

        string volba = Console.ReadLine();

        if (volba == "1")
        {
            EndGame("Lowk nám došel čas, představ si bossfight s drakem, víš že by jsme ho dokázaly udělat :) Tady máš kompromis: ( ͜. ㅅ ͜. )");
        }
        else if (volba == "2")
        {
            EndGame("S Fandysem jsi šel do bordelu a zbytek večera si už nepamatuješ");
        }
        else
        {
            PrintLine("Fandys nerozumí tvé odpovědi.");
            SpustDialog(); // loop znovu na dialog
        }
    }
}

public class Enemy
{
    public string EName { get; set; }
    public int EHealth { get; set; }
    public int EAttack { get; set; }

    public Enemy(string Ename, int Ehealth, int Eattack, int Edefense)
    {
        EName = Ename;
        EHealth = Ehealth;
        EAttack = Eattack;
    }
}

public class Item
{
    public string Name { get; set; }
    public int Price { get; set; }
    public Action Effect { get; set; }

    public Item(string name, int price, Action effect)
    {
        Name = name;
        Price = price;
        Effect = effect;
    }
}

