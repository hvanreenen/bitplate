using System;
using System.ComponentModel;

namespace BitSite._bitPlate.RewriteForm
{
    public delegate void SubstituteValueEventHandler(object sender, SubstituteValueEventArgs e);

    public class SubstituteValueEventArgs : CancelEventArgs
    {
        public SubstituteValueEventArgs(string name, string oldvalue)
            : base()
        {
            this._name = name;
            this._oldvalue = oldvalue;
            this._newvalue = oldvalue;
        }

        private string _name;
        private string _newvalue;
        private string _oldvalue;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string OldValue
        {
            get { return _oldvalue; }
            set { _oldvalue = value; }
        }

        public string NewValue
        {
            get { return _newvalue; }
            set { _newvalue = value; }
        }
    }
}
