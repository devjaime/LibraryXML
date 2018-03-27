//===============================================================================
//.NET Framework Developer's Guide   
//
//Creating GenericPrincipal and GenericIdentity Objects
//===============================================================================
// Author: Magno Cardona Heck m@vs.cl
// Component: Genereic Securite Role Base System 
//===============================================================================

using System;
using System.Data;
using System.Globalization;
using System.Security.Principal;
using System.Threading;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Drawing;

namespace library.Security {
    public class GenericSecurity {

        public static string[] SplitStrToArray(string Str, string delimStr) {
            char[] delimiter=delimStr.ToCharArray();
            string[] split=null;
            split=Str.Split(delimiter);
            return split;
        }

        public static int StoreRole(string Identity, string role) {

            //Lista De Roles que el usuario tiene. = role

            string delimStr=",";
            char[] delimiter=delimStr.ToCharArray();

            string[] split=null;
            split=GenericSecurity.SplitStrToArray(role, ",");

            //Create generic identity.
            GenericIdentity MyIdentity=new GenericIdentity(Identity);

            //Create generic principal.
            String[] MyStringArray=split;
            GenericPrincipal MyPrincipal=new GenericPrincipal(MyIdentity, MyStringArray);

            //Attach the principal to the current thread.
            //This is not required unless repeated validation must occur,
            //other code in your application must validate, or the 
            // PrincipalPermisson object is used. 
            Thread.CurrentPrincipal=MyPrincipal;

            //String Name =  MyPrincipal.Identity.Name;
            //bool Auth =  MyPrincipal.Identity.IsAuthenticated; 
            //bool IsInRole =  MyPrincipal.IsInRole("Admin");


            return 0;
        }

        public static bool IsInRole(string role) {
            return Thread.CurrentPrincipal.IsInRole(role);
        }

        public static bool IsAuthenticated() {
            return Thread.CurrentPrincipal.Identity.IsAuthenticated;
        }

        public static string WhoseIdentity() {
            return Thread.CurrentPrincipal.Identity.Name;
        }


        public static void SessionNameFinishRedirect(Page Me, string SessionName) {
            string pag = Me.Request.Url.AbsoluteUri;
            try {
                if(Me.Session[SessionName]==null) {
                    pag = pag.Replace( "&", "|" );
                    //Me.Response.Redirect("frm_sys_logout.htm");
                    Me.Response.Redirect("frm_sys_login.aspx?url="+pag);
                    //Me.Response.End();
                }
            } catch(NullReferenceException e) {
                Console.WriteLine("{0} Caught Security exception #1.", e);
                Me.Response.Redirect("frm_sys_logout.htm");
                Me.Response.End();
            }
        }

        public static void SessionNameFinishRedirectADM(Page Me, string SessionName) {
            try {
                if(Me.Session[SessionName].Equals(false)) {
                    Me.Response.Redirect("frm_sys_logout.htm");
                    Me.Response.End();
                }
            } catch(NullReferenceException e) {
                Console.WriteLine("{0} Caught Security exception #1.", e);
                Me.Response.Redirect("frm_sys_logout.htm");
                Me.Response.End();
            }
        }

        public static void SessionNameFinishRedirect(Page Me, string SessionName, string UrlJump) {
            try {
                if(Me.Session[SessionName]==null) {
                    Me.Response.Redirect(UrlJump);
                    Me.Response.End();
                }
            } catch(NullReferenceException e) {
                Console.WriteLine("{0} Caught Security exception #1.", e);
                Me.Response.Redirect(UrlJump);
                Me.Response.End();
            }
        }

        public static void CheckPgBySession(Page Me, string PageRole) {

        }

        public static void CheckPg(Page Me, string PageRole) {

            //bool IsInRole = false;
            string valor=null;
            try {
                //valor = Me.Server.HtmlEncode(Me.Request.Cookies[Name].Value);
                valor=Me.Server.HtmlEncode(Me.Request.Cookies["SYS_LOGIN"].Value);
            } catch(NullReferenceException e) {
                Console.WriteLine("{0} Caught Security exception #1.", e);
                Me.Response.Redirect("frm_sys_logout.htm");
                Me.Response.End();
            }

            string Logged=(string)(Me.Server.HtmlEncode(Me.Request.Cookies["SYS_LOGIN"].Value));
            string MyIdentityId=(string)(Me.Server.HtmlEncode(Me.Request.Cookies["SYS_MyIdentityId"].Value));
            string MyIdentityLogin=(string)(Me.Server.HtmlEncode(Me.Request.Cookies["SYS_MyIdentityFullName"].Value));
            string MyIdentityFullName=(string)(Me.Server.HtmlEncode(Me.Request.Cookies["SYS_MyIdentityFullName"].Value));
            //string MyIdentityEmail = (string)( Me.Server.HtmlEncode(Me.Request.Cookies["SYS_MyIdentityEmail"].Value) );
            string MyIdentityRoles=(string)(Me.Server.HtmlEncode(Me.Request.Cookies["SYS_MyIdentityRoles"].Value));



            if(Logged.Equals("true")) {
                GenericSecurity.StoreRole(MyIdentityId, MyIdentityRoles);

                //Once its log Check For Roles Access. PageRole Contains all roles a user can have to access.
                //				String[] PageRolesArray = null;
                //				PageRolesArray = SplitStrToArray(PageRole,",");


                //				foreach(string ItemRole in PageRolesArray) 
                //				{
                //					IsInRole = IsInRole || GenericSecurity.IsInRole(ItemRole);
                //				}
                //			
                //				if (!IsInRole) 
                //				{
                //					Me.Response.Redirect("frm_sys_logout.htm");
                //				}


            } else {
                Me.Response.Redirect("frm_sys_logout.htm");
            }
        }
    }
}