using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarShopLibrary
{
    public class OpenXmlTools
    {
        const string lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent quam augue, tempus id metus in, laoreet viverra quam. Sed vulputate risus lacus, et dapibus orci porttitor non.";

        public static void GeneraVolantinoDocx(string filePath)
        {
            using (WordprocessingDocument wordDocument =
                WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document, true))
            {
                // Add a main document part. 
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                // Create the document structure and add some text.
                mainPart.Document = new Document();
                Body docBody = new Body();
                mainPart.Document.AppendChild(docBody);

                // 3 paragrafi semplici
                docBody.Append(CreaParagrafo("1° Paragrafo: " + lorem));
                docBody.Append(CreaParagrafo("2° Paragrafo: " + lorem, JustificationValues.Center, "Comics Sans", 22));
                docBody.Append(CreaParagrafo("3° Paragrafo: " + lorem, JustificationValues.Right));

                // paragrafo con contenuto formattato nei run
                Paragraph p = CreaParagrafo();
                Run r = CreaRun("Testo normale, ");
                p.Append(r);
                r = CreaRun("testo grassetto, ", true);
                p.Append(r);
                r = CreaRun("testo corsivo, ", false, true);
                p.Append(r);
                r = CreaRun("testo sottolineato, ", false, false, true);
                p.Append(r);
                r = CreaRun("testo grassetto, corsivo, sottolineato, arancione.", true, true, true, "FF9900");
                p.Append(r);
                docBody.Append(p);
            }
        }

        private static Paragraph CreaParagrafo(string contenuto = "", 
            JustificationValues giustificazione = JustificationValues.Left,
            string fontFace = "Calibri", double fontSize = 11)
        {
            Paragraph p = new Paragraph();
            if (giustificazione != JustificationValues.Left)
            {
                ParagraphProperties pp = new ParagraphProperties();
                pp.Justification = new Justification() { Val = giustificazione };
                p.Append(pp);
            }
            if (contenuto.Length > 0)
            {
                Run r = new Run();

                // gestione font
                RunProperties rp = new RunProperties();
                RunFonts rf = new RunFonts() { Ascii = fontFace };
                fontSize *= 2;
                rp.FontSize = new FontSize() { Val = fontSize.ToString() };
                rp.Append(rf);
                r.Append(rp);

                Text t = new Text(contenuto);
                r.Append(t);
                p.Append(r);
            }
            return p;
        }

        private static Run CreaRun(string contenuto, 
            bool isGrassetto = false, bool isCorsivo = false, bool isSottolineato = false, 
            string colore = "000000")
        {
            Run r = new Run();

            RunProperties rp = new RunProperties();
            if (isGrassetto) rp.Bold = new Bold();
            if (isCorsivo) rp.Italic = new Italic();
            if (isSottolineato) rp.Underline = new Underline() { Val = UnderlineValues.Single };
            rp.Color = new Color() { Val = colore };

            Text t = new Text(contenuto) { Space = SpaceProcessingModeValues.Preserve };

            r.Append(rp);
            r.Append(t);

            return r;
        }
    }
}
