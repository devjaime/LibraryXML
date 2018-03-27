using System;

namespace library.Error {
    /// <summary>
    /// Descripción breve de MyException.
    /// </summary>
    public class MyException : Exception {
        private string descerror="";
        public MyException() {
            //
            // TODO: agregar aquí la lógica del constructor
            //
        }

        public MyException(string strError) {
            descerror=strError;
        }

        public string getError() {
            return this.descerror;
        }


    }
}