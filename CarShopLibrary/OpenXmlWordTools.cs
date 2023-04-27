using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Color = DocumentFormat.OpenXml.Wordprocessing.Color;

namespace CarShopLibrary
{
    public class OpenXmlWordTools
    {
        public static WordprocessingDocument CreaDocumento(string filePath)
        {
            WordprocessingDocument wordDocument =
                WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document, true);
            // Add a main document part
            MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

            // Create the StyleDefinitionsPart
            StyleDefinitionsPart styleDefinitionsPart = mainPart.AddNewPart<StyleDefinitionsPart>();
            Styles styles = styleDefinitionsPart.Styles;
            if (styles == null)
            {
                styleDefinitionsPart.Styles = new Styles();
                styleDefinitionsPart.Styles.Save();
            }

            // Create the NumberingDefinitionsPart (support for bullet list)
            NumberingDefinitionsPart numberingPart =
            mainPart.AddNewPart<NumberingDefinitionsPart>("UnorderedList");
            Numbering element =
              new Numbering(
                new AbstractNum(
                  new Level(
                    new NumberingFormat() { Val = NumberFormatValues.Bullet },
                    new LevelText() { Val = "•" }
                  )
                  { LevelIndex = 0 }
                )
                { AbstractNumberId = 1 },
                new NumberingInstance(
                  new AbstractNumId() { Val = 1 }
                )
                { NumberID = 1 });
            element.Save(numberingPart);

            // Create the document structure
            mainPart.Document = new Document();
            Body docBody = new Body();
            mainPart.Document.AppendChild(docBody);
            return wordDocument;
        }

        public static Style CreaStile(string nomeStile, string colore,
            string tipoFont, double dimensioneFont,
            int spazioPrima = 0, int spazioDopo = 8,
            JustificationValues giustificazione = JustificationValues.Left)
        {
            string styleId = nomeStile.Replace(" ", "");

            // Create a new paragraph style and specify some of the properties.
            Style style = new DocumentFormat.OpenXml.Wordprocessing.Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = styleId,
                CustomStyle = true,
                Default = false
            };
            style.Append(new StyleName() { Val = nomeStile });
            style.Append(new BasedOn() { Val = "Normal" });
            style.Append(new NextParagraphStyle() { Val = "Normal" });
            style.Append(new UIPriority() { Val = 900 });

            ParagraphProperties paragraphProperties = new ParagraphProperties();
            string before = (spazioPrima * 20).ToString();
            string after = (spazioDopo * 20).ToString();
            paragraphProperties.SpacingBetweenLines = new SpacingBetweenLines() { Before = before, After = after };
            paragraphProperties.Justification = new Justification() { Val = giustificazione };
            style.Append(paragraphProperties);

            StyleRunProperties styleRunProperties = new StyleRunProperties();
            RunFonts font = new RunFonts() { Ascii = tipoFont };
            dimensioneFont *= 2;
            FontSize fontSize = new FontSize() { Val = dimensioneFont.ToString() };
            styleRunProperties.Append(new Color() { Val = colore });
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

