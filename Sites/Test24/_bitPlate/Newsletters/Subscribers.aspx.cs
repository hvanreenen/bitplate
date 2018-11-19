using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitPlate.Domain.Autorisation;
using HJORM;
using System.IO;
using System.Text.RegularExpressions;
using BitPlate.Domain.Newsletters;

namespace BitSite._bitPlate.Newsletters
{
    public partial class Subscribers : BasePage
    {
        //private string fileName;

        /*protected void Page_Load(object sender, EventArgs e)
        {
             base.CheckLoginAndLicense();
             base.CheckPermissions(FunctionalityEnum.SiteUserManagement);
          
            if (importNewsgroupSelector.Items.Count == 0)
             {

                 foreach (NewsletterGroup group in NewsletterService.LoadNewsletterGroupList())
                 {
                     ListItem li = new ListItem(group.Name, group.ID.ToString());
                     importNewsgroupSelector.Items.Add(li);
                 }
             }

            if (Request.Files.Count > 0)
             {
                BaseCollection<NewsletterGroup> groups = new BaseCollection<NewsletterGroup>();
                 foreach (ListItem li in importNewsgroupSelector.Items)
                 {
                     if (li.Selected)
                     {
                         NewsletterGroup group = BaseObject.GetById<NewsletterGroup>(Guid.Parse(li.Value));
                         groups.Add(group);
                     }
                 }

                HttpPostedFile HttpFile = Request.Files[0];
                 FileInfo file = new FileInfo(HttpFile.FileName);
                 string extension = file.Extension;

                 if (extension.ToLower() == ".csv")
                 {
                     if (!Directory.Exists(Server.MapPath("~/_temp/upload/csv/")))
                     {
                         Directory.CreateDirectory(Server.MapPath("~/_temp/upload/csv/"));
                     }
                         HttpFile.SaveAs(Server.MapPath("~/_temp/upload/csv/") + HttpFile.FileName);
                         file = new FileInfo(Server.MapPath("~/_temp/upload/csv/") + HttpFile.FileName);
                         fileName = Server.MapPath("~/_temp/upload/csv/") + HttpFile.FileName;
      
                 }
                 else if (extension.ToLower() == ".txt")
                 {  
                     if(!Directory.Exists(Server.MapPath("~/_temp/upload/txt/")))
                     {
                         Directory.CreateDirectory(Server.MapPath("~/_temp/upload/txt/"));
                     }
                         HttpFile.SaveAs(Server.MapPath("~/_temp/upload/txt/") + HttpFile.FileName);
                         file = new FileInfo(Server.MapPath("~/_temp/upload/txt/") + HttpFile.FileName);
                         fileName = Server.MapPath("~/_temp/upload/txt/") + HttpFile.FileName;
                 }
                 else if (extension.ToLower() == ".xml")
                 {
                     if (!Directory.Exists(Server.MapPath("~/_temp/upload/xml/")))
                     {
                         Directory.CreateDirectory(Server.MapPath("~/_temp/upload/xml"));
                     }
                     HttpFile.SaveAs(Server.MapPath("~/_temp/upload/xml/") + HttpFile.FileName);
                     file = new FileInfo(Server.MapPath("~/_temp/upload/xml/") + HttpFile.FileName);
                     fileName = Server.MapPath("~/_temp/upload/xml/") + HttpFile.FileName;
                 }
                 else
                 {
                     file.Delete();
                 }

                // Huidige functie om te importeren 
                 /*if (file.Extension.ToLower() == ".csv")
                 {
                     StreamReader reader = new StreamReader(file.FullName);
                     int lineNumber = 1;
                     while (reader.Peek() >= 0)
                     {
                         string line = reader.ReadLine();
                         string[] dataLine = Regex.Split(line, ";");
                         if (lineNumber > 1)
                         {
                             this.CreateSubscriber(dataLine, groups);
                         }
                         lineNumber++;
                     }
                     reader.Close();
                 }
                 file.Delete();
             }
         }*/

            /*private void CreateSubscriber(string[] data, BaseCollection<NewsletterGroup> newsGroups)
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
                    subscriber.CompanyName = data[0];
                    subscriber.SubscribedGroups.AddRange(newsGroups);
                    subscriber.Save();
                }

           
                BaseCollection<SiteUser> Users = BaseCollection<SiteUser>.Get("FK_Site = '" + SessionObject.CurrentSite.ID.ToString() + "' AND Email = '" + subscriber.EmailAddress + "'");
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
                            Active = BitPlate.Domain.ActiveEnum.Active,
                            Address = data[7],
                            BirthDate = birthDate,
                            City = data[9],
                            CompanyName = data[0],
                            Country = data[10],
                            CreateDate = DateTime.Parse(data[17]),
                            Email = data[4],
                            ForeName = data[1],
                            Gender = GetGender(data[5]),
                            Name = data[3],
                            NamePrefix = data[2],
                            Site = SessionObject.CurrentSite,
                            Telephone = data[11]
                        };
                        user.Save();
                        subscriber.User = user;
                        subscriber.Save();
                    }
                }
            }*/

            /* private BaseUser.SexeEnum GetGender(string inputNumber)
            {
                if (inputNumber == "1")
                {
                    return BaseUser.SexeEnum.Male;
                }

                if (inputNumber == "2")
                {
                    return BaseUser.SexeEnum.Female;
                }
                return BaseUser.SexeEnum.Undefined;
            } */
        
    }
}