using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;



using library.common;
using library.xml;
using System.Xml;
using library.Error;
using library.Security;


namespace library.Error {
    /// <summary>
    /// Descripción breve de Error.
    /// </summary>
    public class MyError {
        public MyError() {
            //
            // TODO: agregar aquí la lógica del constructor
            //
        }

        //public static void GrabarError ( Exception ex ) 
        //{
        //     ExceptionPolicy.HandleException(ex, "BDError");
        //     ExceptionPolicy.HandleException(ex, "EmailError");
        //}

        //public static void GrabarError ( Exception ex, string strError ) 
        //{
        //    ExceptionPolicy.HandleException(ex, "BDError");
        //    ExceptionPolicy.HandleException(ex, "EmailError");
        //    throw new MyException(strError);
        //}

        /*public static void ErrorGeneral(Exception ex, string strError, string strPage, Page Me) {
            //Iniciar Variables 
            XmlDocument XmlFinal=null;
            if(strHelper.isEmpty(strPage)) {
                strPage=cnfHelper.GetConfigAppVariable("pageDefaultError");
            }
            if(strHelper.isEmpty(strError)) {
                strError=cnfHelper.GetConfigAppVariable("generalError");
            }
            //Crear XML Schema 
            XmlFinal=XmlHelper.CrearPaginaEsquema(XmlFinal, Me);
            XmlFinal=XmlHelper.AgregaNodoConTexto(XmlFinal, "//app", "msgError", strError);

            //Imprimir Pantalla de HTML 
            //XmlHelper.appDibujarHTML(XmlFinal, Me.Request.MapPath(strPage) ,Me); 
            Me.Response.Redirect("frm_sys_recurso_error.aspx?msgError="+strError);
            //Limpiar Todo 
            XmlFinal=null;

            Me.Response.End();
        }*/

    }
}