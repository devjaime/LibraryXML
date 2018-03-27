//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Data Access Application Block
// Component: Globals 
//===============================================================================

using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Web.UI;
using library.xml;
using System.Text.RegularExpressions;
using library.CSharpInformation;

using System.Collections.Specialized;
using System.IO;
//using System.Web.Mail;
using System.Net.Mail;

namespace library.common {

    /// <summary>
    /// string Helper Functions.
    /// </summary>

    /*public class errorEmail {

        private string from = "test@web.cl";
        private string mailTo = "";
        private string ccTo = "";
        private string subjectTo = "";

        public errorEmail() {
            mailTo = ConfigurationSettings.AppSettings["mailTo"];
            ccTo = ConfigurationSettings.AppSettings["ccTo"];
            subjectTo = ConfigurationSettings.AppSettings["subjectTo"];
        }

        private static string CurrentDateAndTimeInFileFormat() {
            string CurrDate = DateTime.Now.ToString();
            CurrDate = CurrDate.Replace( '/', '-' );
            return CurrDate = CurrDate.Replace( ':', '.' );

        }

        public bool enviarErrorEmail( Exception ex ) {
            //This code will check configfile Email options is exists to notify the Error

            if( this.mailTo.Length > 0 ) {
                string body = ex.Message + "\n" + ex.StackTrace + ex.TargetSite;
                SmtpMail.Send( this.mailTo, this.from, this.subjectTo, body );
                return true;
            } else {
                return false;
            }

        }
    }*/

    /// <summary>
    /// string Helper Functions.
    /// </summary>
    public class strHelper {
        public strHelper() {
        }
        /// <summary>
        /// Coloca ceros delante del rut. y le saca el dv.
        /// </summary>
        /// <param name="Rut"></param>
        /// <returns></returns>
        public static string FormatRut( string Rut ) {
            return colocaCeros( RutEntero( Rut ), "10" );
        }

        public static string FormatCifras( string Cifra ) {
            //La Cifra
            string strCifra = "";
            //El formato
            int largoCifra = Cifra.Length;
            //Encontrar el separador
            //ej 898799,999999,$999.999
            for( int i = largoCifra; i >= 0; i++ ) {
                if( i % 3 == 0 ) {
                    strCifra = strCifra + "." + Cifra.Substring( i, 1 );
                } else {
                    strCifra = strCifra + Cifra.Substring( i, 1 );
                }
            }

            return strCifra;
        }

        /// <summary>
        /// Saca el dv del rut
        /// </summary>
        /// <param name="RUT"></param>
        /// <returns></returns>
        public static string RutEntero( string RUT ) {
            if( RUT.Equals( "" ) ) {
                return "";
            } else {
                RUT = RUT.Substring( 0, RUT.Length - 2 );
                return RUT;
            }
        }




        public static string GetRutDV( string RutSinDV ) {
            if( RutSinDV.Equals( "" ) ) {
                return "";
            } else {
                int x = 2;
                int s = 0;
                int dv;
                string Cola;
                for( int i = 0; i < RutSinDV.Length; i++ ) {
                    if( x > 7 ) {
                        x = 2;
                    }
                    s += int.Parse( RutSinDV.Substring( i, 1 ) ) * x;
                    x++;
                }

                dv = 11 - ( s % 11 );

                Cola = dv.ToString();
                if( dv == 10 ) {
                    Cola = "K";
                }
                if( dv == 11 ) {
                    Cola = "0";
                }

                return RutSinDV + "-" + Cola;
            }
        }


        /// <summary>
        /// Is string null or empty.
        /// </summary>
        /// <param name="strTxt">string to check</param>
        /// <returns>boolean true if empty</returns>
        public static bool isEmpty( string strTxt ) {
            if( strTxt == null ) {
                strTxt = String.Empty;
            }

            return ( strTxt == String.Empty );
        }

        /// <summary>
        /// splits a string into an string array.
        /// </summary>
        /// <param name="Str">string hash</param>
        /// <param name="delimStr">delimiter for the split</param>
        /// <returns>string array</returns>
        public static string[] SplitStrToArray( string Str, string delimStr ) {
            char[] delimiter = delimStr.ToCharArray();
            string[] split = null;
            split = Str.Split( delimiter );
            return split;
        }

