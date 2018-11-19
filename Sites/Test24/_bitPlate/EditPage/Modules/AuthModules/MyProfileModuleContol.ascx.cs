using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HJORM;
using BitPlate.Domain;
using BitPlate.Domain.Modules;
using BitSite._bitPlate.bitAjaxServices;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Utils;

namespace BitSite._bitPlate.EditPage.Modules.AuthModules
{
    public partial class MyProfileModuleControl : BaseModuleUserControl
    {
        protected TextBox NameTextbox;
        protected TextBox ForeNameTextbox;
        protected TextBox NamePrefixTextbox;
        protected TextBox EmailTextbox;

        protected TextBox CurrentPasswordTextbox;
        protected TextBox NewPasswordTextbox;
        protected TextBox RetypePasswordTextbox;

        protected TextBox CompanyTextbox;

        protected RadioButton SexeMaleRadio;
        protected RadioButton SexeFemaleRadio;
        protected RadioButton SexeUnknownRadio;

        protected TextBox AddressTextbox;
        protected TextBox PostcodeTextbox;
        protected TextBox CityTextbox;
        protected TextBox BirthDateTextbox;
        protected TextBox TelephoneTextbox;
        protected TextBox CountryTextbox;

        protected Button SubmitButton;
        protected LinkButton LinkSubmitButton;
        protected Label MessageLabel;

        protected BaseUser User;

        protected void Page_Load(object sender, EventArgs e)
        {
            base.Load(sender, e);
            if (base.CheckAutorisation())
            {
                this.FindControls();
                this.SetUserVariable();
                if (!IsPostBack)
                {
                    this.FillControlsWithData();
                }

                if (SubmitButton != null)
                {
                    SubmitButton.Click += SubmitButton_Click;
                }

                if (LinkSubmitButton != null)
                {
                    LinkSubmitButton.Click += SubmitButton_Click;
                }
            }
        }

        private void FindControls()
        {
            NameTextbox = (TextBox)this.FindControl("NameTextbox" + this.ModuleID.ToString("N"));
            ForeNameTextbox = (TextBox)this.FindControl("ForeNameTextbox" + this.ModuleID.ToString("N"));
            NamePrefixTextbox = (TextBox)this.FindControl("NamePrefixTextbox" + this.ModuleID.ToString("N"));
            EmailTextbox = (TextBox)this.FindControl("EmailTextbox" + this.ModuleID.ToString("N"));

            CurrentPasswordTextbox = (TextBox)this.FindControl("CurrentPasswordTextbox" + this.ModuleID.ToString("N"));
            NewPasswordTextbox = (TextBox)this.FindControl("NewPasswordTextbox" + this.ModuleID.ToString("N"));
            RetypePasswordTextbox = (TextBox)this.FindControl("RetypePasswordTextbox" + this.ModuleID.ToString("N"));

            CompanyTextbox = (TextBox)this.FindControl("CompanyTextbox" + this.ModuleID.ToString("N"));

            SexeMaleRadio = (RadioButton)this.FindControl("SexeMaleRadio" + this.ModuleID.ToString("N"));
            SexeFemaleRadio = (RadioButton)this.FindControl("SexeFemaleRadio" + this.ModuleID.ToString("N"));
            SexeUnknownRadio = (RadioButton)this.FindControl("SexeUnknownRadio" + this.ModuleID.ToString("N"));

            AddressTextbox = (TextBox)this.FindControl("AddressTextbox" + this.ModuleID.ToString("N"));
            PostcodeTextbox = (TextBox)this.FindControl("PostcodeTextbox" + this.ModuleID.ToString("N"));
            CityTextbox = (TextBox)this.FindControl("CityTextbox" + this.ModuleID.ToString("N"));
            BirthDateTextbox = (TextBox)this.FindControl("BirthDateTextbox" + this.ModuleID.ToString("N"));
            TelephoneTextbox = (TextBox)this.FindControl("TelephoneTextbox" + this.ModuleID.ToString("N"));
            CountryTextbox = (TextBox)this.FindControl("CountryTextbox" + this.ModuleID.ToString("N"));

            //TextBox Captcha = (TextBox)this.FindControl();

            SubmitButton = (Button)this.FindControl("SubmitButton" + this.ModuleID.ToString("N"));
            LinkSubmitButton = (LinkButton)this.FindControl("SubmitLinkButton" + this.ModuleID.ToString("N"));

            MessageLabel = (Label)this.FindControl("MessageLabel" + this.ModuleID.ToString("N"));
        }

        private void SetUserVariable()
        {
            if (SessionObject.CurrentBitplateUser != null)
            {
                this.User = SessionObject.CurrentBitplateUser;
            }
            else
            {
                this.User = SessionObject.CurrentBitSiteUser;
            }
        }

