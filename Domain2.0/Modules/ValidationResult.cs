using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Modules
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                if (_message != null && _message != "")
                {
                    IsValid = false;
                }
            }
        }
        public ValidationResult()
            : this(true)
        {
        }
        public ValidationResult(bool isValid)
        {
            IsValid = isValid;
            Message = "";
        }
        public ValidationResult(string message)
        {
            Message = message;
            IsValid = false;
        }

        public override string ToString()
        {
            if (IsValid) return "true";
            else return "false, " + Message;
        }
    }

}