        /// <summary>
        /// Coloca Ceros hasta que el largo cumple.
        /// </summary>
        /// <param name="valor">la cadena a llenar</param>
        /// <param name="largo">largo de cadena, si es menor llena con ceros</param>
        /// <returns></returns>
        public static string colocaCeros( string valor, string largo ) {
            string sCeros;
            int i, larg = valor.Length;
            sCeros = "";
            for( i = 0; i < int.Parse( largo ) - larg; i++ ) {
                sCeros = sCeros + "0";
            }
            return ( sCeros + valor );
        }

        /// <summary>
        ///  Borra Ceros Antes del Numero
        /// </summary>
        /// <param name="strNumber">El Numero Con ZeroFills</param>
        /// <returns></returns>
        public static string removerCeros( string strNumber ) {

            while( strNumber.StartsWith( "0" ) ) {
                strNumber = strNumber.Remove( 0, 1 );
            }

            return strNumber;
        }

        public static string Substring( string str, int start, int largo ) {

            if( strHelper.isEmpty( str ) ) {
                str = "";
            } else {
                str = str.Substring( start, largo );
            }

            return str;
        }


        public static string GetRandomNumberRange( int min, int max ) {
            Random RandomClass = new Random();
            int RandomNumber = RandomClass.Next( min, max );
            return RandomNumber.ToString();
        }


        public static string CapText( Match m ) {
            // get the matched string
            string x = m.ToString();
            // if the first char is lower case
            if( char.IsLower( x[0] ) )
                // capitalize it
                return char.ToUpper( x[0] ) + x.Substring( 1, x.Length - 1 );
            return x;
        }

        public static string CapFirstLetterText( string text ) {
            string pattern = @"\w+";
            string result = Regex.Replace( text, pattern, new MatchEvaluator( CapText ) );
            return text;
        }
    }


    /// <summary>
    /// cookie Helper Functions.
    /// </summary>
    public class cookieHelper {
        public cookieHelper() {
        }

        /// <summary>
        /// Sets a Cookie.
        /// </summary>
        /// <param name="Name">New Cookie name</param>
        /// <param name="Value">Value of the Cookie</param>
        /// <param name="ExpiresMin">Expires in Min</param>
        /// <param name="Me">Page to set Cookie.</param>
        public static void SetCookies( string Name, string Value, string ExpiresMin, Page Me ) {
            Me.Response.Cookies[Name].Value = Value;
            Me.Response.Cookies[Name].Expires = DateTime.Now.AddMinutes( int.Parse( ExpiresMin ) );

        }


        /// <summary>
        /// Gets a cookie from the webpage.
        /// </summary>
        /// <param name="Name">Name of the cookie</param>
        /// <param name="Me">Page to Check Cookie.</param>
        /// <returns>The value of the cookie</returns>
        public static string GetCookies( string Name, Page Me ) {
            string valor = "";
            try {
                valor = Me.Server.HtmlEncode( Me.Request.Cookies[Name].Value );

            } catch( NullReferenceException e ) {
                Console.WriteLine( "{0} Caught GetCookies exception #1.", e );
            }
            return valor;
        }

        public static void DeleteCookies( string Name, Page Me ) {


            try {
                Me.Response.Cookies[Name].Value = null;
                Me.Response.Cookies[Name].Expires = DateTime.Now.AddDays( -360 );

            } catch( NullReferenceException e ) {
                Console.WriteLine( "{0} Caught GetCookies exception #1.", e );
            }

        }

    }


    public class sessionHelper {
        public sessionHelper() {
        }

        public static string GetSession( string Name, Page Me ) {
            string valor = "";
            try {
                valor = Me.Session[Name].ToString();

            } catch( NullReferenceException e ) {
                Console.WriteLine( "{0} Caught GetSession exception #1.", e );
            }
            return valor;
        }

        public static string GetSession( string Name, string DefaultValue, Page Me ) {
            string valor = DefaultValue;
            try {
                valor = Me.Session[Name].ToString();

            } catch( NullReferenceException e ) {
                Console.WriteLine( "{0} Caught GetSession exception #1.", e );
            }
            return valor;
        }

