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
                Console.WriteLine("\n- GESTIUNE BUGET -");
                Console.WriteLine("\n1. Adaugare tranzactie");
                Console.WriteLine("2. Stergere tranzactie");
                Console.WriteLine("3. Afisare lista tranzactii");
                Console.WriteLine("4. Calcul total venituri");
                Console.WriteLine("5. Calcul total cheltuieli");
                Console.WriteLine("6. Calcul sold");
                Console.WriteLine("7. Afisare raport financiar");
                Console.WriteLine("8. Modificare date");
                Console.WriteLine("9. Salveaza datele in fisier text.");
                Console.WriteLine("0. Iesire");

                Console.Write("\nAlegeti optiunea: ");
                string opt = Console.ReadLine();


                if (opt == "1")
                {
                    Console.Write(" Introduceti suma: ");
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
                    {
                        Console.WriteLine($"{i}. {lista[i].Descriere} - {lista[i].Suma} ({lista[i].Tip})");
                    }

                    Console.Write("Index de sters: ");
                    int index = int.Parse(Console.ReadLine());

                    service.Sterge(index);


                }
                else if (opt == "3")
                {
                    var lista = service.GetAll();
                    if (lista.Count == 0) Console.WriteLine("Lista este goală.");
                    foreach (var t in lista)
                    {
                        t.Afisare();
                    }

                }
                else if (opt == "4")
                {

                    Console.WriteLine($"Total venituri: {service.TotalVenituri()}");

                }
                else if (opt == "5")
                {

                    Console.WriteLine($"Total cheltuieli: {service.TotalCheltuieli()}");

                }
                else if (opt == "6")
                {

                    Console.WriteLine($"Sold: {service.Sold()}");


                }
                else if (opt == "7")
                {

                    Console.WriteLine("\n--- RAPORT ---");
                    Console.WriteLine($"Venituri: {service.TotalVenituri()}");
                    Console.WriteLine($"Cheltuieli: {service.TotalCheltuieli()}");
                    Console.WriteLine($"Sold: {service.Sold()}");

                }

                else if (opt == "8")
                {

                    var lista = service.GetAll();
                    for (int i = 0; i < lista.Count; i++)
                    {
                        Console.WriteLine($"{i}. {lista[i].Descriere} - {lista[i].Suma} RON");
                    }

                    Console.Write("\nIntroduceti indexul tranzactiei de modificat: ");
                    int index = int.Parse(Console.ReadLine());

                    if (index >= 0 && index < lista.Count)
                    {
                        Console.WriteLine("--- Introduceti noile date ---");

                        Console.Write("Noua Suma: ");
                        double suma = double.Parse(Console.ReadLine());

                        Console.Write("Nou Tip (0-Venit, 1-Cheltuiala): ");
                        TipTranzactie tip = (TipTranzactie)int.Parse(Console.ReadLine());

                        Console.Write("Noi Optiuni (1-Urgent, 2-Personal, 4-Recurent): ");
                        OptiuniTranzactie optiuni = (OptiuniTranzactie)int.Parse(Console.ReadLine());

                        Console.Write("Noua Descriere: ");
                        string descriere = Console.ReadLine();


                        Tranzactie tranzactieEditata = new Tranzactie(suma, tip, optiuni, DateTime.Now, descriere);


                        service.Modifica(index, tranzactieEditata);


                        fileService.SalveazaTranzactii(service.GetAll());

                        Console.WriteLine("Tranzactia a fost modificatt si salvata!");
                    }



                }
                else if (opt == "9")
                {
                    fileService.SalveazaTranzactii(service.GetAll());
                    Console.WriteLine("Datele au fost salvate in fisier!");
                }
                else if (opt == "0")
                {
                    Console.WriteLine("La revedere!");
                    return;
                }
                else
                {
                    Console.WriteLine("Index invalid!");
                }

            }
        }
    }
}