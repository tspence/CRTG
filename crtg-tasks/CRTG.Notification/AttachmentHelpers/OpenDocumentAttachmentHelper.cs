using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;

namespace CRTG.Notification
{
    public class OpenDocumentAttachmentHelper
    {
        /// <summary>
        /// Shortcut to construct an attachment object using Excel open document file formats
        /// </summary>
        /// <param name="report_data"></param>
        /// <param name="attachment_filename"></param>
        /// <returns></returns>
        public static Attachment BuildAttachment(DataTable report_data, string attachment_filename)
        {
            string fn = Path.GetTempFileName() + ".xlsx";
            try {
                using (var workbook = SpreadsheetDocument.Create(fn, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook, true)) {

                    // Construct the workbook parts
                    var workbookPart = workbook.AddWorkbookPart();
                    workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
                    var sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();
                    workbook.WorkbookPart.Workbook.Sheets = sheets;

                    // Set up this sheet
                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                    sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);
                    string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);
                    uint sheetId = 1;
                    if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0) {
                        sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    // Make this sheet a table
                    string name = report_data.TableName;
                    if (String.IsNullOrWhiteSpace(name)) {
                        name = Path.GetFileNameWithoutExtension(attachment_filename);
                    }
                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = name };
                    sheets.Append(sheet);

                    // Set up header row
                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    List<String> columns = new List<string>();
                    foreach (System.Data.DataColumn column in report_data.Columns) {
                        columns.Add(column.ColumnName);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName);
                        headerRow.AppendChild(cell);
                    }
                    sheetData.AppendChild(headerRow);

                    // Set up data rows
                    foreach (System.Data.DataRow dsrow in report_data.Rows) {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (String col in columns) {
                            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dsrow[col].ToString());
                            newRow.AppendChild(cell);
                        }
                        sheetData.AppendChild(newRow);
                    }

                    // Close the workbook and save it
                    workbook.Close();
                }

                // Here's your attachment
                byte[] bytes = File.ReadAllBytes(fn);
                Attachment a = new Attachment(new MemoryStream(bytes), new System.Net.Mime.ContentType("application/vnd.ms-excel"));
                a.Name = attachment_filename;
                return a;

                // Clean up afterwards
            } finally {
                if (File.Exists(fn)) File.Delete(fn);
            }
        }
    }
}