        public static void SetCookies( string Name, string Value, Page Me ) {
            Me.Session.Add( Name, Value );
        }
    }



    /// <summary>
    /// Configuration Helper, Web.config
    /// </summary>
    public class cnfHelper {
        public cnfHelper() {
        }

        /// <summary>
        /// Get a variable from the webconfing
        /// </summary>
        /// <param name="appSettingsName">Web.config Variable Name</param>
        /// <returns>the variable in web.config. null if doesnt exists.</returns>
        public static string GetConfigAppVariable( string appSettingsName ) {
            string tmp;
            try {
            tmp = ConfigurationManager.AppSettings[appSettingsName];

            } catch {
                tmp = null;
            }
            return tmp;
        }
    }



    public class IOHelper {
        public IOHelper() {
        }

        public static bool MKDIR( string Path, string Subpath ) {
            // Specify the directories you want to manipulate.
            string path = Path;
            string subPath = Subpath;

            try {
                // Determine whether the directory exists.
                if( !Directory.Exists( path ) ) {
                    // Create the directory.
                    Directory.CreateDirectory( path );
                }


                if( !Directory.Exists( subPath ) ) {
                    // Create the directory.
                    Directory.CreateDirectory( subPath );
                    return true;
                }


            } catch( Exception e ) {
                Console.WriteLine( "The process failed: {0}", e.ToString() );
            } finally {
            }

            return false;
        }
        public static bool RMDIR( string Path ) {
            // Specify the directories you want to manipulate.
            string path = Path;

            try {
                // Determine whether the directory exists.
                if( Directory.Exists( path ) ) {
                    // Create the directory.
                    Directory.Delete( path, true );
                    return true;
                }


            } catch( Exception e ) {
                Console.WriteLine( "The process failed: {0}", e.ToString() );
            } finally {
            }

            return false;
        }


        // This will succeed because subdirectories are being deleted.
        //Console.WriteLine("I am about to attempt to delete {0}", path);
        //Directory.Delete(path, true);
        //Console.WriteLine("The Delete operation was successful.");
    }


    /// <summary>
    /// easy handle of dates.
    /// </summary>
    public class dateHelper {
        public dateHelper() {
        }

        public static string GetMesNombrePorNumero( int MesNumero ) {
            DateTimeFormatInfo myDTFI = new CultureInfo( "es-CL", false ).DateTimeFormat;
            return myDTFI.GetMonthName( MesNumero );
        }

        public static XmlDocument GetLlenaMesesAnos() {
            DateTimeFormatInfo myDTFI = new CultureInfo( "es-CL", false ).DateTimeFormat;
            XmlDocument xmlSalida = new XmlDocument();
            xmlSalida = XmlHelper.CrearRaizXdoc( "periodos" );
            xmlSalida = XmlHelper.AgregaNodo( xmlSalida, "//periodos", "listaAnos" );
            int anoActual = DateTime.Today.Year;
            int valor = 0;
            for( int i = 0; i <= 10; i++ ) {
                valor = anoActual - i;
                xmlSalida = XmlHelper.AgregaNodo( xmlSalida, "//listaAnos", "anos" );
                xmlSalida = XmlHelper.AgregaNodoConTexto( xmlSalida, "//anos[position() = last()]", "valor", valor.ToString() );
                xmlSalida = XmlHelper.AgregaNodoConTexto( xmlSalida, "//anos[position() = last()]", "nombre", valor.ToString() );
            }
            xmlSalida = XmlHelper.AgregaNodo( xmlSalida, "//periodos", "listaMeses" );
            string mes = "";
            for( int i = 1; i <= 12; i++ ) {
                if( i.ToString().Length == 1 ) {
                    mes = "0" + i.ToString();
                } else {
                    mes = i.ToString();
                }
                xmlSalida = XmlHelper.AgregaNodo( xmlSalida, "//listaMeses", "meses" );
                xmlSalida = XmlHelper.AgregaNodoConTexto( xmlSalida, "//meses[position() = last()]", "valor", mes );
                xmlSalida = XmlHelper.AgregaNodoConTexto( xmlSalida, "//meses[position() = last()]", "nombre", myDTFI.GetMonthName( i ) );
            }
            return xmlSalida;
        }


