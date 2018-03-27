//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Data Access Application Block
// Tinet Componets
//===============================================================================
// Author: jaime hernandez
// Component: Globals Xml Helper.
//===============================================================================


using System;
using System.Web.UI;
using System.Xml;
using System.Xml.Xsl;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using library.common;
using library.CSharpInformation;

//using Saxon.Api;


namespace library.xml {
    /// <summary>
    /// Xml Helper Functions.
    /// </summary>
    public class XmlHelper {
        /// <summary>
        /// xmlRows, retornado de un for xml raw.
        /// </summary>
        public string xmlRows = null;
        /// <summary>
        /// XmlFinal XmlDocument Con Datos del xmlrows.
        /// </summary>
        public XmlDocument XmlFinal = null;
        /// <summary>
        /// Definicion en español del xml document.
        /// </summary>
        public static string XML_TAG_ES = "<?xml version=\"1.0\" encoding=\"iso-8859-1\" ?>";//podria ser utf-8


        public static string XML_TAG_UTF16 = "<?xml version=\"1.0\" encoding=\"UTF-16\" ?>";
        /// <summary>
        /// Definicion en ingles del xml document.
        /// </summary>
        public static string XML_TAG_EN = "<?xml version=\"1.0\" ?>";

        /// <summary>
        /// constructor void
        /// </summary>
        public XmlHelper() {

        }
        /// <summary>
        /// Xml Constructor
        /// </summary>
        /// <param name="xmlRows">the xml rows, node "raiz" created as root.</param>
        public XmlHelper(string xmlRows) {
            this.xmlRows = xmlRows;
            this.XmlFinal = XmlHelper.CrearRaizXdoc("raiz");
            this.XmlFinal.DocumentElement.InnerXml = xmlRows;
        }

        /// <summary>
        /// Adds Error To Schemas. Only if CodError= -1.
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <returns></returns>
        public XmlDocument AgregarErrorEsquema(XmlDocument XmlFinal) {
            int CodError = GetErrorCode();
            string GlsError = GetGlsError();

            if (CodError == -1) {
                XmlFinal = XmlHelper.AgregaNodoConTexto(XmlFinal, "//errorContainer", "CodError", CodError.ToString());
                XmlFinal = XmlHelper.AgregaNodoConTexto(XmlFinal, "//errorContainer", "GlsError", GlsError);
            }

            return XmlFinal;
        }

        /// <summary>
        /// Force Error To Schemas. CodError, GlsError
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="CodError">Error Codigo</param>
        /// <param name="GlsError">Error Glosa</param>
        /// <returns>Xml Con Error Personalizado</returns>
        public static XmlDocument AgregarErrorPersonalizadoEsquema(XmlDocument XmlFinal, string CodError, string GlsError) {
            XmlFinal = XmlHelper.AgregaNodoConTexto(XmlFinal, "//errorContainer", "CodError", CodError.ToString());
            XmlFinal = XmlHelper.AgregaNodoConTexto(XmlFinal, "//errorContainer", "GlsError", GlsError);
            return XmlFinal;
        }

