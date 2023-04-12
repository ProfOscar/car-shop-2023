using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using excel = DocumentFormat.OpenXml.Spreadsheet;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using word = DocumentFormat.OpenXml.Wordprocessing;
using X14 = DocumentFormat.OpenXml.Office2010.Excel;
using X15 = DocumentFormat.OpenXml.Office2013.Excel;

namespace CarShopLibrary
{
    public class OpenXmlExcelTools
    {

        public static string CreateXlsx(List<Veicolo> veicoli, string outputPath)
        {
            var datetime = DateTime.Now.ToString().Replace("/", "_").Replace(":", "_");
            if (File.Exists(outputPath))
            {
                outputPath = Path.GetFileNameWithoutExtension(outputPath) + "_" + datetime + "-" + ".xlsx";
            }

            using (SpreadsheetDocument package = SpreadsheetDocument.Create(outputPath, SpreadsheetDocumentType.Workbook))
            {
                CreatePartsForExcel(package, veicoli);
            }
            return outputPath;
        }

        private static void CreatePartsForExcel(SpreadsheetDocument package, List<Veicolo> veicoli)
        {
            excel.SheetData partSheetData = GenerateSheetdataForDetails(veicoli);

            WorkbookPart workbookPart1 = package.AddWorkbookPart();
            GenerateWorkbookPartContent(workbookPart1);

            WorkbookStylesPart workbookStylesPart1 = workbookPart1.AddNewPart<WorkbookStylesPart>("rId3");
            GenerateWorkbookStylesPartContent(workbookStylesPart1);

            WorksheetPart worksheetPart1 = workbookPart1.AddNewPart<WorksheetPart>("rId1");
            GenerateWorksheetPartContent(worksheetPart1, partSheetData);
        }

        private static excel.SheetData GenerateSheetdataForDetails(List<Veicolo> veicoli)
        {
            excel.SheetData sheetData1 = new excel.SheetData();
            excel.Row headerRow = new excel.Row();
            headerRow.Append(CreateCell("VOLANTINO VEICOLI", 2U));
            sheetData1.Append(headerRow);
            sheetData1.Append(CreateHeaderRowForExcel());
            excel.Author autore = new excel.Author(Environment.UserName);
            sheetData1.Append(autore);
            foreach (Veicolo veicolo in veicoli)
            {
                excel.Row partsRows = GenerateRowForChildPartDetail(veicolo);
                sheetData1.Append(partsRows);
            }
            return sheetData1;
        }
        private static excel.Row GenerateRowForChildPartDetail(Veicolo veicolo)
        {
            excel.Row tRow = new excel.Row();
            tRow.Append(CreateCell(veicolo.GetType().Name.ToString()));
            tRow.Append(CreateCell(veicolo.Marca));
            tRow.Append(CreateCell(veicolo.Modello));
            tRow.Append(CreateCell(veicolo.DataImmatricolazione.Year.ToString()));
            tRow.Append(CreateCell(veicolo.Prezzo.ToString()));

            return tRow;
        }

        private static EnumValue<excel.CellValues> ResolveCellDataTypeOnValue(string text)
        {
            int intVal;
            double doubleVal;
            if (int.TryParse(text, out intVal) || double.TryParse(text, out doubleVal))
            {
                return excel.CellValues.Number;
            }
            else
            {
                return excel.CellValues.String;
            }
        }

        private static excel.Row CreateHeaderRowForExcel()
        {
            excel.Row workRow = new excel.Row();
            workRow.Append(CreateCell("Tipo", 2U));
            workRow.Append(CreateCell("Marca", 2U));
            workRow.Append(CreateCell("Modello", 2U));
            workRow.Append(CreateCell("Data", 2U));
            workRow.Append(CreateCell("Prezzo", 2U));
            return workRow;
        }
        private static excel.Cell CreateCell(string text, uint styleIndex = 1U)
        {
            excel.Cell cell = new excel.Cell();
            cell.StyleIndex = styleIndex;
            cell.DataType = ResolveCellDataTypeOnValue(text);
            cell.CellValue = new excel.CellValue(text);
            return cell;
        }
        private static void GenerateWorksheetPartContent(WorksheetPart worksheetPart1, excel.SheetData sheetData1)
        {
            excel.Worksheet worksheet1 = new excel.Worksheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            worksheet1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            worksheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            worksheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
            excel.SheetDimension sheetDimension1 = new excel.SheetDimension() { Reference = "A1" };

            excel.SheetViews sheetViews1 = new excel.SheetViews();

            excel.SheetView sheetView1 = new excel.SheetView() { TabSelected = true, WorkbookViewId = (UInt32Value)0U };
            excel.Selection selection1 = new excel.Selection() { ActiveCell = "A1", SequenceOfReferences = new ListValue<StringValue>() { InnerText = "A1" } };

            sheetView1.Append(selection1);

            sheetViews1.Append(sheetView1);
            excel.SheetFormatProperties sheetFormatProperties1 = new excel.SheetFormatProperties() { DefaultRowHeight = 15D, DyDescent = 0.25D };

            excel.PageMargins pageMargins1 = new excel.PageMargins() { Left = 0.7D, Right = 0.7D, Top = 0.75D, Bottom = 0.75D, Header = 0.3D, Footer = 0.3D };
            worksheet1.Append(sheetDimension1);
            worksheet1.Append(sheetViews1);
            worksheet1.Append(sheetFormatProperties1);
            worksheet1.Append(sheetData1);
            worksheet1.Append(pageMargins1);
            worksheetPart1.Worksheet = worksheet1;
        }