        /// <summary>
        /// Cambiar el formato de la fecha, se debe expicificar Input, y Output Format
        /// //La fecha
        /// string strfecha="2004-04-12 19:05:39.0";
        /// //El formato
        /// string formatfechaInput="YYYY-MM-DD TIME";
        /// string formatfechaOutput="YYYYMMDD";
        /// </summary>
        /// <param name="Fecha">Fecha a cambiar</param>
        /// <param name="FechaInput">formato input</param>
        /// <param name="FechaOutput">formato final</param>
        /// <returns></returns>
        public static string FormatDate( string Fecha, string FechaInput, string FechaOutput ) {
            char[] separadores = { '-', '/' };

            //La fecha
            string strfecha = Fecha;

            //El formato
            string formatfechaInput = FechaInput;
            string formatfechaOutput = FechaOutput;

            //Encontrar el separador
            //int PrimerIndexSeparador = formatfechaInput.IndexOfAny(separadores);
            //string DateSeparator = formatfechaInput.Substring(PrimerIndexSeparador,1);


            int PrimerIndexTime = formatfechaInput.IndexOf( "TIME" );
            int PrimerIndexYYYY = formatfechaInput.IndexOf( "YYYY" );
            int PrimerIndexMM = formatfechaInput.IndexOf( "MM" );
            int PrimerIndexDD = formatfechaInput.IndexOf( "DD" );

            string YYYY = "", MM = "", DD = "", TIME = "";

            try {
                if( PrimerIndexYYYY >= 0 && strfecha.Length >= PrimerIndexYYYY ) {
                    YYYY = strfecha.Substring( PrimerIndexYYYY, 4 ).ToString();
                }

                if( PrimerIndexMM >= 0 && strfecha.Length >= PrimerIndexMM ) {
                    MM = strfecha.Substring( PrimerIndexMM, 2 );
                }
                if( PrimerIndexDD >= 0 && strfecha.Length >= PrimerIndexDD ) {
                    DD = strfecha.Substring( PrimerIndexDD, 2 );
                }

                if( PrimerIndexTime >= 0 && strfecha.Length >= PrimerIndexTime ) {
                    TIME = strfecha.Substring( PrimerIndexTime );
                }
            } catch {
                formatfechaOutput = "";
            }
            formatfechaOutput = formatfechaOutput.Replace( "YYYY", YYYY );
            formatfechaOutput = formatfechaOutput.Replace( "MM", MM );
            formatfechaOutput = formatfechaOutput.Replace( "DD", DD );
            formatfechaOutput = formatfechaOutput.Replace( "TIME", TIME );
            return formatfechaOutput;
        }


        /// <summary>
        /// Saca la Diferencia de dos datetime en timespan.
        /// </summary>
        /// <param name="Inicio">datetime inicio</param>
        /// <param name="Fin">datetime fin</param>
        /// <returns></returns>
        public static TimeSpan RestarFecha( DateTime Inicio, DateTime Fin ) {
            System.TimeSpan diff1 = Fin.Subtract( Inicio );
            return diff1;
        }


        public static string GetDateNow() {
            //Gets todays date.
            return TodaysDateWithDiference( "0" );
        }

        public static string GetHourNow() {
            //Gets todays date.
            string NOW = TodaysDateFullDate();
            string timenow = dateHelper.FormatDate( NOW, "DD/MM/YYYY TIME", "TIME" );
            try {
                timenow = timenow.Substring( 0, 2 );
            } catch {
                timenow = "";
            }

            return timenow;
        }

        public static string GetMinuteNow() {
            //Gets todays date.
            string NOW = TodaysDateFullDate();
            string timenow = dateHelper.FormatDate( NOW, "DD/MM/YYYY TIME", "TIME" );
            try {
                timenow = timenow.Substring( 3, 2 );
            } catch {
                timenow = "";
            }

            return timenow;
        }

