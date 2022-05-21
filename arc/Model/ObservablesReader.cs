using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Advise_IHM.Model
{
    /// <summary>
    /// AUTEUR - Karl TROUILLET
    /// DESCRIPTION -     *********************************************************
    /// Reader to get all failure code from a file
    /// HISTORIQUE -      *********************************************************
    /// * V0 - 01/06/2021 -      ==================================================
    /// => Creation du document
    /// </summary>
    class ObservablesReader : IDisposable
    {
        // FIELDS
        string _fileName;

        // CONSTRUCTOR
        public ObservablesReader(string fileName, ref Dictionary<string, CodeObservable> dico)
        {
            this._fileName = fileName;
            dico = new Dictionary<string, CodeObservable>();
            this.extractLine(ref dico);
        }

        // METHOD
        /// <summary>
        /// READ FILE which contains all observable available and format line to observable
        /// </summary>
        /// <param name="dico">Dictionnary empty at start</param>
        /// <returns>Fill dictionnary Dico from parameter input</returns>
        public string extractLine(ref Dictionary<string, CodeObservable> dico)
        {
            string norm = "";
            CodeObservable codeObservable;
            const Int32 BufferSize = 1024;
            using (var fileStream = File.OpenRead(this._fileName))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF7, true, BufferSize))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (line.ToUpper().StartsWith("NORM"))
                    {
                        norm = line;
                        continue;
                    }  
                    if (line.StartsWith("//") || string.IsNullOrWhiteSpace(line) || line.StartsWith("["))
                        continue;
                    codeObservable = new CodeObservable(line);
                    if (codeObservable.CodeName.Length < 3)
                        continue;
                    dico.Add(codeObservable.CodeName, codeObservable);
                }
            }
            return norm;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            // this.Dispose();
        }
    }
}