        private static void GenerateWorkbookStylesPartContent(WorkbookStylesPart workbookStylesPart1)
        {
            excel.Stylesheet stylesheet1 = new excel.Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

            excel.Fonts fonts1 = new excel.Fonts() { Count = (UInt32Value)2U, KnownFonts = true };

            excel.Font font1 = new excel.Font();
            excel.FontSize fontSize1 = new excel.FontSize() { Val = 11D };
            excel.Color color1 = new excel.Color() { Theme = (UInt32Value)1U };
            excel.FontName fontName1 = new excel.FontName() { Val = "Calibri" };
            excel.FontFamilyNumbering fontFamilyNumbering1 = new excel.FontFamilyNumbering() { Val = 2 };
            excel.FontScheme fontScheme1 = new excel.FontScheme() { Val = excel.FontSchemeValues.Minor };

            font1.Append(fontSize1);
            font1.Append(color1);
            font1.Append(fontName1);
            font1.Append(fontFamilyNumbering1);
            font1.Append(fontScheme1);

            excel.Font font2 = new excel.Font();
            excel.Bold bold1 = new excel.Bold();
            excel.FontSize fontSize2 = new excel.FontSize() { Val = 11D };
            excel.Color color2 = new excel.Color() { Theme = (UInt32Value)1U };
            excel.FontName fontName2 = new excel.FontName() { Val = "Calibri" };
            excel.FontFamilyNumbering fontFamilyNumbering2 = new excel.FontFamilyNumbering() { Val = 2 };
            excel.FontScheme fontScheme2 = new excel.FontScheme() { Val = excel.FontSchemeValues.Minor };

            font2.Append(bold1);
            font2.Append(fontSize2);
            font2.Append(color2);
            font2.Append(fontName2);
            font2.Append(fontFamilyNumbering2);
            font2.Append(fontScheme2);

            fonts1.Append(font1);
            fonts1.Append(font2);

            excel.Fills fills1 = new excel.Fills() { Count = (UInt32Value)2U };

            excel.Fill fill1 = new excel.Fill();
            excel.PatternFill patternFill1 = new excel.PatternFill() { PatternType = excel.PatternValues.None };

            fill1.Append(patternFill1);

            excel.Fill fill2 = new excel.Fill();
            excel.PatternFill patternFill2 = new excel.PatternFill() { PatternType = excel.PatternValues.Gray125 };

            fill2.Append(patternFill2);

            fills1.Append(fill1);
            fills1.Append(fill2);

            excel.Borders borders1 = new excel.Borders() { Count = (UInt32Value)2U };

            excel.Border border1 = new excel.Border();
            excel.LeftBorder leftBorder1 = new excel.LeftBorder();
            excel.RightBorder rightBorder1 = new excel.RightBorder();
            excel.TopBorder topBorder1 = new excel.TopBorder();
            excel.BottomBorder bottomBorder1 = new excel.BottomBorder();
            excel.DiagonalBorder diagonalBorder1 = new excel.DiagonalBorder();

            border1.Append(leftBorder1);
            border1.Append(rightBorder1);
            border1.Append(topBorder1);
            border1.Append(bottomBorder1);
            border1.Append(diagonalBorder1);

            excel.Border border2 = new excel.Border();

            excel.LeftBorder leftBorder2 = new excel.LeftBorder() { Style = excel.BorderStyleValues.Thin };
            excel.Color color3 = new excel.Color() { Indexed = (UInt32Value)64U };

            leftBorder2.Append(color3);

            excel.RightBorder rightBorder2 = new excel.RightBorder() { Style = excel.BorderStyleValues.Thin };
            excel.Color color4 = new excel.Color() { Indexed = (UInt32Value)64U };

            rightBorder2.Append(color4);

            excel.TopBorder topBorder2 = new excel.TopBorder() { Style = excel.BorderStyleValues.Thin };
            excel.Color color5 = new excel.Color() { Indexed = (UInt32Value)64U };

            topBorder2.Append(color5);

            excel.BottomBorder bottomBorder2 = new excel.BottomBorder() { Style = excel.BorderStyleValues.Thin };
            excel.Color color6 = new excel.Color() { Indexed = (UInt32Value)64U };

            bottomBorder2.Append(color6);
            excel.DiagonalBorder diagonalBorder2 = new excel.DiagonalBorder();

            border2.Append(leftBorder2);
            border2.Append(rightBorder2);
            border2.Append(topBorder2);
            border2.Append(bottomBorder2);
            border2.Append(diagonalBorder2);

            borders1.Append(border1);
            borders1.Append(border2);

            excel.CellStyleFormats cellStyleFormats1 = new excel.CellStyleFormats() { Count = (UInt32Value)1U };
            excel.CellFormat cellFormat1 = new excel.CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };

