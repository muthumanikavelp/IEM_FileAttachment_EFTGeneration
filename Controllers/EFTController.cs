using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using Excel;
using ClosedXML.Excel;
using System.IO;
using Upload.Models;
using System.Text;
using System.Web.Mvc; 
namespace Upload.Controllers
{
    public class EFTController : ApiController
    {
        proLib plib = new proLib();
        EFTModel objmod = new EFTModel();

        //private readonly HtmlViewRenderer htmlViewRenderer;
        //Downloading Notepad File.. when we hit this action.
                
        public string GenerateDocuments(EFTDataModel objmodel)
        {
            try
            {
                string IsFileGenerated = "";
                string _Date = "", _SerialNo = "", _PayMode = "", _ViewType = "", _BankName = "";
                _PayMode = objmodel.Id;
                _ViewType = objmodel.subId == "" || objmodel.subId == null ? "0" : objmodel.subId;

                //string PvIds = "";
                //PvIds = TempData["PVIds"] != null ? TempData["PVIds"].ToString() : "";
                DataSet ds = objmod.PrintEFTMemoDetails(objmodel.PvIds, _PayMode, _ViewType, objmodel.PayBankGId, objmodel.LoginUserId);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        _Date = ds.Tables[0].Rows[0]["CurrentDate"].ToString();
                        _SerialNo = ds.Tables[0].Rows[0]["SerialNo"].ToString();
                        if (_PayMode.ToLower().Trim() != "rrp")
                            _BankName = ds.Tables[0].Rows[0]["PayBankname"].ToString();
                    }

                    if (_Date != string.Empty && _SerialNo != string.Empty)
                    {
                        IsFileGenerated = objmod.IsCheckFolder(_Date, _SerialNo, _PayMode, _ViewType, _BankName, "");
                    }

                    //Generate Online Template for EFT Template.
                    if (_PayMode.ToLower().Trim() == "eft" && _ViewType == "0" && IsFileGenerated != string.Empty)
                    {
                        string _NormalFile = "", _EncryptedFile = "";
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            _NormalFile = string.Format("{0}/{1}.txt", IsFileGenerated, ds.Tables[0].Rows[0]["NormalTextFile"].ToString());

                            string _encrypt = plib.EncryptMemoDownloadUrl;
                            DirectoryInfo dir = new DirectoryInfo(_encrypt);

                            //check folder Presence
                            if (dir.Exists)
                            {
                                _EncryptedFile = string.Format("{0}/{1}~{2}~Online~{3}.txt", _encrypt, _Date, _SerialNo, ds.Tables[0].Rows[0]["Encrypted"].ToString());
                            }
                            else
                            {
                                _EncryptedFile = "";
                            }
                        }

                        objmod.GenerateNotePadFile(_NormalFile, _EncryptedFile, ds.Tables[1]);
                    }

                    //Ramya added for Employee Claim
                    //Generate Online Template for EFT Template.
                    if (_PayMode.ToLower().Trim() == "era" && _ViewType == "0" && IsFileGenerated != string.Empty)
                    {
                        string _NormalFile = "", _EncryptedFile = "";
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            _NormalFile = string.Format("{0}/{1}.txt", IsFileGenerated, ds.Tables[0].Rows[0]["NormalTextFile"].ToString());

                            string _encrypt = plib.EncryptMemoDownloadUrl;
                            DirectoryInfo dir = new DirectoryInfo(_encrypt);

                            //check folder Presence
                            if (dir.Exists)
                            {
                                _EncryptedFile = string.Format("{0}/{1}~{2}~Online~{3}.txt", _encrypt, _Date, _SerialNo, ds.Tables[0].Rows[0]["Encrypted"].ToString());
                            }
                            else
                            {
                                _EncryptedFile = "";
                            }
                        }

                        objmod.GenerateNotePadFile(_NormalFile, _EncryptedFile, ds.Tables[1]);
                    }

