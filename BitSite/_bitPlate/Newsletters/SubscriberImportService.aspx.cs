using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Newsletters;
using HJORM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitSite._bitPlate.Newsletters
{
    public partial class SubscriberImportService : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BaseCollection<NewsletterGroup> Groups = new BaseCollection<NewsletterGroup>();
            if (Request.Form.AllKeys.Contains("subscribedNewsgroups"))
            {
                var value = Request["subscribedNewsgroups"];
            }

            if (Request.Files.Count > 0)
            {
                HttpPostedFile HttpFile = Request.Files[0];
                HttpFile.SaveAs(Server.MapPath("~/_temp/") + HttpFile.FileName);
                FileInfo file = new FileInfo(Server.MapPath("~/_temp/") + HttpFile.FileName);
                if (file.Extension.ToLower() == ".csv")
                {
                    StreamReader reader = new StreamReader(file.FullName);
                    int lineNumber = 1;
                    while (reader.Peek() >= 0)
                    {
                        string line = reader.ReadLine();
                        string[] dataLine = Regex.Split(line, ";");
                        if (lineNumber > 1)
                        {
                            this.CreateSubscriber(dataLine);

                        }
                        lineNumber++;
                    }
                    reader.Close();
                }
                
                file.Delete();
            }
        }
        private void CreateSubscriber(string[] data)
        {
            BaseCollection<NewsletterSubscriber> subscribers = BaseCollection<NewsletterSubscriber>.Get("FK_Site = '" + SessionObject.CurrentSite.ID.ToString() + "' AND Name = '" + data[4].Trim() + "'");
            NewsletterSubscriber subscriber;
            if (subscribers.Count > 0)
            {
                subscriber = subscribers[0];
            }
            else
            {
                subscriber = new NewsletterSubscriber();
                subscriber.Site = SessionObject.CurrentSite;
                subscriber.Email = data[4].Trim();
                subscriber.Confirmed = (data[14] == "1");
                if (data[19].Trim() != "")
                {
                    subscriber.UnsubscribeDate = DateTime.Parse(data[19].Trim());
                }
                subscriber.ForeName = data[1];
                subscriber.Gender = (BaseUser.SexeEnum)int.Parse(data[5]);
                subscriber.Name = data[3];
                subscriber.NamePrefix = data[2];
                subscriber.Save();
            }
           

            /* BaseCollection<SiteUser> Users = BaseCollection<SiteUser>.Get("FK_Site = '" + SessionObject.CurrentSite.ID.ToString() + "' AND Email = '" + subscriber.EmailAddress + "'");
            if (Users.Count > 0)
            {
                subscriber.User = Users[0];
                subscriber.Save();
            }
            else
            {
                if (data[3].Trim() != "") //als achternaam niet leeg is dan create siteuser 
                {
                    DateTime birthDate;
                    DateTime.TryParse(data[12], out birthDate);

                    SiteUser user = new SiteUser()
                    {
                        //Active = BitPlate.Domain.ActiveEnum.Active,
                        Address = data[7],
                        BirthDate = birthDate,
                        City = data[9],
                        CompanyName = data[0],
                        Country = data[10],
                        //CreateDate = DateTime.Parse(data[17]),
                        //Email = data[4],
                        //ForeName = data[1],
                        //Gender = GetGender(data[5]),
                        //Name = data[3],
                        //NamePrefix = data[2],
                        Site = SessionObject.CurrentSite,
                        Telephone = data[11]
                    };
                    user.Save();
                    subscriber.User = user;
                    subscriber.Save();
                }
            } */
        }

        //private BaseUser.SexeEnum GetGender(string inputNumber)
        //{
        //    if (inputNumber == "1")
        //    {
        //        return BaseUser.SexeEnum.Male;
        //    }

        //    if (inputNumber == "2")
        //    {
        //        return BaseUser.SexeEnum.Female;
        //    }
        //    return BaseUser.SexeEnum.Undefined;
        //}
    }       
}