            cellStyleFormats1.Append(cellFormat1);

            excel.CellFormats cellFormats1 = new excel.CellFormats() { Count = (UInt32Value)3U };
            excel.CellFormat cellFormat2 = new excel.CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };
            excel.CellFormat cellFormat3 = new excel.CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyBorder = true };
            excel.CellFormat cellFormat4 = new excel.CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = true };

            cellFormats1.Append(cellFormat2);
            cellFormats1.Append(cellFormat3);
            cellFormats1.Append(cellFormat4);

            excel.CellStyles cellStyles1 = new excel.CellStyles() { Count = (UInt32Value)1U };
            excel.CellStyle cellStyle1 = new excel.CellStyle() { Name = "Normal", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };

            cellStyles1.Append(cellStyle1);
            excel.DifferentialFormats differentialFormats1 = new excel.DifferentialFormats() { Count = (UInt32Value)0U };
            excel.TableStyles tableStyles1 = new excel.TableStyles() { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleLight16" };

            excel.StylesheetExtensionList stylesheetExtensionList1 = new excel.StylesheetExtensionList();

            excel.StylesheetExtension stylesheetExtension1 = new excel.StylesheetExtension() { Uri = "{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}" };
            stylesheetExtension1.AddNamespaceDeclaration("x14", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");
            X14.SlicerStyles slicerStyles1 = new X14.SlicerStyles() { DefaultSlicerStyle = "SlicerStyleLight1" };

            stylesheetExtension1.Append(slicerStyles1);

            excel.StylesheetExtension stylesheetExtension2 = new excel.StylesheetExtension() { Uri = "{9260A510-F301-46a8-8635-F512D64BE5F5}" };
            stylesheetExtension2.AddNamespaceDeclaration("x15", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/main");
            X15.TimelineStyles timelineStyles1 = new X15.TimelineStyles() { DefaultTimelineStyle = "TimeSlicerStyleLight1" };

            stylesheetExtension2.Append(timelineStyles1);

            stylesheetExtensionList1.Append(stylesheetExtension1);
            stylesheetExtensionList1.Append(stylesheetExtension2);

            stylesheet1.Append(fonts1);
            stylesheet1.Append(fills1);
            stylesheet1.Append(borders1);
            stylesheet1.Append(cellStyleFormats1);
            stylesheet1.Append(cellFormats1);
            stylesheet1.Append(cellStyles1);
            stylesheet1.Append(differentialFormats1);
            stylesheet1.Append(tableStyles1);
            stylesheet1.Append(stylesheetExtensionList1);

            workbookStylesPart1.Stylesheet = stylesheet1;
        }

        private static void GenerateWorkbookPartContent(WorkbookPart workbookPart1)
        {
            excel.Workbook workbook1 = new excel.Workbook();
            excel.Sheets sheets1 = new excel.Sheets();
            excel.Sheet sheet1 = new excel.Sheet() { Name = "Sheet1", SheetId = (UInt32Value)1U, Id = "rId1" };
            sheets1.Append(sheet1);
            workbook1.Append(sheets1);
            workbookPart1.Workbook = workbook1;
        }

    }
}