        public static string GetYearNow() {
            //Gets todays date.
            string NOW = TodaysDateWithDiference( "0" );

            return dateHelper.FormatDate( NOW, "DD/MM/YYYY TIME", "YYYY" );
        }

        public static string GetMonthNow() {
            //Gets todays date.
            string NOW = TodaysDateWithDiference( "0" );

            return dateHelper.FormatDate( NOW, "DD/MM/YYYY TIME", "MM" );
        }



        private static bool checkTyeAdd( string TypoDelta ) {
            switch( TypoDelta ) {
                case "months":
                    return true;
                case "minutes":
                    return true;
                case "hours":
                    return true;
                default:
                    return false;
            }
        }

        public static string GetYearNow( string WithDiference, string TypeAdd ) {
            if( checkTyeAdd( TypeAdd ) ) {
                //Gets todays date.
                string NOW = TodaysDateWithDiference( WithDiference, TypeAdd );
                return dateHelper.FormatDate( NOW, "DD/MM/YYYY TIME", "YYYY" );
            } else {
                return ( "TypeAdd only (months,minutes,hours" );
            }
        }


        public static string GetMonthNow( string WithDiference, string TypeAdd ) {
            if( checkTyeAdd( TypeAdd ) ) {
                //Gets todays date.
                string NOW = TodaysDateWithDiference( WithDiference, TypeAdd );

                return dateHelper.FormatDate( NOW, "DD/MM/YYYY TIME", "MM" );
            } else {
                return ( "TypeAdd only (months,minutes,hours" );
            }
        }


        public static string GetDayhNow() {
            //Gets todays date.
            string NOW = TodaysDateWithDiference( "0" );

            return dateHelper.FormatDate( NOW, "DD/MM/YYYY TIME", "DD" );
        }


        public static string TodaysDateWithDiference( string DaysDelta ) {
            //Gets todays date.
            DateTime NowsDate = DateTime.Now;
            NowsDate = NowsDate.AddDays( double.Parse( DaysDelta ) );
            string FinalChileanDate = strHelper.colocaCeros( NowsDate.Day.ToString(), "2" ) + "/" + strHelper.colocaCeros( NowsDate.Month.ToString(), "2" ) + "/" + strHelper.colocaCeros( NowsDate.Year.ToString(), "4" );
            return FinalChileanDate;
        }


        public static string TodaysDateFullDate() {
            //Gets todays date.
            DateTime NowsDate = DateTime.Now;

            string FinalChileanDate = strHelper.colocaCeros( NowsDate.Day.ToString(), "2" ) + "/" + strHelper.colocaCeros( NowsDate.Month.ToString(), "2" ) + "/" + strHelper.colocaCeros( NowsDate.Year.ToString(), "4" );
            FinalChileanDate += " " + strHelper.colocaCeros( NowsDate.Hour.ToString(), "2" ) + ":" + strHelper.colocaCeros( NowsDate.Minute.ToString(), "2" );
            return FinalChileanDate;
        }


        public static string TodaysDateWithDiference( string Delta, string TypeAdd ) {
            //Gets todays date.
            DateTime NowsDate = DateTime.Now;

            if( TypeAdd.Equals( "months" ) )
                NowsDate = NowsDate.AddMonths( int.Parse( Delta ) );
            if( TypeAdd.Equals( "minutes" ) )
                NowsDate = NowsDate.AddMinutes( double.Parse( Delta ) );
            if( TypeAdd.Equals( "hours" ) )
                NowsDate = NowsDate = NowsDate.AddHours( int.Parse( Delta ) );

            string FinalChileanDate = strHelper.colocaCeros( NowsDate.Day.ToString(), "2" ) + "/" + strHelper.colocaCeros( NowsDate.Month.ToString(), "2" ) + "/" + strHelper.colocaCeros( NowsDate.Year.ToString(), "4" );
            return FinalChileanDate;
        }


