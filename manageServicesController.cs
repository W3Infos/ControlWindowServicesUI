using ApolloCare.API.Entities;
using ApolloCare.Core.Interfaces.Service.Administration;
using ApolloCare.Core.Interfaces.Service.Guidelines;
using ApolloCare.Data.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Claims;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceProcess;
using System.Web;
using System.Web.Http;
using System.Management;
using System.Configuration;
using System.Collections;
namespace ApolloCare.API.Controllers
{
     [RoutePrefix("api/manageServices")]
    public class manageServicesController : ApiController
    {
         

             IAspNetUserService _userService;
             IRoleService _roleService;
             IUserRoleService _userroleService;
             IUserMenuService _usermenuService;
             IProfileService _profileService;
             IGuidelineService _currentService;
             string portalTempleteUrl;
             private string StrUserID = "";
             private Guid UserID = new Guid();
             private Nullable<int> currentUserType = 1;

             ServiceController sc;
             ManageServicesView serviceView = new ManageServicesView();

             //public List<ManageServicesView> items;
             //ArrayList list = new ArrayList();
       



             public manageServicesController(IAspNetUserService aspnetuserService, IRoleService roleService, IUserRoleService userroleService, IUserMenuService usermenuService, IProfileService profileService, IGuidelineService currentService)
             {
                 _userService = aspnetuserService;
                 _roleService = roleService;
                 _userroleService = userroleService;
                 _usermenuService = usermenuService;
                 _profileService = profileService;
                 _currentService = currentService;
                 portalTempleteUrl = System.Configuration.ConfigurationManager.AppSettings["portalUrl"];
                 this.UserID = getUserId();
                 this.StrUserID = UserID.ToString();
             }

           
             [Authorize]
             [Route("getDefaults")]
             [HttpPost]
             public HttpResponseMessage getDefaults()
             {

                 UserView filter = new UserView();
                 gridDefauls gd = new gridDefauls();
                 gd.columns = _userService.GetColumns("manageservices", null, currentUserType);
                 gd.filters = new List<UserView>();
                 gd.filters.Add(filter);
                 gd.pageTitle = "Manage Services";
                 return new HttpResponseMessage()
                 {
                     Content = new ObjectContent<gridDefauls>(gd, new System.Net.Http.Formatting.JsonMediaTypeFormatter())
                 };
             }
             //Service controls functionality started here


          
             [Authorize]
             [HttpPost]
             [Route("ServiceDetails")]
             public List<ManageServicesView> ServiceDetails(ManageServicesView Post)
             {

                 //ServiceController[] scServices;
                 //scServices = ServiceController.GetServices();
                 List<ManageServicesView> Result = new List<ManageServicesView>();
                  
                 //Get configured services
                 serviceView.ServicesList = ConfigurationManager.AppSettings["Services"].Split(',');
                 ServiceController service = new ServiceController();

                 // Get the services details which are configured
                 foreach (var item in serviceView.ServicesList)
                 {
                     try
                     {
                         ManageServicesView PullData = new ManageServicesView();

                         //Get service details
                         service.ServiceName = item;
                         PullData.ServiceStatus = service.Status.ToString();
                         PullData.DisplayName = service.DisplayName;
                         PullData.ServiceName = service.ServiceName;

                         if (service.MachineName == ".")
                             PullData.ServiceUser = "Local System";

                         var scObj = new ManagementObject(new ManagementPath(string.Format("Win32_Service.Name='{0}'", service.ServiceName)));
                         if (scObj["Description"] != null)
                             PullData.Description = scObj["Description"].ToString();
                         else
                             PullData.Description = null;

                         //Adding service details to Result list
                         Result.Add(PullData);
                         
                      
                     }
                     catch (Exception ex)
                     {
                         writeLogs(service.ServiceName, ex.Message, DateTime.Now);
                     }

                    
                 }//Ends foreach loop

                 //return result list

               
                 return Result ;


             }
             
