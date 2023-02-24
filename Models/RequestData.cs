using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
namespace Upload.Models
{
    public class RequestData
    {
        public string FileString { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
    }
    public class proLib
    {
        public string DownloadMemoUrl
        {
            get
            {
                string x = "";
                try
                {
                    //x = System.Configuration.ConfigurationManager.AppSettings["DownloadMemoLocal"].ToString();
                    x= WebConfigurationManager.AppSettings["DownloadMemoLocal"];
                }
                catch { x = ""; }
                return (x == null || x.Trim() == "") ? "" : x;
            }
        }
        public string MemoDownloadUrl
        {
            get
            {
                string x = "";
                try
                {
                    //x = System.Configuration.ConfigurationManager.AppSettings["MemoLocal"].ToString();
                    x = WebConfigurationManager.AppSettings["MemoLocal"];
                }
                catch { x = ""; }
                return (x == null || x.Trim() == "") ? "" : x;
            }
        }

        public string EncryptMemoDownloadUrl
        {
            get
            {
                string x = "";
                try
                {
                    //x = System.Configuration.ConfigurationManager.AppSettings["MemoLocalEncrypt"].ToString();
                    x = WebConfigurationManager.AppSettings["MemoLocalEncrypt"];
                }
                catch { x = ""; }
                return (x == null || x.Trim() == "") ? "" : x;
            }
        }
    }
     
}