                    //EFT Template Work [RTGS/NEFT]
                    if (_PayMode.ToLower().Trim() == "eft" && _ViewType == "1")
                    {
                        XLWorkbook xl = null;
                        if (ds.Tables[1].Rows.Count > 0)
                        {

                            DataTable dt = new DataTable();
                            dt.Columns.Add("SL No", typeof(string));
                            dt.Columns.Add("Name of Vendor", typeof(string));
                            dt.Columns.Add("Account No", typeof(string));
                            dt.Columns.Add("Bank Name", typeof(string));
                            dt.Columns.Add("IFSC Code", typeof(string));
                            dt.Columns.Add("Amount", typeof(string));
                            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                            {
                                dt.Rows.Add(new object[] { i + 1, ds.Tables[1].Rows[i]["BenName"].ToString(), ds.Tables[1].Rows[i]["BenAccNo"].ToString(), ds.Tables[1].Rows[i]["BenBankname"].ToString(), ds.Tables[1].Rows[i]["BenBankIfscCode"].ToString(), ds.Tables[1].Rows[i]["Amount"].ToString() });
                            }

                            MemoDetail result = new MemoDetail();
                            result = objmod.GetMemoDetailsPrint(ds.Tables[1], ds.Tables[1]);

                            IsFileGenerated = objmod.IsCheckFolder(_Date, _SerialNo, _PayMode, _ViewType, _BankName, "NEFT");

                            xl = new XLWorkbook();
                            xl.Worksheets.Add(dt, "HDFC_NEFT");
                            xl.SaveAs(string.Format("{0}/HDFC_NEFT.xlsx", IsFileGenerated));

                            //save the DD Content to local folder.
                            string fileName = "";
                            fileName = string.Format("{0}/NEFT.html", IsFileGenerated);

                            // Render the view html to a string.
                            //string htmlText = this.htmlViewRenderer.RenderViewToString(this, "TmpRTGS_HDFC_A_NEW", result);
                            string htmlText = objmod.TmpRTGS_HDFC_A_NEW(result); 
                             
                            //save the document as html.
                            using (FileStream fs = new FileStream(fileName, FileMode.Create))
                            {
                                using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                                {
                                    w.Write(htmlText);
                                }
                            }
                            // Let the html be rendered into a PDF document through iTextSharp.
                            //byte[] buffer = standardPdfRenderer.Render(htmlText, "");

                            //using (FileStream fs = new FileStream(fileName, FileMode.Create))
                            //{
                            //    fs.Write(buffer, 0, buffer.Length);
                            //}
                        }

                        if (ds.Tables[2].Rows.Count > 0)
                        {

                            DataTable dt = new DataTable();
                            dt.Columns.Add("SL No", typeof(string));
                            dt.Columns.Add("Name of Vendor", typeof(string));
                            dt.Columns.Add("Account No", typeof(string));
                            dt.Columns.Add("Bank Name", typeof(string));
                            dt.Columns.Add("IFSC Code", typeof(string));
                            dt.Columns.Add("Amount", typeof(string));
                            for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
                            {
                                dt.Rows.Add(new object[] { i + 1, ds.Tables[2].Rows[i]["BenName"].ToString(), ds.Tables[2].Rows[i]["BenAccNo"].ToString(), ds.Tables[2].Rows[i]["BenBankname"].ToString(), ds.Tables[2].Rows[i]["BenBankIfscCode"].ToString(), ds.Tables[2].Rows[i]["Amount"].ToString() });
                            }

                            MemoDetail result = new MemoDetail();
                            result = objmod.GetMemoDetailsPrint(ds.Tables[2], ds.Tables[2]);

                            IsFileGenerated = objmod.IsCheckFolder(_Date, _SerialNo, _PayMode, _ViewType, _BankName, "RTGS");

                            //save the DD Content to local folder.
                            string fileName = "";
                            fileName = string.Format("{0}/RTGS.html", IsFileGenerated);

                            xl = new XLWorkbook();
                            xl.Worksheets.Add(dt, "HDFC_NEFT");
                            xl.SaveAs(string.Format("{0}/HDFC_NEFT.xlsx", IsFileGenerated));

                            // Render the view html to a string.
                            //string htmlText = this.htmlViewRenderer.RenderViewToString(this, "TmpRTGS_HDFC_A_NEW", result);
                            string htmlText = objmod.TmpRTGS_HDFC_A_NEW(result);
                            
                            //save the document as html.
                            using (FileStream fs = new FileStream(fileName, FileMode.Create))
                            {
                                using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                                {
                                    w.Write(htmlText);
                                }
                            }
                            // Let the html be rendered into a PDF document through iTextSharp.
                            //byte[] buffer = standardPdfRenderer.Render(htmlText, "");

                            //using (FileStream fs = new FileStream(fileName, FileMode.Create))
                            //{
                            //    fs.Write(buffer, 0, buffer.Length);
                            //}
                        }
                    }

