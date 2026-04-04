using System;
using BugetApp.Models;
using BugetApp.Services;
using System.Collections.Generic;
using BugetApp.Persistence;

namespace BugetApp.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            Buget service = new Buget();
            FileService fileService = new FileService();
            var dateIncarcate = fileService.IncarcaTranzactii();
            service.SetAll(dateIncarcate);

            while (true)
            {
                Console.WriteLine("\n--- GESTIUNE BUGET ---");
                Console.WriteLine("1. Adaugare tranzactie");
                Console.WriteLine("2. Stergere tranzactie");
                Console.WriteLine("3. Afisare lista completa tranzactii");
                Console.WriteLine("4. Afisare Sold si Raport Financiar");
                Console.WriteLine("5. Cautare dupa Tip (Venit/Cheltuiala)"); 
                Console.WriteLine("6. Modificare date"); 
                Console.WriteLine("7. Salveaza datele in fisier"); 
                Console.WriteLine("0. Iesire");

                Console.Write("\nAlegeti optiunea: ");
                string opt = Console.ReadLine();

                if (opt == "1")
                {
                    Console.Write("Introduceti suma: ");
                    double suma = double.Parse(Console.ReadLine());
                    Console.Write("Tip (0-Venit, 1-Cheltuiala): ");
                    TipTranzactie tip = (TipTranzactie)int.Parse(Console.ReadLine());
                    Console.WriteLine("Optiuni (1-Urgent, 2-Personal, 4-Recurent, 8-Esential): ");
                    OptiuniTranzactie optiuni = (OptiuniTranzactie)int.Parse(Console.ReadLine());
                    Console.Write("Descriere: ");
                    string descriere = Console.ReadLine();

                    Tranzactie nouaTranzactie = new Tranzactie(suma, tip, optiuni, DateTime.Now, descriere);
                    service.AdaugaTranzactie(nouaTranzactie);
                    Console.WriteLine("Tranzactie adaugata cu succes!");
                }
                else if (opt == "2")
                {
                    var lista = service.GetAll();
                    for (int i = 0; i < lista.Count; i++)
                        Console.WriteLine($"{i}. {lista[i].Descriere} - {lista[i].Suma} ({lista[i].Tip})");

                    Console.Write("Index de sters: ");
                    int index = int.Parse(Console.ReadLine());
                    service.Sterge(index);
                    Console.WriteLine("Tranzactie stearsa!");
                }
                else if (opt == "3")
                {
                    var lista = service.GetAll();
                    if (lista.Count == 0) Console.WriteLine("Lista este goala.");
                    foreach (var t in lista) t.Afisare();
                }
                else if (opt == "4") 
                {
                    Console.WriteLine("\n--- RAPORT FINANCIAR COMPLET ---");
                    Console.WriteLine($"Total Venituri:   {service.TotalVenituri()} RON");
                    Console.WriteLine($"Total Cheltuieli: {service.TotalCheltuieli()} RON");
                    Console.WriteLine("-------------------------------");
                    Console.WriteLine($"SOLD CURENT:      {service.Sold()} RON");
                }
                else if (opt == "5") 
                {
                    Console.Write("Introduceti tipul cautat (0-Venit, 1-Cheltuiala): ");
                    TipTranzactie tipCautat = (TipTranzactie)int.Parse(Console.ReadLine());
                    service.CautaDupaTip(tipCautat);
                }
                else if (opt == "6") 
                {
                    var lista = service.GetAll();
                    for (int i = 0; i < lista.Count; i++)
                        Console.WriteLine($"{i}. {lista[i].Descriere} - {lista[i].Suma} RON");

                    Console.Write("\nIndex tranzactie de modificat: ");
                    int index = int.Parse(Console.ReadLine());

                    if (index >= 0 && index < lista.Count)
                    {
                        Console.Write("Noua Suma: ");
                        double suma = double.Parse(Console.ReadLine());
                        Console.Write("Nou Tip (0-Venit, 1-Cheltuiala): ");
                        TipTranzactie tip = (TipTranzactie)int.Parse(Console.ReadLine());
                        Console.Write("Noi Optiuni (1-Urgent, 2-Personal, 4-Recurent): ");
                        OptiuniTranzactie optiuni = (OptiuniTranzactie)int.Parse(Console.ReadLine());
                        Console.Write("Noua Descriere: ");
                        string descriere = Console.ReadLine();

                        service.Modifica(index, new Tranzactie(suma, tip, optiuni, DateTime.Now, descriere));
                        Console.WriteLine("Modificare realizata!");
                    }
                }
                else if (opt == "7") 
                {
                    fileService.SalveazaTranzactii(service.GetAll());
                    Console.WriteLine("Date salvate cu succes!");
                }
                else if (opt == "0")
                {
                    Console.WriteLine("La revedere!");
                    return;
                }
                else
                {
                    Console.WriteLine("Optiune invalida!");
                }
            }
        }
    }
}