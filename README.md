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
# -------------------------------------------------------------
# HTML and JS code :
# ----------------------------------------------------------------

<body style="margin:50px 500PX;">
    <div class="container">

        <input id="start" type="button" class="btn btn-success serviceCmd" value="START SERVICE" />

        <input id="stop" type="button" class="btn btn-danger serviceCmd" value="STOP SERVICE" />
        
    </div>


    <footer>
        <script src="Scripts/jquery-1.10.2.min.js"></script>
        <script src="Scripts/bootstrap.min.js"></script>
        <script type="text/javascript">
            //var trs = $.noConflict();
            $(document).ready(function () {
                console.log('DOM ready');
                $(".serviceCmd").each(function () {
                    $(this).click(function () {
                        var id = $(this).attr("value");
                        $.ajax({
                            url: "http://localhost:1672/api/ConversionService/ServiceHandler?id=" + id,
                            type: "GET",
                            success: function () {
                            },
                            error: function () {
                            }
                        });
                    });//click ends
                });
                
            })//END DOM
        </script>

    </footer>
</body>
