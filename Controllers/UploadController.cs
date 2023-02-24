using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Upload.Models;
using System.Web;
using System.Web.Configuration;

namespace Upload.Controllers
{
    public class UploadController : ApiController
    {

        [HttpPost]
        public IHttpActionResult FileSave(RequestData data)
        {
            try
            {
                var path = WebConfigurationManager.AppSettings["FileUpload"];
                byte[] buf = Convert.FromBase64String(data.FileString);

                File.WriteAllBytes(string.Format(path + "{0}", data.FileName, data.Extension), buf);
                return Content(HttpStatusCode.OK, true);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        [HttpGet]
        public IHttpActionResult FileGet(string filename)
        {
            try
            {
                //UrlEncoder urlencode = new UrlEncoder();
                //filename = urlencode.Decrypt(filename);
                var path = WebConfigurationManager.AppSettings["FileUpload"];
                byte[] buf = File.ReadAllBytes(string.Format(path + "{0}", filename));
                string data1 = Convert.ToBase64String(buf);
                return Content(HttpStatusCode.OK, data1);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
