using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Text;
namespace Upload.Models
{
    public class EFTModel : sqlLib
    {
        proLib plib = new proLib();
        public DataSet PrintEFTMemoDetails(string PvIds, string Paymode, string ViewType, string PayBankGId, string LoginUserId)
        {
            ProcedureName = "PR_FS_Get_OnlineMemoFilePrint";
            AddParameter("PvIds", PvIds);
            AddParameter("Paymode", Paymode);
            AddParameter("ViewType", ViewType);
            AddParameter("PayBankGId", PayBankGId);
            AddParameter("UId", LoginUserId);
            return ExecuteProcedure;
        }

        public void GenerateNotePadFile(string NormalFile, string Encrypted, DataTable dt)
        {
            //string delemited = ",";

            //NORMAL FILE -- Create Notepad
            //clear the previous data's
            FileStream fs = new FileStream(NormalFile, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter _fsstr = new StreamWriter(fs);
            _fsstr.WriteLine(string.Empty);
            _fsstr.Flush(); _fsstr.Close();

            //update the notepad content for downloading.
            if (dt.Rows.Count > 0)
            {
                StreamWriter str = new StreamWriter(NormalFile, false, System.Text.Encoding.Default);
                foreach (DataRow datarow in dt.Rows)
                {
                    string row = string.Empty;
                    row = datarow["TransactionType"].ToString() + "," + datarow["SupplierCode"].ToString() + "," + datarow["AccNo"].ToString() + ","
                        + datarow["PVAmount"].ToString() + "," + datarow["SupplierName"].ToString() + ",,,,,,,,," + datarow["PVNo"].ToString() + ",,,,,,,,,"
                        + datarow["FileSpoolingDate"].ToString() + ",," + datarow["IFSCCode"].ToString() + "," + datarow["Bankname"].ToString() + ",,";
                    str.WriteLine(row);
                }
                str.Flush();
                str.Close();
            }

            //ENCRYPT FILE -- Create Notepad
            //clear the previous data's
            if (Encrypted != "")
            {
                FileStream efs = new FileStream(Encrypted, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter _efsstr = new StreamWriter(efs);
                _efsstr.WriteLine(string.Empty);
                _efsstr.Flush(); _efsstr.Close();

                //update the notepad content for downloading.
                if (dt.Rows.Count > 0)
                {
                    StreamWriter str = new StreamWriter(Encrypted, false, System.Text.Encoding.Default);
                    foreach (DataRow datarow in dt.Rows)
                    {
                        string row = string.Empty;
                        row = datarow["TransactionType"].ToString() + "," + datarow["SupplierCode"].ToString() + "," + datarow["AccNo"].ToString() + ","
                            + datarow["PVAmount"].ToString() + "," + datarow["SupplierName"].ToString() + ",,,,,,,,," + datarow["PVNo"].ToString() + ",,,,,,,,,"
                            + datarow["FileSpoolingDate"].ToString() + ",," + datarow["IFSCCode"].ToString() + "," + datarow["Bankname"].ToString() + ",,";
                        str.WriteLine(row);
                    }
                    str.Flush();
                    str.Close();
                }
            }
        }

        public void GenerateNotePad(string NormalFile, DataTable dt)
        {
            FileStream fs = new FileStream(NormalFile, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter _fsstr = new StreamWriter(fs);
            _fsstr.WriteLine(string.Empty);
            _fsstr.Flush(); _fsstr.Close();

            //update the notepad content for downloading.
            if (dt.Rows.Count > 0)
            {
                StreamWriter str = new StreamWriter(NormalFile, false, System.Text.Encoding.Default);
                foreach (DataRow datarow in dt.Rows)
                {
                    string row = string.Empty;
                    row = datarow["AccNo"].ToString() + "          " + datarow["Amount"].ToString();
                    str.WriteLine(row);
                }
                str.Flush();
                str.Close();
            }
        }
        public string IsCheckFolder(string Date, string SerialNo, string PayMode, string SubFolder, string BankName, string Mode)
        {
            try
            {
                bool _MFolder = false, _DFolder = false, _SerFolder = false, _SBFolder = false, _SBFolder1 = false, _SUBFolder2 = false;
                string _mainFolder = "", _dateFolder = "", _serialFolder = "", _subFolder = "", _subFolder1 = "", _subFolder2 = "";


                //Create Root Folder
                _mainFolder = plib.DownloadMemoUrl;
                _MFolder = FolderCreation(_mainFolder);

                //Create Date Folder
                _dateFolder = string.Format("{0}/{1}", _mainFolder, Date);
                _DFolder = FolderCreation(_dateFolder);

                //Create Serial No Folder
                _serialFolder = string.Format("{0}/{1}", _dateFolder, SerialNo);
                _SerFolder = FolderCreation(_serialFolder);

                if ((PayMode.ToLower().Trim() == "eft" || PayMode.ToLower().Trim() == "era") && SubFolder == "0")
                {
                    _subFolder1 = string.Format("{0}/{1}", _serialFolder, "Online");
                    _SBFolder1 = FolderCreation(_subFolder1);
                }
                else if (PayMode.ToLower().Trim() == "dd")
                {
                    _subFolder1 = string.Format("{0}/{1}", _serialFolder, "DD");
                    _SBFolder1 = FolderCreation(_subFolder1);
                }
                else if (PayMode.ToLower().Trim() == "rrp")
                {
                    _subFolder1 = string.Format("{0}/{1}", _serialFolder, "RRP");
                    _SBFolder1 = FolderCreation(_subFolder1);
                }
                else
                {
                    _subFolder = string.Format("{0}/{1}", _serialFolder, "MEMO");
                    _SBFolder = FolderCreation(_subFolder);

                    //Create Memo Folder
                    _subFolder2 = string.Format("{0}/{1}", _subFolder, PayMode);
                    _SUBFolder2 = FolderCreation(_subFolder2);

                    if (PayMode.ToLower().Trim() == "era")
                    {
                        if (BankName.ToLower().Contains("citi"))
                        {
                            _subFolder1 = string.Format("{0}/{1}", _subFolder2, "CITI");
                            _SBFolder1 = FolderCreation(_subFolder1);
                        }
                        else if (BankName.ToLower().Contains("icici"))
                        {
                            _subFolder1 = string.Format("{0}/{1}", _subFolder2, "ICICI");
                            _SBFolder1 = FolderCreation(_subFolder1);
                        }
                        else if (BankName.ToLower().Contains("sbi") || BankName.ToUpper().Contains("STATE BANK OF INDIA"))
                        {
                            _subFolder1 = string.Format("{0}/{1}", _subFolder2, "SBI");
                            _SBFolder1 = FolderCreation(_subFolder1);
                        }
                        else if (BankName.ToLower().Contains("uti") || BankName.ToUpper().Contains("AXIS"))
                        {
                            _subFolder1 = string.Format("{0}/{1}", _subFolder2, "AXIS");
                            _SBFolder1 = FolderCreation(_subFolder1);
                        }
                        else
                        {
                            _subFolder1 = string.Format("{0}/{1}", _subFolder2, "HDFC");
                            _SBFolder1 = FolderCreation(_subFolder1);
                        }
                    }
                    else if (PayMode.ToLower().Trim() == "eft")
                    {
                        if (Mode.ToLower().Contains("rtgs"))
                        {
                            _subFolder1 = string.Format("{0}/{1}", _subFolder2, "HDFC - RTGS");
                            _SBFolder1 = FolderCreation(_subFolder1);
                        }
                        else if (Mode.ToLower().Contains("neft"))
                        {
                            _subFolder1 = string.Format("{0}/{1}", _subFolder2, "HDFC - NEFT");
                            _SBFolder1 = FolderCreation(_subFolder1);
                        }
                    }
                }

                return _subFolder1;
            }
            catch
            {
                return "";
            }
        }


        public bool FolderCreation(string FolderName)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(FolderName);

                //check folder Presence
                if (!dir.Exists)
                {
                    dir.Create();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public DDTemplate GetDDTemplate(DataTable dtDet, DataTable dtRem)
        {
            DDTemplate rec = new DDTemplate();
            DataTable dt = null;
            if (dtDet != null)
            {
                if (dtDet.Rows.Count > 0)
                {
                    rec.Date = dtDet.Rows[0]["Date"].ToString();
                    rec.LetterNo = dtDet.Rows[0]["MemoNo"].ToString();
                    rec.BankAddress = dtDet.Rows[0]["BankAddress"].ToString();
                    rec.CompanyAccountNo = dtDet.Rows[0]["CompanyAccNo"].ToString();
                    //kavitha for dd favor ,pay at shows blank
                    rec.DDFavoring = dtDet.Rows[0]["DDFavoring"].ToString();
                    rec.PayableAt = dtDet.Rows[0]["PayableAt"].ToString();
                    rec.Amount = dtDet.Rows[0]["Amount"].ToString();
                    rec.AmountInWords = dtDet.Rows[0]["AmountInWords"].ToString();

                    dt = dtRem;//LoadChildList(dtRem, dtDet.Rows[i]["PvId"].ToString());

                    List<DDInnerDetails> childArray = new List<DDInnerDetails>();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow cdr in dt.Rows)
                        {
                            DDInnerDetails child = new DDInnerDetails();
                            child.DDFavoring = cdr["DDFavoring"].ToString();
                            child.PayableAt = cdr["PayableAt"].ToString();
                            child.Amount = cdr["Amount"].ToString();
                            childArray.Add(child);
                        }
                        rec.Totalamount = childArray.Sum(item => Convert.ToDecimal(item.Amount));
                    }
                    rec.DetailArray = childArray;
                }
            }
            return rec;
        }

        public ERATemplate GetERATemplate(DataTable dtDet, DataTable dtRem)
        {
            ERATemplate rec = new ERATemplate();
            DataTable dt = null;
            if (dtDet != null)
            {
                if (dtDet.Rows.Count > 0)
                {
                    rec.Date = dtDet.Rows[0]["Date"].ToString();
                    rec.LetterNo = dtDet.Rows[0]["MemoNo"].ToString();
                    rec.BankAddress = dtDet.Rows[0]["BankAddress"].ToString();
                    rec.CompanyAccountNo = dtDet.Rows[0]["CompanyAccNo"].ToString();
                    rec.CompanyAccountNo = dtDet.Rows[0]["CompanyAccNo"].ToString();
                    rec.KindAttan = dtDet.Rows[0]["KindAttan"].ToString();
                    rec.Amount = dtDet.Rows[0]["Amount"].ToString();
                    rec.AmountInWords = dtDet.Rows[0]["AmountInWords"].ToString();

                    dt = dtRem;
                    List<ERAInnerDetails> childArray = new List<ERAInnerDetails>();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow cdr in dt.Rows)
                        {
                            ERAInnerDetails child = new ERAInnerDetails();
                            child.VendorCode = cdr["EmployeeSupplierCode"].ToString();
                            child.NameOfVendor = cdr["EmployeeSupplierName"].ToString();
                            child.BankAccountNo = cdr["AccNo"].ToString();
                            child.IFSCCode = cdr["IFSCCode"].ToString();
                            child.Amount = cdr["Amount"].ToString();
                            child.RemittanceDetails = cdr["RemitterDetails"].ToString();
                            childArray.Add(child);
                        }
                    }
                    rec.DetailArray = childArray;
                }
            }
            return rec;
        }

