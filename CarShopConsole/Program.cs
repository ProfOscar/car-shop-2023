using CarShopLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CarShopConsole
{
    class Program
    {
        static List<Veicolo> ParcoMezzi = new List<Veicolo>();

        static void Main(string[] args)
        {
            // CreaDatiDiProva();
            CaricaDati();
            char scelta = ' ';
            while (scelta.ToString().ToLower() != "q")
            {
                scelta = ScriviMenu();
                switch (scelta)
                {
                    case 'c':
                    case 'C':
                        CaricaDati();
                        break;
                    case 's':
                    case 'S':
                        SalvaDati();
                        break;
                    case '1':
                        ElencoVeicoli("\n*** Elenco Generale Veicoli ***");
                        break;
                    case '2':
                        ElencoVeicoli("\n*** Elenco AUTO ***", typeof(Auto));
                        break;
                    case '3':
                        ElencoVeicoli("\n*** Elenco MOTO ***", typeof(Moto));
                        break;
                    case 'h':
                    case 'H':
                        EsportaHtml();
                        break;
                    case 'w':
                    case 'W':
                        EsportaWord();
                        break;
                    default:
                        break;
                }
            }
        }

        private static void EsportaHtml()
        {
            int num = 0;
            do
            {
                Console.Write("\nInserisci il numero d'ordine del veicolo: ");
            } while (!int.TryParse(Console.ReadLine(), out num));
            if (num > 0 && num < ParcoMezzi.Count)
            {
                Console.Clear();
                Console.WriteLine("\n" + ParcoMezzi[num - 1] + "\n");
                // Produrre volantino in html
                Process.Start(AppDomain.CurrentDomain.BaseDirectory + "/html/index.html");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("\nMezzo non esistente!\n");
            }
        }

        private static void EsportaWord()
        {
            int num = 0;
            do
            {
                Console.Write("\nInserisci il numero d'ordine del veicolo: ");
            } while (!int.TryParse(Console.ReadLine(), out num));
            if (num > 0 && num < ParcoMezzi.Count)
            {
                Console.Clear();
                Console.WriteLine("\n" + ParcoMezzi[num - 1] + "\n");
                // Produrre volantino in word
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "/test.docx";
                OpenXmlTools.GeneraVolantinoDocx(filePath);
                Process.Start(filePath);
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("\nMezzo non esistente!\n");
            }
        }


        private static void SalvaDati()
        {
            if (JsonTools.SalvaDati(ParcoMezzi))
            {
                Console.WriteLine("\n*** SCRITTURA DATI OK ***");
                Thread.Sleep(2000);
                Console.Clear();
            }
        }

        private static void CaricaDati()
        {
            ParcoMezzi = JsonTools.CaricaDati();
            if (ParcoMezzi != null)
            {
                Console.WriteLine("\n*** CARICAMENTO DATI OK ***");
                Thread.Sleep(2000);
                Console.Clear();
            }
        }

        private static void ElencoVeicoli(string titolo, Type tipo = null)
        {
            Console.Clear();
            Console.WriteLine(titolo);
            int conta = 0;
            foreach (var item in ParcoMezzi)
            {
                if (tipo == null || tipo == item.GetType()) { 
                    conta++;
                    Console.WriteLine(conta.ToString() + " - " +  item.ToString(true));
                }
            }
            Console.WriteLine("\n");
        }

        private static char ScriviMenu()
        {
            Console.WriteLine("*** GESTIONE RIVENDITA VEICOLI USATI ***");
            Console.WriteLine("".PadLeft(30, '_'));
            Console.WriteLine("C - CARICA Dati");
            Console.WriteLine("S - SALVA Dati");
            Console.WriteLine("".PadLeft(30, '_'));
            Console.WriteLine("1 - Visualizza TUTTI i veicoli");
            Console.WriteLine("2 - Visualizza le AUTO");
            Console.WriteLine("3 - Visualizza le MOTO");
            Console.WriteLine("".PadLeft(30, '_'));
            Console.WriteLine("H - Esporta Volatino HTML");
            Console.WriteLine("W - Esporta Volatino DOCX");
            Console.WriteLine("".PadLeft(30, '_'));
            Console.WriteLine("\nQ - USCITA");
            return Console.ReadKey(true).KeyChar;
        }

        static void CreaDatiDiProva()
        {
            Veicolo v = new Auto("BMW", "Serie 3", EAlimentazione.Benzina, "Blu");
            ParcoMezzi.Add(v);
            v = new Auto("Mercedes", "CLA", EAlimentazione.Diesel, "Grigio", true, 5, 18);
            ParcoMezzi.Add(v);
            v = new Moto("Yamaha", "KZ5", EAlimentazione.Benzina, "Verde", 
                new StructDimensioni(270,87,68), "A34DE76PLYT90", 3500, 210,
                130, new DateTime(2021, 03, 15), 12750, "",
                ETipoMoto.Enduro, 4);
            ParcoMezzi.Add(v);
            v = new Moto("Ducati", "RossoFuoco", EAlimentazione.Benzina, "Rosso", ETipoMoto.Strada, 4);
            ParcoMezzi.Add(v);
            v = new Auto("Fiat", "500", EAlimentazione.Elettrica, "Bianco",
                new StructDimensioni(320, 160, 140), "TR5654ER55YJT5", 37500, 140,
                90, new DateTime(2021, 10, 13), 17500, "",
                false, 3, 16);
            ParcoMezzi.Add(v);
        }
    }
}
