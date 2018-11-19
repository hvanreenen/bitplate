using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM.Attributes;

namespace BitPlate.Domain.Modules
{
    public class InputFormModule
    {
        /* public InputFormModule()
            : base()
        {
            this.Type = "InputFormModule";
            this.Name = "Formulier";
            this.UseInNewsletter = false;
            this.Tags_old.AddRange(new string[] { "{textBoxName_EmailFrom}", "{textBoxName_EmailSubject}", "{textboxName}", "{textboxForeName}", "{textboxNamePrefix}", "{textboxEmail}", "{textboxCompany}", "{radioSexeMale}", "{radioSexeFemale}", "{radioSexeUnknown}", 
                    "{textboxAddress}", "{textboxPostalCode}", "{textboxPlace}", "{textboxCountry}", "{textboxBirthDate}", "{textboxTelephone}", 
                    "{opt-inCheckbox}", "{captcha}", "{submitbutton}{/submitbutton}" });
            
            this.ContentSamples.Add(@"<div>Bevestig formulier
    <table>
        
        <tr>
            <td>
                Tekst
            </td>
            <td>
                <input type='text' name='Naam' class='required'/>
            </td>
        </tr>
        <tr>
            <td>
                Kies kleur
            </td>
            <td>
                <select name='Kleur'><option value='rood'>Rood</option><option value='groen'>Groen</option><option value='geel'>Geel</option></select>
            </td>
        </tr>
        <tr>
            <td>
                Aan/uit
            </td>
            <td>
                <input name='Aan/uit' type='checkbox' value='aan'/>
            </td>
        </tr>
        <tr>
            <td>
                Bestand
            </td>
            <td>
                <input name='Bestand' type='file' />
            </td>
        </tr>
        <tr>
            <td>
                Keuzerondjes
            </td>
            <td>
                <input  name='Keuzerondjes' type='radio' value='1' checked='checked'/> 1
                <input  name='Keuzerondjes' type='radio' value='2'/> 2
                <input  name='Keuzerondjes' type='radio' value='3'/> 3
            </td>
        </tr></table>{submitbutton}test{/submitbutton}</div><div style=""display: none;""><div id=""bitFormAcceptResponse{ID}""></div><div id=""bitFormErrorResponse{ID}""></div></div>");
        }

        public override string ToString(ModeEnum mode)
        {
            string moduleContentHTML = "<form id=\"bitForm{ID}\">" + this.Content + "</form>";

            moduleContentHTML = moduleContentHTML.Replace("{submitbutton}", String.Format("<input type=\"hidden\" name=\"bitFormId\" value=\"" + this.ID + "\" /><button type=\"button\" id=\"buttonSearch\" onclick=\"BITINPUTFORMMODULE.submitForm('{0}');\">", this.ID));
            moduleContentHTML = moduleContentHTML.Replace("{/submitbutton}", "</button>");
            moduleContentHTML = moduleContentHTML.Replace("{ID}", this.ID.ToString());
            
            moduleContentHTML = moduleContentHTML.Replace("{captcha}", "<div class='QapTcha'></div><script type=\"text/javascript\">$(document).ready(function() { $('.QapTcha').QapTcha({disabledSubmit:false,autoRevert:true,autoSubmit:false}); });</script>");
            moduleContentHTML = moduleContentHTML.Replace("{textboxForeName}", "<input type='text' id='bitTextboxForeName' name='bitTextboxForeName' data-validation=\"required\" class='required'/>");
            moduleContentHTML = moduleContentHTML.Replace("{textboxNamePrefix}", "<input type='text' id='bitTextboxNamePrefix' name='bitTextboxNamePrefix' />");

            moduleContentHTML = moduleContentHTML.Replace("{textboxFirstName}", "<input type='text' id='bitTextboxFirstName' name='bitTextboxFirstName' data-validation=\"required\" />");
            moduleContentHTML = moduleContentHTML.Replace("{textboxName}", "<input type='text' id='bitTextboxName'  name='bitTextboxName' class='required' data-validation=\"required\"/>");
            moduleContentHTML = moduleContentHTML.Replace("{textboxEmail}", "<input type='text' id='bitTextboxEmail' name='bitTextboxEmail' class='required email' data-validation=\"email\" />");
            moduleContentHTML = moduleContentHTML.Replace("{textboxCompany}", "<input type='text' id='bitTextboxCompany' name='bitTextboxCompany' />");
            moduleContentHTML = moduleContentHTML.Replace("{radioSexeMale}", "<input type='radio' name='bitRadioGroupSexe' value='1' />");
            moduleContentHTML = moduleContentHTML.Replace("{radioSexeFemale}", "<input type='radio' name='bitRadioGroupSexe' value='2' />");
            moduleContentHTML = moduleContentHTML.Replace("{radioSexeUnknown}", "<input type='radio' name='bitRadioGroupSexe' value='0' checked='checked'/>");
            moduleContentHTML = moduleContentHTML.Replace("{textboxAddress}", "<input type='text' id='bitTextboxAddress' name='bitTextboxAddress'/>");
            moduleContentHTML = moduleContentHTML.Replace("{textboxPostalCode}", "<input type='text' id='bitTextboxPostalCode' name='bitTextboxPostalCode'/>");
            moduleContentHTML = moduleContentHTML.Replace("{textboxPlace}", "<input type='text' id='bitTextboxPlace' name='bitTextboxPlace'/>");
            moduleContentHTML = moduleContentHTML.Replace("{textboxCountry}", "<input type='text' id='bitTextboxCountry' name='bitTextboxCountry'/>");
            moduleContentHTML = moduleContentHTML.Replace("{textboxBirthDate}", "<input type='text' id='bitTextboxBirthDate' name='bitTextboxBirthDate'/>");
            moduleContentHTML = moduleContentHTML.Replace("{textboxTelephone}", "<input type='text' id='bitTextboxTelephone' name='bitTextboxTelephone'/>");

            moduleContentHTML = moduleContentHTML.Replace("{opt-inCheckbox}", "<input type='checkbox' id='bitCheckboxOptIn'  name='bitCheckboxOptIn' checked='checked'/>");

            return moduleContentHTML;
        }

        private string _emailFrom = "";
        public string EmailFrom
        {
            get
            {
                if (_emailFrom == "")
                {
                    _emailFrom = "";
                }
                return _emailFrom;
            }
            set
            {
                _emailFrom = value;
            }
        }

        private string _emailTo = "";
        public string EmailTo
        {
            get
            {
                if (_emailTo == "")
                {
                    _emailTo = ""; //Site.FromEmail;
                }
                return _emailTo;
            }
            set
            {
                _emailTo = value;
            }
        }

        public string EmailSubject { get; set; }
        public string EmailStart { get; set; }
        public bool SaveInDatabase { get; set; }
        public bool SendEmail { get; set; }
        private string _extraJavascript = "";
        public string ExtraJavascript
        {
            get
            {
                if (_extraJavascript == "")
                {
                    _extraJavascript = @"function validateForm(){
    //hier validatie
    return true;
}";
                }
                return _extraJavascript;
            }
            set
            {
                _extraJavascript = value;
            }
        } */
    }
}
