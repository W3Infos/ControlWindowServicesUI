# ControlWindowServicesUI
# 1.Create a empty API Controller with a method paste below code
# 2. Create a HTML file paste code from compy froom index.html file which is

Controls start,stop,restart the services through UI
 var id = HttpContext.Current.Request.QueryString.Get("id");
            string serviceName = "";
            if (id == "START SERVICE")
            {

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
