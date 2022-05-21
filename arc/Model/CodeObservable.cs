using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Advise_IHM.Model
{
    /// <summary>
    /// Associated to field
    /// None: unvisible, Optionnal (can be filled), Mandatory (must be filled)
    /// </summary>
    public enum FieldStatus { None, Optionnal, Mandatory };
    /// <summary>
    /// AUTEUR - Karl TROUILLET
    /// DESCRIPTION -     *********************************************************
    /// Represent code observable (failure code)
    /// Used for code in code observable file and in edition view 
    /// HISTORIQUE -      *********************************************************
    /// * V0 - 01/06/2021 -      ==================================================
    /// => Creation du document
    /// </summary>
    public class CodeObservable : INotifyPropertyChanged
    {
        // FIELDS
        public string CodeName { get; set; }                // trigram + cara1 + cara2
        public string Trigram { get; set; }                 // failure code filter with the first 3 letters
        public string Description { get; set; }             // text to describe failure
        public string Tag { get; set; }                     // define in code observable file (it is description secondary) [NOT USED]
        public string AnalogCode { get; set; }              // define in code observable file. when another code is similar [NOT USED]
        public string LabelQuantification1 { get; set; }    // this is label display for cara1 (ex: Angle)
        public string UnitQuantification1 { get; set; }     // this is unit display for cara1 (ex: °)
        public string LabelQuantification2 { get; set; }    // this is label display for cara2 (ex: Angle)
        public string UnitQuantification2 { get; set; }     // this is unit display for cara2 (ex: °)
        public string RawCodeDescription { get; set; }      // raw code get from code observable file (format: A;B;...)
        //public bool isCaracterisation1 { get; set; }        // allow to display or not fields in view link to cara1 (if can/must be filled)
        //public bool isCaracterisation2 { get; set; }        // allow to display or not fields in view link to cara2 (if can/must be filled)

        public FieldStatus Caracterisation1Status { get; set; } // allow to display or not fields in view link to cara1 (if can/must be filled)
        public FieldStatus Caracterisation2Status { get; set; } // allow to display or not fields in view link to cara2 (if can/must be filled)
        public FieldStatus Quantification1Status { get; set; }  // allow to display or not fields in view link to quanti1 (if can/must be filled)
        public FieldStatus Quantification2Status { get; set; }  // allow to display or not fields in view link to quanti2 (if can/must be filled)
        public FieldStatus Horodate1Status { get; set; }        // allow to display or not fields in view link to horo1 (if can/must be filled)
        public FieldStatus Horodate2Status { get; set; }        // allow to display or not fields in view link to horo2 (if can/must be filled)

        // CONSTRUCTOR
        public CodeObservable(string observableLine)
        {
            RawCodeDescription = observableLine;
            this.format(observableLine);
        }
        public CodeObservable(string codeName, string description)
        {
            this.CodeName = codeName;
            this.Description = description;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // METHOD

        /// <summary>
        /// Convert text line with format "A;B; ... ;T[...]...
        /// To CodeObservable object used in edition view
        /// </summary>
        /// <param name="observableLine">text line to convert</param>
        /// <returns>null</returns>
        public bool? format(string observableLine)
        {
            // gestion du code
            string[] splitLine = observableLine.Split(';');
            for (int i = 0; i < 5; i++)
            {
                if (splitLine[i] == "@") continue;
                else this.CodeName += splitLine[i];
            }

            if (this.CodeName.Length >= 3) this.Trigram = this.CodeName.Substring(0, 3);
            //isCaracterisation1 = this.CodeName.Length >= 4 && this.CodeName[3] != ' ' ? true : false;
            //isCaracterisation2 = this.CodeName.Length > 4 && this.CodeName[4] != ' ' ? true : false;

            // gestion de la description
            this.Description = splitLine[5];

            if (splitLine.Length < 6)
                return null;

            // gestion des caractérisation/quantification/autre
            for (int i = 6; i < splitLine.Length; i++)
            {
                if (splitLine[i].StartsWith("Q1"))
                {
                    string[] tmp = splitLine[i].Replace(";", ",").Replace("Q1[", "").TrimEnd(']').Split(',');
                    LabelQuantification1 = tmp[1];
                    UnitQuantification1 = tmp[0];
                }
                else if (splitLine[i].StartsWith("Q2"))
                {
                    string[] tmp = splitLine[i].Replace(";", ",").Replace("Q2[", "").TrimEnd(']').Split(',');
                    LabelQuantification2 = tmp[1];
                    UnitQuantification2 = tmp[0];
                }
                else if (splitLine[i].StartsWith("T"))
                {
                    Tag = splitLine[i].Replace("T[", "").TrimEnd(']');
                }
                else if (splitLine[i].StartsWith("A"))
                {
                    AnalogCode = splitLine[i].Replace("A[", "").TrimEnd(']');
                }
                if (splitLine[i].StartsWith("AD"))
                {
                    List<String> tmp = new List<string>();
                    foreach (var item in splitLine[i].Replace("AD[", "").TrimEnd(']').Split(','))
                    {
                        tmp.Add(item);
                    }
                    Caracterisation1Status = tmp.Contains("OC1") ? FieldStatus.Optionnal : tmp.Contains("MC1") ? FieldStatus.Mandatory : FieldStatus.None;
                    Caracterisation2Status = tmp.Contains("OC2") ? FieldStatus.Optionnal : tmp.Contains("MC2") ? FieldStatus.Mandatory : FieldStatus.None;
                    Quantification1Status = tmp.Contains("OQ1") ? FieldStatus.Optionnal : tmp.Contains("MQ1") ? FieldStatus.Mandatory : FieldStatus.None;
                    Quantification2Status = tmp.Contains("OQ2") ? FieldStatus.Optionnal : tmp.Contains("MQ2") ? FieldStatus.Mandatory : FieldStatus.None;
                    Horodate1Status = tmp.Contains("OH1") ? FieldStatus.Optionnal : tmp.Contains("MH1") ? FieldStatus.Mandatory : FieldStatus.None;
                    Horodate2Status = tmp.Contains("OH2") ? FieldStatus.Optionnal : tmp.Contains("MH2") ? FieldStatus.Mandatory : FieldStatus.None;
                }
            }
            return null;
        }

        public override string ToString()
        {
            return CodeName.Substring(CodeName.Length - 1) + " - " + Description;
        }
    }
}
