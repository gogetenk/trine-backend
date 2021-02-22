using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using Dto;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Assistance.Operational.Bll.Impl.Helpers
{
    public static class ExcelGenerator
    {
        public static byte[] GenerateCommonTemplate(ActivityDto activityDto)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add($"Rapport activité {activityDto.StartDate.Month}");

                worksheet.Cells[3, 3].Value = "COMPTE RENDU D'ACTIVITE";
                worksheet.Cells[3, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[3, 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(1, 150, 150, 150));
                worksheet.Cells[3, 3].Style.Font.Color.SetColor(Color.White);
                worksheet.Cells[3, 3].Style.Font.Bold = true;
                worksheet.Cells[3, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[3, 3].Style.Font.Size = 14;
                worksheet.Cells[3, 3, 3, 38].Merge = true;

                worksheet.Cells[6, 1].Value = "Nom Intervenant :";
                worksheet.Cells[6, 1].Style.Font.Size = 14;
                worksheet.Cells[6, 1].Style.Font.UnderLine = true;

                worksheet.Cells[8, 1].Value = "Période :";
                worksheet.Cells[8, 1].Style.Font.Size = 14;
                worksheet.Cells[8, 1].Style.Font.UnderLine = true;

                worksheet.Cells[10, 1].Value = "Dates d'interventions :";
                worksheet.Cells[10, 1].Style.Font.Size = 14;
                worksheet.Cells[10, 1].Style.Font.Bold = true;

                worksheet.Cells[6, 3].Value = $"{activityDto.Consultant.Firstname} {activityDto.Consultant.Lastname?.ToUpper()}";
                worksheet.Cells[6, 3].Style.Font.Size = 11;
                worksheet.Cells[6, 3].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[6, 4].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[6, 5].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[6, 6].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[6, 7].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[6, 3, 6, 7].Merge = true;

                worksheet.Cells[8, 3].Value = activityDto.StartDate.ToShortDateString();
                worksheet.Cells[8, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[8, 3].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[8, 4].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[8, 3, 8, 4].Merge = true;

                worksheet.Cells[8, 5].Value = "au";
                worksheet.Cells[8, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells[8, 6].Value = activityDto.EndDate.ToShortDateString();
                worksheet.Cells[8, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[8, 6].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[8, 7].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[8, 6, 8, 7].Merge = true;

                var calendar = CultureInfo.InvariantCulture.Calendar;
                var currentWeekNumber = calendar.GetWeekOfYear(activityDto.StartDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

                var memorizedIndex = 3;
                var index = 0;

                worksheet.Cells[10, memorizedIndex].Value = currentWeekNumber.ToString();

                //worksheet.Cells[13, 1].Value = mission.OrganizationName?.ToUpper();
                worksheet.Cells[13, 1].Style.Font.Size = 12;
                worksheet.Cells[13, 1].Style.Font.Bold = true;
                worksheet.Cells[13, 1].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[13, 2].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[13, 1, 13, 2].Merge = true;

                worksheet.Cells[14, 1].Value = "Dépassement à la demande du Client (h.)";
                worksheet.Cells[14, 1].Style.Font.Size = 12;
                worksheet.Cells[14, 1].Style.Font.Bold = true;
                worksheet.Cells[14, 1].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[14, 2].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[14, 1, 14, 2].Merge = true;

                worksheet.Cells[15, 1].Value = "Astreintes";
                worksheet.Cells[15, 1, 15, 2].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[15, 1, 15, 2].Style.Font.Bold = true;
                worksheet.Cells[15, 1, 15, 2].Merge = true;
                worksheet.Cells[15, 1, 15, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                worksheet.Cells[12, 1].Value = "Assistance Technique";
                worksheet.Cells[12, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[12, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(1, 255, 204, 153));
                worksheet.Cells[12, 1].Style.Font.Bold = true;
                worksheet.Cells[12, 1].Style.Font.Size = 12;
                worksheet.Cells[12, 1].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[12, 2].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[12, 1, 12, 2].Merge = true;

                worksheet.Cells[16, 1].Value = "Autre";
                worksheet.Cells[16, 1].Style.Font.Bold = true;
                worksheet.Cells[16, 1].Style.Font.Size = 12;
                worksheet.Cells[16, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[16, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(1, 255, 204, 153));
                worksheet.Cells[16, 1, 16, 2].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[16, 1, 16, 2].Merge = true;

                worksheet.Cells[17, 1].Value = "Formation";
                worksheet.Cells[17, 1].Style.Font.Bold = true;
                worksheet.Cells[17, 1].Style.Font.Size = 12;
                worksheet.Cells[17, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[17, 1, 17, 2].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[17, 1, 17, 2].Merge = true;

                worksheet.Cells[18, 1].Value = "Congés";
                worksheet.Cells[18, 1].Style.Font.Bold = true;
                worksheet.Cells[18, 1].Style.Font.Size = 12;
                worksheet.Cells[18, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[18, 1, 18, 2].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[18, 1, 18, 2].Merge = true;

                worksheet.Cells[19, 1].Value = "Absences";
                worksheet.Cells[19, 1].Style.Font.Bold = true;
                worksheet.Cells[19, 1].Style.Font.Size = 12;
                worksheet.Cells[19, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[19, 1, 19, 2].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[19, 1, 19, 2].Merge = true;

                worksheet.Cells[20, 1].Value = "Maladies";
                worksheet.Cells[20, 1].Style.Font.Bold = true;
                worksheet.Cells[20, 1].Style.Font.Size = 12;
                worksheet.Cells[20, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[20, 1, 20, 2].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[20, 1, 20, 2].Merge = true;

                worksheet.Cells[21, 1].Value = "récupération convenue avec le Client";
                worksheet.Cells[21, 1].Style.Font.Bold = true;
                worksheet.Cells[21, 1].Style.Font.Size = 12;
                worksheet.Cells[21, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[21, 1, 21, 2].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[21, 1, 21, 2].Merge = true;

                worksheet.Cells[22, 1].Value = "Heures sup validées";
                worksheet.Cells[22, 1].Style.Font.Bold = true;
                worksheet.Cells[22, 1].Style.Font.Size = 12;
                worksheet.Cells[22, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[22, 1, 22, 2].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[22, 1, 22, 2].Merge = true;

                worksheet.Cells[23, 1].Value = "Télétravail (avec accord préalable)";
                worksheet.Cells[23, 1].Style.Font.Bold = true;
                worksheet.Cells[23, 1].Style.Font.Size = 12;
                worksheet.Cells[23, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[23, 1, 23, 2].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[23, 1, 23, 2].Merge = true;

                var totalOpenDay = 0;

                while (index < activityDto.Days.Count)
                {
                    var columnIndex = index + 3;
                    var item = activityDto.Days[index];

                    var dayOfWeek = calendar.GetDayOfWeek(item.Day);
                    var weekNumber = calendar.GetWeekOfYear(item.Day, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

                    if (currentWeekNumber != weekNumber)
                    {
                        var cell = worksheet.Cells[10, memorizedIndex, 10, columnIndex - 1 >= memorizedIndex ? columnIndex - 1 : memorizedIndex];
                        cell.Merge = true;
                        cell.Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                        currentWeekNumber = weekNumber;
                        memorizedIndex = columnIndex;
                    }

                    worksheet.Cells[10, columnIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[10, columnIndex].Value = currentWeekNumber.ToString();
                    worksheet.Cells[10, columnIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[10, columnIndex].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(1, 255, 128, 128));
                    worksheet.Cells[10, columnIndex].Style.Font.Bold = true;
                    worksheet.Cells[10, columnIndex].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);

                    var dayLabel = "";
                    switch (dayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            dayLabel = "Lu";
                            break;
                        case DayOfWeek.Tuesday:
                            dayLabel = "Ma";
                            break;
                        case DayOfWeek.Wednesday:
                            dayLabel = "Me";
                            break;
                        case DayOfWeek.Thursday:
                            dayLabel = "Je";
                            break;
                        case DayOfWeek.Friday:
                            dayLabel = "Ve";
                            break;
                        case DayOfWeek.Saturday:
                            dayLabel = "Sa";
                            break;
                        case DayOfWeek.Sunday:
                            dayLabel = "Di";
                            break;
                        default:
                            break;
                    }

                    worksheet.Cells[11, columnIndex].Value = dayLabel;
                    worksheet.Cells[11, columnIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[11, columnIndex].Style.Font.Size = 12;
                    worksheet.Cells[11, columnIndex].Style.Font.Bold = true;
                    worksheet.Cells[11, columnIndex].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);

                    worksheet.Cells[12, columnIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells[13, columnIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells[13, columnIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    var openDay = dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday;

                    Color background = openDay ? Color.FromArgb(1, 192, 192, 192) : Color.FromArgb(1, 214, 227, 188);

                    worksheet.Cells[13, columnIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[13, columnIndex].Style.Fill.BackgroundColor.SetColor(background);

                    Color bg = openDay ? Color.FromArgb(1, 192, 192, 192) : Color.FromArgb(1, 255, 255, 255);

                    if (!openDay)
                    {
                        ++totalOpenDay;
                    }

                    worksheet.Cells[14, columnIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[14, columnIndex].Style.Fill.BackgroundColor.SetColor(bg);
                    worksheet.Cells[14, columnIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    worksheet.Cells[15, columnIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[15, columnIndex].Style.Fill.BackgroundColor.SetColor(bg);
                    worksheet.Cells[15, columnIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    for (int i = 17; i < 24; i++)
                    {
                        worksheet.Cells[i, columnIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[i, columnIndex].Style.Fill.BackgroundColor.SetColor(bg);
                        worksheet.Cells[i, columnIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    string value = "";
                    switch (item.WorkedPart)
                    {
                        case DayPartEnum.Morning:
                        case DayPartEnum.Afternoon:
                            value = "0,5";
                            break;
                        case DayPartEnum.Full:
                            value = "1";
                            break;
                    }

                    worksheet.Cells[13, columnIndex].Value = value;

                    if (!(item.Absence is null))
                    {
                        worksheet.Cells[19, columnIndex].Value = "1";
                    }

                    ++index;
                }

                worksheet.Cells[10, memorizedIndex, 10, index + 2].Merge = true;


                worksheet.Cells[12, 3, 15, index + 2].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[17, 3, 23, index + 2].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);

                worksheet.Cells[30, 1].Value = "Total Nb jour ouvrés / Mois";
                worksheet.Cells[30, 1].Style.Font.Bold = true;
                worksheet.Cells[30, 1].Style.Font.Size = 12;
                worksheet.Cells[30, 1, 30, 2].Merge = true;

                worksheet.Cells[30, 3].Value = totalOpenDay;
                worksheet.Cells[30, 3].Style.Font.Size = 12;
                worksheet.Cells[30, 3].Style.Font.Bold = true;

                worksheet.Cells[25, 2].Value = "Commentaire";
                worksheet.Cells[25, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[25, 2].Style.Font.Bold = true;
                worksheet.Cells[25, 2].Style.Font.Size = 12;

                worksheet.Cells[26, 2].Value = "(explications selon le cas)";
                worksheet.Cells[26, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[26, 2].Style.Font.Bold = true;
                worksheet.Cells[26, 2].Style.Font.Size = 12;

                var totalWorkedDays = activityDto.Days.Sum(d =>
                {
                    double value = 0.0;
                    switch (d.WorkedPart)
                    {
                        case DayPartEnum.Morning:
                        case DayPartEnum.Afternoon:
                            value = 0.5;
                            break;
                        case DayPartEnum.Full:
                            value = 1;
                            break;
                    }
                    return value;
                });

                worksheet.Cells[29, index - 6].Value = "NOMBRE DE JOURS CLIENT";
                worksheet.Cells[29, index - 6].Style.Font.Size = 14;
                worksheet.Cells[29, index - 6].Style.Font.Bold = true;
                worksheet.Cells[29, index - 6, 29, index + 3].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);

                worksheet.Cells[29, index + 3].Value = totalWorkedDays;
                worksheet.Cells[29, index + 3].Style.Font.Size = 14;
                worksheet.Cells[29, index + 3].Style.Font.Bold = true;
                worksheet.Cells[29, index - 6, 29, index + 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[29, index - 6, 29, index + 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(1, 234, 241, 221));

                worksheet.Cells[25, 3, 28, 21].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                worksheet.Cells[25, 3, 28, 21].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[25, 3, 28, 21].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(1, 204, 204, 255));

                worksheet.Cells[13, index + 3].Value = totalWorkedDays;
                worksheet.Cells[13, index + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[13, index + 3].Style.Font.Bold = true;
                worksheet.Cells[13, index + 3].Style.Font.Size = 12;
                worksheet.Cells[13, index + 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[13, index + 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(1, 255, 128, 128));
                worksheet.Cells[13, index + 3].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);

                //worksheet.Cells[32, 1].Value = "Date du jour ";
                //worksheet.Cells[32, 1].Style.Font.Size = 14;
                //worksheet.Cells[32, 2].Value = DateTime.Now.ToString();

                worksheet.Cells[32, 3, 40, 21].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                var consultantSignatureCell = worksheet.Cells[39, 10, 40, 14];
                consultantSignatureCell.Merge = true;
                consultantSignatureCell.Value = $"Signé par {activityDto.Consultant.Firstname} {activityDto.Consultant.Lastname} le {activityDto.Consultant.SignatureDate?.ToShortDateString()}";
                consultantSignatureCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                consultantSignatureCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                if (activityDto.Customer != null)
                {
                    var customerSignatureCell = worksheet.Cells[39, 4, 40, 8];
                    customerSignatureCell.Merge = true;
                    customerSignatureCell.Value = $"Signé par {activityDto.Customer.Firstname} {activityDto.Customer.Lastname} le {activityDto.Customer.SignatureDate?.ToShortDateString()}";
                    customerSignatureCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    customerSignatureCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                //var imageUrl = activityDto.Customer?.SignatureUri;
                //if (!string.IsNullOrEmpty(imageUrl))
                //{
                //    var filename = activityDto.Customer.SignatureUri.Split('/').LastOrDefault();
                //    var myTempFile = Path.Combine(Path.GetTempPath(), filename);
                //    new WebClient().DownloadFile(imageUrl, myTempFile);
                //    var img = new FileInfo(myTempFile);
                //    var signature = worksheet.Drawings.AddPicture("customer-signature.png", img);
                //    signature.SetPosition(32, 0, 4, 0);
                //    signature.SetSize(200, 100);
                //    File.Delete(myTempFile);

                //    var customerSignatureCell = worksheet.Cells[39, 4, 40, 8];
                //    customerSignatureCell.Merge = true;
                //    customerSignatureCell.Value = $"Signé par {activityDto.Customer.Firstname} {activityDto.Customer.Lastname} le {activityDto.Customer.SignatureDate?.ToShortDateString()}";
                //    customerSignatureCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //    customerSignatureCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //}

                //var consultantSignatureUrl = activityDto.Consultant?.SignatureUri;
                //if (!string.IsNullOrEmpty(consultantSignatureUrl))
                //{
                //    var filename = activityDto.Consultant.SignatureUri.Split('/').LastOrDefault();
                //    var myTempFile = Path.Combine(Path.GetTempPath(), filename);
                //    try
                //    {
                //        new WebClient().DownloadFile(consultantSignatureUrl, myTempFile);
                //    }
                //    catch
                //    {
                //        return package.GetAsByteArray();
                //    }

                //    var img = new FileInfo(myTempFile);
                //    var signature = worksheet.Drawings.AddPicture("consultant-signature.png", img);
                //    signature.SetPosition(32, 0, 10, 0);
                //    signature.SetSize(200, 100);
                //    File.Delete(myTempFile);

                //    var consultantSignatureCell = worksheet.Cells[39, 10, 40, 14];
                //    consultantSignatureCell.Merge = true;
                //    consultantSignatureCell.Value = $"Signé par {activityDto.Consultant.Firstname} {activityDto.Consultant.Lastname} le {activityDto.Consultant.SignatureDate?.ToShortDateString()}";
                //    consultantSignatureCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //    consultantSignatureCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //}

                return package.GetAsByteArray();
            }
        }
    }
}
