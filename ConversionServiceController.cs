# ControlWindowServicesUI
Controls start,stop,restart the services through UI
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceProcess;
using System.Threading;
using System.Web.Http;
using ConversionService;
using System.Web;
using System.Globalization;

namespace ConversionService.Controllers
{
    
    public class ConversionServiceController : ApiController
    {


          

        [HttpGet]
        public  void ServiceHandler()
        {

            var id = HttpContext.Current.Request.QueryString.Get("id");
            string serviceName = "";
            if (id == "START SERVICE")
            {
                //ProcessStartInfo startInfo = new ProcessStartInfo();
                //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //startInfo.FileName = "cmd.exe";
                //startInfo.Arguments = "/C sc start ConversionService";

                //Process process = new Process();
                //process.StartInfo = startInfo;
                //process.Start();

                serviceName = "ConversionService";
                ProcessStartInfo processstartInfo = new ProcessStartInfo("sc.exe");
                processstartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "start {0} ", serviceName);
                processstartInfo.RedirectStandardOutput = true;
                processstartInfo.UseShellExecute = false;
                using (System.Diagnostics.Process p = new System.Diagnostics.Process())
                {
                    p.StartInfo = processstartInfo;
                    if (p.Start())
                    {
                        p.WaitForExit();
                    }
                } 

             
                
            }
            else if (id == "STOP SERVICE")
            {
                serviceName = "ConversionService";
                ProcessStartInfo processstartInfo = new ProcessStartInfo("sc.exe");
                processstartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "stop {0} ", serviceName);
                processstartInfo.RedirectStandardOutput = true;
                processstartInfo.UseShellExecute = false;
                using (System.Diagnostics.Process p = new System.Diagnostics.Process())
                {
                    p.StartInfo = processstartInfo;
                    if (p.Start())
                    {
                        p.WaitForExit();
                    }
                } 
                
                
            }
          
        }
  
    }
}