        public MemoDetail GetMemoDetailsPrint(DataTable dtDet, DataTable dtRem)
        {
            MemoDetail rec = new MemoDetail();
            DataTable dt = null;
            if (dtDet != null)
            {

                //Top Details
                rec.Mode = dtDet.Rows[0]["TransactionType"].ToString();
                rec.AccountType = dtDet.Rows[0]["AccountType"].ToString();
                rec.BranchDetails = dtDet.Rows[0]["branchCode"].ToString() != "" ? dtDet.Rows[0]["branchCode"].ToString() : dtDet.Rows[0]["branchName"].ToString();
                rec.MemoNo = dtDet.Rows[0]["MemoNo"].ToString();
                rec.MemoTime = dtDet.Rows[0]["Time"].ToString();
                rec.MemoDate = dtDet.Rows[0]["Date"].ToString();

                //Beneficiary Details
                if (dtRem.Rows.Count > 1)
                {
                    rec.BenName = "AS PER LIST";
                    rec.BenAccNo = "AS PER LIST";
                    rec.BenAddress = "AS PER LIST";
                    rec.BenBankname = "AS PER LIST";
                    rec.BenBankIfscCode = "AS PER LIST";
                    rec.AmountInWords = "AS PER LIST";
                    rec.AmountInFigures = "AS PER LIST";
                }
                else
                {
                    rec.BenName = dtDet.Rows[0]["BenName"].ToString();
                    rec.BenAccNo = dtDet.Rows[0]["BenAccNo"].ToString();
                    rec.BenAddress = dtDet.Rows[0]["BenAddress"].ToString();
                    rec.BenBankname = dtDet.Rows[0]["BenBankname"].ToString();
                    rec.BenBankIfscCode = dtDet.Rows[0]["BenBankIfscCode"].ToString();
                    rec.AmountInWords = dtDet.Rows[0]["AmountInWords"].ToString();
                    rec.AmountInFigures = dtDet.Rows[0]["Amount"].ToString();
                }

                //Our Details
                if (dtRem.Rows.Count > 1)
                {
                    rec.RemitterName = "AS PER LIST";
                    rec.RemitterAccNo = "AS PER LIST";
                    rec.RemitterCashDeposited = "AS PER LIST";
                    rec.RemitterMobilePhoneNo = "AS PER LIST";
                    rec.RemitterEmailId = "AS PER LIST";
                    rec.RemitterAddress = "AS PER LIST";
                    rec.Remarks = "AS PER LIST";
                }
                else
                {
                    rec.RemitterName = dtDet.Rows[0]["RemitterName"].ToString();
                    rec.RemitterAccNo = dtDet.Rows[0]["RemitterAccNo"].ToString();
                    rec.RemitterCashDeposited = dtDet.Rows[0]["RemitterCashDeposited"].ToString();
                    rec.RemitterMobilePhoneNo = dtDet.Rows[0]["RemitterMobilePhoneNo"].ToString();
                    rec.RemitterEmailId = dtDet.Rows[0]["RemitterEmailId"].ToString();
                    rec.RemitterAddress = dtDet.Rows[0]["RemitterAddress"].ToString();
                    rec.Remarks = dtDet.Rows[0]["Remarks"].ToString();
                }


                /*rec.TotalAmount = dtDet.Rows[i]["TotAmount"].ToString();
                rec.Amount = dtDet.Rows[i]["Amount"].ToString();
                rec.BatchNo = dtDet.Rows[i]["BatchNo"].ToString();
                rec.PvId = dtDet.Rows[i]["PvId"].ToString();
                rec.EmployeeSupplierCode = dtDet.Rows[i]["EmployeeSupplierCode"].ToString();
                rec.EmployeeSupplierName = dtDet.Rows[i]["EmployeeSupplierName"].ToString();
                rec.RemitterDetails = dtDet.Rows[i]["RemitterDetails"].ToString();
                rec.RemitterCode = dtDet.Rows[i]["RemitterCode"].ToString();
                rec.CompanyAccNo = dtDet.Rows[i]["CompanyAccNo"].ToString();
                rec.BankAddress = dtDet.Rows[i]["BankAddress"].ToString();*/

                dt = dtRem;

                List<InnerDetails> childArray = new List<InnerDetails>();
                if (dt != null && dt.Rows.Count > 1)
                {
                    foreach (DataRow cdr in dt.Rows)
                    {
                        InnerDetails child = new InnerDetails();
                        child.NameOfVendor = cdr["BenName"].ToString();
                        child.BankAccNo = cdr["BenAccNo"].ToString();
                        child.BankName = cdr["BenBankname"].ToString();
                        child.IFSCCode = cdr["BenBankIfscCode"].ToString();
                        child.Amount = cdr["Amount"].ToString() == "" ? "0" : cdr["Amount"].ToString();
                        child.RemitterDetails = cdr["RemitterDetails"].ToString();
                        childArray.Add(child);
                    }
                    rec.TotalAmount = childArray.Sum(item => Convert.ToDecimal(item.Amount));
                }
                else
                {
                    childArray = null;
                    string _tmpVal = "";
                    _tmpVal = dtDet.Rows[0]["Amount"].ToString() == "" ? "0" : dtDet.Rows[0]["Amount"].ToString();
                    rec.TotalAmount = Convert.ToDecimal(_tmpVal);
                }

                rec.DetailArray = childArray;

            }
            return rec;
        }