        private void FillControlsWithData()
        {
            if (this.User == null)
            {
                this.User = new BaseUser();
                if (MessageLabel != null) MessageLabel.Text = "Kan geen profiel gegevens laden!";
                if (SubmitButton != null) SubmitButton.Enabled = false;
                if (LinkSubmitButton != null) LinkSubmitButton.Enabled = false;
            }
            else
            {
                if (MessageLabel != null) MessageLabel.Text = "";
                if (SubmitButton != null) SubmitButton.Enabled = true;
                if (LinkSubmitButton != null) LinkSubmitButton.Enabled = true;
            }

            if (NameTextbox != null) NameTextbox.Text = this.User.Name;
            if (ForeNameTextbox != null) ForeNameTextbox.Text = this.User.ForeName;
            if (NamePrefixTextbox != null) NamePrefixTextbox.Text = this.User.NamePrefix;
            if (EmailTextbox != null) EmailTextbox.Text = this.User.Email;

            /* if (CurrentPasswordTextbox != null) CurrentPasswordTextbox.Text = "";
            if (NewPasswordTextbox != null) NewPasswordTextbox.Text = "";
            if (RetypePasswordTextbox != null) RetypePasswordTextbox.Text = ""; */

            //if (CompanyTextbox != null) CompanyTextbox.Text = "" //this.User.;

            switch (this.User.Gender)
            {
                case BaseUser.SexeEnum.Female:
                    if (SexeFemaleRadio != null) SexeFemaleRadio.Checked = true;
                    break;

                case BaseUser.SexeEnum.Male:
                    if (SexeMaleRadio != null) SexeMaleRadio.Checked = true;
                    break;

                default:
                    if (SexeUnknownRadio != null) SexeUnknownRadio.Checked = true;
                    break;
            }

            if (AddressTextbox != null) AddressTextbox.Text = this.User.Address;
            if (PostcodeTextbox != null) PostcodeTextbox.Text = this.User.Postalcode;
            if (CityTextbox != null) CityTextbox.Text = this.User.City;
            
            if (BirthDateTextbox != null && this.User.BirthDate != null) BirthDateTextbox.Text = this.User.BirthDate.ToString();

            //if (TelephoneTextbox != null) TelephoneTextbox.Text = this.User.;
            if (CountryTextbox != null) CountryTextbox.Text = this.User.Country;
        }

        public void SubmitButton_Click(object sender, EventArgs e)
        {
            //this.SetUserVariable();
            if (MessageLabel != null) MessageLabel.Text = "";
            if (this.User != null)
            {
                bool inputAccepted = true;
                this.User.Name = (NameTextbox != null) ? NameTextbox.Text : this.User.Name;
                this.User.ForeName = (ForeNameTextbox != null) ? ForeNameTextbox.Text : this.User.ForeName;
                this.User.NamePrefix = (NamePrefixTextbox != null) ? NamePrefixTextbox.Text : this.User.NamePrefix;
                this.User.Email = (EmailTextbox != null) ? EmailTextbox.Text : this.User.Email;
                if (CurrentPasswordTextbox != null && NewPasswordTextbox != null && RetypePasswordTextbox != null)
                {
                    if (NewPasswordTextbox.Text.Trim().Length != 0)
                    {
                        if (NewPasswordTextbox.Text.Trim().Length >= 6)
                        {
                            if (NewPasswordTextbox.Text == RetypePasswordTextbox.Text)
                            {
                                string md5CurrentPassword = Encrypter.CalculateMD5Hash(CurrentPasswordTextbox.Text);
                                if (md5CurrentPassword == this.User.Password)
                                {
                                    this.User.Password = Encrypter.CalculateMD5Hash(NewPasswordTextbox.Text);
                                }
                                else
                                {
                                    inputAccepted = false;
                                    if (MessageLabel != null) MessageLabel.Text += "Uw huidige wachtwoord is onjuist. ";
                                }
                            }
                            else
                            {
                                inputAccepted = false;
                                if (MessageLabel != null) MessageLabel.Text += "Uw nieuwe wachtwoord is niet gelijk aan het wachtwoord verificatie veld. ";
                            }
                        }
                        else
                        {
                            inputAccepted = false;
                            if (MessageLabel != null) MessageLabel.Text += "Uw nieuwe wachtwoord moet minimaal uit 6 aaneengesloten tekens bestaan. ";
                        }
                    }
                }
                else if (CurrentPasswordTextbox != null)
                {
                    if (Encrypter.CalculateMD5Hash(CurrentPasswordTextbox.Text) != this.User.Password)
                    {
                        inputAccepted = false;
                        if (MessageLabel != null) MessageLabel.Text += "Uw huidige wachtwoord is onjuist. ";
                    }
                }

                if (SexeFemaleRadio != null && SexeFemaleRadio.Checked) this.User.Gender = BaseUser.SexeEnum.Female;
                if (SexeMaleRadio != null && SexeMaleRadio.Checked) this.User.Gender = BaseUser.SexeEnum.Male;
                if (SexeUnknownRadio != null && SexeUnknownRadio.Checked) this.User.Gender = BaseUser.SexeEnum.Undefined;

                this.User.Address = (AddressTextbox != null) ? AddressTextbox.Text : this.User.Address;
                this.User.Postalcode = (PostcodeTextbox != null) ? PostcodeTextbox.Text : this.User.Postalcode;
                this.User.City = (CityTextbox != null) ? CityTextbox.Text : this.User.City;
                if (BirthDateTextbox != null) {
                    DateTime birthDate;
                    DateTime.TryParse(BirthDateTextbox.Text, out birthDate);
                    this.User.BirthDate = birthDate;
                }
                this.User.Country = (CountryTextbox != null) ? CountryTextbox.Text : this.User.Country;

                if (inputAccepted)
                {
                    this.User.Save();
                    if (MessageLabel != null) MessageLabel.Text += "Uw gegevens zijn opgeslagen.";
                }
                else
                {
                    if (MessageLabel != null) MessageLabel.Text += "Uw gegevens zijn niet opgeslagen.";
                }
            }
            else
            {
                if (MessageLabel != null) MessageLabel.Text = "Kan geen profiel gegevens laden!";
            }
        }

        public override void Reload(BaseModuleUserControl sender, NavigationParameterObject args = null)
        {
            if (base.CheckAutorisation())
            {
                this.FindControls();
                if (MessageLabel != null) MessageLabel.Text = "";
                this.SetUserVariable();
                this.FillControlsWithData();
            }
        }
    }
}