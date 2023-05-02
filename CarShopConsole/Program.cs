using CarShopLibrary;
using DocumentFormat.OpenXml.Packaging;
using Excel = DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
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
        const string lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent quam augue, tempus id metus in, laoreet viverra quam. Sed vulputate risus lacus, et dapibus orci porttitor non.";

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
                    case 'x':
                    case 'X':
                        EsportaExcel();
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
                // Process.Start(AppDomain.CurrentDomain.BaseDirectory + "/html/index.html");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("\nMezzo non esistente!\n");
            }
        }

        private static void EsportaExcel()
        {
            Console.Clear();
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "/test.xlsx";
            GeneraReportXlsx(filePath);
            Process.Start(filePath);
        }

        public static void GeneraReportXlsx(string filePath)
        {
            SpreadsheetDocument reportDocument = OpenXmlExcelTools.CreaDocumento(filePath);
            using (reportDocument)
            {
                IEnumerable<Excel.Sheet> sheets = reportDocument.WorkbookPart.Workbook.GetFirstChild<Excel.Sheets>().Elements<Excel.Sheet>();
                WorksheetPart worksheetPart = (WorksheetPart)reportDocument.WorkbookPart.GetPartById(sheets.First().Id.Value);
                Excel.SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<Excel.SheetData>();
                string[] intestazione =
                {
                    "ID","MARCA","MODELLO","DATA", "PREZZO"
                };
                Excel.Row row = OpenXmlExcelTools.CreaIntestazione(intestazione);
                sheetData.Append(row);
            }
        }

        private static void EsportaWord()
        {
            //int num = 0;
            //do
            //{
            //    Console.Write("\nInserisci il numero d'ordine del veicolo: ");
            //} while (!int.TryParse(Console.ReadLine(), out num));
            int num = 1;
            if (num > 0 && num < ParcoMezzi.Count)
            {
                Console.Clear();
                Console.WriteLine("\n" + ParcoMezzi[num - 1] + "\n");
                // Produrre volantino in word
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "/test.docx";
                GeneraVolantinoDocx(filePath);
                Process.Start(filePath);
                // Console.ReadKey();
            }
            else
            {
                Console.WriteLine("\nMezzo non esistente!\n");
            }
        }


        public static void GeneraVolantinoDocx(string filePath)
        {
            WordprocessingDocument volantinoDocument = OpenXmlWordTools.CreaDocumento(filePath);
            using (volantinoDocument)
            {
                Body docBody = volantinoDocument.MainDocumentPart.Document.Body;

                // creiamo alcuni stili
                Style titolo1Style = OpenXmlWordTools.CreaStile("Mio Titolo 1", "1100CC", "Lucida Console", 24, 20, 30);
                volantinoDocument.MainDocumentPart.StyleDefinitionsPart.Styles.Append(titolo1Style);
                Style titolo2Style = OpenXmlWordTools.CreaStile("Mio Titolo 2", "AA5522", "Tahoma", 18, 0, 8, JustificationValues.Center);
                volantinoDocument.MainDocumentPart.StyleDefinitionsPart.Styles.Append(titolo2Style);
                Style codiceStyle = OpenXmlWordTools.CreaStile("Codice", "333333", "Courier New", 14);
                volantinoDocument.MainDocumentPart.StyleDefinitionsPart.Styles.Append(codiceStyle);

                // 3 paragrafi semplici
                docBody.Append(OpenXmlWordTools.CreaParagrafo("1° Paragrafo: " + lorem));
                docBody.Append(OpenXmlWordTools.CreaParagrafo("2° Paragrafo: " + lorem, JustificationValues.Center, "Comics Sans", 22));
                docBody.Append(OpenXmlWordTools.CreaParagrafo("3° Paragrafo: " + lorem, JustificationValues.Right));

                // paragrafo con contenuto formattato nei run
                Paragraph p = OpenXmlWordTools.CreaParagrafo();
                Run r = OpenXmlWordTools.CreaRun("Testo normale, ");
                p.Append(r);
                r = OpenXmlWordTools.CreaRun("testo grassetto, ", true);
                p.Append(r);
                r = OpenXmlWordTools.CreaRun("testo corsivo, ", false, true);
                p.Append(r);
                r = OpenXmlWordTools.CreaRun("testo sottolineato, ", false, false, true);
                p.Append(r);
                r = OpenXmlWordTools.CreaRun("testo grassetto, corsivo, sottolineato, arancione.", true, true, true, "FF9900");
                p.Append(r);
                docBody.Append(p);

                // test paragrafo coin stile
                p = OpenXmlWordTools.CreaParagrafoConStile("Test Titolo 1", "Mio Titolo 1");
                docBody.Append(p);
                p = OpenXmlWordTools.CreaParagrafoConStile("Test Titolo 2", "Mio Titolo 2");
                docBody.Append(p);
                p = OpenXmlWordTools.CreaParagrafoConStile("Test Codice", "Codice");
                docBody.Append(p);

                // test tabella
                string[,] contenutoTabella = new string[4, 2]
                {
                    { "MARCA" , "MODELLO" },
                    { "IBM" , "Toledo" },
                    { "Asus" , "Fulmine" },
                    { "Lenovo" , "Super Professional" }
                };
                Table t = OpenXmlWordTools.CreaTabella(contenutoTabella, TableRowAlignmentValues.Right, "FF22AA", 200);
                docBody.Append(t);

                // test elenco puntato
                string[] contenutoElenco = new string[4]
                {
                    "Prima riga elenco",
                    "Seconda riga elenco",
                    "Terza riga elenco",
                    "Quarta riga elenco"
                };
                List<Paragraph> elenco = OpenXmlWordTools.CreaElenco(contenutoElenco);
                foreach (var item in elenco) docBody.Append(item);
                elenco = OpenXmlWordTools.CreaElenco(contenutoElenco, true, "Tahoma", 20, "FF0000");
                foreach (var item in elenco) docBody.Append(item);

                // test immagine
                p = OpenXmlWordTools.AggiungiImmagine(volantinoDocument.MainDocumentPart,
                    "https://www.robinsonpetshop.it/news/cms2017/wp-content/uploads/2022/07/GattinoPrimiMesi.jpg",
                    "center", 100, 100);
                docBody.Append(p);
                p = OpenXmlWordTools.CreaParagrafo(); docBody.Append(p);
                p = OpenXmlWordTools.AggiungiImmagine(volantinoDocument.MainDocumentPart,
                    "https://www.robinsonpetshop.it/news/cms2017/wp-content/uploads/2022/07/GattinoPrimiMesi.jpg",
                    "left", 200, 200);
                docBody.Append(p);
                p = OpenXmlWordTools.CreaParagrafo(); docBody.Append(p);
                p = OpenXmlWordTools.AggiungiImmagine(volantinoDocument.MainDocumentPart,
                    "https://www.robinsonpetshop.it/news/cms2017/wp-content/uploads/2022/07/GattinoPrimiMesi.jpg",
                    "right", 150, 150);
                docBody.Append(p);

                // test hyperlink
                p = OpenXmlWordTools.CreaParagrafo();
                Hyperlink hl = OpenXmlWordTools.CreaHyperlink(volantinoDocument.MainDocumentPart,
                    "http://www.vallauri.edu", "testo grassetto, corsivo, sottolineato, arancione con link al sito del vallauri.",
                    true, true, true, "FF9900");
                p.Append(hl);
                docBody.Append(p);
                p = OpenXmlWordTools.CreaParagrafoConStile("", "Codice");
                hl = OpenXmlWordTools.CreaHyperlink(volantinoDocument.MainDocumentPart,
                    "http://www.vallauri.edu", "testo semplice con link al sito del vallauri.");
                p.Append(hl);
                docBody.Append(p);
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
                if (tipo == null || tipo == item.GetType())
                {
                    conta++;
                    Console.WriteLine(conta.ToString() + " - " + item.ToString(true));
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
            Console.WriteLine("X - Esporta Volatino XLSX");
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
                new StructDimensioni(270, 87, 68), "A34DE76PLYT90", 3500, 210,
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
