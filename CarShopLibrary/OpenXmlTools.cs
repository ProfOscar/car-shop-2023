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
                pp.ParagraphStyleId = new ParagraphStyleId();
                pp.ParagraphStyleId.Val = "Titolo1";
                p.PrependChild<ParagraphProperties>(pp);
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
            StyleDefinitionsPart styleDefinitionsPart = mainPart.AddNewPart<StyleDefinitionsPart>();
            Styles styles = styleDefinitionsPart.Styles;
            if (styles == null)
            {
                styleDefinitionsPart.Styles = new Styles();
                styleDefinitionsPart.Styles.Save();
            }

            CreateAndAddParagraphStyle(styleDefinitionsPart, "test", "Test 1");

            // Create the document structure and add some text
            mainPart.Document = new Document();
            Body docBody = new Body();
            mainPart.Document.AppendChild(docBody);
            return wordDocument;
        }

        // Create a new paragraph style with the specified style ID, primary style name, and aliases and 
        // add it to the specified style definitions part.
        public static void CreateAndAddParagraphStyle(StyleDefinitionsPart styleDefinitionsPart,
            string styleid, string stylename, string aliases = "")
        {
            // Access the root element of the styles part.
            Styles styles = styleDefinitionsPart.Styles;
            if (styles == null)
            {
                styleDefinitionsPart.Styles = new Styles();
                styleDefinitionsPart.Styles.Save();
            }

            // Create a new paragraph style element and specify some of the attributes.
            Style style = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = styleid,
                CustomStyle = true,
                Default = false
            };

            /*
            // Create and add the child elements (properties of the style).
            Aliases aliases1 = new Aliases() { Val = aliases };
            AutoRedefine autoredefine1 = new AutoRedefine() { Val = OnOffOnlyValues.Off };
            BasedOn basedon1 = new BasedOn() { Val = "Normal" };
            LinkedStyle linkedStyle1 = new LinkedStyle() { Val = "OverdueAmountChar" };
            Locked locked1 = new Locked() { Val = OnOffOnlyValues.Off };
            PrimaryStyle primarystyle1 = new PrimaryStyle() { Val = OnOffOnlyValues.On };
            StyleHidden stylehidden1 = new StyleHidden() { Val = OnOffOnlyValues.Off };
            SemiHidden semihidden1 = new SemiHidden() { Val = OnOffOnlyValues.Off };
            StyleName styleName1 = new StyleName() { Val = stylename };
            NextParagraphStyle nextParagraphStyle1 = new NextParagraphStyle() { Val = "Normal" };
            UIPriority uipriority1 = new UIPriority() { Val = 1 };
            UnhideWhenUsed unhidewhenused1 = new UnhideWhenUsed() { Val = OnOffOnlyValues.On };
            if (aliases != "")
                style.Append(aliases1);
            style.Append(autoredefine1);
            style.Append(basedon1);
            style.Append(linkedStyle1);
            style.Append(locked1);
            style.Append(primarystyle1);
            style.Append(stylehidden1);
            style.Append(semihidden1);
            style.Append(styleName1);
            style.Append(nextParagraphStyle1);
            style.Append(uipriority1);
            style.Append(unhidewhenused1);
            */

            // Create the StyleRunProperties object and specify some of the run properties.
            StyleRunProperties styleRunProperties1 = new StyleRunProperties();
            Bold bold1 = new Bold();
            Color color1 = new Color() { ThemeColor = ThemeColorValues.Accent2 };
            RunFonts font1 = new RunFonts() { Ascii = "Lucida Console" };
            Italic italic1 = new Italic();
            // Specify a 12 point size.
            FontSize fontSize1 = new FontSize() { Val = "24" };
            styleRunProperties1.Append(bold1);
            styleRunProperties1.Append(color1);
            styleRunProperties1.Append(font1);
            styleRunProperties1.Append(fontSize1);
            styleRunProperties1.Append(italic1);

            // Add the run properties to the style.
            style.Append(styleRunProperties1);

            // Add the style to the styles part.
            styles.Append(style);
        }

        public static Style CreaStile(string styleName)
        {
            string styleId = styleName.Replace(" ", "");
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
            RunFonts font = new RunFonts() { Ascii = "Lucida Console" };
            FontSize fontSize = new FontSize() { Val = "60" };
            styleRunProperties.Append(color);
            styleRunProperties.Append(font);
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