        /// <summary>
        /// Checks There is an CodError == -1
        /// </summary>
        /// <returns>bool true</returns>
        public bool HasError() {
            int CodError = GetErrorCode();
            string GlsError = GetGlsError();

            if (CodError == -1) {
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Checks There is an CodError == -1
        /// </summary>
        /// <returns>bool true</returns>
        public bool HasError35() {

            int Resp = XmlFinal.SelectNodes("//row/@error").Count;
            int CodError = GetErrorCode();

            if (Resp > 0 || CodError==-1) {
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// If HasError Constructor rows is true, then adds Error to Schema XmlFinal.
        /// </summary>
        /// <param name="XmlFinal">XmlFinal with error code</param>
        /// <returns>XmlDocument XmlFinal,with or without and error schema.</returns>
        public XmlDocument HasErrorAgregarEsquema(XmlDocument XmlFinal) {
            if (HasError()) {
                return AgregarErrorEsquema(XmlFinal);
            }
            else {
                return XmlFinal;
            }
        }

        /// <summary>
        /// Gets the error code from the Construtor
        /// </summary>
        /// <returns>int CodError</returns>
        public int GetErrorCode() {
            string CodError = this.strGetNodoGetAttributo("//raiz/row", "CodError");

            if (CodError == string.Empty) {
                return -9999;
            }
            else {
                return int.Parse(CodError);
            }
        }

        public static int GetErrorCode(XmlDocument XmlFinal) {
            string CodError = string.Empty;
            XmlAttribute atrTmp = null;

            XmlNode Nodo = XmlFinal.SelectSingleNode("//raiz/row");

            if (Nodo != null) {
                atrTmp = Nodo.Attributes["CodError"];
                if (atrTmp != null) {
                    CodError = atrTmp.Value;
                }
            }

            if (CodError == string.Empty) {
                return -9999;
            }
            else {
                return int.Parse(CodError);
            }
        }

        /// <summary>
        /// Gets Glosa erro from the Construtor
        /// </summary>
        /// <returns></returns>
        public string GetGlsError() {
            return this.strGetNodoGetAttributo("//raiz/row", "GlsError");
        }

        /// <summary>
        /// Counts Nodes
        /// </summary>
        /// <param name="XmlDoc">XmlDoc</param>
        /// <param name="Xpath">Xpath To Nodes To Count</param>
        /// <returns></returns>
        public static int intContarNodos(XmlDocument XmlDoc, string Xpath) {
            return XmlDoc.SelectNodes(Xpath).Count;
        }
        public static string strContarNodos( XmlDocument XmlDoc, string Xpath ) { 
            return intContarNodos(  XmlDoc,  Xpath ).ToString();
        }

        /// <summary>
        /// Adds Paging Calculations to the xml Final. uses parameter for total elelemts in each page.
        /// </summary>
        /// <param name="Pagina">The Actual Page Number</param>
        /// <param name="TotalDeRegistros">Total of registers to paginate</param>
        /// <param name="XmlFinal"></param>
        /// <param name="RegPorPag">Registros por pagina</param>
        /// <returns></returns>
        public static XmlDocument GeneraPaginacion( string Pagina, string TotalDeRegistros, XmlDocument XmlFinal, string RegPorPag ) {

            int Pag, TotalRegs = 0, Resto, RegFirst = 0, RegLast = 0, i = 0, PagSgte = 0, PagAnt = 0, NumPaginas = 0;

            int CTE_REGS_POR_PAG = int.Parse( RegPorPag );


            if( Pagina == string.Empty ) {
                Pag = 1;
            }
            else {
                try {
                    Pag = int.Parse( Pagina );
                }
                catch {
                    Pag = 1;
                }

                if( Pag == 0 )
                    Pag = 1;
            }

            //si es negativo traerlo al primero.
            if( Pag < 0 )
                Pag = 1;

            if( TotalDeRegistros == string.Empty ) {
                TotalRegs = 0;
            }
            else {
                TotalRegs = int.Parse( TotalDeRegistros );
            }

            if( TotalRegs != 0 ) {
                Resto = TotalRegs % CTE_REGS_POR_PAG;

                if( Resto == 0 ) {
                    NumPaginas = TotalRegs / CTE_REGS_POR_PAG;
                }
                else {
                    NumPaginas = TotalRegs / CTE_REGS_POR_PAG + 1;
                }
            }

            //Pagina actual mayor que el total. mandarlo al ultimo
            if( Pag > NumPaginas ) {
                Pag = NumPaginas;
            }

            RegFirst = Pag * CTE_REGS_POR_PAG - CTE_REGS_POR_PAG + 1;
            RegLast = Pag * CTE_REGS_POR_PAG;

            if( RegLast > TotalRegs ) {
                RegLast = TotalRegs;
            }

            if( Pag < NumPaginas ) {
                PagSgte = Pag + 1;
            }
            else {
                PagSgte = 0;
            }

            if( Pag > 1 ) {
                PagAnt = Pag - 1;
            }
            else {
                PagAnt = 0;
            }


            string NmRoot, NmBase;

            NmRoot = "Paginador";
            NmBase = "raiz/" + NmRoot;

            XmlFinal = AgregaNodo( XmlFinal, "raiz", NmRoot );
            XmlFinal = AgregaNodoConTexto( XmlFinal, NmBase, "NumPaginas", NumPaginas.ToString( ) );
            XmlFinal = AgregaNodoConTexto( XmlFinal, NmBase, "PagSgte", PagSgte.ToString( ) );
            XmlFinal = AgregaNodoConTexto( XmlFinal, NmBase, "PagAnt", PagAnt.ToString( ) );
            XmlFinal = AgregaNodoConTexto( XmlFinal, NmBase, "PagAct", Pag.ToString( ) );
            XmlFinal = AgregaNodoConTexto( XmlFinal, NmBase, "RegFirst", RegFirst.ToString( ) );
            XmlFinal = AgregaNodoConTexto( XmlFinal, NmBase, "RegLast", RegLast.ToString( ) );
            XmlFinal = AgregaNodoConTexto( XmlFinal, NmBase, "Total", TotalRegs.ToString( ) );
            XmlFinal = AgregaNodoConTexto( XmlFinal, NmBase, "RegPorPag", RegPorPag.ToString( ) );
            

            if( cnfHelper.GetConfigAppVariable( "PagTxt" ) != null ) {
                string PagTxt = cnfHelper.GetConfigAppVariable( "PagTxt" ).Replace( "{Total}", TotalRegs.ToString( ) ).Replace( "{PagAct}", Pag.ToString( ) ).Replace( "{NumPaginas}", NumPaginas.ToString( ) );
                XmlFinal = AgregaNodoConTexto( XmlFinal, NmBase, "PagTxt", PagTxt );
            }

            XmlFinal = AgregaNodo( XmlFinal, NmBase, "Paginas" );

            for( i = 1; i <= NumPaginas; i++ ) {
                XmlFinal = AgregaNodoConTexto( XmlFinal, NmBase + "/Paginas", "NumPag", i.ToString( ) );
            }

            return XmlFinal;

        }


        /// <summary>
        /// Adds Paging Calculations to the xml Final. uses web.config for total elelemts in each page.
        /// </summary>
        /// <param name="Pagina">The Actual Page Number</param>
        /// <param name="TotalDeRegistros">Total of registers to paginate</param>
        /// <param name="XmlFinal"></param>
        /// <param name="MulTiple"></param>
        /// <returns></returns>
        public static XmlDocument GeneraMultiplePaginacion(string Pagina, string TotalDeRegistros, XmlDocument XmlFinal, string MulTiple) {

            int Pag, TotalRegs = 0, Resto, RegFirst = 0, RegLast = 0, i = 0, PagSgte = 0, PagAnt = 0, NumPaginas = 0;

            int CTE_REGS_POR_PAG = int.Parse(cnfHelper.GetConfigAppVariable(MulTiple));


            if (Pagina == string.Empty) {
                Pag = 1;
            }
            else {
                try {
                    Pag = int.Parse(Pagina);
                }
                catch {
                    Pag = 1;
                }

                if (Pag == 0)
                    Pag = 1;
            }

            //si es negativo traerlo al primero.
            if (Pag < 0)
                Pag = 1;

            if (TotalDeRegistros == string.Empty) {
                TotalRegs = 0;
            }
            else {
                TotalRegs = int.Parse(TotalDeRegistros);
            }

            if (TotalRegs != 0) {
                Resto = TotalRegs % CTE_REGS_POR_PAG;

                if (Resto == 0) {
                    NumPaginas = TotalRegs / CTE_REGS_POR_PAG;
                }
                else {
                    NumPaginas = TotalRegs / CTE_REGS_POR_PAG + 1;
                }
            }

            //Pagina actual mayor que el total. mandarlo al ultimo
            if (Pag > NumPaginas) {
                Pag = NumPaginas;
            }

            RegFirst = Pag * CTE_REGS_POR_PAG - CTE_REGS_POR_PAG + 1;
            RegLast = Pag * CTE_REGS_POR_PAG;

            if (RegLast > TotalRegs) {
                RegLast = TotalRegs;
            }

            if (Pag < NumPaginas) {
                PagSgte = Pag + 1;
            }
            else {
                PagSgte = 0;
            }

            if (Pag > 1) {
                PagAnt = Pag - 1;
            }
            else {
                PagAnt = 0;
            }

            string NmRoot, NmBase;

            NmRoot = "M_" + MulTiple;
            NmBase = "raiz/" + NmRoot;

            XmlFinal = AgregaNodo(XmlFinal, "raiz", NmRoot);
            XmlFinal = AgregaNodoConTexto(XmlFinal, NmBase, "NumPaginas", NumPaginas.ToString());
            XmlFinal = AgregaNodoConTexto(XmlFinal, NmBase, "PagSgte", PagSgte.ToString());
            XmlFinal = AgregaNodoConTexto(XmlFinal, NmBase, "PagAnt", PagAnt.ToString());
            XmlFinal = AgregaNodoConTexto(XmlFinal, NmBase, "PagAct", Pag.ToString());
            XmlFinal = AgregaNodoConTexto(XmlFinal, NmBase, "RegFirst", RegFirst.ToString());
            XmlFinal = AgregaNodoConTexto(XmlFinal, NmBase, "RegLast", RegLast.ToString());
            XmlFinal = AgregaNodoConTexto(XmlFinal, NmBase, "Total", TotalRegs.ToString());

            if (cnfHelper.GetConfigAppVariable("PagTxt") != null) {
                string PagTxt = cnfHelper.GetConfigAppVariable("PagTxt").Replace("{Total}", TotalRegs.ToString()).Replace("{PagAct}", Pag.ToString()).Replace("{NumPaginas}", NumPaginas.ToString());
                XmlFinal = AgregaNodoConTexto(XmlFinal, NmBase, "PagTxt", PagTxt);
            }

            XmlFinal = AgregaNodo(XmlFinal, NmBase, "Paginas");

            for (i = 1; i <= NumPaginas; i++) {
                XmlFinal = AgregaNodoConTexto(XmlFinal, NmBase + "/Paginas", "NumPag", i.ToString());
            }

            return XmlFinal;
        }

        /// <summary>
        /// Makes the Basic Shema For the webpage.
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="Me">Webpage</param>
        /// <returns>XmlFinal Shema.</returns>
        public static XmlDocument CrearPaginaEsquema(XmlDocument XmlFinal, Page Me) {
            //Crear Scheame del Documento XML Principal
            XmlFinal = XmlHelper.CrearRaizXdoc("raiz");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//raiz", "errorContainer");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//raiz", "app");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//raiz", "servicios");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//app", "requestHiddens");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//app", "parametrosFunciones");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//app", "requestFormPost");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//app", "requestFormApp");
            //Pasar Todo Los Post Al XMLDOC (requestFormPost,requestFormApp)
            XmlFinal = XmlHelper.AgregaRequestForm(XmlFinal, Me);
            return XmlFinal;
        }

        /// <summary>
        /// Does Basic Schema, incluiding form post and gets. into requestFormPost and requestFormApp
        /// </summary>
        /// <param name="XmlFinal"></param>
        /// <param name="Me"></param>
        /// <returns></returns>
        public static XmlDocument CrearPaginaEsquemaAll(XmlDocument XmlFinal, Page Me) {
            //Crear Scheame del Documento XML Principal
            XmlFinal = XmlHelper.CrearRaizXdoc("raiz");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//raiz", "errorContainer");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//raiz", "app");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//raiz", "servicios");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//app", "requestHiddens");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//app", "parametrosFunciones");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//app", "requestFormPost");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//app", "requestFormApp");
            //Pasar Todo Los Post Al XMLDOC (requestFormPost,requestFormApp)
            XmlFinal = XmlHelper.AgregaRequestAll(XmlFinal, Me);
            return XmlFinal;
        }

        /// <summary>
        /// Does Basic Schema, separeting the form post and gets requestFormPost,requestFormQuery.
        /// requestFormApp only recieves the post.
        /// </summary>
        /// <param name="XmlFinal"></param>
        /// <param name="Me"></param>
        /// <returns></returns>
        public static XmlDocument CrearPaginaEsquemaQueryForm(XmlDocument XmlFinal, Page Me) {
            //Crear Scheame del Documento XML Principal
            XmlFinal = XmlHelper.CrearRaizXdoc("raiz");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//raiz", "errorContainer");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//raiz", "app");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//raiz", "servicios");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//app", "requestHiddens");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//app", "parametrosFunciones");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//app", "requestFormPost");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//app", "requestFormQuery");
            XmlFinal = XmlHelper.AgregaNodo(XmlFinal, "//app", "requestFormApp");

            //Pasar Todo Los Post Al XMLDOC (requestFormPost,requestFormApp)

            XmlFinal = XmlHelper.AgregaRequestForm(XmlFinal, Me);
            XmlFinal = XmlHelper.AgregaRequestQuery(XmlFinal, Me);

            return XmlFinal;
        }

        /// <summary>
        /// Agrega En El Esquema Final en la seccion //requestFormQuery, todas las variables del
        /// QueryString metodo, en nodos con sus valores respectivos.
        /// La Estructura Query No Existe en el xml schema final.
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="Me">this page</param>
        /// <returns></returns>
        public static XmlDocument AgregaRequestQuery(XmlDocument XmlFinal, Page Me) {
            foreach (string item in Me.Request.QueryString) {
                //Creamos Nodo llamado servicio
                XmlElement newElem = XmlFinal.CreateElement(item);
                newElem.InnerText = Me.Request.QueryString[item].ToString();
                XmlFinal.SelectSingleNode("//requestFormQuery").AppendChild(newElem);
            }
            return XmlFinal;
        }

        /// <summary>
        /// Agrega nodos del post al Esquema en la seccion requestHiddens.
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="strCollectionNodos">nombre de las variables post a buscar y agregar al esquema requestHiddens.</param>
        /// <returns>XmlDocument XmlFinal</returns>
        public static XmlDocument AgregarRequestHiddens(XmlDocument XmlFinal, string strCollectionNodos) {
            string[] strLista = strHelper.SplitStrToArray(strCollectionNodos, ",");
            foreach (string item in strLista) {
                //cada item es un nodo nombre.requestFormApp
                XmlNode Nodo = XmlFinal.SelectSingleNode("//requestFormPost/" + item);
                if (Nodo != null) {
                    //si no hat ese elemento, debemos ingresarlo con "T"
                    XmlElement newElem = XmlFinal.CreateElement("row");
                    //creamos un atributo llamado name.
                    XmlAttribute newAttr = XmlFinal.CreateAttribute("name");
                    newAttr.Value = item;
                    newElem.Attributes.Append(newAttr);
                    //otro con valor
                    XmlAttribute newAttr2 = XmlFinal.CreateAttribute("value");
                    newAttr2.Value = Nodo.InnerText;
                    newElem.Attributes.Append(newAttr2);
                    XmlFinal.SelectSingleNode("//requestHiddens").AppendChild(newElem);
                }
            }
            return XmlFinal;
        }

        /// <summary>
        /// Agrega nodos al Esquema en la seccion requestHiddens Con Valores Forzados.
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="strCollectionNodos">New Nodos al requestHiddens separados por coma</param>
        /// <param name="strCollectionNodosValue">Valor de Nodos por coma</param>
        /// <returns></returns>
        public static XmlDocument AgregarRequestHiddensForced(XmlDocument XmlFinal, string strCollectionNodos, string strCollectionNodosValue) {
            string[] strLista = strHelper.SplitStrToArray(strCollectionNodos, ",");
            string[] strListaValor = strHelper.SplitStrToArray(strCollectionNodosValue, ",");
            int x = 0;
            foreach (string item in strLista) {
                //cada item es un nodo nombre.requestFormApp
                XmlNode Nodo = XmlFinal.SelectSingleNode("//requestFormPost/" + item);
                if (Nodo != null) {
                    //si no hat ese elemento, debemos ingresarlo con "T"
                    XmlElement newElem = XmlFinal.CreateElement("row");

                    //creamos un atributo llamado name.
                    XmlAttribute newAttr = XmlFinal.CreateAttribute("name");
                    newAttr.Value = item;
                    newElem.Attributes.Append(newAttr);
                    //otro con valor
                    XmlAttribute newAttr2 = XmlFinal.CreateAttribute("value");
                    newAttr2.Value = strListaValor[x];
                    x += 1;
                    newElem.Attributes.Append(newAttr2);


                    XmlFinal.SelectSingleNode("//requestHiddens").AppendChild(newElem);
                }
            }
            return XmlFinal;
        }

        /// <summary>
        /// pasa los campos deseados con un token definido al esquema en la seccion requestFormApp.
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="strCollectionNodos">Nodos Agregar Separados Por Coma</param>
        /// <param name="ToStr">Valor de nodo</param>
        /// <returns></returns>
        public static XmlDocument ArreglarRequestFormApp(XmlDocument XmlFinal, string strCollectionNodos, string ToStr) {

            string[] strLista = strHelper.SplitStrToArray(strCollectionNodos, ",");

            foreach (string item in strLista) {
                //cada item es un nodo nombre.requestFormApp
                XmlNodeList NodoListas = XmlFinal.SelectNodes("//requestFormApp/" + item);
                if (NodoListas.Count == 0) {
                    //si no hat ese elemento, debemos ingresarlo con "T"
                    XmlElement newElem = XmlFinal.CreateElement(item);
                    newElem.InnerText = ToStr;
                    XmlFinal.SelectSingleNode("//requestFormApp").AppendChild(newElem);
                }
                foreach (XmlNode nodeItem in NodoListas) {
                    if (nodeItem.InnerText == string.Empty) {
                        nodeItem.InnerText = ToStr;
                    }
                }
            }
            return XmlFinal;
        }

        /// <summary>
        /// Agrega En El Esquema Final en la seccion //requestFormPost, todas las variables del
        /// post metodo, en nodos con sus valores respectivos.
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="Me">this page</param>
        /// <returns></returns>
        public static XmlDocument AgregaRequestForm(XmlDocument XmlFinal, Page Me) {

            foreach (string item in Me.Request.Form) {
                XmlElement newElem = XmlFinal.CreateElement(item);
                newElem.InnerText = Me.Request.Form[item].ToString();

                XmlElement newElem2 = XmlFinal.CreateElement(item);
                newElem2.InnerText = Me.Request.Form[item].ToString();

                XmlFinal.SelectSingleNode("//requestFormPost").AppendChild(newElem2);
                XmlFinal.SelectSingleNode("//requestFormApp").AppendChild(newElem);
            }

            return XmlFinal;
        }

        /// <summary>
        /// Agrega En El Esquema Final en la seccion //requestFormPost, todas las variables del
        /// post metodo, en nodos con sus valores respectivos.
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="Me">this page</param>
        /// <returns></returns>
        public static XmlDocument AgregaRequestAll(XmlDocument XmlFinal, Page Me) {

            foreach (string item in Me.Request.Form) {
                //XmlElement newElem = XmlFinal.CreateElement(item);
                //newElem.InnerText = Me.Request.Form[item].ToString();

                XmlElement newElem2 = XmlFinal.CreateElement(item);
                newElem2.InnerText = Me.Request.Form[item].ToString();

                XmlFinal.SelectSingleNode("//requestFormPost").AppendChild(newElem2);
                //XmlFinal.SelectSingleNode("//requestFormApp").AppendChild(newElem);
            }

            foreach (string item in Me.Request.QueryString) {
                //Creamos Nodo llamado servicio
                if (item != null) {
                    //XmlElement newElem = XmlFinal.CreateElement(item);
                    //newElem.InnerText = Me.Request.QueryString[item].ToString();

                    XmlElement newElem2 = XmlFinal.CreateElement(item);
                    newElem2.InnerText = Me.Request.QueryString[item].ToString();

                    XmlFinal.SelectSingleNode("//requestFormPost").AppendChild(newElem2);
                    //XmlFinal.SelectSingleNode("//requestFormApp").AppendChild(newElem);
                }
            }

            return XmlFinal;
        }

        /// <summary>
        /// Moves one level up a node. Works on level 4 and over.
        /// </summary>
        /// <param name="XmlFinal">xmldoc</param>
        /// <param name="XpathSingleNodeSubir">the node to move one level up.</param>
        /// <returns></returns>
        public static XmlDocument SubirNodoUnNivel(XmlDocument XmlFinal, string XpathSingleNodeSubir) {
            XmlDocument xmlA = new XmlDocument();
            XmlDocument xmlB = new XmlDocument();
            XmlNode NodoAbuelo, NodoPadre, NodoSubir, NodoClone;

            if (XmlHelper.ExisteXpath(XmlFinal, XpathSingleNodeSubir)) {
                NodoSubir = XmlFinal.SelectSingleNode(XpathSingleNodeSubir);
                NodoClone = NodoSubir.CloneNode(true);

                NodoPadre = NodoSubir.ParentNode;
                NodoAbuelo = NodoPadre.ParentNode; //Sacar El Abuelo.

                NodoPadre.RemoveChild(NodoSubir);


                NodoAbuelo.AppendChild(NodoClone);
            }
            return XmlFinal;
        }

        public static XmlDocument RenameNodes(XmlDocument XmlFinal, string XpathNodes, string NewNodeName) {
            XmlElement newNodeElement;

            XmlNodeList ListaNodos = XmlFinal.SelectNodes(XpathNodes);
            foreach (XmlNode oldNode in ListaNodos) {
                //Extraer Todo el SubArbol.
                string strXmlNodoOld = oldNode.InnerXml;
                newNodeElement = XmlFinal.CreateElement(NewNodeName);

                //Paso atributos
                foreach (XmlAttribute Atributo in oldNode.Attributes) {
                    newNodeElement.SetAttribute(Atributo.Name, Atributo.Value);
                }

                newNodeElement.InnerXml = strXmlNodoOld;
                oldNode.ParentNode.ReplaceChild(newNodeElement, oldNode);
            }

            return XmlFinal;
        }

        /// <summary>
        /// Renames A Single Node.
        /// </summary>
        /// <param name="XmlFinal">xmldoc</param>
        /// <param name="XpathSingleNode">the node u want to rename</param>
        /// <param name="NewNodeName">new node name</param>
        /// <returns></returns>
        public static XmlDocument RenameNodeSingle(XmlDocument XmlFinal, string XpathSingleNode, string NewNodeName) {
            XmlNode oldNode;
            XmlElement newNodeElement;


            if (XmlHelper.ExisteXpath(XmlFinal, XpathSingleNode)) {
                oldNode = XmlFinal.SelectSingleNode(XpathSingleNode);

                newNodeElement = XmlFinal.CreateElement(NewNodeName);

                //Paso atributos
                foreach (XmlAttribute Atributo in oldNode.Attributes) {
                    newNodeElement.SetAttribute(Atributo.Name, Atributo.Value);
                }

                //Pasar Hijos
                foreach (XmlNode nodeTmp in oldNode.ChildNodes) {
                    newNodeElement.AppendChild(nodeTmp);
                }

                oldNode.ParentNode.ReplaceChild(newNodeElement, oldNode);

            }
            return XmlFinal;
        }

        //select a node then recursively.
        /// <summary>
        /// Finds empty nodes then fills them with a whitespace #160.
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="XpathNode">Los nodos para arreglar: ex.- XpathNode + //*[.='']</param>
        /// <returns></returns>
        public static XmlDocument ArreglarNulosToWhiteSpace(XmlDocument XmlFinal, string XpathNode) {
            XmlNodeList NodoListas = XmlFinal.SelectNodes(XpathNode + "//*[.='']");
            foreach (XmlNode nodeItem in NodoListas) {
                nodeItem.InnerText = " ";
            }
            return XmlFinal;
        }

        public static XmlDocument ArreglarNulosEmptyAlValor(XmlDocument XmlFinal, string XpathNode, string AlValor) {
            XmlNodeList NodoListas = XmlFinal.SelectNodes(XpathNode);
            string oldValor;
            foreach (XmlNode nodeItem in NodoListas) {
                oldValor = nodeItem.InnerText;
                if (oldValor.Trim().Replace(" ", "").ToString().Equals(string.Empty)) {
                    nodeItem.InnerText = "0";
                }
            }
            return XmlFinal;
        }


        /// <summary>
        /// Elimina Los Ceros antes del Rut
        /// </summary>
        /// <param name="XmlFinal"></param>
        /// <param name="XpathNode"></param>
        /// <returns></returns>
        public static XmlDocument CastRut(XmlDocument XmlFinal, string XpathNode) {
            XmlFinal = XmlHelper.removerCerosDeNumerosNodos(XmlFinal, XpathNode);
            return XmlFinal;
        }

        /// <summary>
        /// Remover Ceros De Los Numeros De Los Nodos (UnZeroFill).
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="XpathNode">Los Nodos A Selecionar</param>
        /// <returns></returns>
        public static XmlDocument removerCerosDeNumerosNodos(XmlDocument XmlFinal, string XpathNode) {
            XmlNodeList NodoListas = XmlFinal.SelectNodes(XpathNode);
            foreach (XmlNode nodeItem in NodoListas) {
                nodeItem.InnerText = strHelper.removerCeros(nodeItem.InnerText);
            }
            return XmlFinal;
        }

        /// <summary>
        /// Remplaza nodos desde una lista de nodos separados por coma.
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="strCollectionNodos">nodos separados por comma</param>
        /// <param name="Replace">el texto a remplazar</param>
        /// <returns>XmlDocument XmlFinal</returns>
        public static XmlDocument ReplaceNodoTexto(XmlDocument XmlFinal, string strCollectionNodos, string ReplaceText) {
            string[] strLista = strHelper.SplitStrToArray(strCollectionNodos, ",");
            foreach (string item in strLista) {
                //cada item es un nodo nombre.
                XmlNodeList NodoListas = XmlFinal.GetElementsByTagName(item);
                foreach (XmlNode nodeItem in NodoListas) {
                    nodeItem.InnerXml = ReplaceText;
                }
            }
            return XmlFinal;
        }

        /// <summary>
        /// Recursive String.Replace Nodes 
        /// </summary>
        /// <param name="XmlFinal">xml doc</param>
        /// <param name="strCollectionNodos">Only Nodes Names Separated With Comma</param>
        /// <param name="ReplaceText">find text</param>
        /// <param name="ReplaceWith">replace with</param>
        /// <returns></returns>
        public static XmlDocument ReplaceNodoTextoWith(XmlDocument XmlFinal, string strCollectionNodos, string ReplaceText, string ReplaceWith) {
            string[] strLista = strHelper.SplitStrToArray(strCollectionNodos, ",");
            foreach (string item in strLista) {
                //cada item es un nodo nombre.
                XmlNodeList NodoListas = XmlFinal.GetElementsByTagName(item);
                foreach (XmlNode nodeItem in NodoListas) {
                    nodeItem.InnerText = nodeItem.InnerText.Replace(ReplaceText, ReplaceWith);
                }
            }
            return XmlFinal;
        }

        /// <summary>
        /// Recusive Append DV to Rut.
        /// </summary>
        /// <param name="XmlFinal">xmldoc</param>
        /// <param name="strCollectionNodos">name of node, separated with comma</param>
        /// <returns></returns>
        public static XmlDocument ReplaceRutConDV(XmlDocument XmlFinal, string strCollectionNodos) {
            string[] strLista = strHelper.SplitStrToArray(strCollectionNodos, ",");
            foreach (string item in strLista) {
                //cada item es un nodo nombre.
                XmlNodeList NodoListas = XmlFinal.GetElementsByTagName(item);
                foreach (XmlNode nodeItem in NodoListas) {
                    nodeItem.InnerXml = strHelper.GetRutDV(nodeItem.InnerXml);
                }
            }
            return XmlFinal;
        }

        /// <summary>
        /// Replace Caracter To The InnerXml then uppers.
        /// </summary>
        /// <param name="XmlFinal">xmldoc</param>
        /// <param name="strCollectionNodos">collection of nodes</param>
        /// <param name="caracter">find this caracter</param>
        /// <param name="reemplazo">replacement</param>
        /// <returns></returns>
        public static XmlDocument ReplaceNodoTextoCaracteres(XmlDocument XmlFinal, string strCollectionNodos, string caracter, string reemplazo) {
            string[] strLista = strHelper.SplitStrToArray(strCollectionNodos, ",");
            foreach (string item in strLista) {
                //cada item es un nodo nombre.
                XmlNodeList NodoListas = XmlFinal.GetElementsByTagName(item);
                foreach (XmlNode nodeItem in NodoListas) {
                    nodeItem.InnerXml = nodeItem.InnerXml.Replace(caracter, reemplazo).ToUpper();
                }
            }
            return XmlFinal;
        }

        /// <summary>
        /// Agrega Atributos a un nodo, separarlos por separador ,
        /// </summary>
        /// <param name="XmlFinal">xmlfinal para cambiar</param>
        /// <param name="XPath">el nodo para agregar atributo</param>
        /// <param name="NombreAtributo">nombre(s)</param>
        /// <param name="ValorAtributo">valore(s)</param>
        /// <returns></returns>
        public static XmlDocument AgregarAtributo(XmlDocument XmlFinal, string XPath, string strCollectionAtributo, string strCollectionValorAtributo) {
            if (ExisteXpath(XmlFinal, XPath)) {
                string[] strListaAttrb = strHelper.SplitStrToArray(strCollectionAtributo, ",");
                string[] strListaValor = strHelper.SplitStrToArray(strCollectionValorAtributo, ",");
                int j = 0;
                foreach (string item in strListaAttrb) {
                    //creamos un atributo llamado name.
                    XmlAttribute newAttr = XmlFinal.CreateAttribute(item);
                    newAttr.Value = strListaValor[j];

                    //creamos una colecion de attibutos.
                    XmlAttributeCollection attrColl = XmlFinal.SelectSingleNode(XPath).Attributes;

                    //se le agrega.
                    attrColl.SetNamedItem(newAttr);
                    j++;
                }
            }
            return XmlFinal;
        }

        /// <summary>
        /// Se agrega un nodo en el xpath.
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="XPath">Donde En El Xml</param>
        /// <param name="NombreElemento">El Nombre del nodo nuevo</param>
        /// <returns>XmlDocument XmlFinal Con Nodo Nuevo</returns>
        public static XmlDocument AgregaNodo(XmlDocument XmlFinal, string XPath, string NombreElemento) {
            if (ExisteXpath(XmlFinal, XPath)) {
                XmlElement newElem = XmlFinal.CreateElement(NombreElemento);
                XmlFinal.SelectSingleNode(XPath).AppendChild(newElem);
            }
            return XmlFinal;

        }

        public static XmlDocument AgregaNodoConAtributos(XmlDocument XmlFinal, string XPath, string NombreElemento, string strCollectionAtributo, string strCollectionValorAtributo) {
            if (ExisteXpath(XmlFinal, XPath)) {
                XmlElement newElem = XmlFinal.CreateElement(NombreElemento);
                string[] strListaAttrb = strHelper.SplitStrToArray(strCollectionAtributo, ",");
                string[] strListaValor = strHelper.SplitStrToArray(strCollectionValorAtributo, ",");
                int j = 0;
                foreach (string item in strListaAttrb) {
                    //creamos un atributo llamado name.
                    XmlAttribute newAttr = XmlFinal.CreateAttribute(item);
                    newAttr.Value = strListaValor[j];

                    //creamos una colecion de attibutos.
                    XmlAttributeCollection attrColl = newElem.Attributes;

                    //se le agrega.
                    attrColl.SetNamedItem(newAttr);
                    j++;
                }
                XmlFinal.SelectSingleNode(XPath).AppendChild(newElem);
            }
            return XmlFinal;
        }

        public static XmlDocument AgregaNodoConAtributosporPuntoComa(XmlDocument XmlFinal, string XPath, string NombreElemento, string strCollectionAtributo, string strCollectionValorAtributo) {
            if (ExisteXpath(XmlFinal, XPath)) {
                XmlElement newElem = XmlFinal.CreateElement(NombreElemento);
                string[] strListaAttrb = strHelper.SplitStrToArray(strCollectionAtributo, ";");
                string[] strListaValor = strHelper.SplitStrToArray(strCollectionValorAtributo, ";");
                int j = 0;
                foreach (string item in strListaAttrb) {
                    //creamos un atributo llamado name.
                    XmlAttribute newAttr = XmlFinal.CreateAttribute(item);
                    newAttr.Value = strListaValor[j];

                    //creamos una colecion de attibutos.
                    XmlAttributeCollection attrColl = newElem.Attributes;

                    //se le agrega.
                    attrColl.SetNamedItem(newAttr);
                    j++;
                }
                XmlFinal.SelectSingleNode(XPath).AppendChild(newElem);
            }
            return XmlFinal;
        }

        /// <summary>
        /// Se agrega un nodo en el xpath con el text (valor de nodo) deseado
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="XPath">Donde En El Xml</param>
        /// <param name="NombreElemento">El Nombre del Nodo Nuevo</param>
        /// <param name="strTxt">El Contenido del Nodo Nuevo</param>
        /// <returns>XmlDocument XmlFinal Con Nodo Nuevo</returns>
        public static XmlDocument AgregaNodoConTexto(XmlDocument XmlFinal, string XPath, string NombreElemento, string strTxt) {
            if (ExisteXpath(XmlFinal, XPath)) {
                XmlElement newElem = XmlFinal.CreateElement(NombreElemento);
                newElem.InnerText = strTxt;
                XmlFinal.SelectSingleNode(XPath).AppendChild(newElem);
            }
            return XmlFinal;
        }

        /// <summary>
        /// Se agrega un nodo en el xpath con el text (valor de nodo) deseado
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="XPath">Donde En El Xml</param>
        /// <param name="NombreElemento">El Nombre del Nodo Nuevo</param>
        /// <param name="strTxt">El Contenido del Nodo Nuevo</param>
        /// <returns>XmlDocument XmlFinal Con Nodo Nuevo</returns>
        public static XmlDocument AgregaMultiNodoConTexto(XmlDocument XmlFinal, string XPath, string NombreElementos, string TextElemento, string Separador) {
            if (ExisteXpath(XmlFinal, XPath)) {
                if (Separador.Equals(""))
                    Separador = ",";

                string[] strListaNodoName = strHelper.SplitStrToArray(NombreElementos, Separador);
                string[] strListaNodoText = strHelper.SplitStrToArray(TextElemento, Separador);

                int CounterName = strListaNodoName.Length;
                int CounterText = strListaNodoText.Length;

                for (int x = 0; x < CounterName; x++) {
                    XmlElement newElem = XmlFinal.CreateElement(strListaNodoName[x]);

                    //si es text count es menor que el total entonces agregar el texto al nodo.
                    if (x <= CounterText) {
                        newElem.InnerText = strListaNodoText[x];
                    }

                    XmlFinal.SelectSingleNode(XPath).AppendChild(newElem);
                }

            }
            return XmlFinal;
        }

        /// <summary>
        /// Agrega Nodo Unico, Si el nodo ya es igual a un hermano, no se agrega.
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="XPath">El Nodo Padre</param>
        /// <param name="NombreElemento">Nombre del Nuevo Nodo</param>
        /// <param name="strTxt">Texto del nodo</param>
        /// <returns>XmlDocument</returns>
        public static XmlDocument AgregaNodoUnico(XmlDocument XmlFinal, string XPath, string NombreElemento, string strTxt) {
            if (ExisteXpath(XmlFinal, XPath)) {
                XmlElement newElem = XmlFinal.CreateElement(NombreElemento);
                newElem.InnerText = strTxt;
                if (XmlFinal.SelectSingleNode(XPath + "/" + NombreElemento) == null) {
                    XmlFinal.SelectSingleNode(XPath).AppendChild(newElem);
                }
            }
            return XmlFinal;
        }

        /// <summary>
        /// Agrega Nodo Unico, Si el nodo ya es igual a un hermano, no se agrega.
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="XPath">El Nodo Padre</param>
        /// <param name="NombreElemento">Nombre del Nuevo Nodo</param>
        /// <param name="strTxt">Texto del nodo</param>
        /// <returns>XmlDocument</returns>
        public static XmlDocument AgregaNodoUnicoTextoUnico(XmlDocument XmlFinal, string XPath, string NombreElemento, string strTxt) {
            if (ExisteXpath(XmlFinal, XPath)) {
                XmlElement newElem = XmlFinal.CreateElement(NombreElemento);
                newElem.InnerText = strTxt;

                //Elegir un elemento y con el texto igual
                if (XmlFinal.SelectSingleNode(XPath + "/" + NombreElemento + "[.='" + strTxt + "']") == null) {
                    XmlFinal.SelectSingleNode(XPath).AppendChild(newElem);
                }
            }
            return XmlFinal;
        }

        /// <summary>
        /// Agrega Un Servicio Nuevo a la raiz del XmlFinal Schema:
        /// Se crear un nodo "servicio" con atributo name deseado.
        /// Se agrega dentro del nodo servicio el xmlRows (desde un xml for raw)
        /// Se ingresa en XmlFinal DocumentElement(raiz) El Nuevo servicio.
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="NombreServicio">Nuevo Nombre Del Servicio</param>
        /// <param name="xmlRows">for xml raw</param>
        /// <returns>XmlDocument XmlFinal Con Servio A Raiz</returns>
        public static XmlDocument UnirServicioRaiz(XmlDocument XmlFinal, string NombreServicio, string xmlRows) {
            //Creamos Nodo llamado servicio
            XmlElement newElem = XmlFinal.CreateElement("servicio");
            //creamos un atributo llamado name.
            XmlAttribute newAttr = XmlFinal.CreateAttribute("name");
            newAttr.Value = NombreServicio;
            //asigno atributo al nodo
            newElem.Attributes.Append(newAttr);
            //cargamos text del nodo
            newElem.InnerXml = xmlRows;
            //Asignal al documentXML el nodo final.
            XmlFinal.DocumentElement.AppendChild(newElem);
            return XmlFinal;
        }

        /// <summary>
        /// Agrega Un Servicio Nuevo a XmlFinal Schema, donde el Xpath apunta al nodo desea.:
        /// Se crear un nodo "servicio" con atributo name deseado.
        /// Se agrega dentro del nodo servicio el xmlRows (desde un xml for raw)
        /// Se ingresa el servicio en SelectSingleNode(XPath) del XmlFinal 
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="NombreServicio">Nuevo Nombre Del Servicio</param>
        /// <param name="xmlRows">for xml raw</param>
        /// <param name="XPath">El nodo que contendra el nuevo servicio</param>
        /// <returns>XmlDocument XmlFinal Con Servio A Nodo Deseado</returns>
        public static XmlDocument UnirServicioConNodo(XmlDocument XmlFinal, string NombreServicio, string xmlRows, string XPath) {
            //Creamos Nodo llamado servicio
            XmlElement newElem = XmlFinal.CreateElement("servicio");
            //creamos un atributo llamado name.
            XmlAttribute newAttr = XmlFinal.CreateAttribute("name");
            newAttr.Value = NombreServicio;
            //asigno atributo al nodo
            newElem.Attributes.Append(newAttr);
            //cargamos text del nodo
            newElem.InnerXml = xmlRows;
            //Selecionamos nodo y agregamos el servicio
            XmlFinal.SelectSingleNode(XPath).AppendChild(newElem);
            return XmlFinal;
        }

        /// <summary>
        /// Borrar un Padre Nodo, si el XPathCondicion del hijo cumple. Arbol Listado/Record/fono. Condicion ej: "//fono[.='']".
        /// Borrar el Record que no tenga fono 
        /// </summary>
        /// <param name="xmlArbol">xmlArbol ejemplo: Listado/Record/fono</param>
        /// <param name="XPathCondicion">Condicion ej: "//fono[.='']" Borrar el Record que no tenga fono </param>
        /// <returns>XmlDocument xmlArbol Con Nodos Borrados Segun Condicion</returns>
        public static XmlDocument RemoverNodo(XmlDocument xmlArbol, string XPathCondicion) {
            XmlNode NodoPadre;
            //Xpath para borrar Naturaleza  que no sean.

            XmlNodeList NodoListas = xmlArbol.SelectNodes(XPathCondicion);
            if (NodoListas != null) {
                foreach (XmlNode nodeItem in NodoListas) {
                    NodoPadre = nodeItem.ParentNode.ParentNode;
                    NodoPadre.RemoveChild(nodeItem.ParentNode);
                    NodoPadre = null;
                }
            }
            return xmlArbol;
        }

        /// <summary>
        /// Borrar Ese Nodo, si el XPathCondicion cumple. 
        /// Borrar el Record que no tenga fono 
        /// </summary>
        /// <param name="xmlArbol">xmlArbol </param>
        /// <param name="XPathNodeCondition">EL Nodo </param>
        /// <returns>XmlDocument xmlArbol Con Nodos Borrados Segun Condicion</returns>
        public static XmlDocument RemoverThisNodo(XmlDocument xmlArbol, string XPathNodeCondition) {
            XmlNode NodoPadre;
            XmlNodeList NodoListas = xmlArbol.SelectNodes(XPathNodeCondition);
            if (NodoListas != null) {
                foreach (XmlNode nodeItem in NodoListas) {
                    NodoPadre = nodeItem.ParentNode;
                    NodoPadre.RemoveChild(nodeItem);
                    NodoPadre = null;
                }
            }
            return xmlArbol;
        }

        /// <summary>
        /// Remover nodos padres que no esten el rango numerico, x mayor igual que 'Desde' x o menor igual que 'Hasta'. Basado de
        /// string Condicion = XPathNodeCondition + " [not (. mayor= " + Desde + " and . menor=  " + Hasta +")]"; 
        /// </summary>
        /// <param name="xmlArbol">Arbol Con Datos</param>
        /// <param name="XPathNodeFechaIni">El Nodo Hijo A condicionar ex.-"//fecha"</param>
        /// <param name="Desde">desde igual</param>
        /// <param name="Hasta">hasta igual</param>
        /// <returns>Arbol Filtrado por rango.</returns>
        public static XmlDocument RemoverNodosPorRango(XmlDocument xmlArbol, string XPathNodeFechaIni, string Desde, string Hasta) {
            XmlDocument XmlAuxA = xmlArbol;
            if (!(strHelper.isEmpty(Desde) && strHelper.isEmpty(Hasta))) {
                string Condicion = XPathNodeFechaIni + " [not (. >= " + Desde + " and . <=  " + Hasta + ")]";
                XmlAuxA = XmlHelper.RemoverNodo(XmlAuxA, Condicion);
            }
            return XmlAuxA;
        }

        //une un subarbol a un nodo de otro arbol padre., Xpath es el nodo del arbol padre.
        /// <summary>
        /// Unir Arbol A Un Nodo de otro Arbol
        /// </summary>
        /// <param name="XmlFinal">XmlDocument XmlFinal (Con Nodo Para Ingresar Nuevo Arbol)</param>
        /// <param name="XmlAuxA">XmlDocument Arbol Nuevo </param>
        /// <param name="Xpath">El Nodo En XmlFinal, que ingresa el nuevo arbol</param>
        /// <returns>XmlDocument XmlFinal Con Un Nodo LLeno de Otro Arbol.</returns>
        public static XmlDocument UnirArbol(XmlDocument XmlFinal, XmlDocument XmlAuxA, string Xpath) {
            if (ExisteXpath(XmlFinal, Xpath)) {
                XmlNode newSubArbol = XmlFinal.ImportNode(XmlAuxA.DocumentElement, true);
                XmlFinal.SelectSingleNode(Xpath).AppendChild(newSubArbol);
            }
            return XmlFinal;
        }

        /// <summary>
        /// Obtiene el contenido del nodo, si nodo es null retorna string.Empty
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="Xpath">Nodo Deseado</param>
        /// <returns>valor, texto, contenido del nodo</returns>
        public static string strGetNodoTexto(XmlDocument XmlFinal, string Xpath) {
            XmlNode objNode = XmlFinal.SelectSingleNode(Xpath);
            string strRet = string.Empty;
            if (objNode != null) {
                strRet = objNode.InnerText.ToString();
            }
            return strRet;
        }

        /// <summary>
        /// Obtiene el contenido del nodo, si nodo es null o vacio, busca en una cokiee, sino retorna string.Empty
        /// guarda el valor en el nodo y en la cokiee si no esta antes
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="Xpath">Nodo Deseado</param>
        /// <param name="Pag">La pagina actual</param>
        /// <returns>valor, texto, contenido del nodo</returns>
        public static string GetFilter(XmlDocument XmlFinal, string Xpath, Page Pag)
        {
            string strRet = string.Empty;
            strRet = strGetNodoTexto(XmlFinal, Xpath);
            if (strRet == ""){
                strRet = sessionHelper.GetSession(Xpath, "", Pag);
                XmlHelper.ReCreatedTextNode(XmlFinal, Xpath, strRet);
            }
            sessionHelper.SetCookies(Xpath, strRet, Pag);            
            return strRet;
        }

        public static string sGNT(XmlDocument XmlFinal, string Xpath) {
            return strGetNodoTexto(XmlFinal, Xpath);
        }

        /// <summary>
        /// Obtener el atributo desde un nodo
        /// </summary>
        /// <param name="XpathNodo">Nodo Deseado</param>
        /// <param name="XpathAttributo">Nombre del Atributo Deseado, sin @</param>
        /// <returns>valor del atributo, si nulo retorna string.Empty</returns>
        public string strGetNodoGetAttributo(string XpathNodo, string XpathAttributo) {
            string strRetorno = string.Empty;
            XmlAttribute atrTmp = null;

            XmlNode Nodo = this.XmlFinal.SelectSingleNode(XpathNodo);

            if (Nodo != null) {
                atrTmp = Nodo.Attributes[XpathAttributo];
                if (atrTmp != null) {
                    strRetorno = atrTmp.Value;
                }
            }
            return strRetorno;
        }

        public static string strGetAttributo(XmlNode Nodo, string XpathAttributo) {
            string strRetorno = string.Empty;
            XmlAttribute atrTmp = null;
            if (Nodo != null) {
                atrTmp = Nodo.Attributes[XpathAttributo];
                if (atrTmp != null) {
                    strRetorno = atrTmp.Value;
                }
            }
            return strRetorno;
        }

        public static string strGetNodoGetAttributo(XmlDocument XmlFinal, string XpathNodo, string XpathAttributo) {
            string strRetorno = string.Empty;
            XmlAttribute atrTmp = null;

            XmlNode Nodo = XmlFinal.SelectSingleNode(XpathNodo);

            if (Nodo != null) {
                atrTmp = Nodo.Attributes[XpathAttributo];
                if (atrTmp != null) {
                    strRetorno = atrTmp.Value;
                }
            }
            return strRetorno;
        }

        /// <summary>
        /// Creates a Session for the XmlPopUp
        /// </summary>
        /// <param name="XmlFinal">XmlFinal we want to save for the windows pop up.</param>
        /// <param name="Me">this page</param>
        public static void appCreaPopXml(XmlDocument XmlFinal, Page Me) {
            if (cnfHelper.GetConfigAppVariable("PopUpXML").ToUpper().Equals("TRUE")) {
                Me.Session.Add("XmlFinal", XmlFinal.InnerXml);
            }
            else {
                Me.Session.Add("XmlFinal", "PopUpXML.Equals.FALSE. Check webconfig");
            }
        }

        /// <summary>
        /// If this goes before XmlHelper.appDibujarHTML, then it will send an excel file content disposition. 
        /// then  draws (using another simpl xsl). Finaly ends this page.
        /// </summary>
        /// <param name="XmlFinal">XmlFinal To Export</param>
        /// <param name="PathXsl">new xsl template</param>
        /// <param name="Me">this page</param>
        /// <param name="Accion">if starts with EXCEL,then exports.</param>
        /// <param name="FileNameExport">the Excel FileName</param>
        public static void appExcelExportHTML(XmlDocument XmlFinal, string PathXsl, Page Me, string Accion, string FileNameExport) {
            if (Accion.ToUpper().StartsWith("EXCEL")) {
                Me.Response.Buffer = false;
                Me.Response.ContentType = "application/vnd.ms-excel";
                Me.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + FileNameExport + "\"");
                XmlHelper.appDibujarHTML(XmlFinal, PathXsl, Me);
                Me.Response.End();
            }
        }

        /// <summary>
        /// Prints The Transformation with XmlFinal and XSL.
        /// xml version="1.0" encoding="ISO-8859-1" should be in top of xsl.
        /// for spanish characters like ñ to work properly.
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="PathXsl">the path where xsl resides</param>
        /// <param name="Me">this page.</param>
        public static void appDibujarHTML(XmlDocument XmlFinal, string PathXsl, Page Me) {

            string CacheType = cnfHelper.GetConfigAppVariable("CacheType");
            Me.Response.Buffer = false;

            switch (CacheType) {
                case "no-cache":
                //Me.Response.Expires= -1;
                Me.Response.CacheControl = "no-cache";
                break;
                case "":
                //Me.Response.Expires= -1;
                Me.Response.CacheControl = "no-cache";
                break;
                case null:
                //Me.Response.Expires= -1;
                Me.Response.CacheControl = "no-cache";
                break;
                case "public":
                Me.Response.Expires = 60;
                Me.Response.CacheControl = "public";
                break;
            }

            Me.Response.ContentType = "text/html";
            //Me.Response.Charset = "ISO-8859-1";
            //Me.Response.ContentEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
            Me.Response.ContentEncoding = Encoding.GetEncoding("utf-8");
            Me.EnableViewState = false;

            #region ".NET1.1 XSLT"
            MemoryStream oMS = new MemoryStream();

            //Load XSL
            XslTransform oXsl = new XslTransform();

            /**************** Fix Jose *************************/
            // Set credentials for loading the stylesheet
            XmlUrlResolver resolver = new XmlUrlResolver();
            resolver.Credentials = System.Net.CredentialCache.DefaultCredentials;
            oXsl.Load(PathXsl, resolver);
            /**************** Fix Jose *************************/
            //oXsl.Load(PathXsl ,XsltSettings.Default, resolver);


            /*****************************/
            //jose custom trans
            XsltArgumentList xslArgs = new XsltArgumentList();
            //create custom object
            NSDirectic myNSDirectic = new NSDirectic();
            //pass an instance of the custom object
            xslArgs.AddExtensionObject("urn:Directic", myNSDirectic);

            //Join XML with XSL
            oXsl.Transform(XmlFinal, xslArgs, oMS, null);
            /********************************/

            //Join XML with XSL
            //oXsl.Transform(XmlFinal,null, oMS,null);

            //Send HTML
            oMS.Position = 0;
            StreamReader oSR = new StreamReader(oMS, Encoding.GetEncoding("ISO-8859-1"));//Encoding.GetEncoding("ISO-8859-1")
            //string FinalText =  oSR.ReadToEnd(); 

            //si el xsl no trae un <?xml version="1.0" encoding="ISO-8859-1"?>
            //se debe poner esto pq las ñ no funcionan
            //FinalText = FinalText.Replace("¤","ñ");
            //FinalText = FinalText.Replace("¥","Ñ");
            Me.Response.Write(oSR.ReadToEnd());
            oSR.Close();
            oMS.Close();

            //XmlHelper.appWriteLogFile(XmlFinal);
            #endregion

            #region "****************  saxon XSLT  *************************"
            //			StreamReader SRXsl = null;
            //
            //			try	{				
            //				// Create a Processor instance.
            //				Processor processor = new Processor();
            //
            //				// Set credentials for loading the stylesheet
            //				XmlUrlResolver resolver = new XmlUrlResolver();
            //				resolver.Credentials =System.Net.CredentialCache.DefaultCredentials;				
            //				processor.XmlResolver  = resolver;
            //
            //				// Load the source document
            //				XdmNode input = processor.NewDocumentBuilder().Build(XmlFinal);
            //
            //				//stream lectura xsl
            //				SRXsl = new StreamReader(PathXsl,System.Text.Encoding.GetEncoding("ISO-8859-1"));
            //
            //				// Note that in this case the XML encoding can not be processed!
            ////				Reader       xslReader = new BufferedReader(new FileReader(PathXsl));
            ////				StreamSource xslSource = new StreamSource(xslReader);
            ////
            ////				xslSource.setSystemId(PathXsl);
            //
            //				// Create a XsltCompiler for the stylesheet.
            //				XsltCompiler Compiler = processor.NewXsltCompiler();
            //				//Direccion base para los include y doc() en el xsl
            //				//Compiler.BaseUri=new Uri(PathXsl);
            //
            //				// Create a transformer for the stylesheet.
            //				XsltTransformer transformer = Compiler.Compile(SRXsl.BaseStream).Load();
            //
            //				// Set the root node of the source document to be the initial context node
            //				transformer.InitialContextNode = input;
            //
            //				// Create a serializer
            //				Serializer serializer = new Serializer();
            //				serializer.SetOutputWriter(Me.Response.Output);
            //
            //				// Transform the source XML to final html
            //				transformer.Run(serializer);				
            //			}
            //			catch(Exception ex) 
            //			{
            //				Me.Response.Write(ex.Message+"<br/>");
            //				Me.Response.Write(ex.InnerException+"<br/>");
            //				Me.Response.Write(ex.Source+"<br/>");
            //				Me.Response.Write(ex.StackTrace+"<br/>");
            //			}
            //			finally{
            //				//cierra y desbloquea el xsl
            //				SRXsl.Close();
            //				Me.Response.End();	
            //			}
            //
            //		/**************** saxon *************************/ 
            #endregion
        }//fin_appDibujarHTML

        /// <summary>
        /// Prints The Transformation with XmlFinal and XSL.
        /// xml version="1.0" encoding="iso" should be in top of xsl.
        /// for spanish characters like ñ to work properly.
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="PathXsl">the path where xsl resides</param>
        /// <param name="Me">this page.</param>
        public static void appDibujarHTML35(XmlDocument XmlFinal, string PathXsl, Page Me) {

            string CacheType = cnfHelper.GetConfigAppVariable("CacheType");
            Me.Response.Buffer = false;

            switch (CacheType) {
                case "no-cache":
                //Me.Response.Expires= -1;
                Me.Response.CacheControl = "no-cache";
                break;
                case "":
                //Me.Response.Expires= -1;
                Me.Response.CacheControl = "no-cache";
                break;
                case null:
                //Me.Response.Expires= -1;
                Me.Response.CacheControl = "no-cache";
                break;
                case "public":
                Me.Response.Expires = 60;
                Me.Response.CacheControl = "public";
                break;
            }

            Me.Response.ContentType = "text/html";
            Me.Response.Charset = "iso-8859-1";
            Me.Response.ContentEncoding = System.Text.Encoding.GetEncoding("iso-8859-1");
            Me.EnableViewState = false;

            #region ".NET3.5 XSLT"
            ///**************** Fix Jose *************************/
            //// Set credentials for loading the stylesheet
            //XmlUrlResolver resolver = new XmlUrlResolver();
            //resolver.Credentials = System.Net.CredentialCache.DefaultCredentials;            
            ///**************** Fix Jose *************************/ 

            /*****************************/
            //jose custom trans
            XsltArgumentList xslArgs = new XsltArgumentList();//create custom object
            NSDirectic myNSDirectic = new NSDirectic(); //pass an instance of the custom object
            xslArgs.AddExtensionObject("urn:Directic", myNSDirectic);
            /********************************/

            // Load the style sheet.
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(PathXsl);

            //XmlWriterSettings settings = new XmlWriterSettings();            
            //settings.ConformanceLevel = ConformanceLevel.Fragment;
            //XmlWriter xmlWriter = XmlWriter.Create(Me.Response.OutputStream, settings);

            // Execute the transform and output the results to a file.
            xslt.Transform(new XmlNodeReader(XmlFinal), xslArgs, Me.Response.OutputStream);
            #endregion

        }//fin_appDibujarHTML

        public static string appDibujarXSLT(XmlDocument XmlFinal, string PathXsl) {

            MemoryStream oMS = new MemoryStream();

            //Load XSL
            XslTransform oXsl = new XslTransform();

            /**************** Fix Jose *************************/
            // Set credentials for loading the stylesheet
            //			XmlUrlResolver resolver = new XmlUrlResolver();
            //			resolver.Credentials =System.Net.CredentialCache.DefaultCredentials;
            //			oXsl.Load(PathXsl ,resolver);
            /**************** Fix Jose *************************/
            //oXsl.Load(PathXsl ,XsltSettings.Default, resolver);
            oXsl.Load(PathXsl);


            /*****************************/
            //jose custom trans
            XsltArgumentList xslArgs = new XsltArgumentList();
            //create custom object
            NSDirectic myNSDirectic = new NSDirectic();
            //pass an instance of the custom object
            xslArgs.AddExtensionObject("urn:Directic", myNSDirectic);

            //Join XML with XSL
            oXsl.Transform(XmlFinal, xslArgs, oMS, null);
            /********************************/

            //Join XML with XSL
            //oXsl.Transform(XmlFinal,null, oMS,null);

            //Send HTML
            oMS.Position = 0;
            StreamReader oSR = new StreamReader(oMS, System.Text.Encoding.GetEncoding("ISO-8859-1"));
            string FinalText = oSR.ReadToEnd();
            oSR.Close();
            oMS.Close();

            return FinalText;
        }

        /// <summary>
        /// Write To log\XmlFinal.xml all the XmlFinal XmlDocument.
        /// </summary>
        /// <param name="XmlFinal">XmlDocument</param>
        public static void appWriteLogFile(XmlDocument XmlFinal) {

            if (cnfHelper.GetConfigAppVariable("TraceXML").ToUpper().Equals("TRUE")) {

                string path = cnfHelper.GetConfigAppVariable("XmlFinalLog");
                // Delete the file if it exists.
                if (File.Exists(path)) {
                    File.Delete(path);
                }

                // Create the file.
                using (FileStream fs = File.Create(path, 1024)) {
                    Byte[] info = new UTF8Encoding(true).GetBytes(XmlFinal.OuterXml);
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
            }
        }

        /// <summary>
        /// Write To log\XmlFinal.xml all the XmlFinal XmlDocument.
        /// </summary>
        /// <param name="XmlFinal">XmlDocument</param>
        public static void WriteFile(XmlDocument XmlToWrite, string relativeFileName, Page Me) {
            string path = Me.Request.MapPath(relativeFileName);
            // Delete the file if it exists.
            if (File.Exists(path)) {
                File.Delete(path);
            }

            // Create the file.
            using (FileStream fs = File.Create(path, 1024)) {
                Byte[] info = new UTF8Encoding(true).GetBytes(XmlToWrite.OuterXml);
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }
        }

        /// <summary>
        /// Clones The Node, Appends it to the Xpath.
        /// </summary>
        /// <param name="XmlFinal">xmldoc</param>
        /// <param name="XpathParent">The New Clone Position</param>
        /// <param name="NodeSonName">The ADN</param>
        /// <param name="CloneName">New Clone Name</param>
        /// <returns></returns>

        public static XmlDocument CloneNodesAs(XmlDocument XmlFinal, string XpathParent, string NodeSonName, string CloneName) {
            XmlNodeList NodoListas = XmlFinal.SelectNodes(XpathParent);
            foreach (XmlNode nodeItem in NodoListas) {

                try {
                    string valor = nodeItem.SelectSingleNode(NodeSonName).InnerText;
                    XmlElement newElem3 = XmlFinal.CreateElement(CloneName);
                    newElem3.InnerText = valor;
                    nodeItem.AppendChild(newElem3);
                }
                catch //si nodo no existe en arbol, crear nodo con vacio.
                {
                    string valor = "";
                    XmlElement newElem3 = XmlFinal.CreateElement(CloneName);
                    newElem3.InnerText = valor;
                    nodeItem.AppendChild(newElem3);
                }
            }
            return XmlFinal;
        }

        /// <summary>
        /// Agrega Nodos En Cada Padre Encontrado
        /// </summary>
        /// <param name="XmlFinal">xmlfinal</param>
        /// <param name="XpathParent">Xpath a los nodos ejemplo //Record</param>
        /// <param name="NodeName">nombre</param>
        /// <param name="NodoValor">valor</param>
        /// <returns></returns>
        public static XmlDocument AgregarMultiNodoListaNodos(XmlDocument XmlFinal, string XpathPadreListaNodo, string NodoNombre, string NodoValor) {
            XmlNodeList NodoListas = XmlFinal.SelectNodes(XpathPadreListaNodo);
            foreach (XmlNode nodeItem in NodoListas) {
                XmlElement newElem3 = XmlFinal.CreateElement(NodoNombre);
                newElem3.InnerText = NodoValor;
                nodeItem.AppendChild(newElem3);
            }
            return XmlFinal;
        }

        /// <summary>
        /// Finds Nodos Y Los Remplaza con el texto a cambiar
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="XpathNode">Los nodos para arreglar: ex.- XpathNode + //*[.='']</param>
        /// <param name="TextoCambiar">texto fijo</param>
        /// <returns></returns>
        public static XmlDocument ReplaceTextNode(XmlDocument XmlFinal, string XpathNode, string TextoCambiar) {
            XmlNodeList NodoListas = XmlFinal.SelectNodes(XpathNode);
            foreach (XmlNode nodeItem in NodoListas) {
                nodeItem.InnerText = TextoCambiar;
            }
            return XmlFinal;
        }

        /// <summary>
        /// Busca el Nodo Y Lo Remplaza con el texto a cambiar, si no existe lo crea
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="XpathNode">El nodo para arreglar: ex.- XpathNode + //app/filtro</param>
        /// <param name="TextoCambiar">Texto fijo</param>
        /// <returns></returns>
        public static XmlDocument ReCreatedTextNode(XmlDocument XmlFinal, string XpathNode, string TextoCambiar) {
            if (ExisteXpath(XmlFinal, XpathNode)) {
                XmlFinal.SelectSingleNode(XpathNode).InnerText = TextoCambiar;
            }
            else {
                string Elemento = Path.GetFileName(XpathNode);
                if (Elemento != "") {
                    XpathNode = XpathNode.Replace("/" + Elemento, "");
                    AgregaNodoConTexto(XmlFinal, XpathNode, Elemento, TextoCambiar);
                }
            }
            return XmlFinal;
        }

        /// <summary>
        /// Formeatea Texto, primero minusculas luego primera letra mayuscula.
        /// </summary>
        /// <param name="XmlFinal">xmlfinal</param>
        /// <param name="XpathNode">los nodos</param>
        /// <param name="FormatType">el formato deseado, empty es defualt</param>
        /// <returns></returns>
        public static XmlDocument CapTextNodes(XmlDocument XmlFinal, string XpathNode, string FormatType) {
            XmlNodeList NodoListas = XmlFinal.SelectNodes(XpathNode);

            foreach (XmlNode nodeItem in NodoListas) {
                nodeItem.InnerText = strHelper.CapFirstLetterText(nodeItem.InnerText.ToLower());
            }
            return XmlFinal;
        }

        /// <summary>
        /// Formatea Fecha YYYY-MM-DD TIME
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="XpathNode">Los nodos para arreglar el formato de fecha: ex.- XpathNode + //*[.='']</param>
        /// <returns></returns>
        public static XmlDocument FormatDateNodes(XmlDocument XmlFinal, string XpathNodeFecha, string FechaInput, string FechaOutput) {
            XmlNodeList NodoListas = XmlFinal.SelectNodes(XpathNodeFecha);
            foreach (XmlNode nodeItem in NodoListas) {
                nodeItem.InnerText = dateHelper.FormatDate(nodeItem.InnerText, FechaInput, FechaOutput);

            }
            return XmlFinal;
        }

        public static XmlDocument FormatDateAttribute(XmlDocument XmlFinal, string XpathNodeFecha, string Attribute, string FechaInput, string FechaOutput) {
            XmlAttribute atrTmp = null;
            string attribute = string.Empty;
            XmlNodeList NodoListas = XmlFinal.SelectNodes(XpathNodeFecha);
            foreach (XmlNode nodeItem in NodoListas) {
                //nodeItem.InnerText = dateHelper.FormatDate(nodeItem.InnerText,FechaInput,FechaOutput);
                if (nodeItem != null) {
                    atrTmp = nodeItem.Attributes[Attribute];
                    if (atrTmp != null) {
                        XmlAttribute newAttr = XmlFinal.CreateAttribute(atrTmp.Name);
                        newAttr.Value = dateHelper.FormatDate(atrTmp.Value.ToString(), "YYYY MM DD TIME", "DD/MM/YYYY");

                        XmlAttributeCollection attrColl = nodeItem.Attributes;

                        //se le agrega.
                        attrColl.SetNamedItem(newAttr);
                    }
                }

            }
            return XmlFinal;
        }

        public static XmlDocument FormatDateAttributeByOutput(XmlDocument XmlFinal, string XpathNodeFecha, string Attribute, string FechaInput, string FechaOutput) {
            XmlAttribute atrTmp = null;
            string attribute = string.Empty;
            XmlNodeList NodoListas = XmlFinal.SelectNodes(XpathNodeFecha);
            foreach (XmlNode nodeItem in NodoListas) {
                if (nodeItem != null) {
                    atrTmp = nodeItem.Attributes[Attribute];
                    if (atrTmp != null) {
                        XmlAttribute newAttr = XmlFinal.CreateAttribute(atrTmp.Name);
                        newAttr.Value = dateHelper.FormatDate(atrTmp.Value.ToString(), FechaInput, FechaOutput);

                        XmlAttributeCollection attrColl = nodeItem.Attributes;

                        //se le agrega.
                        attrColl.SetNamedItem(newAttr);
                    }
                }

            }
            return XmlFinal;
        }

        public static XmlDocument TrimAllAttribute(XmlDocument XmlFinal, string XpathNode, string Attribute) {
            XmlAttribute atrTmp = null;
            string attribute = string.Empty;
            XmlNodeList NodoListas = XmlFinal.SelectNodes(XpathNode);
            foreach (XmlNode nodeItem in NodoListas) {
                //nodeItem.InnerText = dateHelper.FormatDate(nodeItem.InnerText,FechaInput,FechaOutput);
                if (nodeItem != null) {
                    atrTmp = nodeItem.Attributes[Attribute];
                    if (atrTmp != null) {
                        XmlAttribute newAttr = XmlFinal.CreateAttribute(atrTmp.Name);
                        newAttr.Value = atrTmp.Value.Trim();

                        XmlAttributeCollection attrColl = nodeItem.Attributes;

                        //se le agrega.
                        attrColl.SetNamedItem(newAttr);
                    }
                }

            }
            return XmlFinal;
        }

        public static XmlDocument SubStringAttribute(XmlDocument XmlFinal, string XpathNode, string Attribute, int Start, int End, string subfix) {
            XmlAttribute atrTmp = null;
            string attribute = string.Empty;
            XmlNodeList NodoListas = XmlFinal.SelectNodes(XpathNode);
            foreach (XmlNode nodeItem in NodoListas) {
                //nodeItem.InnerText = dateHelper.FormatDate(nodeItem.InnerText,FechaInput,FechaOutput);
                if (nodeItem != null) {
                    atrTmp = nodeItem.Attributes[Attribute];
                    if (atrTmp != null) {
                        XmlAttribute newAttr = XmlFinal.CreateAttribute(atrTmp.Name);

                        if (End > atrTmp.Value.Length) {
                            End = atrTmp.Value.Length;
                        }
                        newAttr.Value = atrTmp.Value.Substring(Start, End) + subfix;

                        XmlAttributeCollection attrColl = nodeItem.Attributes;

                        //se le agrega.
                        attrColl.SetNamedItem(newAttr);
                    }
                }

            }
            return XmlFinal;
        }

        /// <summary>
        /// Finds if the xpath exists.
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="XpathNode">Los nodos para arreglar: ex.- XpathNode + //*[.='']</param>
        /// <returns></returns>
        public static bool ExisteXpath(XmlDocument XmlFinal, string Xpath) {
            XmlNode objNode = XmlFinal.SelectSingleNode(Xpath);

            if (objNode != null) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remueve nodos que estan repetidos
        /// </summary>
        /// <param name="XmlFinal">XmlFinal</param>
        /// <param name="Xp">El Xpath del padre</param>
        /// <param name="Xs">El Nodo Hijo que no se quiere repetido mas de una vez.(Solo en position 1)</param>
        /// <returns></returns>
        public static XmlDocument RemoverNodosPathRepetido(XmlDocument XmlFinal, string Xp, string Xs) {

            XmlDocument tmpXml = new XmlDocument();
            tmpXml.LoadXml(XmlFinal.OuterXml);
            XmlNodeList NodoListas = XmlFinal.SelectNodes("//" + Xp);
            foreach (XmlNode nodeItem in NodoListas) {
                string NodeInnerText = nodeItem.SelectSingleNode("//" + Xs).InnerText.ToString();
                string condicion = string.Format("//" + Xp + "[{0}='{1}' and position()!=1]", Xs, NodeInnerText);
                tmpXml = XmlHelper.RemoverThisNodo(tmpXml, condicion);
                XmlHelper.appWriteLogFile(tmpXml);
            }
            XmlHelper.appWriteLogFile(tmpXml);
            return tmpXml;
        }

        public static XmlDocument RemoverNodosContenidoRepetido(XmlDocument XmlFinal, string XpathPadre) {

            XmlDocument xmlArbolVacio = new XmlDocument();
            //Extraer el nombre del padre.

            xmlArbolVacio = XmlHelper.ExtraerSubArbol(XmlFinal, XpathPadre);
            //Tengo Mi RecordSet1(no tiene record)
            string NombreRaiz = xmlArbolVacio.DocumentElement.Name;
            xmlArbolVacio.DocumentElement.InnerXml = "";
            XmlHelper.appWriteLogFile(xmlArbolVacio);

            XmlNodeList NodoListas = XmlFinal.SelectNodes(XpathPadre + "/*");
            foreach (XmlNode nodeItem in NodoListas) {
                string NombreNodo = nodeItem.Name;
                string TextNodo = nodeItem.InnerText;

                XmlHelper.AgregaNodoUnicoTextoUnico(xmlArbolVacio, "//" + NombreRaiz, NombreNodo, TextNodo);
                XmlHelper.appWriteLogFile(xmlArbolVacio);
            }

            XmlNode ChangeSubArbol = XmlFinal.SelectSingleNode(XpathPadre);
            ChangeSubArbol.InnerXml = xmlArbolVacio.DocumentElement.InnerXml;
            XmlHelper.appWriteLogFile(XmlFinal);
            return XmlFinal;
        }

        /// <summary>
        /// Separa XmlFinal, en la cantidad de xml deseados, en un arreglo.
        /// ejemplo.- SepararXmlEnArreglosXml("<a><b/><b/><b/><b/><b/><b/></a>",4)
        /// separaria <a><b/><b/></a>, etc. el ultimo xmldocumento estaria sin nodo b <a/>
        /// </summary>
        /// <param name="XmlFinal">xml a separar</param>
        /// <param name="Cantidad">dimension del arreglo con los xmldocuments separados</param>
        /// <returns>
        /// XmlDocument[] Arr;
        /// Arr = XmlHelper.SepararXmlEnArreglosXml(xmldoc,4);
        /// </returns>
        public static XmlDocument[] SepararXmlEnArreglosXml(XmlDocument XmlFinal, double Cantidad) {
            XmlDocument XmlAux = new XmlDocument();
            XmlDocument[] ArregloXML;
            ArregloXML = new XmlDocument[(int)Cantidad];

            //Primero Sacar El Nombre del tag raiz.
            string RaizNombre = XmlFinal.DocumentElement.Name;

            for (int i = 0; i < Cantidad; i++) {
                XmlDocument XmlAuxB = new XmlDocument();
                XmlAuxB = XmlHelper.CrearRaizXdoc(RaizNombre);
                ArregloXML[i] = XmlAuxB;
            }

            XmlNodeList NodoListas = XmlFinal.SelectNodes("//" + RaizNombre + "/*");
            double nodos = NodoListas.Count;
            double regxthr = Math.Ceiling(nodos / Cantidad);


            int PosArreglo = 0;
            int contador = 0;
            foreach (XmlNode nodeItem in NodoListas) {
                XmlAux.LoadXml(nodeItem.OuterXml);
                XmlHelper.appWriteLogFile(XmlAux);


                if (contador < regxthr) {
                    XmlHelper.UnirArbol(ArregloXML[PosArreglo], XmlAux, "//" + RaizNombre);
                    XmlHelper.appWriteLogFile(ArregloXML[PosArreglo]);
                }
                else {
                    PosArreglo++;
                    contador = 0;
                    XmlHelper.UnirArbol(ArregloXML[PosArreglo], XmlAux, "//" + RaizNombre);
                    XmlHelper.appWriteLogFile(ArregloXML[PosArreglo]);
                }
                contador++;
            }

            return ArregloXML;
        }

        /// <summary>
        /// Xmlhelper para parametos usados en Stp. Para Pasar Parametros.
        /// Crea un arbol con nodo raiz parametros
        /// </summary>
        /// <returns></returns>
        public static XmlDocument StpCrearParametros() {
            XmlDocument xmlTmp = new XmlDocument();
            xmlTmp = XmlHelper.CrearRaizXdoc("parametros");
            return xmlTmp;
        }

        public static XmlDocument StpCrearParametrosUFT16() {
            XmlDocument xmlTmp = new XmlDocument();
            xmlTmp = XmlHelper.CrearRaizXdocUTF16("parametros");
            return xmlTmp;
        }

        /// <summary>
        /// Ayuda a ingresar parametros al xml, estos se usan para los stp de basededatos
        /// </summary>
        /// <param name="XmlPara"></param>
        /// <param name="NodoName"></param>
        /// <param name="NodoText"></param>
        /// <returns></returns>
        public static XmlDocument StpAgregarNodoParametros(XmlDocument XmlPara, string NodoName, string NodoText) {
            XmlHelper.AgregaNodoUnico(XmlPara, "//parametros", NodoName, NodoText);
            return XmlPara;
        }

        public static XmlDocument SANP(XmlDocument XmlPara, string NodoName, string NodoText) {
            return StpAgregarNodoParametros(XmlPara, NodoName, NodoText);
        }



        /// <summary>
        /// Se Agrega un nuevo nodo llamado NombreNodo y Con ValorNodo en XmlNode PadreNodo (PadreNodo pertenece al XmlFinal)
        /// </summary>
        /// <param name="XmlFinal">El Arbol XML</param>
        /// <param name="PadreNodo">El XmlNode que se agregara un nuevo nodo</param>
        /// <param name="NombreNodo">El nombre del nuevo node</param>
        /// <param name="ValorNodo">El valor del nuevo nodo</param>
        /// <returns></returns>
        public static XmlNode NodeAgregar(XmlDocument XmlFinal, XmlNode PadreNodo, string NombreNodo, string ValorNodo) {
            XmlElement newElem = XmlFinal.CreateElement(NombreNodo);
            newElem.InnerText = ValorNodo;
            PadreNodo.AppendChild(newElem);
            return PadreNodo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NodeItemParent">XmlNode That Will Be Appended a XmlInner from a source xmldoc</param>
        /// <param name="XmlSource"></param>
        /// <param name="XpathSource"></param>
        /// <returns></returns>
        public static XmlNode NodeAppendXmlInner(XmlNode NodeItemParent, XmlDocument XmlSource, string XpathParentSourceInnerXml) {
            XmlDocument tmp = XmlHelper.ExtraerSubArbol(XmlSource, XpathParentSourceInnerXml);
            XmlHelper.appWriteLogFile(tmp);
            NodeItemParent.InnerXml += XmlHelper.ExtraerSubArbol(XmlSource, XpathParentSourceInnerXml).DocumentElement.InnerXml;
            return NodeItemParent;
        }

        /// <summary>
        /// Obtener un SubArbol De Un XML
        /// </summary>
        /// <param name="XmlFinal">El xmlfinal</param>
        /// <param name="XmlPath">Apuntar a un subarbol</param>
        /// <returns></returns>
        public static XmlDocument ExtraerSubArbol(XmlDocument XmlFinal, string XmlPath) {
            XmlDocument xmlaux = new XmlDocument();

            string xmlraw = "<nullsubtree/>";

            if (ExisteXpath(XmlFinal, XmlPath)) {
                xmlraw = XmlFinal.SelectSingleNode(XmlPath).OuterXml;
            }
            xmlaux.LoadXml(xmlraw);
            return xmlaux;
        }

        /// <summary>
        /// Con dos documentxml (XmlDocPadre,XmlDocNodosLista), se seleciona nodos desde el xmldocnodolista
        /// y luego con el xpathpadre se los agrega. Se retorno
        /// </summary>
        /// <param name="XmlDocPadre">el xml que obtendra los nuevs</param>
        /// <param name="XpathNodoPadre">el nodo del padre que obtendra mas nodos</param>
        /// <param name="XmlDocNodosLista">el xmldoc que tien los nodos </param>
        /// <param name="XpathNodosLista">el XPATH nodos que se quiere agregar al XmlDocPadre</param>
        /// <returns>XmlDocPadre Con Los Nuevos Nodos</returns>
        public static XmlDocument InsertarXmlInnerDesdeNodos(XmlDocument XmlDocPadre, string XpathNodoPadre, XmlDocument XmlDocNodosLista, string XpathNodosLista) {

            if (XmlHelper.ExisteXpath(XmlDocPadre, XpathNodoPadre) && XmlHelper.ExisteXpath(XmlDocNodosLista, XpathNodosLista)) {
                //si existe entonces selecionar ese nodo.
                XmlNode tmpNodoTarget = XmlDocPadre.SelectSingleNode(XpathNodoPadre);

                XmlNodeList Lista = XmlDocNodosLista.SelectNodes(XpathNodosLista);

                string InnerPadre = tmpNodoTarget.InnerXml;
                string NodosInner = "";

                foreach (XmlNode NodeItem in Lista) {
                    //sacar el nodoinnerxml.
                    NodosInner += NodeItem.OuterXml;
                }

                tmpNodoTarget.InnerXml = InnerPadre + NodosInner;

            }
            return XmlDocPadre;
        }

        //al parecer este encuentra todo los nodos del mismo nombre, se adjunta al primero sus hermanos
        //luego se borran de donde se encontraron.
        public static XmlDocument JoinSameNodes(XmlDocument XmlFinal, string XpathSameNodes, bool RemoveSiblings) {

            string InnerXmlPrimos = "";
            if (XmlHelper.ExisteXpath(XmlFinal, XpathSameNodes)) {

                XmlNode Primero = XmlFinal.SelectSingleNode(XpathSameNodes);

                XmlNodeList Lista = XmlFinal.SelectNodes(XpathSameNodes); //Toda La Lista

                foreach (XmlNode Primos in Lista) {
                    InnerXmlPrimos += Primos.InnerXml;
                }
                Primero.InnerXml = InnerXmlPrimos;


                if (RemoveSiblings) {
                    for (int x = 1; x < Lista.Count; x++) {
                        XmlNode nodeItem = Lista.Item(x);
                        XmlNode NodoPadre = nodeItem.ParentNode;
                        NodoPadre.RemoveChild(nodeItem);
                    }
                }

            }
            return XmlFinal;
        }

        /// <summary>
        /// Transform XML using A XSL File
        /// </summary>
        /// <param name="XmlData"></param>
        /// <param name="PathXsl"></param>
        /// <param name="Me"></param>
        /// <returns></returns>
        public static XmlDocument XslTransform(XmlDocument XmlData, string PathXslFile, Page Me) {

            XmlDocument xmlFinal = new XmlDocument();
            MemoryStream oMS = new MemoryStream();
            XslTransform oXsl = new XslTransform();

            try {
                oXsl.Load(Me.Request.MapPath(PathXslFile)); //Load XSL
                oXsl.Transform(XmlData, null, oMS, null); //Transform XML with XSL

                oMS.Position = 0;
                StreamReader oSR = new StreamReader(oMS, System.Text.Encoding.GetEncoding("ISO-8859-1"));
                xmlFinal.LoadXml(oSR.ReadToEnd());
            }
            catch (Exception Ex) {
                xmlFinal = XmlHelper.CrearXmlNull("XslTransformError");
                XmlHelper.AgregaNodoConTexto(xmlFinal, "//XslTransformError", "errorMsg", Ex.Message.Replace("'", "\""));
                throw Ex;
            }

            return xmlFinal;
        }

        /// <summary>
        /// Transforma Los Attributos A Nodos Hijos
        /// </summary>
        /// <param name="XmlFinal">xmldoc</param>
        /// <param name="Me"></param>
        /// <returns></returns>
        public static XmlDocument XslTransform_AttributesToNodes(XmlDocument XmlFinal, Page Me) {
            XmlDocument XmlAux = new XmlDocument();
            try {
                XmlFinal = XslTransform(XmlFinal, @"xsl\configuration\row_atttributes_to_nodes.xsl", Me);
            }
            catch {
                XmlFinal = CrearXmlNull("AttributesToNodesFailed");
            }
            return XmlFinal;
        }

        /// <summary>
        /// Crea arbol con nodo raiz personalizado. si nombre es nulo crea un arbol 
        /// con nodo raiz XmlNull
        /// </summary>
        /// <param name="raizname">nombre de la raiz, si es vacio es XmlNull</param>
        /// <returns>xmldocument</returns>
        public static XmlDocument CrearXmlNull(string raizname) {
            XmlDocument xmlTmp = new XmlDocument();

            if (strHelper.isEmpty(raizname)) {
                raizname = "XmlNull";
            }
            xmlTmp = XmlHelper.CrearRaizXdoc(raizname);
            return xmlTmp;
        }

        /// <summary>
        /// Crea un arbol con nombre de raiz personalizada, y cadena de xml
        /// </summary>
        /// <param name="raizname">nombre de la raiz, si es empty se usa "parametros" por defecto</param>
        /// <param name="GetRow">el xml cadena del arbol nuevo</param>
        /// <returns>XmlDocument.</returns>
        public static XmlDocument CrearXmlDocumentConGetRow(string raizname, string GetRow) {
            XmlDocument xmlTmp = new XmlDocument();

            if (strHelper.isEmpty(raizname)) {
                raizname = "parametros";
            }

            xmlTmp.LoadXml(string.Format(XML_TAG_ES + "<{0}>{1}</{0}>", raizname, GetRow));
            return xmlTmp;
        }

        public static XmlDocument CrearXmlDocumentConGetRowUTF16(string raizname, string GetRow) {
            XmlDocument xmlTmp = new XmlDocument();

            if (strHelper.isEmpty(raizname)) {
                raizname = "parametros";
            }

            xmlTmp.LoadXml(string.Format(XML_TAG_UTF16 + "<{0}>{1}</{0}>", raizname, GetRow));
            return xmlTmp;
        }

        /// <summary>
        /// con una cadena llena un arbol con nodo raiz salida
        /// </summary>
        /// <param name="strXmlPara">cadena de xml</param>
        /// <returns>xmldocument con nodo raiz salida</returns>
        public static XmlDocument strTrasformarXmlDoc(string strXml) {
            XmlDocument XmlSalida = XmlHelper.CrearRaizXdoc("salida");
            XmlSalida.SelectSingleNode("//salida").InnerXml = strXml;
            return XmlSalida;
        }

        /// <summary>
        /// Crear una raiz con el nombre deseado. Agrega el XmlHelper.XML_TAG_ES = iso-8859-1
        /// </summary>
        /// <param name="NombreRaiz">Nombre de la raiz</param>
        /// <returns>XmlDocument Con La Raiz Deseada</returns>
        public static XmlDocument CrearRaizXdoc(string NombreRaiz) {
            XmlDocument XmlFinal = new XmlDocument();
            XmlFinal.LoadXml(XmlHelper.XML_TAG_ES + "\n" + "<" + NombreRaiz + "/>");
            return XmlFinal;

        }

        public static XmlDocument CrearRaizXdocUTF16(string NombreRaiz) {
            XmlDocument XmlFinal = new XmlDocument();
            XmlFinal.LoadXml(XmlHelper.XML_TAG_UTF16 + "\n" + "<" + NombreRaiz + "/>");
            return XmlFinal;

        }

        /// <summary>
        /// Por Cadena Agregar una Raiz a una cadena XML.
        /// </summary>
        /// <param name="strXML">row a=1 n=2</param>
        /// <param name="NombreRaiz">nuevo nombre para la raiz</param>
        /// <returns>Retorna cadena pero con raiz nueva y llena del strXML. </returns>
        public static string strAgregarNodoRaiz(string strXML, string NombreRaiz) {
            // <row a=1 n=2>
            // "<raiz><row a=1 n=2>></rai>"
            string tmp = "<" + NombreRaiz + ">" + strXML + "</" + NombreRaiz + ">";
            return tmp;
        }

        /// <summary>
        /// Digiere una respuesta de un for xml raw. si esta vacia entrega un nodo "null_rows"
        /// </summary>
        /// <param name="strRows">for xml raw as string</param>
        /// <returns>itself or if strHelper.isEmpty return a root called null_rows</returns>
        public static string strDigestRows(string strRows) {

            if (strHelper.isEmpty(strRows)) {
                strRows = "<null_rows/>";
            }
            return strRows;
        }

        public static string strTransformSeudoXml(string strSeudoXml) {
            return CreaXmlDocConSeudoXml(strSeudoXml).OuterXml;
        }

        public static XmlDocument CreaXmlDocConSeudoXml(string strSeudoXml) {
            XmlDocument xmlfinal = null;

            string text = strSeudoXml;
            //first grab the first node. and create the root
            string pattern = @"(\[{1,1}.+\]{1,1})";
            MatchCollection Collection = Regex.Matches(text, pattern);
            string NewText = Collection[0].Value;
            string strNodoName = Regex.Replace(text, @"\[.+\]", string.Empty);

            xmlfinal = XmlHelper.CrearRaizXdoc(strNodoName);
            XmlNode PadreNodo = xmlfinal.DocumentElement; //apunto al primer nodo.

            try {
                string LastName = string.Empty;
                string innertext = string.Empty;
                //desde newtext empezar la busqueda.
                for (int x = 1; x < NewText.Length - 1; x++) {
                    string ch = NewText.Substring(x, 1);
                    switch (ch) {
                        case ",": //agregar un hijo.
                        innertext = string.Empty;
                        if (LastName.IndexOf("=") > 0) {
                            string[] vector = LastName.Split('=');
                            LastName = vector[0];
                            innertext = vector[1];
                        }
                        XmlHelper.NodeAgregar(xmlfinal, PadreNodo, LastName, innertext);
                        LastName = string.Empty;
                        break;
                        case "["://agregar un nodo
                        innertext = string.Empty;
                        if (LastName.IndexOf("=") > 0) {
                            string[] vector = LastName.Split('=');
                            LastName = vector[0];
                            innertext = vector[1];
                        }
                        XmlHelper.NodeAgregar(xmlfinal, PadreNodo, LastName, innertext);
                        LastName = string.Empty;
                        PadreNodo = PadreNodo.SelectSingleNode("*[last()]");

                        break;
                        case "]"://subir al padre
                        innertext = string.Empty;
                        if (LastName.IndexOf("=") > 0) {
                            string[] vector = LastName.Split('=');
                            LastName = vector[0];
                            innertext = vector[1];
                        }
                        XmlHelper.NodeAgregar(xmlfinal, PadreNodo, LastName, innertext);

                        PadreNodo = PadreNodo.ParentNode;
                        x++; //adelante el curso. para saltar una , o caracter.
                        LastName = string.Empty;
                        break;
                        default:
                        LastName += ch;
                        break;

                    }
                }
            }
            catch {
                XmlHelper.AgregaNodoConTexto(xmlfinal, "//*", "ErrorTransformar", "");
            }

            return xmlfinal;
        }


        /// <summary>
        /// FormateaMontos 
        /// </summary>
        /// <param name="XmlFinal"></param>
        /// <param name="XpathNodeCifra"></param>
        /// <returns></returns>
        public static XmlDocument FormatPesosCifras(XmlDocument XmlFinal, string XpathNodeCifra) {
            XmlNodeList NodoListas = XmlFinal.SelectNodes(XpathNodeCifra);
            foreach (XmlNode nodeItem in NodoListas) {
                nodeItem.InnerText = strHelper.FormatCifras(nodeItem.InnerText);

            }
            return XmlFinal;
        }

        /// <summary>
        /// Selecciona Nodos unicos 
        /// </summary>
        /// <param name="XmlFinal"></param>
        /// <param name="campo unico"></param>
        /// <returns></returns>
        public static string Distinct(XmlDocument XmlFinal, string Campo) {
            ArrayList NodoListas = new ArrayList();
            string r = "";
            foreach (XmlNode nodeItem in XmlFinal.SelectNodes("//row/@" + Campo)) {
                string dato = nodeItem.InnerText;
                if (!NodoListas.Contains(dato)) {
                    NodoListas.Add(dato);
                    r = r + "<row " + Campo + "='" + nodeItem.InnerText + "' />";
                }
            }
            return r;
        }

        /// <summary>
        /// Selecciona Nodos unicos con algun filtro tipo xpath
        /// </summary>
        /// <param name="XmlFinal"></param>
        /// <param name="campo unico"></param>
        /// <returns></returns>
        public static string Distinct( XmlDocument XmlFinal, string Campo, string Filtro ) {
            ArrayList NodoListas = new ArrayList( );
            string r = "";
            foreach( XmlNode nodeItem in XmlFinal.SelectNodes( "//row[" + Filtro + "]/@" + Campo ) ) {
                string dato = nodeItem.InnerText;
                if( !NodoListas.Contains( dato ) ) {
                    NodoListas.Add( dato );
                    r = r + "<row " + Campo + "='" + nodeItem.InnerText + "' />";
                }
            }
            return r;
        }

        ///// <summary>   
        ///// ********* NO FUNCIONA *************
        ///// Selecciona Nodos unicos con algun filtroy con todos los campos
        ///// </summary>
        ///// <param name="XmlFinal"></param>
        ///// <param name="campo unico"></param>
        ///// <returns></returns>
        //public static string Distinct(XmlDocument XmlFinal, string Campo, string filtro) {
        //    ArrayList NodoListas = new ArrayList();
        //    string r = "";
        //    foreach (XmlNode nodeItem in XmlFinal.SelectNodes("//row[" + filtro + "]/@" + Campo)) {
        //        string dato = nodeItem.InnerText;
        //        if (!NodoListas.Contains(dato)) {
        //            NodoListas.Add(dato);

        //            string fila = "<row ";
        //            XmlNode RowNode = nodeItem.ParentNode;
        //            foreach (XmlAttribute att in RowNode.Attributes) {
        //                fila += att.Name + "='" + att.Value + "' ";
        //            }
        //            r = r + fila + " />";

        //        }//fin_if
        //    }//fin_foreach
        //    return r;

        //}//fin_Distinct
    }

    //our custom class
    // usar con xmlns:d="urn:Directic" en la plantilla xslt
    public class NSDirectic {
        //function that gets called from XSLT
        public double Round(string numero, string decimales) {
            double num = 0;
            int dec = 0;

            if (numero != "") {
                num = System.Convert.ToDouble(numero, new System.Globalization.CultureInfo("en-US"));
            }
            if (decimales != "") {
                dec = System.Convert.ToInt32(decimales, new System.Globalization.CultureInfo("en-US"));
            }
            return Math.Round(num, dec);
        }

        public double Trunc( string numero ) {
            double num = 0;

            if( numero != "" ) {
                num = System.Convert.ToDouble( numero, new System.Globalization.CultureInfo( "en-US" ) );
            }

            return Math.Truncate( num );
        }

        public string DateFormat(string fecha) {
            string r = " ";

            if (fecha.Trim() != "")
                r = fecha.Substring(6, 2) + "/" + fecha.Substring(4, 2) + "/" + fecha.Substring(0, 4);

            return r;
        }

        public string TimeFormat(string Tiempo) {
            string r = " ";

            if (Tiempo.Trim() != "")
                r = Tiempo.Substring(0, 2) + ":" + Tiempo.Substring(2, 2) + ":" + Tiempo.Substring(4, 2);

            return r;
        }
        
        public double Log10( string numero ) {
            return Log( numero, "10" );
        }

        public double Log( string numero ,string _Base ) {
            double num = 0;
            int _Bas = 0;

            if( numero != "" ) {
                num = System.Convert.ToDouble( numero, new System.Globalization.CultureInfo( "en-US" ) );
            }
            if( _Base != "" ) {
                _Bas = System.Convert.ToInt32( _Base, new System.Globalization.CultureInfo( "en-US" ) );
            }
            return Math.Log( num, _Bas );
        }

        public bool Contiene( string Lista, string ABuscar ) {

            foreach( string item in Lista.Split(',') ) {
                if( item == ABuscar )
                    return true;
            }
            return false;
        }
        
		public string Replace( string srtInput, string oldValue, string newValue ) {
            return srtInput.Replace(oldValue, newValue);
        }

    }//fin class NS
}