        /// <summary>
        /// Resta dos fechas. Formato del tipo {YYYY MM DD H M S}
        /// </summary>
        /// <param name="strInicio">fecha de inicio YYYY MM DD H M S, separado por espacio</param>
        /// <param name="strFin">fecha de fin YYYY MM DD H M S, separado por espacio</param>
        /// <returns></returns>
        public static string strRestarFecha( string strInicio, string strFin, bool format ) {

            string diference = "";
            try {

                string[] strArrDate1 = strInicio.Split( ' ' );
                string[] strArrDate2 = strFin.Split( ' ' );

                //Parsear los numeros a entero
                int[] fch1, fch2;
                fch1 = new int[8];
                fch2 = new int[8];

                for( int x = 0; x <= 5; x++ ) {
                    fch1[x] = int.Parse( strArrDate1[x] );
                    fch2[x] = int.Parse( strArrDate2[x] );
                }

                System.DateTime date1 = new System.DateTime( fch1[0], fch1[1], fch1[2], fch1[3], fch1[4], fch1[5] );
                System.DateTime date2 = new System.DateTime( fch2[0], fch2[1], fch2[2], fch2[3], fch2[4], fch2[5] );


                System.TimeSpan diff1 = RestarFecha( date1, date2 );

                diference = diff1.ToString();
                if( format ) {
                    diference = diference.Replace( ".", " dia(s) - " );
                }
            } catch {
                //si no hay diferencia, o hay un error en los formatos tirar diferencia en {00:00:00}
                diference = "{00:00:00}";
            }
            return ( diference );
        }

        public static string FormatChileDateAsNumeric( string ChilesDate ) {
            //Chilean date = DD/MM/YYYY or DD-MM-YYYY or DD MM YYYY
            //Numeric is = YYYYMMDD
            string NumericDate = dateHelper.FormatDate( ChilesDate, "MM DD YYYY TIME", "YYYYMMDD" );
            return NumericDate;
        }


        public static XmlDocument NodesDateFormat( XmlDocument XmlFinal, string NodeName, string FechaInput, string FechaOutput ) {
            //cada item es un nodo nombre.
            XmlNodeList NodoListas = XmlFinal.GetElementsByTagName( NodeName );
            foreach( XmlNode nodeItem in NodoListas ) {
                nodeItem.InnerText = dateHelper.FormatDate( nodeItem.InnerText, FechaInput, FechaOutput );
            }
            return XmlFinal;
        }

        public static string DT2CD( DateTime inFecha ) {
            string fecha = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;

            fecha = ( ( inFecha.Day < 10 ) ? "0" : "" ) + inFecha.Day + "/" + ( ( inFecha.Month < 10 ) ? "0" : "" ) + inFecha.Month + "/" + inFecha.Year;

            return fecha;
        }

        public static DateTime CD2DT( string inFecha ) {
            int dia = 1;
            int mes = 1;
            int anno = 1900;

            dia = Convert.ToInt32( inFecha.Substring( 0, 2 ) );
            mes = Convert.ToInt32( inFecha.Substring( 3, 2 ) );
            anno = Convert.ToInt32( inFecha.Substring( 6, 4 ) );

            return new DateTime( anno, mes, dia );
        }

    }
    /// <summary>
    /// Dataset Helper.
    /// </summary>
    public class DsCtlHelper {


        public DataSet GlobalDs = null;


        /// <summary>
        /// Constructs the DsCtlHelper
        /// </summary>
        /// <param name="OriginalDs">A Dataset to Use</param>
        public DsCtlHelper( DataSet OriginalDs ) {
            GlobalDs = OriginalDs.Copy();
        }


        //From a select for xml raw, get all the lines.

        /// <summary>
        /// Transform A DataSet to XML. Or joins dr[0]
        /// </summary>
        /// <returns>string of xmlraw. //row nodes.</returns>
        public string GetDsForXmlRaw() {
            string tmp = null;

            //concatenate if at least one table exist.
            if( GlobalDs.Tables.Count >= 1 ) {
                foreach( DataRow dr in GlobalDs.Tables[0].Rows ) {
                    tmp = tmp + dr[0];
                }
            }
            return tmp;
        }


        public string GetDsForXmlRaw2Sets() {
            string tmp = null;
            string tmp1 = null;

            //concatenate if at least one table exist.
            if( GlobalDs.Tables.Count >= 1 ) {
                foreach( DataRow dr in GlobalDs.Tables[0].Rows ) {
                    tmp = tmp + dr[0];
                }

                foreach( DataRow dr in GlobalDs.Tables[1].Rows ) {
                    tmp1 = tmp1 + dr[0];
                }
            }
            return "<table1>" + tmp + "</table1><table2>" + tmp1 + "</table2>";
        }