                    //DD Template Work
                    if (_PayMode.ToLower().Trim() == "dd" && IsFileGenerated != string.Empty)
                    {
                        XLWorkbook xl = null;
                        DataTable dt = new DataTable();
                        dt.Columns.Add("SL No", typeof(string));
                        dt.Columns.Add("DD Favoring", typeof(string));
                        dt.Columns.Add("Payable at", typeof(string));
                        dt.Columns.Add("Amount", typeof(string));

                        for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                        {
                            dt.Rows.Add(new object[] { i + 1, ds.Tables[1].Rows[i]["DDFavoring"].ToString(), ds.Tables[1].Rows[i]["PayableAt"].ToString(), ds.Tables[1].Rows[i]["Amount"].ToString() });
                        }
                        DDTemplate result = new DDTemplate();
                        result = objmod.GetDDTemplate(ds.Tables[0], ds.Tables[1]);

                        xl = new XLWorkbook();
                        xl.Worksheets.Add(dt, "HDFC_DD");
                        xl.SaveAs(string.Format("{0}/HDFC_DD.xlsx", IsFileGenerated));

                        //save the DD Content to local folder.
                        string fileName = "";
                        fileName = string.Format("{0}/HDFC_DD.html", IsFileGenerated);

                        // Render the view html to a string.
                        //string htmlText = this.htmlViewRenderer.RenderViewToString(this, "TmpDD_HDFC", result);
                        string htmlText = objmod.TmpDD_HDFC(result);
                        
                        //save the document as html.
                        using (FileStream fs = new FileStream(fileName, FileMode.Create))
                        {
                            using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                            {
                                w.Write(htmlText);
                            }
                        }
                        // Let the html be rendered into a PDF document through iTextSharp.
                        //byte[] buffer = standardPdfRenderer.Render(htmlText, "");

                        //using (FileStream fs = new FileStream(fileName, FileMode.Create))
                        //{
                        //    fs.Write(buffer, 0, buffer.Length);
                        //}
                    }

                    if (_PayMode.ToLower().Trim() == "rrp" && IsFileGenerated != string.Empty)
                    {
                        XLWorkbook xl = null;
                        DataTable dt = new DataTable();
                        dt.Columns.Add("SL No", typeof(string));
                        dt.Columns.Add("Pay Number", typeof(string));
                        dt.Columns.Add("Employee Code", typeof(string));
                        dt.Columns.Add("Employee Name", typeof(string));
                        dt.Columns.Add("Location", typeof(string));
                        dt.Columns.Add("Amount", typeof(string));
                        dt.Columns.Add("ECF No", typeof(string));

                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            dt.Rows.Add(new object[] { i + 1, ds.Tables[0].Rows[i]["PVNo"].ToString(), ds.Tables[0].Rows[i]["EmployeeSupplierCode"].ToString(), ds.Tables[0].Rows[i]["EmployeeSupplierName"].ToString()
                        , ds.Tables[0].Rows[i]["Location"].ToString(), ds.Tables[0].Rows[i]["Amount"].ToString(), ds.Tables[0].Rows[i]["ECFNo"].ToString()});
                        }