             [Authorize]
             [Route("ServiceControls")]
             [HttpPost]
             [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
             [SuppressUnmanagedCodeSecurity]
             public ManageServicesView ServiceControls(ManageServicesView Post)
             {
                 ManageServicesView serviceView = new ManageServicesView();
                 sc = new ServiceController(Post.ServiceName);

                 if (Post.ServiceStatus == "Running")
                 {


                     try
                     {
                       
                         if (sc.Status == ServiceControllerStatus.Running)
                         {
                             TimeSpan timeout = TimeSpan.FromMilliseconds(5000);
                             sc.Stop();
                             sc.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                             serviceView.ServiceName = Post.ServiceName;
                             serviceView.ServiceStatus = Post.ServiceStatus;
                             serviceView.ServiceUser = Post.ServiceUser;

                             writeLogs(serviceView.ServiceName, "Stopped successfully", DateTime.Now);
                         }



                     }
                     catch (SystemException ex)
                     {
                         writeLogs(Post.ServiceName, ex.Message, DateTime.Now);
                     }

                 }
                 else if (Post.ServiceStatus == "Stopped")
                 {
                     try.
                     {
                         //sc = new ServiceController(Post.ServiceName);
                         if (sc.Status != ServiceControllerStatus.Running)
                         {
                             TimeSpan timeout = TimeSpan.FromMilliseconds(5000);
                             sc.Start();
                             sc.WaitForStatus(ServiceControllerStatus.Running, timeout);
                             serviceView.ServiceName = Post.ServiceName;
                             serviceView.ServiceStatus = Post.ServiceStatus;
                             serviceView.ServiceUser = Post.ServiceUser;

                             writeLogs(serviceView.ServiceName, "Started successfully", DateTime.Now);
                         }



                     }
                     catch (SystemException ex) 
                     {
                         writeLogs(sc.ServiceName, ex.Message, DateTime.Now);
                     }
                    

                 }
                 else if (Post.ServiceStatus == "Restart")
                 {
                     try
                     {
                         //if services running then stop and start service(Restart)
                       if (sc.Status == ServiceControllerStatus.Running)
                       {
                           TimeSpan timeout = TimeSpan.FromMilliseconds(5000);
                           sc.Stop();
                           sc.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                           sc.Start();
                           sc.WaitForStatus(ServiceControllerStatus.Running, timeout);

                       }else if(sc.Status != ServiceControllerStatus.Running)
                      {
                          //if services stopped then start service(Restart)
                          TimeSpan timeout = TimeSpan.FromMilliseconds(5000);
                          sc.Start();
                          sc.WaitForStatus(ServiceControllerStatus.Running, timeout);
                      }

                       writeLogs(serviceView.ServiceName, "Restarted successfully", DateTime.Now);
                     }
                     catch (Exception ex)
                     {
                         writeLogs(sc.ServiceName, ex.Message, DateTime.Now);
                     }
                      

                 }

                 return serviceView;
             }



             //Service controls functionality ended here
            

             private Guid getUserId()
             {
                 var identity = (ClaimsIdentity)User.Identity;

                 UserView user = new UserView();
                 user.UserName = identity.Name;
                 user = _userService.GetUser(user);
                 Guid result = Guid.Empty;
                 if (user != null)
                 {
                     currentUserType = user.UserType;
                     result = Guid.Parse(user.Id);
                 }
                 return result;
             }



             public void writeLogs(string ServiceName,string ErrorMessage,DateTime dt)
             {
                 string filePath = ConfigurationManager.AppSettings["ServicesLog"] + "\\ServicesLog.txt";
                 if (!File.Exists(filePath))
                 {
                     File.Create(filePath).Dispose();
                 }
                 using (StreamWriter sw = File.AppendText(filePath))
                 {
                     sw.WriteLine("--------------------------------------------------Logging ----------------------------------");
                     sw.WriteLine("-------------------------------------------------" + dt + "---------------------------------");
                     sw.WriteLine("ServiceName: " + ServiceName);
                     sw.WriteLine("Message: " + ErrorMessage);
                     sw.WriteLine("-------------------------------------------------" + dt + "---------------------------------");

                 }

             }




        //public static SecureString ToSecureString(this string plainString)
        //{
        //    if (plainString == null)
        //        return null;

        //    SecureString secureString = new SecureString();
        //    foreach (char c in plainString.ToCharArray())
        //    {
        //        secureString.AppendChar(c);
        //    }
        //    return secureString;
        //}
         
    }
}