        public List<MemoDetail> GetMemoDetailsPrint(DataTable dtDet, DataTable dtRem, string Mode)
        {
            List<MemoDetail> result = new List<MemoDetail>();
            DataTable dt = null;
            if (dtDet != null)
            {
                for (int i = 0; i < dtDet.Rows.Count; i++)
                {
                    MemoDetail rec = new MemoDetail();
                    rec.Mode = Mode;
                    rec.BranchDetails = dtDet.Rows[i]["branchCode"].ToString() != "" ? dtDet.Rows[i]["branchCode"].ToString() : dtDet.Rows[i]["branchName"].ToString();
                    rec.IsShowTable = dtDet.Rows[i]["Bit"].ToString();
                    rec.PVDate = dtDet.Rows[i]["PVDate"].ToString();
                    rec.PVNo = dtDet.Rows[i]["PVNo"].ToString();
                    //rec.TotalAmount = dtDet.Rows[i]["TotAmount"].ToString();
                    rec.Amount = dtDet.Rows[i]["Amount"].ToString();
                    rec.BatchNo = dtDet.Rows[i]["BatchNo"].ToString();
                    rec.PvId = dtDet.Rows[i]["PvId"].ToString();
                    rec.MemoNo = dtDet.Rows[i]["MemoNo"].ToString();
                    rec.MemoTime = dtDet.Rows[i]["MemoDateTime"].ToString();
                    rec.MemoDate = dtDet.Rows[i]["MemoDate"].ToString();
                    rec.EmployeeSupplierCode = dtDet.Rows[i]["EmployeeSupplierCode"].ToString();
                    rec.EmployeeSupplierName = dtDet.Rows[i]["EmployeeSupplierName"].ToString();
                    rec.BenName = dtDet.Rows[i]["BenName"].ToString();
                    rec.BenAccNo = dtDet.Rows[i]["BenAccNo"].ToString();
                    rec.BenAddress = dtDet.Rows[i]["BenAddress"].ToString();
                    rec.BenBankname = dtDet.Rows[i]["BenBankname"].ToString();
                    rec.BenBankIfscCode = dtDet.Rows[i]["BenBankIfscCode"].ToString();
                    rec.AmountInWords = dtDet.Rows[i]["AmountInWords"].ToString();
                    rec.RemitterName = dtDet.Rows[i]["RemitterName"].ToString();
                    rec.RemitterAccNo = dtDet.Rows[i]["RemitterAccNo"].ToString();
                    rec.RemitterDetails = dtDet.Rows[i]["RemitterDetails"].ToString();
                    rec.RemitterMobilePhoneNo = dtDet.Rows[i]["RemitterMobilePhoneNo"].ToString();
                    rec.RemitterAddress = dtDet.Rows[i]["RemitterAddress"].ToString();
                    rec.RemitterCashDeposited = dtDet.Rows[i]["RemitterCashDeposited"].ToString();
                    rec.RemitterEmailId = dtDet.Rows[i]["RemitterEmailId"].ToString();
                    rec.Remarks = dtDet.Rows[i]["Remarks"].ToString();

                    rec.RemitterCode = dtDet.Rows[i]["RemitterCode"].ToString();
                    rec.CompanyAccNo = dtDet.Rows[i]["CompanyAccNo"].ToString();
                    rec.BankAddress = dtDet.Rows[i]["BankAddress"].ToString();

                    dt = LoadChildList(dtRem, dtDet.Rows[i]["PvId"].ToString());

                    List<InnerDetails> childArray = new List<InnerDetails>();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow cdr in dt.Rows)
                        {
                            InnerDetails child = new InnerDetails();
                            child.NameOfVendor = cdr["RemitterName"].ToString();
                            child.BankAccNo = cdr["RemitterAccNo"].ToString();
                            child.BankName = cdr["RemitterDetails"].ToString();
                            child.IFSCCode = cdr["BenBankIfscCode"].ToString();
                            child.Amount = cdr["Amount"].ToString();
                            child.RemitterDetails = dtDet.Rows[i]["RemitterCode"].ToString();
                            childArray.Add(child);
                        }
                    }
                    rec.DetailArray = childArray;
                    result.Add(rec);
                }
            }
            return result;
        }

        public DataTable LoadChildList(DataTable dt, string FilterId)
        {
            DataRow[] dr = dt.Select("PvId = " + FilterId);
            DataTable tdt = dt.Copy();
            tdt.Rows.Clear();
            foreach (DataRow tdr in dr)
            {
                tdt.ImportRow(tdr);
            }
            return tdt;
        }
                
        public string TmpRTGS_HDFC_A_NEW(MemoDetail result)
        { 
            StringBuilder Content=new StringBuilder("");
                    
            Content.Append("<html><head>    </head><body text='#000000' link='#0000ff' dir='LTR' style='border: 1.50pt double #FFF; padding-top: 0in; padding-bottom: 0in; padding-left: 0.06in; padding-right: 0in'>");
            Content.Append("<br />    <br />    <br />    <br />    <br />    <br />    <br /> ");
            if (result != null)
            {
                Content.Append( "  <div type='HEADER'>            <p align='LEFT' style='margin-bottom: 0.15in'>            </p>        </div>");

                Content.Append( "   <p class='western' align='JUSTIFY'><font face='Times New Roman, serif'><font size='3'><font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'>	</font></font><font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'><b>" + result.MemoNo.ToString() + "</b></font></font></font></font></p>");

                Content.Append( "    <dl style='margin-top: 0px;margin-bottom: 4px;'>            <dd>                <table width='640' cellpadding='3' cellspacing='0'>                    <colgroup>                        <col width='114'>");
                Content.Append( " <col width='58'>                        <col width='4'>                        <col width='282'>                        <col width='110'>                    </colgroup>");
                Content.Append( " <tbody>                        <tr>                            <td width='114' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( " <p align='LEFT'><font size='1' style='font-size: 8pt'><b>Branch Code / Name </b></font> </p></td>                            <td width='58' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( " <p class='western' align='LEFT' style='margin-right: -0.2in'><font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'>" + result.BranchDetails.ToString() + "</font></font></p>");
                Content.Append( " </td>                            <td width='4' style='border-top: none; border-bottom: none; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( " <p class='western' align='CENTER' style='margin-right: -0.2in'><br></p></td><td colspan='2' width='406' bgcolor='#bfbfbf' style='border: 1px solid #000000; padding: 0in 0.08in'><p class='western' align='CENTER' style='margin-right: -0.2in'>");
                Content.Append( " <font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'><b>Maximum Limit for NEFT Transaction</b></font></font></p></td></tr><tr>");
                Content.Append( " <td width='114' height='3' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( " <p align='LEFT'><font size='1' style='font-size: 8pt'><b>Date</b></font></p></td>");
                Content.Append( " <td width='58' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( " <p class='western' align='LEFT' style='margin-right: -0.2in'><font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'>" + result.MemoDate.ToString() + "</font></font></p></td>");
                Content.Append( " <td width='4' style='border-top: none; border-bottom: none; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( " <p class='western' align='CENTER' style='margin-right: -0.2in'>                                    <br>                                </p>                            </td>");
                Content.Append( " <td width='282' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( " <p align='LEFT' style='font-weight: normal'><font size='1' style='font-size: 8pt'>HDFC Bank Customer</font></p></td><td width='110' style='border: 1px solid #000000; padding: 0in 0.08in'><p align='CENTER' style='font-weight: normal'>");
                Content.Append( " <font size='1' style='font-size: 8pt'>No Limit</font></p></td></tr><tr>");
                Content.Append( " <td width='114' height='2' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( "<p align='LEFT'><font size='1' style='font-size: 8pt'><b>Time</b></font></p></td><td width='58' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( "<p class='western' align='LEFT' style='margin-right: -0.2in'><font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'>" + result.MemoTime.ToString() + "</font></font></p></td>");
                Content.Append( "<td width='4' style='border-top: none; border-bottom: none; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( "<p class='western' align='CENTER' style='margin-right: -0.2in'><br></p></td><td width='282' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( "<p align='CENTER' style='font-weight: normal'> <font size='1' style='font-size: 8pt'>Non HDFC Bank Customer &amp; Indo-Nepal NEFT Remittance</font></p></td><td width='110' style='border: 1px solid #000000; padding: 0in 0.08in'><p align='CENTER' style='font-weight: normal'>");
                Content.Append( "<font size='1' style='font-size: 8pt'>Up to INR 50,000/- </font></p></td></tr></tbody></table></dd></dl><h1 class='western' style='margin-left: 0.58in; margin-right: 0.35in; line-height: 115%; text-align:left;'>");
                Content.Append( "<font size='1' style='font-size: 8pt'><span style='font-weight: normal'>You are requested to remit the proceeds as per details below through RTGS ");

                if (result.Mode.ToString().Equals('R'))
                {
                    Content.Append( " <span style='border:1px solid black;'>&nbsp;X&nbsp;</span> ");
                }
                else
                {
                    Content.Append( "  <span style='border:1px solid black;'>&nbsp;&nbsp;&nbsp;</span>");
                }
                Content.Append( "    /  NEFT");
                if (result.Mode.ToString().Equals('N'))
                {
                    Content.Append( "    <span style='border:1px solid black;'>&nbsp;X&nbsp;</span>");
                }
                else
                {
                    Content.Append( "  <span style='border:1px solid black;'>&nbsp;&nbsp;&nbsp;</span>");
                }
                Content.Append( ". </span> </font><font size='1' style='font-size: 8pt'><i> (Tick</i></font><font face='ZapfDingbats, fantasy'><font size='1' style='font-size: 8pt'><i></i></font></font><font size='1' style='font-size: 8pt'>");
                Content.Append( "<i>the appropriate Box</i></font><font size='1' style='font-size: 8pt'><i>).</i></font></h1><h1 class='western' style='margin-left: 0.58in; margin-right: 0.35in; line-height: 115%; text-align:left;'>");
                Content.Append( "<font size='1' style='font-size: 8pt'><span style='font-weight: normal'>Attaching</br>Cheque No. ________________ for Rs.__________________________. (For</br>RTGS draw cheque favouring</span></font><font size='1' style='font-size: 8pt'>");
                Content.Append( " “HDFC Bank Ltd – RTGS” </font><font size='1' style='font-size: 8pt'> <span style='font-weight: normal'>and for NEFT draw cheque favouring </span></font><font size='1' style='font-size: 8pt'>");
                Content.Append( "“HDFC Bank Ltd – NEFT” </font><font size='1' style='font-size: 8pt'><span style='font-weight: normal'>)</span></font></h1><dl style='margin-top: 0px;margin-bottom: 4px;'><dd>");
                Content.Append( "<table width='649' cellpadding='3' cellspacing='0'><colgroup><col width='193'><col width='426'></colgroup><tbody><tr><td colspan='2' width='633' height='2' bgcolor='#d9d9d9' style='border: 1px solid #000000; padding: 0in 0.08in'>");
                Content.Append( "<p class='western' align='CENTER' style='margin-left: 0.06in'><font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'>Beneficiary Details</font></font></p></td></tr>");
                Content.Append( "<tr>                                        <td width='193' height='5' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( "<p class='western' align='LEFT' style='margin-left: -0.01in'>                                                <font face='Arial, sans-serif'>                                                    <font size='1' style='font-size: 8pt'>                                                        Beneficiary");
                Content.Append( " Name  </font>  </font>  </p></td> <td width='426' style='border: 1px solid #000000; padding: 0in 0.08in'>                                            <p class='western' align='LEFT'><font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'>" + result.BenName.ToString() + "</font></font></p>");
                Content.Append( "</td></tr><tr> <td width='193' height='5' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'><p class='western' align='LEFT' style='margin-left: -0.01in'>");
                Content.Append( "<font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'>Beneficiary Account Number </font> </font> </p>  </td> <td width='426' style='border: 1px solid #000000; padding: 0in 0.08in'><p class='western' align='LEFT'><font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'>" + result.BenAccNo.ToString() + "</font></font></p>");
                Content.Append( " </td></tr>");

                Content.Append( "<tr><td width='193' height='5' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( "<p class='western' align='LEFT' style='margin-left: -0.01in'> <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> Beneficiary Address </font> </font> </p> </td>");

                Content.Append( "<td width='426' style='border: 1px solid #000000; padding: 0in 0.08in'>");
                Content.Append( "<p class='western' align='LEFT'><font face='Times New Roman, serif'><font size='3'><font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'>@Model.BenAddress</font></font></font></font></p> </td> </tr>");
                Content.Append( "<tr> <td width='193' height='5' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( "<p class='western' align='LEFT' style='margin-left: -0.01in'> <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> Beneficiary Bank Name &amp; Branch </font> </font> </p> </td> ");
                Content.Append( "<td width='426' style='border: 1px solid #000000; padding: 0in 0.08in'>");
                Content.Append( "<p class='western' align='LEFT'><font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'>" + result.BenBankname.ToString() + "</font></font></p> </td> </tr>");
                Content.Append( "<tr> <td width='193' height='5' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( "<p class='western' align='LEFT' style='margin-left: -0.01in'> <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> Beneficiary Bank IFSC Code </font> </font> </p> </td>");
                Content.Append( "<td width='426' style='border: 1px solid #000000; padding: 0in 0.08in'> <p class='western' align='LEFT'> <table width='100%'> <tr> <td style='text-align:left; padding-right:10px;'>");
                Content.Append( "<font face='Times New Roman, serif'> <font size='3'> <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'>" + result.BenBankIfscCode.ToString() + "</font> </font> </font> </font> </td>");
                Content.Append( "<td style='text-align:right;'> <font face='Times New Roman, serif'> <font size='3'> <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> Account Type : Resident");
                if (result.AccountType.ToString().Equals("Resident"))
                {
                    Content.Append( "<span style='border:1px solid black;'>&nbsp;X&nbsp;</span>");
                }
                else
                {
                    Content.Append( "<span style='border:1px solid black;'>&nbsp;&nbsp;&nbsp;</span>");
                }
                Content.Append( "/ Non Resident");
                if (result.AccountType.ToString().Equals("NonResident"))
                {
                    Content.Append( "<span style='border:1px solid black;'>&nbsp;X&nbsp;</span>");
                }
                else
                {
                    Content.Append( "<span style='border:1px solid black;'>&nbsp;&nbsp;&nbsp;</span>");
                }
                Content.Append( "</font> </font> </font> </font> </td> </tr></table> </p> </td> </tr>");
                Content.Append( "<tr> <td width='193' height='5' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( "<p class='western' align='LEFT' style='margin-left: -0.01in'> <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> Amount (in figures) to be credited </font> </font> </p> </td>");
                Content.Append( "<td width='426' style='border: 1px solid #000000; padding: 0in 0.08in'>");
                Content.Append( "<p class='western' align='JUSTIFY'><font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'>" + result.AmountInFigures.ToString() + "</font></font></p> </td> </tr>");
                Content.Append( "<tr> <td width='193' height='4' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( "<p class='western' align='LEFT' style='margin-left: -0.01in'> <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> Amount (in words) to be credited </font> </font> </p> </td>");
                Content.Append( "<td width='426' style='border: 1px solid #000000; padding: 0in 0.08in'>");
                Content.Append( "<p class='western' align='JUSTIFY'><font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'>" + result.AmountInWords.ToString() + "</font></font></p> </td> </tr> </tbody> </table> </dd> </dl>");


                Content.Append( "<dl style='margin-top: 0px;margin-bottom: 4px;'> <dd> <table width='649' cellpadding='3' cellspacing='0'>");
                Content.Append( "<colgroup> <col width='193'> <col width='426'> </colgroup>");
                Content.Append( "<tbody> <tr> <td colspan='2' width='633' bgcolor='#d9d9d9' style='border: 1px solid #000000; padding: 0in 0.08in'> <p class='western' align='CENTER' style='margin-left: 0.06in'>");
                Content.Append( "<font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> <b> My / Our Details (Remitter) </b> </font> </font> </p> </td> </tr>");
                Content.Append( "<tr> <td width='193' height='2' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( "<p class='western' align='LEFT' style='margin-left: -0.01in'> <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> Remitter (Applicant) Name </font> </font> </p> </td>");
                Content.Append( "<td width='426' style='border: 1px solid #000000; padding: 0in 0.08in'>");
                Content.Append( " <p class='western' align='LEFT' style='margin-left: 0.06in'> <font face='Arial, sans-serif'>");
                Content.Append( "<font size='1' style='font-size: 8pt'> &nbsp;System.Configuration.ConfigurationManager.AppSettings['CompanyFullName'] </font>  </font> </p> </td> </tr>");
                Content.Append( "<tr> <td width='193' height='5' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( "<p class='western' align='LEFT' style='margin-left: -0.01in'> <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> Remitter Account Number </font> </font> </p> </td>");
                Content.Append( "<td width='426' style='border: 1px solid #000000; padding: 0in 0.08in'>");
                Content.Append( "<p class='western' align='LEFT' style='margin-left: 0.06in'>");
                Content.Append( "<font face='Times New Roman, serif'> <font size='3'> <font face='Arial, sans-serif'>");
                Content.Append( "<font size='1' style='font-size: 8pt'> &nbsp; 00600310011700</font> </font> </font> </font> </p> </td> </tr>");
                Content.Append( "<tr> <td width='193' height='8' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( "<p class='western' align='LEFT' style='margin-left: -0.01in'> <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> Cash Deposited (Non HDFC Bank Customer) </font> </font> </p> </td>");

                Content.Append( "<td width='426' style='border: 1px solid #000000; padding: 0in 0.08in'> <p class='western' align='LEFT' style='margin-left: 0.06in'>");
                Content.Append( "<font face='Times New Roman, serif'> <font size='3'> <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> &nbsp;" + result.RemitterCashDeposited.ToString() + "</font> </font> </font> </font> </p> </td> </tr> <tr>");
                Content.Append( "<td width='193' height='5' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( "<p class='western' align='LEFT' style='margin-left: -0.01in'> <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> Mobile / Phone Number of Remitter ( Mandatory) </font></font> </p> </td>");
                Content.Append( "<td width='426' style='border: 1px solid #000000; padding: 0in 0.08in'> <p class='western' align='LEFT' style='margin-left: 0.06in'>");
                Content.Append( "<font face='Times New Roman, serif'> <font size='3'> <font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'>");
                if (result.Mode.ToString().Equals('R'))
                {
                    Content.Append( "<span> 022 42241492/ 022 42241493 </span>");
                }
                else
                {
                    Content.Append( "<span> 044 42886630/ 044 42886639 </span>");
                }
                Content.Append( "</font></font> <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> <b> E-Mail Id: ");
                if (result.Mode.ToString().Equals('R'))
                {
                    Content.Append( "<span style='text-decoration:underline;'>FICCtreasuryops@fullertonindia.com</span>");
                }
                else
                {
                    Content.Append( "<span style='text-decoration:underline;'>expensehelp@fullertonindia.com</span>");
                }

                Content.Append( "</b> </font> </font> </font> </font> </p> </td> </tr>");
                Content.Append( "<tr> <td width='193' height='18' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( "<p class='western' align='LEFT' style='margin-left: -0.01in'> <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> Address of the Remitter (Mandatory for Non – HDFC Bank Customer) </font></font></p> </td>");
                Content.Append( "<td width='426' style='border: 1px solid #000000; padding: 0in 0.08in'> <p class='western' align='LEFT' style='margin-left: 0.06in'>");
                Content.Append( "<font face='Times New Roman, serif'> <font size='3'> <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> &nbsp;");
                if (result.Mode.ToString().Equals('R'))
                {
                    Content.Append( "<span >System.Configuration.ConfigurationManager.AppSettings['CompanyFullName'],System.Configuration.ConfigurationManager.AppSettings['MAddress1'],");
                    Content.Append( "System.Configuration.ConfigurationManager.AppSettings['MAddress2'], System.Configuration.ConfigurationManager.AppSettings['MArea'],System.Configuration.ConfigurationManager.AppSettings['MCity'] .</span>");
                }
                else
                {
                    Content.Append( "<span >System.Configuration.ConfigurationManager.AppSettings['CompanyFullName'],System.Configuration.ConfigurationManager.AppSettings['LAddress1'], ");
                    Content.Append( "System.Configuration.ConfigurationManager.AppSettings['LAddress2'], System.Configuration.ConfigurationManager.AppSettings['LArea'],System.Configuration.ConfigurationManager.AppSettings['LCity'] .</span>");
                }

                Content.Append( "</font> </font> </font> </font> </p></td></tr>");
                Content.Append( "<tr> <td width='193' height='4' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( "<p class='western' align='LEFT' style='margin-left: -0.01in'><font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'>Remarks</font></font></p> </td>");
                Content.Append( "<td width='426' style='border: 1px solid #000000; padding: 0in 0.08in'> <p class='western' align='LEFT' style='margin-left: 0.06in'> <font face='Times New Roman, serif'>");
                Content.Append( "<font size='3'> <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> &nbsp;" + result.Remarks.ToString() + "</font> </font> </font> </font> </p></td></tr></tbody></table></dd></dl>");

                Content.Append( "<p class='western' align='JUSTIFY' style='margin-left: 0.58in; margin-right: 0.06in; margin-top:0px;'> <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> <u> <b> Terms &amp; Conditions </b> </u> </font> </font> </p>");

                Content.Append( "<p class='western' align='JUSTIFY' style='margin-left: 0.58in; margin-right: 0.35in; margin-top: 0px; '> <font face='Times New Roman, serif'> <font size='3'> <font face='Arial, sans-serif'>");
                Content.Append( "<font size='1'> * I / We hereby authorize HDFC Bank Ltd. to carry out the </font> </font><font face='Arial, sans-serif'><font size='1'><b>RTGS</b>");
                if (result.Mode.ToString().Equals('R'))
                {
                    Content.Append( "<span style='border:1px solid black;'>&nbsp;X&nbsp;</span>");
                }
                else
                {
                    Content.Append( "<span style='border:1px solid black;'>&nbsp;&nbsp;&nbsp;</span>");
                }
                Content.Append( "</font></font><font face='Arial, sans-serif'> <font size='1'> </font>");
                Content.Append( "</font><font face='Arial, sans-serif'><font size='1'><b></b></font></font><font face='Arial, sans-serif'><font size='1'>/</font></font><font face='Arial, sans-serif'><font size='1'> <b>NEFT</b>");
                if (result.Mode.ToString().Equals('N'))
                {
                    Content.Append( "<span style='border:1px solid black;'>&nbsp;X&nbsp;</span>");
                }
                else
                {
                    Content.Append( "<span style='border:1px solid black;'>&nbsp;&nbsp;&nbsp;</span>");
                }
                Content.Append( "</font></font><font face='Arial, sans-serif'> <font size='1'> </font> </font><font face='Arial, sans-serif'> <font size='1'> <b> </b> </font>");
                Content.Append( "</font><font face='Arial, sans-serif'> <font size='1'> transaction as per details mentioned above. </font> </font><font face='Arial, sans-serif'>");
                Content.Append( " <font size='1'> <i> <b> (Tick </b> </i> </font> </font><font face='ZapfDingbats, fantasy'><font size='1'><i><b></b></i></font></font><font face='Arial, sans-serif'>");
                Content.Append( "<font size='1'> <i> <b> the appropriate Box) </b> </i> </font> </font> </font> </font> </p>");
                Content.Append( "<p class='western' align='JUSTIFY' style='margin-left: 0.58in; margin-right: 0.35in; margin-top: 0px; '>");
                Content.Append( "<font face='Arial, sans-serif'> <font size='1'> * I / We hereby agree that the aforesaid details including the IFSC code and the beneficiary account are correct. </font> </font> </p>");
                Content.Append( "<p class='western' align='JUSTIFY' style='margin-left: 0.58in; margin-right: 0.35in; margin-top: 0px; '> <font face='Arial, sans-serif'>");
                Content.Append( "<font size='1'> * I / We further acknowledge that HDFC Bank accepts no liability for any consequences arising out of erroneous details provided by me/us. </font> </font> </p>");
                Content.Append( "<p class='western' align='JUSTIFY' style='margin-left: 0.58in; margin-right: 0.35in; margin-top: 0px; '> <font face='Arial, sans-serif'>");
                Content.Append( "<font size='1'> * I / We agree that the credit will be affected solely on the beneficiary account number information and beneficiary name particulars will not be used for the same. </font> </font> </p>");
                Content.Append( "<p class='western' align='JUSTIFY' style='margin-left: 0.58in; margin-right: 0.35in; margin-top: 0px; '> <font face='Arial, sans-serif'>");
                Content.Append( "<font size='1'> * I / We authorize the bank to debit my / our account with the charges plus taxes as applicable for this transaction. </font> </font></p>");
                Content.Append( "<p class='western' align='JUSTIFY' style='margin-left: 0.58in; margin-right: 0.35in; margin-top: 0px; '> <font face='Arial, sans-serif'>");
                Content.Append( "<font size='1'> * I / We agree that requests submitted after the cut off time will be sent in next batch or next working day as applicable. </font> </font> </p>");
                Content.Append( "<p class='western' align='JUSTIFY' style='margin-left: 0.58in; margin-right: 0.35in; margin-top: 0px; '> <font face='Arial, sans-serif'>");
                Content.Append( "<font size='1'> * I / We hereby agree &amp; understand that the RTGS / NEFT request is subject to the RBI regulations and guidelines governing the same. </font> </font> </p>");
                Content.Append( "<p class='western' align='JUSTIFY' style='margin-left: 0.58in; margin-right: 0.35in; margin-top: 0px; '> <font face='Times New Roman, serif'> <font size='3'> <font face='Arial, sans-serif'>");
                Content.Append( "<font size='1'> * I / We also understand that the remitting Bank shall not be liable for any loss of damage arising or resulting from delay in transmission delivery or non-delivery of Electronic message or any mistake, omission, or error in transmission or delivery");
                Content.Append( "thereof or in deciphering the message from any cause whatsoever or from its misinterpretation received or the action of the destination Bank or any act or even beyond control. </font> </font> </font> </font> </p>");
                Content.Append( "<p class='western' align='JUSTIFY' style='margin-left: 0.58in; margin-right: 0.35in; margin-top: 0px;  margin-top: 0px; '> <font face='Arial, sans-serif'>");
                Content.Append( "<font size='1'> * I/We agree that incase of NEFT Transaction if we do not have an account with the bank, we will produce Original identification proof while giving the request. In case I/We submit form 60, we will also submit the address proof. </font> </font> </p>");
                Content.Append( "<p class='western' align='JUSTIFY' style='margin-left: 0.58in; margin-right: 0.35in; margin-top: 0px; '> <font face='Times New Roman, serif'> <font size='3'>");
                Content.Append( "<font face='Arial, sans-serif'> <font size='1'> * In case the RTGS and NEFT option is not ticked by us, I / We authorize you to execute the transaction </font>");
                Content.Append( "</font><font face='Arial, sans-serif'> <font size='1'> <b> less than Rupees Two Lacs through NEFT </b> </font>");
                Content.Append( "</font><font face='Arial, sans-serif'> <font size='1'> and </font> </font><font face='Arial, sans-serif'>");
                Content.Append( "<font size='1'> <b> greater than or equal to Rupees Two Lacs through RTGS </b> </font>");
                Content.Append( "</font><font face='Arial, sans-serif'> <font size='1'> and debit the charges as applicable. </font> </font> </font> </font> </p>");


                Content.Append( "<dl style='margin-top: 0px;margin-bottom: 4px;'> <dd> <table width='649' cellpadding='3' cellspacing='0'> <colgroup> <col width='100'> <col width='519'> </colgroup>");
                Content.Append( "<tbody> <tr> <td width='100' height='71' bgcolor='#d9d9d9' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( "<p class='western' align='CENTER' style='margin-left: -0.01in'> <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> <b> Signature of Authorized Signatory </b> </font> </font> </p> </td>");
                Content.Append( "<td width='519' valign='BOTTOM' style='border: 1px solid #000000; padding: 0in 0.08in'> <p class='western' align='LEFT' style='margin-left: 0.06in'> <br> </p>");
                Content.Append( "<p class='western' align='LEFT' style='margin-left: 0.06in'> <font face='Times New Roman, serif'> <font size='3'> <font color='#000000'>");
                Content.Append( "<font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> <table width='100%'> <tr>");
                Content.Append( "<td style='text-align:left; padding-right:10px;'> ____________________ </td>");
                Content.Append( "<td style='text-align:center; padding-right:10px;'> ____________________ </td>");
                Content.Append( "<td style='text-align:right;'> ____________________ </td> </tr> </table>  </font> </font> </font> </font> </font> </p>");
                Content.Append( "<p class='western' align='LEFT' style='margin-left: 0.06in; margin-top:0px;'> <font color='#000000'> </font><font face='Times New Roman, serif'> <font size='3'> <table width='100%'>");
                Content.Append( " <tr> <td style='text-align:center;'> <font color='#000000'><font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'>1</font></font></font><font color='#000000'><sup><font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'>st</font></font></sup></font>");
                Content.Append( "<font color='#000000'> <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> Signatory </font> </font> </font> </td>");
                Content.Append( "<td style='text-align:center;'> <font color='#000000'><font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'>2</font></font></font><font color='#000000'><sup><font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'>nd</font></font></sup></font> <font color='#000000'>");
                Content.Append( "<font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> Signatory </font> </font> </font> </td>");
                Content.Append( "<td style='text-align:center;'> <font color='#000000'><font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'>3</font></font></font><font color='#000000'><sup><font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'>rd</font></font></sup></font>");
                Content.Append( " <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt'> Signatory </font> </font> </td> </tr> </table> </font> </font> </p>");



                Content.Append( "<p class='western' style='margin-left: 0.06in; text-align:center; margin-top:0px;'> <font face='Times New Roman, serif'> <font size='3'> <font color='#bfbfbf'><font face='Arial, sans-serif'><font size='1' style='font-size: 8pt'> </font></font></font><font color='#000000'>");
                Content.Append( " <font face='Arial, sans-serif'> <font size='1' style='font-size: 8pt;'> <i> Please affix stamp wherever applicable </i> </font> </font> </font> </font> </font> </p> </tr> </tbody> </table> </dd> </dl>");

                Content.Append( "<dl style='margin-top: 0px;margin-bottom: 4px;'> <dd> <table width='649' cellpadding='3' cellspacing='0'> <colgroup> <col width='265'> <col width='98'> <col width='154'> <col width='74'> </colgroup>");
                Content.Append( "<tbody> <tr> <td colspan='4' width='633' bgcolor='#d9d9d9' style='border: 1px solid #000000; padding: 0in 0.08in'> <h1 class='western' style='margin-left: 0.06in'> <font size='1' style='font-size: 8pt'> Branch Use Only </font> </h1> </td> </tr>");
                Content.Append( " <tr> <td width='265' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( "<p class='western' align='LEFT' style='margin-left: -0.01in'> <font face='Arial, sans-serif'> <font size='1'> Transaction Reference Number </font> </font> </p> </td>");
                Content.Append( " <td colspan='2' width='266' valign='BOTTOM' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( "<p class='western' align='CENTER' style='margin-left: 0.06in'> <br> </p> </td>");
                Content.Append( " <td rowspan='5' width='74' valign='BOTTOM' style='border: 1px solid #000000; padding: 0in 0.08in'> <p class='western' align='CENTER'> <font color='#bfbfbf'>");
                Content.Append( "<font face='Arial, sans-serif'> <font size='1'> Branch Stamp, Date &amp; Sign </font> </font> </font> </p> </td> </tr>");
                Content.Append( " <tr> <td width='265' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( " <p class='western' align='LEFT' style='margin-left: -0.01in'> <font face='Arial, sans-serif'> <font size='1'> Transaction Inputted by </font> </font> </p> </td>");
                Content.Append( " <td width='98' valign='BOTTOM' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( " <p class='western' align='CENTER' style='margin-left: 0.06in'> <font color='#bfbfbf'> <font face='Arial, sans-serif'> <font size='1'> Employee Code </font> </font> </font> </p> </td>");
                Content.Append( " <td width='154' valign='BOTTOM' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( " <p class='western' align='CENTER' style='margin-left: 0.06in'><font color='#bfbfbf'><font face='Arial, sans-serif'><font size='1'>Signature</font></font></font></p> </td> </tr>");
                Content.Append( "<tr> <td width='265' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( " <p class='western' align='LEFT' style='margin-left: -0.01in'> <font face='Arial, sans-serif'> <font size='1'> Transaction Authorized by </font> </font> </p> </td>");
                Content.Append( " <td width='98' valign='BOTTOM' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( "<p class='western' align='CENTER' style='margin-left: 0.06in'> <font color='#bfbfbf'> <font face='Arial, sans-serif'>");
                Content.Append( " <font size='1'> Employee Code </font> </font> </font> </p> </td>");
                Content.Append( " <td width='154' valign='BOTTOM' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( " <p class='western' align='CENTER' style='margin-left: 0.06in'><font color='#bfbfbf'><font face='Arial, sans-serif'><font size='1'>Signature</font></font></font></p> </td> </tr>");
                Content.Append( " <tr> <td width='265' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( " <p class='western' align='LEFT' style='margin-left: -0.01in'> <font face='Times New Roman, serif'> <font size='3'>");
                Content.Append( " <font face='Arial, sans-serif'> <font size='1'> Transaction Authorized by (2 </font> </font><sup><font face='Arial, sans-serif'><font size='1'>nd</font></font></sup><font face='Arial, sans-serif'>");
                Content.Append( " <font size='1'> level) (for amount  &gt; Rs. 5 lacs) </font> </font> </font> </font> </p> </td>");
                Content.Append( " <td width='98' valign='BOTTOM' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( " <p class='western' align='CENTER' style='margin-left: 0.06in'> <font color='#bfbfbf'> <font face='Arial, sans-serif'>");
                Content.Append( " <font size='1'> Employee Code </font> </font> </font> </p> </td>");
                Content.Append( " <td width='154' valign='BOTTOM' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( " <p class='western' align='CENTER' style='margin-left: 0.06in'><font color='#bfbfbf'><font face='Arial, sans-serif'><font size='1'>Signature</font></font></font></p> </td> </tr> <tr>");
                Content.Append( " <td width='265' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( " <p class='western' align='LEFT' style='margin-left: -0.01in'> <font face='Arial, sans-serif'> <font size='1'> KYC documentation done by (only for Non-HDFC Bank Customers) </font> </font> </p> </td>");
                Content.Append( " <td width='98' valign='BOTTOM' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( " <p class='western' align='CENTER' style='margin-left: 0.06in'> <font color='#bfbfbf'> <font face='Arial, sans-serif'>");
                Content.Append( " <font size='1'> Employee Code </font> </font> </font> </p> </td>");
                Content.Append( " <td width='154' valign='BOTTOM' style='border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                Content.Append( " <p class='western' align='CENTER' style='margin-left: 0.06in'><font color='#bfbfbf'><font face='Arial, sans-serif'><font size='1'>Signature</font></font></font></p> </td> </tr> </tbody> </table> </dd> </dl>");
                Content.Append( " <p class='western' align='LEFT' style='margin-left: -0.19in; margin-right: -0.39in'> <font face='Times New Roman, serif'> <font size='3'> <font face='Arial, sans-serif'>");
                Content.Append( " <font size='1' style='font-size: 8pt'> <b>");
                Content.Append( " - -  - - - - - - - - - - - - - - - - - - - - -- - - - - - - - - - - -");
                Content.Append( " - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                Content.Append( " - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                Content.Append( " - - - </b> </font> </font> </font> </font> </p>");


                Content.Append( " <dl style='margin-top: 0px;margin-bottom: 4px;'> <dd> <table width='649' cellpadding='3' cellspacing='0'> <colgroup> <col width='633'> </colgroup>");
                Content.Append( " <tbody> <tr> <td width='633' bgcolor='#d9d9d9' style='border: 1px solid #000000; padding: 0in 0.08in'> <p class='western' align='CENTER' style='margin-left: 0.06in'> <font face='Arial, sans-serif'> <font size='1'> <b> Customer Acknowledgement </b> </font> </font> </p> </td> </tr>");
                Content.Append( " <tr> <td width='633' style='border: 1px solid #000000; padding: 0in 0.08in'> <p class='western' align='JUSTIFY' style='margin-top:5px;'> <font face='Times New Roman, serif'>");
                Content.Append( " <font size='3'> <font face='Arial, sans-serif'> <font size='1'> Received  application for RTGS");
                if (result.Mode.ToString().Equals('R'))
                {
                    Content.Append( "<span style='border:1px solid black;'>&nbsp;X&nbsp;</span>");
                }
                else
                {
                    Content.Append( " <span style='border:1px solid black;'>&nbsp;&nbsp;&nbsp;</span>");
                }
                Content.Append( " </font> </font><font face='Arial, sans-serif'> <font size='1'> / NEFT");
                if (result.Mode.ToString().Equals('N'))
                {
                    Content.Append( " <span style='border:1px solid black;'>&nbsp;X&nbsp;</span>");
                }
                else
                {
                    Content.Append( " <span style='border:1px solid black;'>&nbsp;&nbsp;&nbsp;</span>");
                }
                Content.Append( " </font> </font>");

                Content.Append( "<font face='Arial, sans-serif'> <font size='1'>");
                Content.Append( " for an amount of Rs. ____________________ vide cash / cheque number ________________ to be credited to Account Number");
                Content.Append( " ________________________________ of _______________________________ Bank with IFSC Code____________________________. Customers will be guided by the");
                Content.Append( " Terms and Conditions mentioned in the form. HDFC Bank will accept no liability for any consequences arising out of erroneous details provided by the Customer. </font>");
                Content.Append( " </font> </font> </font> </p>");
                Content.Append( " <p class='western' align='JUSTIFY'> <font face='Times New Roman, serif'> <font size='3'> <table width='100%'> <tr> <td style='width:50%; text-align:left;'> <font face='Arial, sans-serif'>");
                Content.Append( " <font size='1'> Date __________________		Time_______________ </font> </font> </td>");
                Content.Append( " <td style='width:50%; text-align:right;'> <font color='#bfbfbf'> <font face='Arial, sans-serif'> <font size='1'>");
                Content.Append( " <u> _Branch Stamp  &amp; Sign __ </u> </font> </font> </font> </td> </tr> </table> </font> </font> </p> </td> </tr> </tbody> </table> </dd> </dl>");
                Content.Append( " <p style='page-break-before:always'></p> <p class='western' align='JUSTIFY'> <br> </p>");

                if (result.DetailArray != null)
                {
                    Content.Append( " <center> <table width='711' style='border-top: 1px solid #000000; border-left: 1px solid #000000;' cellpadding='3' cellspacing='0'> <colgroup>");
                    Content.Append( " <col width='29'> <col width='172'> <col width='121'> <col width='125'> <col width='94'> <col width='84'> </colgroup>");
                    Content.Append( " <tbody> <tr> <td width='29' height='16' style='border-bottom: 1px solid #000000; border-right: 1px solid #000000; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'> <p class='western' align='CENTER' style='margin-left: -0.15in; text-indent: 0.13in'>");
                    Content.Append( " <font face='Arial, sans-serif'><font size='2' style='font-size: 9pt'><b>S.No</b></font></font> </p> </td>");
                    Content.Append( " <td width='172' style='border-bottom: 1px solid #000000; border-right: 1px solid #000000; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                    Content.Append( " <p class='western' align='CENTER'> <font face='Arial, sans-serif'> <font size='2' style='font-size: 9pt'> <b> Name of the vendor </b> </font> </font> </p> </td>");
                    Content.Append( " <td width='121' style='border-bottom: 1px solid #000000; border-right: 1px solid #000000; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                    Content.Append( " <p class='western' align='CENTER'> <font face='Arial, sans-serif'> <font size='2' style='font-size: 9pt'> <b> Bank A/C Number </b> </font> </font> </p> </td>");
                    Content.Append( " <td width='125' style='border-bottom: 1px solid #000000; border-right: 1px solid #000000; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                    Content.Append( " <h1 class='western'> <font size='2' style='font-size: 9pt'> Bank Name </font> </h1> </td>");
                    Content.Append( " <td width='94' style='border-bottom: 1px solid #000000; border-right: 1px solid #000000; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                    Content.Append( " <h1 class='western'><font size='2' style='font-size: 9pt'>IFSC Code</font></h1> </td>");
                    Content.Append( " <td width='84' style='border: 1px solid #000000; padding: 0in 0.08in'> <h1 class='western'><font size='2' style='font-size: 9pt'>Amount</font></h1> </td> </tr>");
                    foreach (var innerDet in result.DetailArray)
                    {
                        Content.Append( " <tr> <td width='29' height='44' style='border-bottom: 1px solid #000000; border-right: 1px solid #000000; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                        Content.Append( " <p class='western' align='CENTER'> (++icount) </p> </td>");
                        Content.Append( " <td width='172' style='border-bottom: 1px solid #000000; border-right: 1px solid #000000; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                        Content.Append( " <p class='western' align='LEFT'> innerDet.NameOfVendor </p> </td>");
                        Content.Append( " <td width='121' style='border-bottom: 1px solid #000000; border-right: 1px solid #000000; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                        Content.Append( " <p class='western' align='LEFT'> innerDet.BankAccNo </p> </td>");
                        Content.Append( " <td width='125' style='border-bottom: 1px solid #000000; border-right: 1px solid #000000; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                        Content.Append( " <p class='western' align='LEFT'> innerDet.BankName </p> </td>");
                        Content.Append( " <td width='94' style='border-bottom: 1px solid #000000; border-right: 1px solid #000000; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                        Content.Append( " <p class='western' align='CENTER'> innerDet.IFSCCode </p> </td>");
                        Content.Append( " <td width='84' style='border: 1px solid #000000; padding: 0in 0.08in'>");
                        Content.Append( " <p class='western' align='RIGHT'> innerDet.Amount </p> </td> </tr>");
                    }
                    Content.Append( " <tr> <td width='29' height='16' style='border-bottom: 1px solid #000000; border-right: 1px solid #000000; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                    Content.Append( " <p class='western' align='CENTER' style='margin-left: -0.15in; text-indent: 0.13in'> <font face='Arial, sans-serif'><font size='2' style='font-size: 9pt'><b>&nbsp;</b></font></font> </p> </td>");
                    Content.Append( " <td width='172' style='border-bottom: 1px solid #000000; border-right: 1px solid #000000; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                    Content.Append( " <p class='western' align='CENTER'> <font face='Arial, sans-serif'> <font size='2' style='font-size: 9pt'> <b> &nbsp; </b> </font> </font> </p> </td>");
                    Content.Append( " <td width='121' style='border-bottom: 1px solid #000000; border-right: 1px solid #000000; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                    Content.Append( " <p class='western' align='CENTER'> <font face='Arial, sans-serif'> <font size='2' style='font-size: 9pt'> <b> &nbsp; </b> </font> </font> </p> </td>");
                    Content.Append( " <td width='125' style='border-bottom: 1px solid #000000; border-right: 1px solid #000000; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                    Content.Append( " <h1 class='western'> <font size='2' style='font-size: 9pt'> &nbsp; </font> </h1> </td>");
                    Content.Append( " <td width='94' style='border-bottom: 1px solid #000000; border-right: 1px solid #000000; padding-top: 0in; padding-bottom: 0in; padding-left: 0.08in; padding-right: 0in'>");
                    Content.Append( " <h1 class='western'><font size='2' style='font-size: 9pt'>Total</font></h1> </td>");
                    Content.Append( " <td width='84' style='border: 1px solid #000000; padding: 0in 0.08in'> <h1 class='western'><font size='2' style='font-size: 9pt'>" + result.TotalAmount.ToString() + "</font></h1> </td> </tr>");
                    Content.Append( "                </tbody>                </table>            </center>");

                     }
                Content.Append( "</body> </html>");
            }
   
          return Content.ToString();
        }
        
        public string TmpDD_HDFC(DDTemplate result)
        {
            StringBuilder Content = new StringBuilder("");
            Content.Append(" <html> <head> <style> #tblsig > tbody > tr > td { margin: 0px !important; white-space: normal; word-break: break-all;}");

            Content.Append(" #tbldet > tbody > tr > td, #tbldet > thead > tr > th { border-bottom: 1px solid black;}");
            Content.Append(" </style> </head> <body style='font-family:Verdana; font-size:13px !important;'>  <br /> <br /> <br /> <br /> <br /> <br /> <br />");
            if (result != null)
            {
                Content.Append("<div style='width:80%;'> Date:" + result.Date.ToString() + "<br />" + result.LetterNo.ToString() + "</div><br /><br />");
                Content.Append(" <div style='width:100%; white-space: normal; word-break: break-all;'> To,<br />");

                Content.Append("The Branch Manager<br /> HDFC Bank Limited<br /> 115  Radha krishnan Salai,<br /> 3rd floor, (Opp. Kalyani Hospital,)<br /> Mylapore,<br /> Chennai 600 004. </div><br /><br />");
                Content.Append(" <span><b>Sub: Request for Demand Drafts</b></span><br /><br /> <span>Ref: Our Account No. <b>00600310011700</b> </span><br /><br /> <span>Kindly issue the following DDs.</span><br /><br />");

                Content.Append(" <div> <table style='border-top: 1px solid black; border-left: 1px solid black; font-size:13px;' width='100%'>");
                Content.Append(" <thead> <tr> <th style='width:10%; border-bottom: 1px solid black; border-right: 1px solid black;'> S.No </th>");
                Content.Append(" <th style='width:30%; border-bottom: 1px solid black; border-right: 1px solid black;'> DD Favoring </th>");
                Content.Append(" <th style='width:40%; max-width:200px; border-bottom: 1px solid black; border-right: 1px solid black;'> Payable at </th>");
                Content.Append(" <th style='width:15%; border-bottom: 1px solid black; border-right: 1px solid black;'> Amount </th> </tr> </thead> <tbody>");
                if (result.DetailArray.Count > 1)
                {
                    Content.Append(" <tr> <td align='center' style='border-bottom: 1px solid black; border-right: 1px solid black;'>1</td> <td style='border-bottom: 1px solid black; border-right: 1px solid black;'>AS PER LIST</td>");
                    Content.Append(" <td style='border-bottom: 1px solid black; border-right: 1px solid black;'>AS PER LIST</td> <td style='border-bottom: 1px solid black; border-right: 1px solid black;'>AS PER LIST</td> </tr>");
                }
                else
                {
                    Content.Append(" <tr> <td style='border-bottom: 1px solid black; border-right: 1px solid black;' align='center'>1</td> <td style='border-bottom: 1px solid black; border-right: 1px solid black;'>" + result.DDFavoring.ToString() + "</td> <td style='border-bottom: 1px solid black; ");
                    Content.Append(" border-right: 1px solid black;'>" + result.PayableAt.ToString() + "</td> <td style='border-bottom: 1px solid black; border-right: 1px solid black;' align='right'>" + result.Amount.ToString() + "</td> </tr>");

                }
                Content.Append(" </tbody> </table> </div> <br />");
                Content.Append(" <span>Amount in words:" + result.AmountInWords.ToString() + "</span><br /> <br />");
                Content.Append(" <span> The DD amount and charges if any towards issue of DD may please be debited to our account mentioned above. </span><br /><br />");
                Content.Append(" <span> Thanking You </span><br /><br /> <span> Yours faithfully </span> <br />");

                Content.Append(" <div style='width:100%;'> <table id='tblsig' width='100%' style='font-size: 13px;'> <tr> <td>For System.Configuration.ConfigurationManager.AppSettings['CompanyFullName']</td> </tr>");
                Content.Append(" <tr> <td style='line-height:50px;'><br /></td> </tr> <tr> <td>Authorized Signatory</td> </tr> </table> </div> <br />");
                Content.Append(" <span>Please hand over the DD to the bearer of the memo.</span> <p style='page-break-before:always'></p> <br /> <br />");

                if (result.DetailArray.Count > 1)
                {
                    Content.Append(" <div> <table style='border-top: 1px solid black; border-left: 1px solid black; font-size:13px;' id='tbldet' width='100%'>");
                    Content.Append(" <thead> <tr> <th style='width:10%; border-bottom: 1px solid black; border-right: 1px solid black;'> S.No </th>");
                    Content.Append(" <th style='width:35%; border-bottom: 1px solid black; border-right: 1px solid black;'> DD Favoring </th>");
                    Content.Append(" <th style='width:40%; max-width:200px; border-bottom: 1px solid black; border-right: 1px solid black;'> Payable at </th>");
                    Content.Append(" <th style='width:15%; border-bottom: 1px solid black; border-right: 1px solid black;'> Amount </th> </tr> </thead> <tbody>");
                    foreach (var innerDet in result.DetailArray)
                    {
                        Content.Append(" <tr> <td style='border-bottom: 1px solid black; border-right: 1px solid black;' align='center'>(++icount)</td>  <td style='border-bottom: 1px solid black; border-right: 1px solid black;'>innerDet.DDFavoring</td>");
                        Content.Append(" <td style='border-bottom: 1px solid black; border-right: 1px solid black;'>innerDet.PayableAt</td> <td style='border-bottom: 1px solid black; border-right: 1px solid black;' align='right'>innerDet.Amount</td> </tr>");
                    }
                    Content.Append(" </tbody> <tfoot> <tr> <th align='right' style='border-bottom: 1px solid black; border-right: 1px solid black;' colspan='3'> Total </th>");
                    Content.Append(" <th align='right' style='border-bottom: 1px solid black; border-right: 1px solid black;'>" + result.Totalamount.ToString() + "</th> </tr> </tfoot> </table> </div>");
                }
            }

            Content.Append("</body> </html>");


            return Content.ToString();
        }

        public string TmpDT_CITI(ERATemplate result)
        {
            StringBuilder Content = new StringBuilder("");
            Content.Append(" <html> <head> <style> #tblsig > tbody > tr > td { margin: 0px !important; white-space: normal; word-break: break-all;}");

            Content.Append("#tbldet > tbody > tr > td, #tbldet > thead > tr > th { border-bottom: 1px solid black; } </style> </head>");
            Content.Append(" <body style='font-family:Verdana; font-size:13px !important;'> <br /> <br /> <br /> <br /> <br /> <br /> <br />");
            if (result != null)
            {
                Content.Append(" <div style='width:100%; text-align:right; padding-right:20px;'> Date:" + result.Date.ToString() + "<br /> " + result.LetterNo.ToString() + "</div><br /><br />");
                Content.Append(" <div style='width:100%; white-space: normal; word-break: break-all;'> To,<br /> The Branch Manager<br /> CITI Bank<br /> Club House Road,<br /> Chennai - 2. </div><br /><br />");
                Content.Append(" <div> <span>Dear Sir / Madam,</span><br /><br /> <span>Sub: Employee Reimbursement Credits</span><br /><br />");
                Content.Append(" <p style='text-indent:25px; text-align:justify;'> We would like to do Employee Reimbursement Account (ERA) transfer for our Employees. Please debit our System.Configuration.ConfigurationManager.AppSettings['CompanyFullName'] ERA A/c No." + result.CompanyAccountNo.ToString() + " for a ");
                Content.Append(" total sum Rs." + result.Amount.ToString() + " (" + result.AmountInWords.ToString() + ") credit our employees as per the annexure attached. </p>");
                Content.Append(" <p style='text-indent:25px; text-align:justify;'> We certify that the account numbers, amounts and attendant names of the beneficiaries mentioned in the soft copy will be same as those mentioned in the hard copy and the Bank will not be responsible for any ");
                Content.Append(" mismatch between the hard copy and the data in the floppy and / or any error in credits arising out of any such mismatch </p><br /><br /> </div>");
                Content.Append(" <div style='width:100%;'> <table id='tblsig' width='100%' style='font-size: 13px;'> <tr> <td style='width:50%;'>For System.Configuration.ConfigurationManager.AppSettings['CompanyFullName']</td>");
                Content.Append(" <td style='width:50%; text-align:right;'>For System.Configuration.ConfigurationManager.AppSettings['CompanyFullName']</td> </tr>");
                Content.Append(" <tr> <td style='line-height:50px;'><br /></td> <td style='line-height:50px;'><br /></td> </tr>");

                Content.Append(" <tr> <td style='width:50%;'>Authorized Signatory</td> <td style='width:50%; text-align:right;'>Authorized Signatory</td> </tr> </table> </div>");
                Content.Append(" <p style='page-break-before:always'></p> <br /> <br />");

                if (result.DetailArray.Count > 0)
                {
                    Content.Append(" <div style='margin-top:40px;'> <table style='border-top: 1px solid black; border-left: 1px solid black; font-size:13px;' id='tbldet' width='100%'> <thead> <tr> <th style='width:10%; border-bottom: 1px solid black; border-right: 1px solid black;'> Sl No </th>");
                    Content.Append(" <th style='width:15%; border-bottom: 1px solid black; border-right: 1px solid black;'> Emp ID </th> <th style='width:30%; border-bottom: 1px solid black; border-right: 1px solid black;'> Emp Name </th>");
                    Content.Append(" <th style='width:25%; max-width:150px; border-bottom: 1px solid black; border-right: 1px solid black;'> Acc No </th> <th style='width:15%; border-bottom: 1px solid black; border-right: 1px solid black;'> Amount </th> </tr> </thead> <tbody>");
                    foreach (var innerDet in result.DetailArray)
                    {
                        Content.Append(" <tr> <td align='center' style='border-bottom: 1px solid black; border-right: 1px solid black;'>(++icount)</td> <td style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.VendorCode</td> <td style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.NameOfVendor</td> ");
                        Content.Append(" <td style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.BankAccountNo</td> <td align='right' style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.Amount</td> </tr>");
                    }
                    Content.Append(" </tbody> <tfoot> <tr> <th align='right' style='border-bottom: 1px solid black; border-right: 1px solid black;' colspan='4'> Total </th> <th align='right' style='border-bottom: 1px solid black; border-right: 1px solid black;'> " + result.Amount.ToString() + " </th> </tr> </tfoot> </table> </div>");
                }
            }
            Content.Append(" </body> </html>");

            return Content.ToString();

        }

        public string TmpDT_HDFC_ERA(ERATemplate result)
        {
            StringBuilder Content = new StringBuilder("");
            Content.Append(" <html> <head> <style> #tblsig > tbody > tr > td { margin: 0px !important; white-space: normal; word-break: break-all; } #tbldet > tbody > tr > td, #tbldet > thead > tr > th { border-bottom: 1px solid black; } </style> </head>");
            Content.Append(" <body style='font-family:Verdana; font-size:13px !important;'> <br /> <br /> <br /> <br /> <br /> <br /> <br />");
            if (result != null)
            {
                Content.Append(" <br /> <div style='width:80%;'> Date:" + result.Date.ToString() + "<br />" + result.LetterNo.ToString() + " </div><br /><br />");
                Content.Append("<div style='width:100%; white-space: normal; word-break: break-all;'> To,<br /> The Branch Manager<br /> HDFC Bank Limited<br /> 115  Radha krishnan Salai,<br /> 3rd floor, (Opp. Kalyani Hospital,)<br /> Mylapore,<br /> Chennai 600 004. </div><br /><br /> <span>Kind Attn:" + result.KindAttan.ToString() + "</span><br /><br />");
                Content.Append(" <div> <span>Dear Sir/Madam,</span><br /><br /> <span><b>Sub:  Account Transfer</b></span><br /><br /> <p style='text-indent:25px; text-align:justify;'>");
                Content.Append(" We request you to make the payments as per the attached sheet through funds transfer and debit our account System.Configuration.ConfigurationManager.AppSettings['CompanyFullName']- Exp Account No:" + result.CompanyAccountNo.ToString() + "  for a total amount of Rs" + result.Amount.ToString() + " (" + result.AmountInWords.ToString() + ") </p><br />");
                Content.Append(" <span> Thanking You </span> <br /><br /> </div> <div style='width:100%;'> <table id='tblsig' width='100%' style='font-size: 13px;'> <tr> <td>For System.Configuration.ConfigurationManager.AppSettings['CompanyFullName']</td> </tr>");
                Content.Append(" <tr> <td style='line-height:50px;'><br /></td> </tr> <tr> <td>Authorized Signatory</td> </tr> </table> </div> <p style='page-break-before:always'></p> <br /> <br />");
                if (result.DetailArray.Count > 0)
                {
                    Content.Append(" <div> <table style='border-top: 1px solid black; border-left: 1px solid black; font-size:13px;' id='tbldet' width='100%'> <thead> <tr> <th style='width:10%; border-bottom: 1px solid black; border-right: 1px solid black;'> S.No. </th>");
                    Content.Append(" <th style='width:30%; border-bottom: 1px solid black; border-right: 1px solid black;'> Name of the vendor </th> <th style='width:25%; max-width:150px; border-bottom: 1px solid black; border-right: 1px solid black;'> Bank Account Number </th>");
                    Content.Append(" <th style='width:25%; max-width:150px; border-bottom: 1px solid black; border-right: 1px solid black;'> Bank IFSC code </th> <th style='width:15%; border-bottom: 1px solid black; border-right: 1px solid black;'> Amount </th>");
                    Content.Append(" <th style='width:15%; max-width:190px; border-bottom: 1px solid black; border-right: 1px solid black;'> Remittance Details </th> </tr> </thead> <tbody>");
                    foreach (var innerDet in result.DetailArray)
                    {
                        Content.Append("<tr> <td align='center' style='border-bottom: 1px solid black; border-right: 1px solid black;'>(++icount)</td> <td style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.NameOfVendor</td> <td style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.BankAccountNo</td>");
                        Content.Append(" <td align='right' style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.IFSCCode</td> <td align='right' style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.Amount</td> <td style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.RemittanceDetails</td> </tr>");
                    }
                    Content.Append(" </tbody> <tfoot> <tr> <th align='right' style='border-bottom: 1px solid black; border-right: 1px solid black;' colspan='4'> Total </th>");
                    Content.Append(" <th align='right' style='border-bottom: 1px solid black; border-right: 1px solid black;'> " + result.Amount.ToString() + " </th> <th align='right' style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;</th> </tr> </tfoot> </table> </div>");
                }
            }
            Content.Append("</body> </html>");
            return Content.ToString();



        }

        public string TmpDT_ICICI(ERATemplate result)
        {
            StringBuilder Content = new StringBuilder("");
            Content.Append(" <html> <head> <style> #tblsig > tbody > tr > td { margin: 0px !important; white-space: normal; word-break: break-all; } #tbldet > tbody > tr > td, #tbldet > thead > tr > th { border-bottom: 1px solid black; } </style> </head>");
            Content.Append("<body style='font-family:Verdana; font-size:13px !important;'> <br /> <br /> <br /> <br /> <br /> <br /> <br />");
            if (result != null)
            {
                Content.Append(" <div style='width:100%; text-align:right; padding-right:20px;'> Date:" + result.Date.ToString() + "<br /> " + result.LetterNo.ToString() + "</div><br /><br />");
                Content.Append(" <div style='width:100%; white-space: normal; word-break: break-all;'> To,<br /> The Branch Manager<br /> ICICI Bank Limited<br /> Salary & Reiembursement HUB (Central Processing Centre),<br /> Anna Salai<br /> Chennai. </div><br /><br />");
                Content.Append(" <div> <span>Dear Sir / Madam,</span><br /><br /> <span>Sub: Employee Reimbursement Credits</span><br /><br /> <p style='text-indent:25px; text-align:justify;'> We would like to do Employee Reimbursement Account (ERA) transfer for our Employees. ");
                Content.Append(" Please debit our System.Configuration.ConfigurationManager.AppSettings['CompanyFullName'] Expense A/c No." + result.CompanyAccountNo.ToString() + " for a total sum Rs." + result.Amount.ToString() + " (" + result.AmountInWords.ToString() + ") credit our employees as per the annexure attached. </p>");
                Content.Append(" <p style='text-indent:25px; text-align:justify;'> We certify that the account numbers, amounts and attendant names of the beneficiaries mentioned in the soft copy will be same as those mentioned in the hard copy and the Bank will not be responsible for any mismatch between the hard copy and the data in the floppy and / or any error in credits arising out of any such mismatch </p><br /><br /> </div>");
                Content.Append(" <div style='width:100%;'> <table id='tblsig' width='100%' style='font-size: 13px;'> <tr> <td style='width:50%;'>For System.Configuration.ConfigurationManager.AppSettings['CompanyFullName']</td> <td style='width:50%; text-align:right;'>For System.Configuration.ConfigurationManager.AppSettings['CompanyFullName']</td> </tr>");
                Content.Append(" <tr> <td style='line-height:50px;'><br /></td> <td style='line-height:50px;'><br /></td> </tr> <tr> <td style='width:50%;'>Authorized Signatory</td> <td style='width:50%; text-align:right;'>Authorized Signatory</td> </tr> </table> </div> <p style='page-break-before:always'></p> <br /> <br />");
                if (result.DetailArray.Count > 0)
                {
                    Content.Append(" <div style='margin-top:40px;'> <table style='border-top: 1px solid black; border-left: 1px solid black; font-size:13px;' id='tbldet' width='100%'> <thead>");
                    Content.Append(" <tr> <th style='width:10%; border-bottom: 1px solid black; border-right: 1px solid black;'> Sl No </th> <th style='width:15%; border-bottom: 1px solid black; border-right: 1px solid black;'> Emp ID </th> <th style='width:30%; border-bottom: 1px solid black; border-right: 1px solid black;'> Emp Name </th>");
                    Content.Append(" <th style='width:25%; max-width:150px; border-bottom: 1px solid black; border-right: 1px solid black;'> Acc No </th> <th style='width:15%; border-bottom: 1px solid black; border-right: 1px solid black;'> Amount </th> </tr> </thead> <tbody>");
                    foreach (var innerDet in result.DetailArray)
                    {
                        Content.Append(" <tr> <td align='center' style='border-bottom: 1px solid black; border-right: 1px solid black;'>(++icount)</td> <td style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.VendorCode</td> <td style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.NameOfVendor</td>");
                        Content.Append(" <td style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.BankAccountNo</td> <td align='right' style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.Amount</td> </tr>");
                    }
                    Content.Append(" </tbody> <tfoot> <tr> <th align='right' style='border-bottom: 1px solid black; border-right: 1px solid black;' colspan='4'> Total </th> <th align='right' style='border-bottom: 1px solid black; border-right: 1px solid black;'> " + result.Amount.ToString() + " </th> </tr> </tfoot> </table> </div>");
                }
            }

            Content.Append(" </body> </html>");
            return Content.ToString();

        }
        
        public string TmpDT_SBI(ERATemplate result)
        {
            StringBuilder Content = new StringBuilder("");
            Content.Append(" <html> <head> <style> #tblsig > tbody > tr > td { margin: 0px !important; white-space: normal; word-break: break-all; } #tbldet > tbody > tr > td, #tbldet > thead > tr > th { border-bottom: 1px solid black; } </style> </head>");
            Content.Append(" <body style='font-family:Verdana; font-size:13px !important;'> <br /> <br /> <br /> <br /> <br /> <br /> <br />");
            if (result != null)
            {
                Content.Append(" <div style='width:100%; text-align:right; padding-right:20px;'> Date:" + result.Date.ToString() + "<br /> " + result.LetterNo.ToString() + " </div><br /><br />");
                Content.Append(" <div style='width:100%; white-space: normal; word-break: break-all;'> To,<br /> The Branch Manager<br /> State Bank of India<br /> Nelson Manickam Road Branch,<br /> 2/38, Railway Colony 3rd Street<br /> Aminjikarai,<br /> Chennai - 600 029. </div><br /><br />");
                Content.Append(" <div> <span>Dear Sir / Madam,</span><br /><br /> <span>Sub: Employee Reimbursement Credits</span><br /><br /> <p style='text-indent:25px; text-align:justify;'> We would like to do Employee Reimbursement Account (ERA) transfer for our Employees.");
                Content.Append(" Please debit our System.Configuration.ConfigurationManager.AppSettings['CompanyFullName'] ERA A/c No." + result.CompanyAccountNo.ToString() + " for a total sum Rs." + result.Amount.ToString() + " (" + result.AmountInWords.ToString() + ") credit our employees as per the annexure attached. </p>");
                Content.Append(" <p style='text-indent:25px; text-align:justify;'> We certify that the account numbers, amounts and attendant names of the beneficiaries mentioned in the soft copy will be same as those mentioned in the hard copy and the Bank will not be");
                Content.Append(" responsible for any mismatch between the hard copy and the data in the floppy and / or any error in credits arising out of any such mismatch. </p><br /><br /> </div>");
                Content.Append(" <div style='width:100%;'> <table id='tblsig' width='100%' style='font-size: 13px;'> <tr> <td style='width:50%;'>For System.Configuration.ConfigurationManager.AppSettings['CompanyFullName']</td> <td style='width:50%; text-align:right;'>For System.Configuration.ConfigurationManager.AppSettings['CompanyFullName']</td> </tr>");
                Content.Append(" <tr> <td style='line-height:50px;'><br /></td> <td style='line-height:50px;'><br /></td> </tr>");
                Content.Append(" <tr> <td style='width:50%;'>Authorized Signatory</td> <td style='width:50%; text-align:right;'>Authorized Signatory</td> </tr> </table> </div> <p style='page-break-before:always'></p> <br /> <br />");
                if (result.DetailArray.Count > 0)
                {
                    Content.Append(" <div style='margin-top:40px;'> <table style='border-top: 1px solid black; border-left: 1px solid black; font-size:13px;' id='tbldet' width='100%'> <thead> <tr> <th style='width:10%; border-bottom: 1px solid black; border-right: 1px solid black;'> Sl No </th>");
                    Content.Append(" <th style='width:15%; border-bottom: 1px solid black; border-right: 1px solid black;'> Emp ID </th> <th style='width:30%; border-bottom: 1px solid black; border-right: 1px solid black;'> Emp Name </th>");
                    Content.Append(" <th style='width:25%; max-width:150px; border-bottom: 1px solid black; border-right: 1px solid black;'> Acc No </th> <th style='width:15%; border-bottom: 1px solid black; border-right: 1px solid black;'> Amount </th> </tr> </thead> <tbody>");
                    foreach (var innerDet in result.DetailArray)
                    {
                        Content.Append(" <tr> <td align='center' style='border-bottom: 1px solid black; border-right: 1px solid black;'>(++icount)</td> <td style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.VendorCode</td> <td style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.NameOfVendor</td>");
                        Content.Append(" <td style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.BankAccountNo</td> <td align='right' style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.Amount</td> </tr>");
                    }
                    Content.Append(" </tbody> <tfoot> <tr> <th align='right' style='border-bottom: 1px solid black; border-right: 1px solid black;' colspan='4'> Total </th> <th align='right' style='border-bottom: 1px solid black; border-right: 1px solid black;'> " + result.Amount.ToString() + " </th> </tr> </tfoot> </table> </div>");
                }
            }
            Content.Append(" </body> </html>");
            return Content.ToString();

        }
        
        public string TmpDT_UTI(ERATemplate result)
        {
            StringBuilder Content = new StringBuilder("");
            Content.Append(" <html> <head> <style> #tblsig > tbody > tr > td { margin: 0px !important; white-space: normal; word-break: break-all; } #tbldet > tbody > tr > td, #tbldet > thead > tr > th { border-bottom: 1px solid black; } </style> </head>");
            Content.Append(" <body style='font-family:Verdana; font-size:13px !important;'> <br /> <br /> <br /> <br /> <br /> <br /> <br />");
            if (result != null)
            {
                Content.Append(" <div style='width:80%;'> Date:" + result.Date.ToString() + "<br /> " + result.LetterNo.ToString() + " </div><br /><br />");
                Content.Append(" <div style='width:100%; white-space: normal; word-break: break-all;'> To,<br />  The Branch Manager<br /> Axis Bank Limited<br /> No.82, Radha Krishnan Salai, <br /> Chennai - 600 004. </div><br /><br />");
                Content.Append(" <div> <span>Dear Sir,</span><br /><br /> <span>Sub: <u><b>Employee Reimbursement Credits.</b></u></span><br /><br /> <p style='text-indent:25px; text-align:justify;'> We would like to do Employee Reimbursement Account (ERA) transfer for our employees. ");
                Content.Append(" Please debit our System.Configuration.ConfigurationManager.AppSettings['CompanyFullName'] Expense A/c No:" + result.CompanyAccountNo.ToString() + " for a total sum of" + result.Amount.ToString() + "(" + result.AmountInWords.ToString() + " ) & credit our employees as per the details given below: </p> <br />");

                if (result.DetailArray.Count > 0)
                {
                    Content.Append(" <div> <table style='border-top: 1px solid black; border-left: 1px solid black; font-size:13px;' id='tbldet' width='100%'>");
                    Content.Append(" <thead> <tr> <th style='width:10%; border-bottom: 1px solid black; border-right: 1px solid black;'> Sl No </th> <th style='width:15%; border-bottom: 1px solid black; border-right: 1px solid black;'> Emp ID </th>");
                    Content.Append(" <th style='width:30%; border-bottom: 1px solid black; border-right: 1px solid black;'> Emp Name </th> <th style='width:25%; max-width:150px; border-bottom: 1px solid black; border-right: 1px solid black;'> Acc No </th>");
                    Content.Append(" <th style='width:15%; border-bottom: 1px solid black; border-right: 1px solid black;'> Amount </th> </tr> </thead> <tbody>");
                    foreach (var innerDet in result.DetailArray)
                    {
                        Content.Append(" <tr> <td align='center' style='border-bottom: 1px solid black; border-right: 1px solid black;'>(++icount)</td> <td style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.VendorCode</td>");
                        Content.Append(" <td style=' border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.NameOfVendor</td> <td style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.BankAccountNo</td> <td align='right' style='border-bottom: 1px solid black; border-right: 1px solid black;'>&nbsp;innerDet.Amount</td> </tr>");
                    }
                    Content.Append(" </tbody> <tfoot> <tr> <th align='right' style='border-bottom: 1px solid black; border-right: 1px solid black;' colspan='4'> Total </th> <th align='right' style='border-bottom: 1px solid black; border-right: 1px solid black;'> " + result.Amount.ToString() + " </th> </tr> </tfoot> </table> </div>");
                }
                Content.Append(" </div><br /> <div style='width:100%;'> <table width='100%' style='font-size: 13px;'>");
                Content.Append(" <tr> <td style='width:50%;'>For System.Configuration.ConfigurationManager.AppSettings['CompanyFullName']</td> <td style='width:50%; text-align:right;'> For System.Configuration.ConfigurationManager.AppSettings['CompanyFullName'].</td> </tr>");
                Content.Append(" <tr> <td style='line-height:50px;'><br /></td> <td style='line-height:50px;'><br /></td> </tr>");
                Content.Append(" <tr> <td style='width:50%;'>Authorized Signatory</td> <td style='width:50%; text-align:right;'>Authorized Signatory</td> </tr> </table> </div>");
            }
            Content.Append(" </body> </html>");
            return Content.ToString();
        }
  

    }

    public class EFTDataModel
    {
        public string Id { get; set; }
        public string subId { get; set; }
        public string PayBankGId { get; set; }
        public string PvIds { get; set; }
        public string LoginUserId { get; set; }
    }


}