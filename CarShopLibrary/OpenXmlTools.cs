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
            WordprocessingDocument volantinoDocument = CreaDocumento(filePath);
            using (volantinoDocument)
            {
                Body docBody = volantinoDocument.MainDocumentPart.Document.Body;
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

                Style titolo1Style = CreaStile("Titolo1");
                volantinoDocument.MainDocumentPart.StyleDefinitionsPart.Styles.Append(titolo1Style);
                p = CreaParagrafo("Test Titolo 1");
                ParagraphProperties pp = new ParagraphProperties();
                pp.ParagraphStyleId = new ParagraphStyleId() { Val = "Titolo1" };
                p.Append(pp);
                docBody.Append(p);
            }
        }

        public static WordprocessingDocument CreaDocumento(string filePath)
        {
            WordprocessingDocument wordDocument =
                WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document, true);
            // Add a main document part
            MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

            // Create the style part
            mainPart.AddNewPart<StyleDefinitionsPart>();
            Styles styles = new Styles();
            styles.Save(mainPart.StyleDefinitionsPart);

            // Create the document structure and add some text
            mainPart.Document = new Document();
            Body docBody = new Body();
            mainPart.Document.AppendChild(docBody);
            return wordDocument;
        }

        public static Style CreaStile(string styleName)
        {
            string styleId = styleName;
            // Create a new paragraph style and specify some of the properties.
            Style style = new DocumentFormat.OpenXml.Wordprocessing.Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = styleId,
                CustomStyle = true
            };
            style.Append(new StyleName() { Val = styleName });
            style.Append(new BasedOn() { Val = "Normal" });
            style.Append(new NextParagraphStyle() { Val = "Normal" });
            style.Append(new UIPriority() { Val = 900 });

            // Heading 1
            StyleRunProperties styleRunProperties = new StyleRunProperties();
            Color color = new Color() { Val = "2F5496" };
            // Specify a 16 point size. 16x2 because it’s half-point size
            DocumentFormat.OpenXml.Wordprocessing.FontSize fontSize = new DocumentFormat.OpenXml.Wordprocessing.FontSize();
            fontSize.Val = new StringValue("32");
            styleRunProperties.Append(color);
            styleRunProperties.Append(fontSize);

            // Add the run properties to the style.
            style.Append(styleRunProperties);

            return style;
        }

        public static Paragraph CreaParagrafo(string contenuto = "",
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

        public static Run CreaRun(string contenuto,
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
