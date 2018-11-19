using BitPlate.Domain.Utils;
using HJORM;
using HJORM.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace BitPlate.Domain.Newsletters
{
    public class NewsletterImportDefinition : BaseDomainSiteObject
    {
        public string FileName { get; set; }
        public string FileExtension { get; set; }

        [NonPersistent]
        public bool SaveDefinition { get; set; }

        [NonPersistent]
        public string ImportLogFile { get; set; }

        [NonPersistent]
        public bool DeleteGroupSubscribers { get; set; }//De gebruikers verwijderen die in de gekozen groepen zitten. Ook de verplichte.

        private BaseCollection<NewsletterGroup> _groups;
        [Persistent("newsletterimportdefinitiongroups")]
        [Association("FK_Definition", "FK_Group")]
        public BaseCollection<NewsletterGroup> Groups
        {
            get
            {
                if (this._groups == null || (_groups != null && !_groups.IsLoaded))
                {
                    string where = "EXISTS (SELECT * FROM newsletterimportdefinitiongroups WHERE FK_Definition = '" + this.ID + "' AND newslettergroup.ID = newsletterimportdefinitiongroups.FK_Group)";
                    this._groups = BaseCollection<NewsletterGroup>.Get(where);
                    this._groups.IsLoaded = true;
                }
                return this._groups;
            }
            set
            {
                this._groups = value;
                this._groups.IsLoaded = true;
            }
        }

        //The ColumNames
        public int NameColumnNo { get; set; }
        public int NamePrefixColumnNo { get; set; }
        public int ForeNameColumnNo { get; set; }
        public int EmailColumnNo { get; set; }
        public int GenderColumnNo { get; set; }

        public string NameColumn { get; set; }
        public string NamePrefixColumn { get; set; }
        public string ForeNameColumn { get; set; }
        public string EmailColumn { get; set; }
        public string GenderColumn { get; set; }

        public string Delimiter { get; set; }//scheidingsteken
        public bool FirstRowIsColumnName { get; set; }//eerste kolom zijn de veld namen
        public bool AutoConfirm { get; set; }
        public bool SkipDoubleRecords { get; set; }//Checkt op dubbele records alleen op email adres. AAN: Het record over slaan. UIT: Record overschrijven, maar niet het email adres.
        public bool EmptyGroups { get; set; }//De koppeling verwijderen tussen de groep en de abonnee. De abonnee blijft nog wel bestaan in de verplichte en andere groepen.
        public bool AppendGroups { get; set; }

        public override void Delete()
        {
            base.Delete();
        }

        public string StartImport(string[] rows)
        {
            string log = string.Empty;

            foreach (string row in rows)
            {
                if (this.FileExtension == ".csv")
                {
                    log += importCsvRow(row);
                }
            }
            return log;
        }

        private string importCsvRow(string row)
        {
            string log = string.Empty;
            string[] fields = row.Split(new char[] { Delimiter[0] });

            string email = fields[EmailColumnNo].Trim();
            string achternaam = fields[NameColumnNo].Trim();
            string voornaam = fields[ForeNameColumnNo].Trim();
            string tussenvoegsel = fields[NamePrefixColumnNo].Trim();

            if (!EmailManager.isValidEmailAddress(email))
            {
                log = "Geen geldig email adres.";
            }
            else
            {
                NewsletterSubscriber subscriber = new NewsletterSubscriber();

                subscriber.Email = email;
                subscriber.Name = achternaam;
                subscriber.NamePrefix = tussenvoegsel;
                subscriber.ForeName = voornaam;
                subscriber.SubscribedGroups = this.Groups;

                subscriber.Save();
            }
            return log;
        }

        //importmanager maken in het domein < denkt na en doet alle moeilijke functies zoals importeren, kijken of een mail adres valid is, enz
        //De service krijgt dan alleen result terug van hoe het is gegaan met importeren(gelukt of niet, fout-log) en true of false of een email valid is.
        //nsltservice krijgt d code en laat de logica over aan een importmanager aan

        public List<string> Validate()
        {
            List<string> log = new List<string>();

            if (this.FileExtension.ToLower() != ".csv")
            {
                log.Add("Alleen .csv bestanden mogelijk");
            }
            if (this.Delimiter == "")
            {
                log.Add("Veld scheidingsteken verplicht.");
            }
            if (this.EmailColumn == "")
            {
                log.Add("EmailColumn verplicht.");
            }
            if (this.SaveDefinition && this.Name == "")
            {
                log.Add("Bij opslaan van definitie is naam verplicht.");
            }
            return log;

        }
        /// <summary>
        /// voeg verplichte groepen toe, zodat die er altijd in staan
        /// </summary>
        public void AddMandatoryGroups()
        {
            BaseCollection<NewsletterGroup> mandatoryGroups = BaseCollection<NewsletterGroup>.Get("FK_Site = '" + WebSessionHelper.CurrentSite.ID.ToString() + "' AND IsMandatoryGroup = true");

            foreach (NewsletterGroup mandatoryGroup in mandatoryGroups)
            {
                bool allreadyAdded = false;
                foreach (NewsletterGroup group in this.Groups)
                {
                    if (mandatoryGroup.Equals(group))
                    {
                        allreadyAdded = true;
                        break;
                    }
                }
                if (!allreadyAdded)
                {
                    this.Groups.Add(mandatoryGroup);
                }
            }
        }

        public List<string> StartImport()
        {
            AddMandatoryGroups();
            List<string> log = new List<string>();
            StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~/_temp/upload/csv/") + this.FileName);

            //alle rijen van het document.
            string RecordLine = sr.ReadLine();
            ArrayList Lines = new ArrayList();
            while (RecordLine != null)
            {
                Lines.Add(RecordLine);
                RecordLine = sr.ReadLine();
            }
            int startRow = 0;
            if (this.FirstRowIsColumnName == true)
            {
                startRow = 1;
            }
            for (int i = startRow; i < Lines.Count; i++)
            {
                string[] dataLine = Regex.Split(Lines[i].ToString(), this.Delimiter);

                int[] importProgress = { i, Lines.Count - 1 };
                string logMsg = importSubscriber(dataLine, importProgress);
                log.Add(logMsg);

            }
            sr.Close();
            

            //else
            //{//EERSTE RIJ IS GEEN KOLOMNAAM
            //    for (int i = 0; i < Lines.Count; i++)
            //    {
            //        string[] dataLine = Regex.Split(Lines[i].ToString(), this.Delimiter);

            //        if (i >= 0)
            //        {
            //            int[] importProgress = { i, Lines.Count };
            //            string[] log = CreateSubscriber(dataLine, this, importProgress);
            //            log.Add(log);
            //            //return JsonResult.CreateResult(true);
            //        }
            //    }
            //    sr.Close();
            //    string ImportLogFile = MakeImportLogFile(log, this);
            //    // this.ImportLog = LogFile;
            //    Dictionary<string, object> ReturnObjects = new Dictionary<string, object>();
            //    ReturnObjects.Add("ImportLogFile", ImportLogFile);
            //    ReturnObjects.Add("ErrorLog", log);
            //    return JsonResult.CreateResult(true, "SUCCES", ReturnObjects);
            //}

            return log;
        }



        private string importSubscriber(string[] data, int[] importProgress)//BaseCollection<NewsletterGroup> newsGroups
        {
            string[] log = new string[6];
            try
            {
                BaseCollection<NewsletterSubscriber> subscribers = BaseCollection<NewsletterSubscriber>.Get("FK_Site = '" + WebSessionHelper.CurrentSite.ID.ToString() + "' AND Email = '" + data[this.EmailColumnNo].Trim() + "'");
                NewsletterSubscriber subscriber;

                var existingSubscriber = subscribers.Where(b => b.Email == data[this.EmailColumnNo]).FirstOrDefault();//Where(b => b.Name == data[this.NameColumnNo]).FirstOrDefault();

                if (existingSubscriber == null)
                {
                    if (EmailManager.isValidEmailAddress(data[this.EmailColumnNo].Trim()))
                    {
                        if (isValidDataLength(data) == true)
                        {
                            subscriber = new NewsletterSubscriber();
                            subscriber.RegistrationType = RegistrationTypeEnum.Import;
                            subscriber.Site = WebSessionHelper.CurrentSite;
                            subscriber.Email = data[this.EmailColumnNo].Trim();
                            subscriber.Confirmed = (this.AutoConfirm);
                            if (this.ForeNameColumn != "" && this.ForeNameColumnNo != 9999) subscriber.ForeName = data[this.ForeNameColumnNo];
                            if (this.NameColumn != "" && this.NameColumnNo != 9999) subscriber.Name = data[this.NameColumnNo];
                            if (this.NamePrefixColumn != "" && this.NamePrefixColumnNo != 9999) subscriber.NamePrefix = data[this.NamePrefixColumnNo];
                            //subscriber.Gender = data[this.GenderCoumnNo] // ERROR: cannot convert string/int > sexeEnum
                            subscriber.SubscribedGroups.AddRange(this.Groups);
                            subscriber.Save();

                            log[0] = importProgress[0].ToString();

                            if (this.NameColumnNo != 9999) log[1] = data[this.NameColumnNo];
                            log[2] = data[this.EmailColumnNo];
                            log[3] = "Geimporteerd";
                            log[4] = importProgress[1].ToString();
                            log[5] = true.ToString();
                        }
                        else
                        {
                            string tooLong = "Het ";

                            if (this.EmailColumnNo != 9999)
                            {
                                if (data[this.EmailColumnNo].ToString().Length > 50)
                                {
                                    tooLong += "email adres, ";
                                }
                            }
                            if (this.NameColumnNo != 9999)
                            {
                                if (data[this.NameColumnNo].ToString().Length > 150)
                                {
                                    tooLong += "achternaam, ";
                                }
                            }
                            if (this.ForeNameColumnNo != 9999)
                            {
                                if (data[this.ForeNameColumnNo].ToString().Length > 250)
                                {
                                    tooLong += "voornaam, ";
                                }
                            }
                            if (this.NamePrefixColumnNo != 9999)
                            {
                                if (data[this.NamePrefixColumnNo].ToString().Length > 25)
                                {
                                    tooLong += "tussenvoegsel, ";
                                }
                            }
                            log[0] = importProgress[0].ToString();
                            if (this.NameColumnNo != 9999) log[1] = data[this.NameColumnNo];
                            log[2] = data[this.EmailColumnNo];
                            log[3] = tooLong += "is te lang";
                            log[4] = importProgress[1].ToString();
                            log[5] = false.ToString();
                        }
                    }
                    else
                    {
                        log[0] = importProgress[0].ToString();
                        if (this.NameColumnNo != 9999) log[1] = data[this.NameColumnNo];
                        log[2] = data[this.EmailColumnNo];
                        log[3] = "Geen geldig email adres";
                        log[4] = importProgress[1].ToString();
                        log[5] = false.ToString();
                    }
                }
                else if (existingSubscriber != null && this.SkipDoubleRecords == false)
                {
                    if (EmailManager.isValidEmailAddress(data[this.EmailColumnNo].Trim()) && isValidDataLength(data) == true)
                    {
                        if (this.AppendGroups == true)
                        {
                            for (int i = 0; i < existingSubscriber.SubscribedGroups.Count(); i++)
                            {
                                for (int j = 0; j < this.Groups.Count(); j++)
                                {
                                    if (this.Groups[j].ID == existingSubscriber.SubscribedGroups[i].ID)
                                    {
                                        this.Groups.RemoveAt(j);
                                        break;

                                    }
                                }
                            }
                        }

                        /* subscriber = new NewsletterSubscriber();
                           subscriber.Site = SessionObject.CurrentSite;
                           subscriber.Email = data[this.EmailColumnNo].Trim();
                           subscriber.Confirmed = (this.AutoConfirm);
                           subscriber.ForeName = data[this.ForeNameColumnNo];
                           subscriber.Name = data[this.NameColumnNo];
                           subscriber.NamePrefix = data[this.NamePrefixColumnNo];
                           //subscriber.Gender = data[this.GenderColumnNo]; // ERROR: cannot convert string/int > sexeEnum
                           subscriber.SubscribedGroups.AddRange(this.Groups);
                           subscriber.Save();*/

                        existingSubscriber.Site = WebSessionHelper.CurrentSite;
                        existingSubscriber.Confirmed = (this.AutoConfirm);

                        if (this.ForeNameColumn != "" && this.ForeNameColumnNo != 9999)
                        {
                            existingSubscriber.ForeName = data[this.ForeNameColumnNo];
                        }
                        else { existingSubscriber.ForeName = ""; }

                        if (this.NameColumn != "" && this.NameColumnNo != 9999)
                        {
                            existingSubscriber.Name = data[this.NameColumnNo];
                        }
                        else { existingSubscriber.Name = ""; }

                        if (this.NamePrefixColumn != "" && this.NamePrefixColumnNo != 9999)
                        {
                            existingSubscriber.NamePrefix = data[this.NamePrefixColumnNo];
                        }
                        else { existingSubscriber.NamePrefix = ""; }

                        if (this.AppendGroups == false) existingSubscriber.SubscribedGroups.Clear();

                        if (existingSubscriber.SubscribedGroups != this.Groups)
                            existingSubscriber.SubscribedGroups.AddRange(this.Groups);

                        existingSubscriber.Save();

                        log[0] = importProgress[0].ToString();
                        if (this.NameColumnNo != 9999) log[1] = data[this.NameColumnNo];
                        log[2] = data[this.EmailColumnNo];
                        log[3] = "Abonnee bestaat al. overschreven";
                        log[4] = importProgress[1].ToString();
                        log[5] = true.ToString();
                    }
                    else if (isValidDataLength(data) == false)
                    {
                        string tooLong = "Het ";

                        if (this.EmailColumnNo != 9999)
                        {
                            if (data[this.EmailColumnNo].ToString().Length > 50)
                            {
                                tooLong += "email adres, ";
                            }
                        }
                        if (this.NameColumnNo != 9999)
                        {
                            if (data[this.NameColumnNo].ToString().Length > 150)
                            {
                                tooLong += "achternaam, ";
                            }
                        }
                        if (this.ForeNameColumnNo != 9999)
                        {
                            if (data[this.ForeNameColumnNo].ToString().Length > 250)
                            {
                                tooLong += "voornaam, ";
                            }
                        }
                        if (this.NamePrefixColumnNo != 9999)
                        {
                            if (data[this.NamePrefixColumnNo].ToString().Length > 25)
                            {
                                tooLong += "tussenvoegsel, ";
                            }
                        }
                        log[0] = importProgress[0].ToString();
                        if (this.NameColumnNo != 9999) log[1] = data[this.NameColumnNo];
                        log[2] = data[this.EmailColumnNo];
                        log[3] = tooLong += "is te lang";
                        log[4] = importProgress[1].ToString();
                        log[5] = false.ToString();
                    }
                    else
                    {
                        log[0] = importProgress[0].ToString();
                        if (this.NameColumnNo != 9999) log[1] = data[this.NameColumnNo];
                        log[2] = data[this.EmailColumnNo];
                        log[3] = "Geen geldig email adres";
                        log[4] = importProgress[1].ToString();
                        log[5] = false.ToString();
                    }
                }
                else
                {
                    log[0] = importProgress[0].ToString();
                    if (this.NameColumnNo != 9999) log[1] = data[this.NameColumnNo];
                    log[2] = data[this.EmailColumnNo];
                    log[3] = "Dubbele abonnee is overgeslagen";
                    log[4] = importProgress[1].ToString();
                    log[5] = true.ToString();
                }
            }
            catch (Exception ex)
            {
                log[0] = importProgress[0].ToString();
                log[2] = "";
                log[3] = "SYSTEEM FOUT: " + ex.Message ;
                log[4] = importProgress[1].ToString();
                log[5] = false.ToString();
            }
            if (log[5] == false.ToString())
            {
                return "FOUT in regel " + log[0] + "/" + log[4] + " : " + log[2] + " = " + log[3] + ".";
            }
            else
            {

                return "GESLAAGD regel " + log[0] + "/" + log[4] + " : " + log[2] + " = " + log[3] + ".";

            }
            

        }

        private bool isValidDataLength(string[] data)
        {//Kijken of het bestaat en of de lengte juist is.

            if (this.EmailColumnNo != 9999 && (this.ForeNameColumnNo != 9999 || this.NameColumnNo != 9999 || this.NamePrefixColumnNo != 9999))
            {
                if (data[this.EmailColumnNo].ToString().Length > 50)// &&(( ) || ( ) || (this.NamePrefixColumnNo!=9999 && data[this.NamePrefixColumnNo].ToString().Length < 25))
                {
                    return false;
                }
                if ((this.NameColumnNo != 9999) && data[this.NameColumnNo].ToString().Length > 150)
                {
                    return false;
                }
                if ((this.ForeNameColumnNo != 9999) && data[this.ForeNameColumnNo].ToString().Length > 250)
                {
                    return false;
                }
                if ((this.NamePrefixColumnNo != 9999) && data[this.NamePrefixColumnNo].ToString().Length > 25)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (this.EmailColumnNo != 9999 && data[this.EmailColumnNo].ToString().Length < 50 && this.ForeNameColumnNo == 9999)//alleen de email column bestaat en de rest is leeg
            {
                return true;
                //als bovenste if false is en het email adres wel bestaat enkel die controleren.
            }
            else
            {
                return false;
            }
        }

        public string MakeErrorLogFile(List<string> log)//,ImportDefinition Definition )
        {
            int gelukt = 0;
            int gefaald = 0;
            string dateTimeString = string.Format("{0:dd-MM-yyyy_hh-mm-ss}", DateTime.Now);
            string filename = "ImportLog" + dateTimeString + ".txt";

            string a = HttpContext.Current.Request.Url.Scheme;//http, https, etc
            string b = HttpContext.Current.Request.Url.Authority;//siteurl + port (localhost:12345)
            string path = a + "://" + b + "/_temp/";
            string downloadPathName = path + filename;
            string fullPathName = HttpContext.Current.Request.MapPath("~/_temp/") + filename;

            StreamWriter sw = new StreamWriter(fullPathName);
            {
                sw.WriteLine("Geimporteerd: " + this.FileName + " op " + DateTime.Now.ToShortDateString());
                sw.WriteLine("ImportTemplate gebruikt: " + this.Name);
                sw.WriteLine();
                sw.WriteLine("De volgende regels zijn niet geimporteerd vanwege fouten:");
                sw.WriteLine();

                foreach (string logRow in log)
                {
                    if (logRow.Contains("FOUT"))
                    {
                        sw.WriteLine(logRow);
                        gefaald++;
                    }
                    else
                    {
                        gelukt++;
                    }
                }
                sw.WriteLine();
                sw.WriteLine("Van de " + log.Count() + " zijn er " + gelukt + " gelukt en " + gefaald + " mislukt.");
                sw.Close();

            }

            //TextWriter tw = new StreamWriter(filename);
            //{
            //    tw.WriteLine("Log van het importeren");

            //}
            //tw.Close();


            return downloadPathName;
        }

    }
}