        public string GetDsForXmlRawMultiple() {
            string tmp;
            int i;
            string resultado = null;

            //concatenate if at least one table exist.
            if( GlobalDs.Tables.Count >= 1 ) {
                for( i = 0; i < GlobalDs.Tables.Count; i++ ) {
                    tmp = null;
                    foreach( DataRow dr in GlobalDs.Tables[i].Rows ) {
                        tmp = tmp + dr[0];
                    }
                    resultado = resultado + "<t" + i + ">" + tmp + "</t" + i + ">";
                }
            }
            return resultado;
        }



        /// <summary>
        ///     Return The Error Code From The DataSet. Second Table "CodError"
        ///      <retvalue>The DataRow</retvalue>
        /// </summary
        public int GetErrorCode() {
            int id = (int) GlobalDs.Tables[1].Rows[0]["CodError"];
            return id;
        }


        /// <summary>
        ///     Return The Error Code From The DataSet. Second Table "CodError"
        ///      <retvalue>The DataRow</retvalue>
        /// </summary
        public int GetErrorCodeGlosa() {
            int id = (int) GlobalDs.Tables[1].Rows[0]["GlsError"];
            return id;
        }

        /// <summary>
        ///     Retrieve a Datarow from a DataSet 
        ///     <param name="TableName">A DataSet Table Name (Can be Numeric)</param>
        ///     <param name="Number">Tables Rows Number We Want To Get Usualy 0.</param>
        ///     <retvalue>The DataRow</retvalue>
        /// </summary>
        public DataRow GetDataRowByNumber( string TableName, int Number ) {
            int TblNum = 0;

            if( Information.IsNumeric( TableName ) ) {
                TblNum = Convert.ToInt32( TableName );
            } else {
                TblNum = (int) GlobalDs.Tables.IndexOf( TableName );
            }

            DataRow dr = null;

            if( Number <= GlobalDs.Tables[TblNum].Rows.Count ) {
                dr = GlobalDs.Tables[TblNum].Rows[Number];
            }

            return dr;
        }


        /// <summary>
        ///     Retrieve Escalar Value from a DataSet 
        ///     <param name="tbl">A DataSet Table Name (Can be Numeric)</param>
        ///     <param name="RowNum">Tables Rows Number We Want To Get Usualy 0.</param>
        ///     <param name="Col">Tables Col Number or Name</param>
        ///     <retvalue>String Escalar</retvalue>
        /// </summary>
        public string GetEscalarRowCol( string tbl, int RowNum, string Col ) {

            int ColNum = 0, TblNum = 0;
            string id = null;

            if( Information.IsNumeric( Col ) && Information.IsNumeric( tbl ) ) {

                ColNum = Convert.ToInt32( Col );
                TblNum = Convert.ToInt32( tbl );
            } else {
                if( Information.IsNumeric( tbl ) ) {
                    TblNum = Convert.ToInt32( tbl );
                } else {
                    TblNum = (int) GlobalDs.Tables.IndexOf( tbl );
                }

                if( Information.IsNumeric( Col ) ) {
                    ColNum = Convert.ToInt32( Col );
                } else {
                    ColNum = (int) GlobalDs.Tables[TblNum].Columns.IndexOf( Col );
                }

                id = GlobalDs.Tables[TblNum].Rows[RowNum][ColNum].ToString();
            }

            return id;
        }



        /// <summary>
        ///     Converts From First Table DataSet, The Column Wanted, into a string separated 
        ///     by a wanted delimiter
        ///		<param name="ColName">Tables Col Number or Name</param>
        ///     <param name="Delimiter">The Delimiter</param>
        ///     <retvalue>String Of All Data Column join by a delimiter</retvalue>
        /// </summary>
        public string ConvertDsColumnToString( string ColName, string Delimiter ) {
            if( GlobalDs.Tables[0].Rows.Count == 0 ) {
                return "";
            }
            string[] myStrArray = new string[GlobalDs.Tables[0].Rows.Count];

            int y = 0;
            foreach( DataRow dr in GlobalDs.Tables[0].Rows ) {
                myStrArray[y] = dr[ColName].ToString();
                y += 1;
            }

            return String.Join( Delimiter, myStrArray );
            ;
        }


    }



