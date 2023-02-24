using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upload.Models
{
    public class MemoDetail
    {
        public string BranchDetails { get; set; }
        public string Mode { get; set; }
        public string AccountType { get; set; }
        public string PVDate { get; set; }
        public string PVNo { get; set; }
        public decimal TotalAmount { get; set; }
        public string Amount { get; set; }
        public string BatchNo { get; set; }
        public string PvId { get; set; }
        public string MemoNo { get; set; }
        public string MemoDate { get; set; }
        public string MemoTime { get; set; }
        public string EmployeeSupplierCode { get; set; }
        public string EmployeeSupplierName { get; set; }
        public string BenName { get; set; }
        public string BenAccNo { get; set; }
        public string BenAddress { get; set; }
        public string BenBankname { get; set; }
        public string BenBankIfscCode { get; set; }
        public string AmountInFigures { get; set; }
        public string AmountInWords { get; set; }
        public string RemitterName { get; set; }
        public string RemitterAccNo { get; set; }
        public string RemitterDetails { get; set; }
        public string RemitterMobilePhoneNo { get; set; }
        public string RemitterAddress { get; set; }
        public string RemitterCashDeposited { get; set; }
        public string RemitterEmailId { get; set; }
        public string Remarks { get; set; }
        public string IsShowTable { get; set; }
        public string RemitterCode { get; set; }
        public string CompanyAccNo { get; set; }
        public string BankAddress { get; set; }
        public List<InnerDetails> DetailArray { get; set; }
    }

    public class InnerDetails
    {
        public string NameOfVendor { get; set; }
        public string BankAccNo { get; set; }
        public string BankName { get; set; }
        public string IFSCCode { get; set; }
        public string Amount { get; set; }
        public string RemitterDetails { get; set; }
    }

    public class DDTemplate
    {
        public string Date { get; set; }
        public string LetterNo { get; set; }
        public string BankAddress { get; set; }
        public string CompanyAccountNo { get; set; }
        public string DDFavoring { get; set; }
        public string PayableAt { get; set; }
        public string Amount { get; set; }
        public string AmountInWords { get; set; }
        public decimal Totalamount { get; set; }
        public List<DDInnerDetails> DetailArray { get; set; }
    }

    public class DDInnerDetails
    {
        public string DDFavoring { get; set; }
        public string PayableAt { get; set; }
        public string Amount { get; set; }
    }

    public class ERATemplate
    {
        public string Date { get; set; }
        public string LetterNo { get; set; }
        public string BankAddress { get; set; }
        public string CompanyAccountNo { get; set; }
        public string Amount { get; set; }
        public string AmountInWords { get; set; }
        public string KindAttan { get; set; }
        public List<ERAInnerDetails> DetailArray { get; set; }
    }

    public class ERAInnerDetails
    {
        public string VendorCode { get; set; }
        public string NameOfVendor { get; set; }
        public string BankAccountNo { get; set; }
        public string Amount { get; set; }
        public string RemittanceDetails { get; set; }
        public string IFSCCode { get; set; }
    }
}