        public static Paragraph CreaParagrafoConStile(string contenuto, string nomeStile)
        {
            Paragraph p = new Paragraph();
            if (contenuto.Length > 0)
            {
                Run r = new Run();
                Text t = new Text(contenuto);
                r.Append(t);
                p.Append(r);
            }
            ParagraphProperties pp = new ParagraphProperties();
            string styleId = nomeStile.Replace(" ", "");
            pp.ParagraphStyleId = new ParagraphStyleId() { Val = styleId };
            p.PrependChild(pp);
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

        public static Hyperlink CreaHyperlink(MainDocumentPart mainPart, string url, string contenuto,
            bool isGrassetto = false, bool isCorsivo = false, bool isSottolineato = true,
            string colore = "000000")
        {
            HyperlinkRelationship hr = mainPart.AddHyperlinkRelationship(new Uri(url), true);
            string hrContactId = hr.Id;
            return
                new Hyperlink(
                    new ProofError() { Type = ProofingErrorValues.GrammarStart },
                    CreaRun(contenuto, isGrassetto, isCorsivo, isSottolineato, colore)
                    )
                { History = OnOffValue.FromBoolean(true), Id = hrContactId };
        }

        public static Table CreaTabella(string[,] contenuto,
        TableRowAlignmentValues giustificazione = TableRowAlignmentValues.Left,
        string colore = "333333", int margine = 80)
        {
            Table table = new Table();
            ModificaProprietaTabella(table, giustificazione, colore, margine);
            for (int i = 0; i < contenuto.GetLength(0); i++)
            {
                TableRow tr = new TableRow();
                for (int j = 0; j < contenuto.GetLength(1); j++)
                {
                    TableCell tc = new TableCell();
                    // per evitare un padding inferiore nella cella
                    TableCellProperties tcp = new TableCellProperties() { TableCellMargin = new TableCellMargin() { BottomMargin = new BottomMargin() { Width = "0" } } };
                    tc.AppendChild(tcp);
                    Paragraph p = new Paragraph(new Run(new Text(contenuto[i, j])));
                    tc.Append(p);
                    tr.Append(tc);
                }
                table.Append(tr);
            }
            return table;
        }

        private static void ModificaProprietaTabella(Table tabella,
            TableRowAlignmentValues giustificazione = TableRowAlignmentValues.Left,
            string colore = "333333", int margine = 80)
        {
            TableProperties tblProperties = new TableProperties();

            tblProperties.TableJustification = new TableJustification() { Val = giustificazione };

            TableBorders tblBorders = new TableBorders();
            TopBorder topBorder = new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Thick), Color = colore };
            tblBorders.AppendChild(topBorder);
            BottomBorder bottomBorder = new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Thick), Color = colore };
            tblBorders.AppendChild(bottomBorder);
            RightBorder rightBorder = new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Thick), Color = colore };
            tblBorders.AppendChild(rightBorder);
            LeftBorder leftBorder = new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Thick), Color = colore };
            tblBorders.AppendChild(leftBorder);
            InsideHorizontalBorder insideHBorder = new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Thick), Color = colore };
            tblBorders.AppendChild(insideHBorder);
            InsideVerticalBorder insideVBorder = new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Thick), Color = colore };
            tblBorders.AppendChild(insideVBorder);

            // margine fra le celle
            TableCellMarginDefault tcm = new TableCellMarginDefault(
                new TopMargin() { Width = margine.ToString(), Type = TableWidthUnitValues.Dxa },
                new StartMargin() { Width = margine.ToString(), Type = TableWidthUnitValues.Dxa },
                new BottomMargin() { Width = margine.ToString(), Type = TableWidthUnitValues.Dxa },
                new EndMargin() { Width = margine.ToString(), Type = TableWidthUnitValues.Dxa });
            tblProperties.AppendChild(tcm);

            tblProperties.AppendChild(tblBorders);
            tabella.Append(tblProperties);
        }

        public static List<Paragraph> CreaElenco(string[] contenuto, bool isOrdered = false,
            string fontFace = "Calibri", double fontSize = 11, string colore = "000000")
        {
            List<Paragraph> retVal = new List<Paragraph>();
            fontSize *= 2;
            // Paragraph properties
            SpacingBetweenLines sblUl = new SpacingBetweenLines() { After = "0" };  // Get rid of space between bullets  
            Indentation iUl = new Indentation() { Left = "260", Hanging = "240" };  // correct indentation  
            NumberingProperties npUl = new NumberingProperties(
                new NumberingLevelReference() { Val = 0 },
                new NumberingId() { Val = (isOrdered ? 2 : 1) }
            );
            ParagraphProperties ppElenco = new ParagraphProperties(npUl, sblUl, iUl);
            ppElenco.ParagraphStyleId = new ParagraphStyleId() { Val = "ListParagraph" };
            RunProperties rpElenco = new RunProperties();
            RunFonts rfElenco = new RunFonts() { Ascii = fontFace };
            rpElenco.FontSize = new FontSize() { Val = fontSize.ToString() };
            rpElenco.Color = new Color() { Val = colore };
            rpElenco.Append(rfElenco);
            ppElenco.Append(rpElenco);

            // Contenuto
            for (int i = 0; i < contenuto.Length; i++)
            {
                Paragraph p = new Paragraph();
                p.ParagraphProperties = new ParagraphProperties(ppElenco.OuterXml);
                Run r = new Run();
                // gestione font
                RunProperties rp = new RunProperties();
                RunFonts rf = new RunFonts() { Ascii = fontFace };
                rp.FontSize = new FontSize() { Val = fontSize.ToString() };
                rp.Append(rf);
                rp.Color = new Color() { Val = colore };
                r.Append(rp);
                Text t = new Text(contenuto[i]);
                r.Append(t);
                p.Append(r);
                retVal.Add(p);
            }
            return retVal;
        }

        public static Paragraph AggiungiImmagine(MainDocumentPart mainPart, string imgPath, string position = "left", int width = 0, int height = 0)
        {
            Paragraph pImg = new Paragraph();
            ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);

            // Stream stream = OpenXMLImageHelper.FromImageUrlToStream(imgPath);

            Image image = Image.FromStream(OpenXMLImageHelper.FromImageUrlToStream(imgPath));
            // imagePart.FeedData(OpenXMLImageHelper.FromImageUrlToStream(imgPath));
            imagePart.FeedData(OpenXMLImageHelper.FromImageObjToStream(image));
            int iWidth = width > 0 ? width * 9525 : (int)Math.Round((decimal)image.Width * 9525);
            int iHeight = height > 0 ? height * 9525 : (int)Math.Round((decimal)image.Height * 9525);

            // 1500000 and 1092000 are img width and height
            Run rImg = new Run(OpenXMLImageHelper.DrawingManager(mainPart.GetIdOfPart(imagePart), "PictureName", iWidth, iHeight, position));
            pImg.Append(rImg);
            return pImg;
        }
    }
}