                        xl = new XLWorkbook();
                        xl.Worksheets.Add(dt, "RRP");
                        xl.SaveAs(string.Format("{0}/" + @System.Configuration.ConfigurationManager.AppSettings["CompanyName"].ToString() + "-RRP-REG-" + _Date.Replace("-", "") + _SerialNo + ".xlsx", IsFileGenerated));
                    }

                    //ERA Template Work
                    if (_PayMode.ToLower().Trim() == "era" && IsFileGenerated != string.Empty)
                    {
                        string _ViewName = "";
                        XLWorkbook xl = null;
                        ERATemplate result = new ERATemplate();

                        result = objmod.GetERATemplate(ds.Tables[0], ds.Tables[2]);

                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && result != null)
                        {

                            if (_BankName.ToLower().Contains("citi"))
                            {
                                _ViewName = "TmpDT_CITI";
                            }

                            if (_BankName.ToLower().Contains("hdfc"))
                            {
                                _ViewName = "TmpDT_HDFC_ERA";
                            }

                            if (_BankName.ToLower().Contains("icici"))
                            {
                                _ViewName = "TmpDT_ICICI";
                            }
                            if (_BankName.ToLower().Contains("sbi") || _BankName.ToUpper().Contains("STATE BANK OF INDIA"))
                            {
                                _ViewName = "TmpDT_SBI";
                            }
                            if (_BankName.ToLower().Contains("uti") || _BankName.ToUpper().Contains("AXIS"))
                            {
                                _ViewName = "TmpDT_UTI";
                            }
                            if (_ViewName == "")
                            {
                                _ViewName = "TmpDT_HDFC_ERA";
                            }
                        }
                        if (_ViewName != "")
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.Add("SL No", typeof(string));
                            dt.Columns.Add("Employee Code", typeof(string));
                            dt.Columns.Add("Employee Name", typeof(string));
                            dt.Columns.Add("Account No", typeof(string));
                            dt.Columns.Add("Amount", typeof(string));
                            //RAMYA for IFSC Code
                            dt.Columns.Add("IFSC Code", typeof(string));
                            for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
                            {
                                dt.Rows.Add(new object[] { i + 1, ds.Tables[2].Rows[i]["EmployeeSupplierCode"].ToString(), ds.Tables[2].Rows[i]["EmployeeSupplierName"].ToString(), ds.Tables[2].Rows[i]["AccNo"].ToString(), ds.Tables[2].Rows[i]["Amount"].ToString(), ds.Tables[2].Rows[i]["IFSCCode"].ToString() });
                            }
                            //save the DD Content to local folder.
                            string fileName = "";
                            string htmlText = "";
                            if (_ViewName == "TmpDT_CITI")
                            {
                                fileName = string.Format("{0}/CITI.html", IsFileGenerated);
                                xl = new XLWorkbook();
                                xl.Worksheets.Add(dt, "CITI");
                                xl.SaveAs(string.Format("{0}/CITI.xlsx", IsFileGenerated));
                                htmlText = objmod.TmpDT_CITI(result);
                                
                            }
                            else if (_ViewName == "TmpDT_HDFC_ERA")
                            {
                                fileName = string.Format("{0}/HDFC.html", IsFileGenerated);
                                xl = new XLWorkbook();
                                xl.Worksheets.Add(dt, "HDFC");
                                xl.SaveAs(string.Format("{0}/HDFC.xlsx", IsFileGenerated));
                                htmlText = objmod.TmpDT_HDFC_ERA(result);
                                
                            }
                            else if (_ViewName == "TmpDT_ICICI")
                            {
                                fileName = string.Format("{0}/ICICI.html", IsFileGenerated);
                                xl = new XLWorkbook();
                                xl.Worksheets.Add(dt, "ICICI");
                                xl.SaveAs(string.Format("{0}/ICICI.xlsx", IsFileGenerated));
                                htmlText = objmod.TmpDT_ICICI(result);
                            }
                            else if (_ViewName == "TmpDT_SBI")
                            {
                                fileName = string.Format("{0}/SBIDT.txt", IsFileGenerated);
                                objmod.GenerateNotePad(fileName, ds.Tables[1]);
                                fileName = string.Format("{0}/SBI.html", IsFileGenerated);
                                xl = new XLWorkbook();
                                xl.Worksheets.Add(dt, "SBI");
                                xl.SaveAs(string.Format("{0}/SBI.xlsx", IsFileGenerated));
                                htmlText = objmod.TmpDT_SBI(result);
                            }
                            else if (_ViewName == "TmpDT_UTI")
                            {
                                fileName = string.Format("{0}/UTI.html", IsFileGenerated);
                                xl = new XLWorkbook();
                                xl.Worksheets.Add(dt, "UTI");
                                xl.SaveAs(string.Format("{0}/UTI.xlsx", IsFileGenerated));
                                htmlText = objmod.TmpDT_UTI(result);
                            }


                            // Render the view html to a string.
                            //string htmlText = this.htmlViewRenderer.RenderViewToString(this, _ViewName, result);

                            //save the document as html.
                            using (FileStream fs = new FileStream(fileName, FileMode.Create))
                            {
                                using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                                {
                                    w.Write(htmlText);
                                }
                            }

                        }
                        // Let the html be rendered into a PDF document through iTextSharp.
                        //byte[] buffer = standardPdfRenderer.Render(htmlText, "");

                        //using (FileStream fs = new FileStream(fileName, FileMode.Create))
                        //{
                        //    fs.Write(buffer, 0, buffer.Length);
                        //}
                    }
                }
                //return Json("OK", JsonRequestBehavior.AllowGet);
                return "OK";
            }
            catch(Exception ex)
            {
                return ex.Message.ToString();
            }
        }
         

    }
}