    public class EnhancedMailMessage : MailMessage {
        private string fromName;
        private string smtpServerName;
        private string smtpUserName;
        private string smtpUserPassword;
        private int smtpServerPort;
        private bool smtpSSL;

        public EnhancedMailMessage() {
            fromName = string.Empty;
            smtpServerName = string.Empty;
            smtpUserName = string.Empty;
            smtpUserPassword = string.Empty;
            smtpServerPort = 25;
            smtpSSL = false;
        }

        /// <summary>
        /// The display name that will appear
        /// in the recipient mail client
        /// </summary>
        public string FromName {
            set {
                fromName = value;
            }
            get {
                return fromName;
            }
        }

        /// <summary>
        /// SMTP server (name or IP address)
        /// </summary>
        public string SMTPServerName {
            set {
                smtpServerName = value;
            }
            get {
                return smtpServerName;
            }
        }

        /// <summary>
        /// Username needed for a SMTP server
        /// that requires authentication
        /// </summary>
        public string SMTPUserName {
            set {
                smtpUserName = value;
            }
            get {
                return smtpUserName;
            }
        }

        /// <summary>
        /// Password needed for a SMTP server
        /// that requires authentication
        /// </summary>
        public string SMTPUserPassword {
            set {
                smtpUserPassword = value;
            }
            get {
                return smtpUserPassword;
            }
        }

        /// <summary>
        /// SMTP server port (default 25)
        /// </summary>
        public int SMTPServerPort {
            set {
                smtpServerPort = value;
            }
            get {
                return smtpServerPort;
            }
        }

        /// <summary>
        /// If SMTP server requires SSL
        /// </summary>
        public bool SMTPSSL {
            set {
                smtpSSL = value;
            }
            get {
                return smtpSSL;
            }
        }

        /*public void Send() {
            if( smtpServerName.Length == 0 ) {
                throw new Exception( "SMTP Server not specified" );
            }

            if( fromName.Length > 0 ) {
                this.Headers.Add( "From",
                    string.Format( "{0} <{1}>",
                    FromName, From ) );
            }

            // set SMTP server name
            this.Fields["http://schemas.microsoft.com/" +
                "cdo/configuration/smtpserver"] = smtpServerName;
            // set SMTP server port
            this.Fields["http://schemas.microsoft.com/cdo" +
                "/configuration/smtpserverport"] = smtpServerPort;
            this.Fields["http://schemas.microsoft.com/" +
                "cdo/configuration/sendusing"] = 2;

            if( smtpUserName.Length > 0 && smtpUserPassword.Length > 0 ) {
                this.Fields["http://schemas.microsoft.com/" +
                    "cdo/configuration/smtpauthenticate"] = 1;

                // set SMTP username
                this.Fields["http://schemas.microsoft.com" +
                    "/cdo/configuration/sendusername"] = smtpUserName;
                // set SMTP user password
                this.Fields["http://schemas.microsoft.com/" +
                    "cdo/configuration/sendpassword"] = smtpUserPassword;
            }

            // ssl if needed
            if( smtpSSL ) {
                this.Fields.Add( "http://schemas.microsoft" +
                    ".com/cdo/configuration/smtpusessl", "true" );
            }

            SmtpMail.SmtpServer = smtpServerName;
            SmtpMail.Send( this );
        }

        public static void QuickSend(
            string SMTPServerName,
            string ToEmail,
            string FromEmail,
            string Subject,
            string Body,
            MailFormat BodyFormat ) {
            EnhancedMailMessage msg = new EnhancedMailMessage();

            msg.From = FromEmail;
            msg.To = ToEmail;
            msg.Subject = Subject;
            msg.Body = Body;
            msg.BodyFormat = BodyFormat;

            msg.SMTPServerName = SMTPServerName;
            msg.Send();
        }*/
    }
}
