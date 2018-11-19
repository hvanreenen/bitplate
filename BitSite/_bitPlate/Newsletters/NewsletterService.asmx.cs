using BitPlate.Domain;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Newsletters;
using BitPlate.Domain.Utils;
using HJORM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.SessionState;
using System.Web.UI;

namespace BitSite._bitPlate.Newsletters
{
    [GenerateScriptType(typeof(Newsletter))]
    [GenerateScriptType(typeof(SiteUser))]
    [GenerateScriptType(typeof(SiteUserGroup))]
    [GenerateScriptType(typeof(NewsletterSubscriber))]
    [GenerateScriptType(typeof(NewsletterImportDefinition))]
    [GenerateScriptType(typeof(NewsletterGroup))]
    // [GenerateScriptType(typeof(ImportDefinitionGroups))]

    [System.Web.Script.Services.ScriptService]
    public partial class NewsletterService :  BaseService
    {
        public NewsletterImportDefinition definition;

        /* [WebMethod(EnableSession = true)]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         public static JsonResult LoadNewsletterConfig()
         {
             NewsletterConfig config;
             BaseCollection<NewsletterConfig> configurations = BaseCollection<NewsletterConfig>.Get("FK_Site = '" + SessionObject.CurrentSite.ID.ToString() + "'");
             config = configurations.FirstOrDefault();
             if (config != null) config.Load();
             BaseCollection<CmsPage> OptPages = BaseCollection<CmsPage>.Get("FK_Site = '" + SessionObject.CurrentSite.ID.ToString() + "'");
             Dictionary<string, object> returnObject = new Dictionary<string, object>();
             returnObject.Add("NewsletterConfig", config);
             returnObject.Add("Pages", OptPages);
             return JsonResult.CreateResult(true, returnObject);
         }

         [WebMethod(EnableSession = true)]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         public static JsonResult SaveNewsletterConfig(NewsletterConfig obj)
         {
             if (obj.Site == null)
             {
                 obj.Site = SessionObject.CurrentSite;
             }
             obj.Save();
             return JsonResult.CreateResult(true);
         } */

        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public static BaseCollection<NewsletterSubscriber> LoadSubscribers()
        //{
        //    BaseCollection<NewsletterSubscriber> subscribers = BaseCollection<NewsletterSubscriber>.Get();
        //    foreach (NewsletterSubscriber subscriber in subscribers)
        //    {
        //        if (subscriber.User == null)
        //        {
        //            subscriber.User = new SiteUser();
        //        }
        //    }
        //    return subscribers;
        //}



        public void UploadSubscriberFile(HttpContext context)
        {
            definition = new NewsletterImportDefinition();

            string fileName;

            if (!Directory.Exists(Server.MapPath("~/_temp"))) Directory.CreateDirectory(Server.MapPath("~/_temp"));
            if (!Directory.Exists(Server.MapPath("~/_temp/upload"))) Directory.CreateDirectory(Server.MapPath("~/_temp/upload"));


            if (context.Request.Files.Count >= 1)
            {
                HttpPostedFile HttpFile = context.Request.Files[0];
                FileInfo file = new FileInfo(HttpFile.FileName);
                string extension = file.Extension.ToLower();

                if (extension.ToLower() == ".csv")
                {
                    if (!Directory.Exists(context.Server.MapPath("~/_temp/upload/csv/")))
                    {
                        Directory.CreateDirectory(context.Server.MapPath("~/_temp/upload/csv/"));
                    }
                    HttpFile.SaveAs(context.Server.MapPath("~/_temp/upload/csv/") + HttpFile.FileName);
                    file = new FileInfo(context.Server.MapPath("~/_temp/upload/csv/") + HttpFile.FileName);
                    fileName = HttpFile.FileName;

                    definition.FileExtension = extension;
                    definition.FileName = fileName;

                    context.Response.Write(definition.ToJsonString());

                    return;
                }
                else if (extension.ToLower() == ".txt")
                {
                    if (!Directory.Exists(context.Server.MapPath("~/_temp/upload/txt/")))
                    {
                        Directory.CreateDirectory(context.Server.MapPath("~/_temp/upload/txt/"));
                    }
                    HttpFile.SaveAs(context.Server.MapPath("~/_temp/upload/txt/") + HttpFile.FileName);
                    file = new FileInfo(Server.MapPath("~/_temp/upload/txt/") + HttpFile.FileName);
                    fileName = context.Server.MapPath("~/_temp/upload/txt/") + HttpFile.FileName;

                    definition.FileExtension = extension;
                    definition.FileName = fileName;

                    context.Response.Write(definition.ToJsonString());

                    return;
                }
                else if (extension.ToLower() == ".xml")
                {
                    if (!Directory.Exists(context.Server.MapPath("~/_temp/upload/xml/")))
                    {
                        Directory.CreateDirectory(context.Server.MapPath("~/_temp/upload/xml"));
                    }
                    HttpFile.SaveAs(context.Server.MapPath("~/_temp/upload/xml/") + HttpFile.FileName);
                    file = new FileInfo(context.Server.MapPath("~/_temp/upload/xml/") + HttpFile.FileName);
                    fileName = context.Server.MapPath("~/_temp/upload/xml/") + HttpFile.FileName;

                    definition.FileExtension = extension;
                    definition.FileName = fileName;

                    context.Response.Write(definition.ToJsonString());
                    return;
                }
                else
                {
                    file.Delete();
                    definition.FileExtension = extension;

                    context.Response.Write(definition.ToJsonString());

                    return;
                }
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<TreeGridItem> GetSubscribers(string sortering, string searchString)
        {

            List<TreeGridItem> returnList = new List<TreeGridItem>();
            BaseCollection<NewsletterSubscriber> subscribers;
            if (searchString != "")
            {
                searchString = "SELECT newslettersubscriber.*  FROM `newslettersubscriber`  WHERE CONCAT (newslettersubscriber.Email, newslettersubscriber.ForeName, newslettersubscriber.NamePrefix, newslettersubscriber.Name, newslettersubscriber.CompanyName) like'%" + searchString + "%'";
                subscribers = BaseCollection<NewsletterSubscriber>.LoadFromSql(searchString);
            }
            else
            {
                subscribers = BaseCollection<NewsletterSubscriber>.Get("FK_Site='" + SessionObject.CurrentSite.ID.ToString() + "' AND UnsubscribeDate IS NULL" + searchString, sortering);
            }

            foreach (NewsletterSubscriber subscriber in subscribers)
            {
                TreeGridItem item = new TreeGridItem();
                item.ID = subscriber.ID;
                item.Field1 = subscriber.Email;
                item.Field3 = subscriber.CompleteName;
                item.CreateDate = subscriber.CreateDate;
                item.Status = (subscriber.Confirmed) ? "<input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\" />" : "<input type=\"checkbox\" disabled=\"disabled\" />";
                foreach (NewsletterGroup group in subscriber.SubscribedGroups)
                {
                    item.Field2 += group.Name + ", ";

                }
                if (item.Field2 != "" && item.Field2 != null)
                {
                    item.Field2 = item.Field2.Substring(0, item.Field2.Length - 2);
                }

                /* if (subscriber.User != null)
                {
                    item.Name = subscriber.User.CompleteName;
                } */
                returnList.Add(item);
            }
            return returnList;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public object GetNewsletterLicenseInfo()
        {
            int usedMailings = BaseCollection<NewsletterMailing>.Get("FK_Site = '" + SessionObject.CurrentSite.ID.ToString() + "'").Count();
            int maxNumberOfNewsletterMailings = (SessionObject.CurrentLicense.MaxNumberOfNewsletterMailings != null) ? (int)SessionObject.CurrentLicense.MaxNumberOfNewsletterMailings : -1;
            Dictionary<string, string> newsletterLicenseInfo = new Dictionary<string, string>();
            newsletterLicenseInfo.Add("sentNewletterCount", (maxNumberOfNewsletterMailings != 1) ? maxNumberOfNewsletterMailings.ToString() : "Onbeperkt");
            newsletterLicenseInfo.Add("sendNewletterCount", usedMailings.ToString());
            newsletterLicenseInfo.Add("UnusedSentTickets", (maxNumberOfNewsletterMailings != 1) ? (maxNumberOfNewsletterMailings - usedMailings).ToString() : "-");
            return newsletterLicenseInfo;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Newsletter GetNewsletter(Guid id)
        {
            Newsletter newsletter = BaseObject.GetById<Newsletter>(id);
            //voor een nieuwe
            if (newsletter == null)
            {
                newsletter = new BitPlate.Domain.Newsletters.Newsletter();
                newsletter.Template = null;
                newsletter.SentFromAddress = SessionObject.CurrentSite.NewsletterSender;
            }
            return newsletter;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public NewsletterSubscriber GetSubscriber(Guid id)
        {
            NewsletterSubscriber subscriber = BaseObject.GetById<NewsletterSubscriber>(id);
            if (subscriber == null)
            {
                subscriber = new BitPlate.Domain.Newsletters.NewsletterSubscriber();
                subscriber.Confirmed = true;
                foreach (NewsletterGroup group in LoadNewsletterGroupList("Name ASC"))
                {
                    if (group.IsMandatoryGroup && !subscriber.SubscribedGroups.Contains(group))
                    {
                        subscriber.SubscribedGroups.Add(group);
                    }
                }
            }

            return subscriber;
        }

        [WebMethod, ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public NewsletterImportDefinition GetImportDefinition(Guid id)
        {
            if (id != Guid.Empty)
            {
                NewsletterImportDefinition definition = BaseObject.GetById<NewsletterImportDefinition>(id);
                // BaseCollection<ImportDefinitionGroups> ImportGroups = BaseCollection<ImportDefinitionGroups>.Get("FK_SITE='" + SessionObject.CurrentSite.ID + "' AND FK_Definition LIKE '" + definition.ID + "'", "FK_Definition");
                //definition.Subscribergroups = new HJORM.BaseCollection<BitPlate.Domain.Newsletters.NewsletterGroup>();

                //foreach (NewsletterGroup ImportGroup in definition.Groups)
                //{
                //NewsletterGroup groups = BaseObject.GetById<NewsletterGroup>(Guid.Parse(ImportGroup.FK_Group));
                //definition.Subscribergroups.Add(ImportGroup);
                //   definition.Save();
                //}
                definition.FileName = string.Empty;

                return definition;
            }
            else
            {
                NewsletterImportDefinition definition = new NewsletterImportDefinition();
                return definition;
            }

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<TreeGridItem> GetImportDefinitions(string sortering, string searchString)
        {
            List<TreeGridItem> returnList = new List<TreeGridItem>();
            BaseCollection<NewsletterImportDefinition> definitions;

            if (searchString != "")
            {
                searchString = "SELECT importdefinition.*  FROM `importdefinition`  WHERE CONCAT (importdefinition.FileName, importdefinition.FileExtension, importdefinition.NameColumn, importdefinition.NamePrefixColumn, importdefinition.ForeNameColumn, importdefinition.EmailColumn, importdefinition.Delimiter ) like'%" + searchString + "%' FK_Site='" + SessionObject.CurrentSite.ID.ToString() + "'";
                definitions = BaseCollection<NewsletterImportDefinition>.LoadFromSql(searchString);
            }
            else
            {
                definitions = BaseCollection<NewsletterImportDefinition>.Get("FK_Site='" + SessionObject.CurrentSite.ID.ToString() + "'" + searchString, sortering);
            }

            foreach (NewsletterImportDefinition definition in definitions)
            {
                TreeGridItem def = new TreeGridItem();
                def.ID = definition.ID;
                def.Name = definition.Name;
                def.Field1 = definition.FileExtension;
                def.Field2 = definition.Delimiter;
                def.CreateDate = definition.CreateDate;

                returnList.Add(def);
            }
            return returnList;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<NewsletterGroup> LoadNewsletterGroupList(string sort)
        {
            BaseCollection<NewsletterGroup> NewsletterGroups = BaseCollection<NewsletterGroup>.Get("FK_Site='" + SessionObject.CurrentSite.ID.ToString() + "'", sort);
            return NewsletterGroups;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<NewsletterGroup> GetMandatoryGroups()
        {
            BaseCollection<NewsletterGroup> MandatoryGroups = BaseCollection<NewsletterGroup>.Get("FK_Site = '" + SessionObject.CurrentSite.ID.ToString() + "' AND IsMandatoryGroup = true");
            return MandatoryGroups;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<NewsletterGroup> LoadChoosableNewsletterGroupList()
        {
            BaseCollection<NewsletterGroup> NewsletterGroups = BaseCollection<NewsletterGroup>.Get("FK_Site='" + SessionObject.CurrentSite.ID.ToString() + "' AND IsMandatoryGroup=0");
            return NewsletterGroups;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult SaveNewsletterGroup(NewsletterGroup obj)
        {

            List<string> returnValue = new List<string>();

            //Dictionary<string, object> returnValue = new Dictionary<string, object>();
            bool Success = true;
            Exception exc = null;
            try
            {
                obj.Save();
            }
            catch (Exception ex)
            {
                Success = false;
                exc = ex;
            }
            if (Success)
            {
                //BaseCollection<NewsletterGroup> Groups = LoadNewsletterGroupList();
                returnValue.Add(obj.ID.ToString());
                return JsonResult.CreateResult(Success, returnValue);
            }
            else
            {
                return JsonResult.CreateResult(Success, exc.Message);
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult DeleteNewsletterGroup(NewsletterGroup obj)
        {
            obj.Delete();
            return JsonResult.CreateResult(true, LoadNewsletterGroupList("Name ASC"));// LoadNewsletterGroupList();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public NewsletterGroup GetNewsletterGroupById(string newsletterId)
        {
            return BaseObject.GetById<NewsletterGroup>(new Guid(newsletterId));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<TreeGridItem> LoadNewsletters(string sort, string searchstring)
        {
            List<TreeGridItem> returnValue = new List<TreeGridItem>();
            if (sort != null && sort != "")
            {
                sort = "ORDER BY " + sort;
            }
            BaseCollection<Newsletter> newsletters = BaseCollection<Newsletter>.Get("FK_Site='" + SessionObject.CurrentSite.ID.ToString() + "' AND Name LIKE '%" + searchstring + "%' " + sort + "");
            foreach (Newsletter newsletter in newsletters)
            {
                TreeGridItem tgi = new TreeGridItem();
                tgi.ID = newsletter.ID;
                tgi.Name = newsletter.Name;
                tgi.CreateDate = newsletter.CreateDate;
                tgi.Field1 = newsletter.MailingGroupsString;
                returnValue.Add(tgi);
            }
            return returnValue;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<TreeGridItem> LoadNewsletterList(NewsletterGroup obj = null)
        {
            List<TreeGridItem> returnValue = new List<TreeGridItem>();
            BaseCollection<Newsletter> newsletters = new HJORM.BaseCollection<BitPlate.Domain.Newsletters.Newsletter>();
            if (obj == null)
            {
                newsletters = BaseCollection<Newsletter>.Get("FK_Site='" + SessionObject.CurrentSite.ID.ToString() + "'");
            }
            else
            {
                newsletters = obj.Newsletters;
            }

            foreach (Newsletter newsletter in newsletters)
            {
                TreeGridItem tgi = new TreeGridItem();
                tgi.ID = newsletter.ID;
                tgi.Name = newsletter.Name;
                tgi.CreateDate = newsletter.CreateDate;
                tgi.Field1 = newsletter.MailingGroupsString;
                returnValue.Add(tgi);
            }
            return returnValue;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult SaveNewsletter(Newsletter newsletter)
        {
            try
            {
                newsletter.Site = SessionObject.CurrentSite;
                newsletter.Save();
                //newsletter.Publish();
                newsletter.Publish2();

            }
            catch (Exception ex)
            {
                return JsonResult.CreateResult(false, ex.Message, null);
            }
            return JsonResult.CreateResult(true, LoadNewsletterList());
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult DeleteNewsletter(Newsletter obj)
        {
            try
            {
                if (obj != null)
                {
                    obj.Delete();
                }
            }
            catch (Exception ex)
            {
                return JsonResult.CreateResult(false, ex.Message);
            }
            return JsonResult.CreateResult(true);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Dictionary<string, object> LoadNewsletterGroupDetails(NewsletterGroup obj)
        {
            BaseCollection<NewsletterSubscriber> Subscribers = obj.Subscribers;
            BaseCollection<Newsletter> Newsletters = obj.Newsletters;
            Dictionary<string, object> ReturnValue = new Dictionary<string, object>();
            ReturnValue.Add("Newsletters", Newsletters);
            ReturnValue.Add("Subscribers", Subscribers);
            return ReturnValue;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult SaveAbbonementEmail(string NewsLetterGroupId, string Email)
        {
            NewsletterSubscriber Subscriber;
            Guid NewsletterGroupID = Guid.Parse(NewsLetterGroupId);
            NewsletterGroup CurrentNewsletterGroup = NewsletterGroup.GetById<NewsletterGroup>(NewsletterGroupID);
            //BaseCollection<NewsletterSubscriber> SubScribers = BaseCollection<NewsletterSubscriber>.Get("EmailAdres = '" + Email + "'");
            BaseCollection<NewsletterSubscriber> SubScribers = BaseCollection<NewsletterSubscriber>.Get("Name = '" + Email + "'");

            if (SubScribers.Count >= 1)
            {
                Subscriber = SubScribers[0];

                if (Subscriber.SubscribedGroups.Where(c => c.ID == NewsletterGroupID).Count() >= 1)
                {
                    return JsonResult.CreateResult(false, "Dit emailadres is al toegekend aan deze nieuwsbrief groep.");
                }
                else
                {
                    Subscriber.SubscribedGroups.Add(CurrentNewsletterGroup);
                    Subscriber.Save();
                }
            }
            else
            {
                Subscriber = new BitPlate.Domain.Newsletters.NewsletterSubscriber();
                Subscriber.Email = Email;
                Subscriber.SubscribedGroups.Add(CurrentNewsletterGroup);
                Subscriber.Save();
            }
            return JsonResult.CreateResult(true, CurrentNewsletterGroup.Subscribers);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult DeleteAbbonementFromGroup(string NewsLetterGroupId, string SubscriberId)
        {

            Guid NewsletterGroupID = Guid.Parse(NewsLetterGroupId);
            Guid SubscriberID = Guid.Parse(SubscriberId);

            NewsletterSubscriber Subscriber = NewsletterSubscriber.GetById<NewsletterSubscriber>(SubscriberID);
            NewsletterGroup SubscriberGroup = NewsletterGroup.GetById<NewsletterGroup>(NewsletterGroupID);

            if (Subscriber.SubscribedGroups.Count > 1)
            {
                Subscriber.SubscribedGroups.Remove(SubscriberGroup);
                Subscriber.Save();
            }
            else
            {
                Subscriber.Delete();
            }
            return JsonResult.CreateResult(true, SubscriberGroup.Subscribers);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<NewsletterSubscriber> GetNewsletterRecipients(Guid newsletterId, string[] NewsletterGroups, bool sendToNewSubscribers)
        {
            int AvailableMailings = -1;
            if (SessionObject.CurrentLicense.MaxNumberOfNewsletterMailings != null)
            {
                AvailableMailings = (int)SessionObject.CurrentLicense.MaxNumberOfNewsletterMailings - BaseCollection<NewsletterMailing>.Get("FK_Site = '" + SessionObject.CurrentSite.ID.ToString() + "'").Count();
            }
             
            string groupIDs = "";
            foreach (string groupID in NewsletterGroups)
            {
                groupIDs += "'" + groupID + "', ";
            }
            groupIDs = groupIDs.Substring(0, groupIDs.Length - 2);
            string where = " EXISTS (SELECT * FROM NewsletterGroupSubscriber WHERE `UnsubscribeDate` IS NULL AND FK_NewsletterSubscriber = NewsletterSubscriber.ID AND FK_NewsletterGroup IN (" + groupIDs + ") AND Confirmed = 1)";
            if (sendToNewSubscribers)
            {
                where = " EXISTS (SELECT * FROM NewsletterGroupSubscriber WHERE `UnsubscribeDate` IS NULL AND FK_NewsletterSubscriber = NewsletterSubscriber.ID AND FK_NewsletterGroup IN (" + groupIDs + ") AND Confirmed = 1 AND NOT EXISTS(SELECT * FROM newslettermailing WHERE FK_Newsletter = '" + newsletterId.ToString() + "' AND FK_NewsletterSubscriber = NewsletterSubscriber.ID))";
            }
            BaseCollection<NewsletterSubscriber> subscribers = BaseCollection<NewsletterSubscriber>.Get(where);
            if (AvailableMailings != -1 && subscribers.Count() > AvailableMailings)
            {
                throw new Exception("Om de nieuwsbrief naar deze maillijst te versturen zijn: " + subscribers.Count().ToString() + " mailingen vereist. \r\n U heeft nog: " + AvailableMailings.ToString() + " mailingen beschikbaar.");
            }
            return subscribers;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SendNewsletterToSubscriber(Guid newsletterId, NewsletterSubscriber obj)
        {
            Newsletter newsletter = BaseObject.GetById<Newsletter>(newsletterId);
            newsletter.Send(obj);
            Thread.Sleep(1000); //Wacht een seconde.
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SendNewsletter(Guid newsletterId, string[] NewsletterGroups, bool sendToNewSubscribers)
        {
            string groupIDs = "";
            foreach (string groupID in NewsletterGroups)
            {
                groupIDs += "'" + groupID + "', ";
            }
            groupIDs = groupIDs.Substring(0, groupIDs.Length - 2);
            string where = " EXISTS (SELECT * FROM NewsletterGroupSubscriber WHERE `UnsubscribeDate` IS NULL AND FK_NewsletterSubscriber = NewsletterSubscriber.ID AND FK_NewsletterGroup IN (" + groupIDs + "))";

            BaseCollection<NewsletterSubscriber> EmailAddressen = BaseCollection<NewsletterSubscriber>.Get(where);
            //BaseObject.GetById<Newsletter>(newsletterId).Send(EmailAddressen, sendToNewSubscribers);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult SaveSubscriber(NewsletterSubscriber obj)
        {
            obj.Site = SessionObject.CurrentSite;
            foreach (NewsletterGroup group in LoadNewsletterGroupList("Name ASC"))
            {
                if (group.IsMandatoryGroup && !obj.SubscribedGroups.Contains(group))
                {
                    obj.SubscribedGroups.Add(group);
                }
            }
            obj.Save();
            return JsonResult.CreateResult(true);

            /* NewsletterSubscriber Subscriber = BaseObject.GetFirst<NewsletterSubscriber>("Name = '" + OldEmail + "'");
            if (Subscriber != null)
            {
                Subscriber.EmailAdres = Email;
                Subscriber.Save();
                Guid NewsletterID = Guid.Parse(NewsLetterGroupId);
                return JsonResult.CreateResult(true, NewsletterGroup.GetById<NewsletterGroup>(NewsletterID).Subscribers);
            } */
            //return JsonResult.CreateResult(false, "Unable to find subscriber.");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult DeleteSubscriber(Guid id)
        {
            bool result = false;
            if (id != null)
            {
                NewsletterSubscriber subscriber = BaseObject.GetById<NewsletterSubscriber>(id);
                subscriber.Delete();
                result = true;
            }
            return JsonResult.CreateResult(result);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult DeleteImportDefinition(Guid id)
        {
            bool result = false;
            if (id != null)
            {
                NewsletterImportDefinition definition = BaseObject.GetById<NewsletterImportDefinition>(id);
                definition.Delete();
                result = true;
            }
            return JsonResult.CreateResult(result);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<NewsletterSubscriber> ListAllSubscribers(string NewsLetterId)
        {
            Newsletter Letter = Newsletter.GetById<Newsletter>(Guid.Parse(NewsLetterId));
            List<NewsletterSubscriber> Subscribers = new List<NewsletterSubscriber>();
            foreach (NewsletterGroup Group in Letter.Groups)
            {
                foreach (NewsletterSubscriber Subscriber in Group.Subscribers)
                {
                    Subscribers.Add(Subscriber);
                }
            }
            return Subscribers;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string[] GetOptInTemplateTags()
        {
            List<string> Tags = new List<string>();
            Tags.Add("[NAME]");
            Tags.Add("[FORENAME]");
            Tags.Add("[NAMEPREFIX]");
            Tags.Add("[USERCODE]"); //= guid kan worden gebruikt in plaats van [OPTIONURL] */

            Tags.Add("[NEWSGROUPS]");
            Tags.Add("[OPTINURL]"); // wordt http://sitedomein/[optinpagina].aspx?subscriber=[guid]
            Tags.Add("[OPTINLINK][/OPTINLINK]");

            return Tags.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public NewsletterImportDefinition SetDelimiter(NewsletterImportDefinition obj)
        {
            return obj;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string[] SetIsFirstRowColomnName(NewsletterImportDefinition obj)
        {

            if (obj.FileName != null)
            {
                //De kolom namen opvragen als het true is, kolom namen maken als het false is.
                StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~/_temp/upload/csv/") + obj.FileName);

                int columns = 0;
                string[] columnNames = null;

                if (obj.Delimiter != null)
                {
                    if (obj.FirstRowIsColumnName == true)
                    {

                        columnNames = sr.ReadLine().Split(new string[] { obj.Delimiter }, StringSplitOptions.None);

                        return columnNames;
                    }
                    else
                    {
                        columnNames = sr.ReadLine().Split(new string[] { obj.Delimiter }, StringSplitOptions.None);
                        columns = columnNames.Count();

                        for (int i = 0; i < columns; i++)
                        {
                            columnNames[i] = "Kolom" + i;
                        }
                        return columnNames;
                    }

                }
            }
            return null;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string[] ReadFile(NewsletterImportDefinition obj)
        {

            // if (Messages == null) Messages = new List<ChatMessage>();
            // Messages.Add(obj);
            // File.WriteAllLines(@"C:\Users\emiel\Desktop\clark jquery\berichten.json", lines);
            // string output = JSONSerializer.Serialize(Messages);

            // TextWriter writer = new StreamWriter(Server.MapPath("")+"/berichten.txt", false);
            // writer.Write();
            // writer.Write(output);
            // writer.Close();
            // writer.Dispose();

            // return obj;
            // List<ImportDefinition> Messages = new List<ImportDefinition>();
            // List<string> ExampleRows = new List<string>();
            string[] examplerows = null;

            if (obj.FileExtension.ToLower() == ".csv" && obj.Delimiter != null && obj.FileName != null)
            {
                StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~/_temp/upload/csv/") + obj.FileName);

                string RecordLine = sr.ReadLine();
                ArrayList Lines = new ArrayList();
                while (RecordLine != null && Lines.Count < 2)
                {
                    Lines.Add(RecordLine);
                    RecordLine = sr.ReadLine();
                }

                if (obj.FirstRowIsColumnName == true)
                {

                    examplerows = Regex.Split(Lines[1].ToString(), obj.Delimiter);

                    sr.Close();
                    return examplerows;
                }
                else
                {
                    examplerows = Regex.Split(Lines[0].ToString(), obj.Delimiter);

                    sr.Close();
                    return examplerows;
                }
            }
            else
            {
                return null;
            }
        }
        //'<tr style="color:red;"><td>' + value[0] + '/' + value[4] + ':</td><td>' + value[2] + '</td><td>: ' + value[3] + '</td></tr>
        
        
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult ImportSubscribers(NewsletterImportDefinition obj)
        {
            
            Dictionary<string, object> returnObject = new Dictionary<string, object>();

            NewsletterImportDefinition importDef = obj;
            
            List<string> log = new List<string>();
            log = importDef.Validate();

            if (log.Count == 0)
            {
                if (importDef.SaveDefinition == true)
                {
                    importDef.Site = SessionObject.CurrentSite;
                    importDef.IsNew = (importDef.ID == Guid.Empty);
                    importDef.Save();
                }

                if (importDef.DeleteGroupSubscribers == true)
                {
                    JsonResult result = DeleteSubscriberFromGroup(importDef.Groups);
                    if (result.Success == true) importDef.EmptyGroups = false;
                }

                if (importDef.EmptyGroups == true)
                {
                    foreach (NewsletterGroup group in importDef.Groups)
                    {
                        group.Load();
                        if (group.IsMandatoryGroup != true)
                        {
                            group.Subscribers.Clear();
                            group.Save();
                        }
                    }
                }

                log = importDef.StartImport();

                string importLogFile = importDef.MakeErrorLogFile(log);
                
                returnObject.Add("ImportLogFile", importLogFile);
                returnObject.Add("Log", log);

               
            }

            return JsonResult.CreateResult(true, "SUCCES", returnObject);
            

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<NewsletterImportDefinition> GetChoosableImportDefinitions(string FileExtension)
        {
            List<TreeGridItem> returnList = new List<TreeGridItem>();
            string Where = string.Format("FK_Site='{0}' AND FileExtension LIKE '{1}' ", SessionObject.CurrentSite.ID, FileExtension.ToString());
            BaseCollection<NewsletterImportDefinition> List = BaseCollection<NewsletterImportDefinition>.Get(Where, "Name");

            foreach (NewsletterImportDefinition definition in List)
            {
                TreeGridItem def = new TreeGridItem();
                def.ID = definition.ID;
                def.Field1 = definition.Name;

                returnList.Add(def);
            }
            return List;
        }

        //private string[] CreateSubscriber(string[] data, NewsletterImportDefinition obj, int[] importProgress)//BaseCollection<NewsletterGroup> newsGroups
        //{
        //    string[] log = new string[6];

        //    BaseCollection<NewsletterSubscriber> subscribers = BaseCollection<NewsletterSubscriber>.Get("FK_Site = '" + SessionObject.CurrentSite.ID.ToString() + "' AND Email = '" + data[obj.EmailColumnNo].Trim() + "'");
        //    NewsletterSubscriber subscriber;

        //    var existingSubscriber = subscribers.Where(b => b.Email == data[obj.EmailColumnNo]).FirstOrDefault();//Where(b => b.Name == data[obj.NameColumnNo]).FirstOrDefault();

        //    if (existingSubscriber == null)
        //    {
        //        if (EmailManager.isValidEmailAddress(data[obj.EmailColumnNo].Trim()))
        //        {
        //            if (isValidDataLength(obj, data) == true)
        //            {
        //                subscriber = new NewsletterSubscriber();
        //                subscriber.Site = SessionObject.CurrentSite;
        //                subscriber.Email = data[obj.EmailColumnNo].Trim();
        //                subscriber.Confirmed = (obj.AutoConfirm);
        //                if (obj.ForeNameColumn != "" && obj.ForeNameColumnNo != 9999) subscriber.ForeName = data[obj.ForeNameColumnNo];
        //                if (obj.NameColumn != "" && obj.NameColumnNo != 9999) subscriber.Name = data[obj.NameColumnNo];
        //                if (obj.NamePrefixColumn != "" && obj.NamePrefixColumnNo != 9999) subscriber.NamePrefix = data[obj.NamePrefixColumnNo];
        //                //subscriber.Gender = data[obj.GenderCoumnNo] // ERROR: cannot convert string/int > sexeEnum
        //                subscriber.SubscribedGroups.AddRange(obj.Groups);
        //                subscriber.Save();

        //                log[0] = importProgress[0].ToString();

        //                if (obj.NameColumnNo != 9999) log[1] = data[obj.NameColumnNo];
        //                log[2] = data[obj.EmailColumnNo];
        //                log[3] = "Geimporteerd";
        //                log[4] = importProgress[1].ToString();
        //                log[5] = true.ToString();
        //            }
        //            else
        //            {
        //                string tooLong = "Het ";

        //                if (obj.EmailColumnNo != 9999)
        //                {
        //                    if (data[obj.EmailColumnNo].ToString().Length > 50)
        //                    {
        //                        tooLong += "email adres, ";
        //                    }
        //                }
        //                if (obj.NameColumnNo != 9999)
        //                {
        //                    if (data[obj.NameColumnNo].ToString().Length > 150)
        //                    {
        //                        tooLong += "achternaam, ";
        //                    }
        //                }
        //                if (obj.ForeNameColumnNo != 9999)
        //                {
        //                    if (data[obj.ForeNameColumnNo].ToString().Length > 250)
        //                    {
        //                        tooLong += "voornaam, ";
        //                    }
        //                }
        //                if (obj.NamePrefixColumnNo != 9999)
        //                {
        //                    if (data[obj.NamePrefixColumnNo].ToString().Length > 25)
        //                    {
        //                        tooLong += "tussenvoegsel, ";
        //                    }
        //                }
        //                log[0] = importProgress[0].ToString();
        //                if (obj.NameColumnNo != 9999) log[1] = data[obj.NameColumnNo];
        //                log[2] = data[obj.EmailColumnNo];
        //                log[3] = tooLong += "is te lang";
        //                log[4] = importProgress[1].ToString();
        //                log[5] = false.ToString();
        //            }
        //        }
        //        else
        //        {
        //            log[0] = importProgress[0].ToString();
        //            if (obj.NameColumnNo != 9999) log[1] = data[obj.NameColumnNo];
        //            log[2] = data[obj.EmailColumnNo];
        //            log[3] = "Geen geldig email adres";
        //            log[4] = importProgress[1].ToString();
        //            log[5] = false.ToString();
        //        }
        //    }
        //    else if (existingSubscriber != null && obj.SkipDoubleRecords == false)
        //    {
        //        if (EmailManager.isValidEmailAddress(data[obj.EmailColumnNo].Trim()) && isValidDataLength(obj, data) == true)
        //        {
        //            if (obj.AppendGroups == true)
        //            {
        //                for (int i = 0; i < existingSubscriber.SubscribedGroups.Count(); i++)
        //                {
        //                    for (int j = 0; j < obj.Groups.Count(); j++)
        //                    {
        //                        if (obj.Groups[j].ID == existingSubscriber.SubscribedGroups[i].ID)
        //                        {
        //                            obj.Groups.RemoveAt(j);
        //                            break;

        //                        }
        //                    }
        //                }
        //            }

        //            /* subscriber = new NewsletterSubscriber();
        //               subscriber.Site = SessionObject.CurrentSite;
        //               subscriber.Email = data[obj.EmailColumnNo].Trim();
        //               subscriber.Confirmed = (obj.AutoConfirm);
        //               subscriber.ForeName = data[obj.ForeNameColumnNo];
        //               subscriber.Name = data[obj.NameColumnNo];
        //               subscriber.NamePrefix = data[obj.NamePrefixColumnNo];
        //               //subscriber.Gender = data[obj.GenderColumnNo]; // ERROR: cannot convert string/int > sexeEnum
        //               subscriber.SubscribedGroups.AddRange(obj.Groups);
        //               subscriber.Save();*/

        //            existingSubscriber.Site = SessionObject.CurrentSite;
        //            existingSubscriber.Confirmed = (obj.AutoConfirm);

        //            if (obj.ForeNameColumn != "" && obj.ForeNameColumnNo != 9999)
        //            {
        //                existingSubscriber.ForeName = data[obj.ForeNameColumnNo];
        //            }
        //            else { existingSubscriber.ForeName = ""; }

        //            if (obj.NameColumn != "" && obj.NameColumnNo != 9999)
        //            {
        //                existingSubscriber.Name = data[obj.NameColumnNo];
        //            }
        //            else { existingSubscriber.Name = ""; }

        //            if (obj.NamePrefixColumn != "" && obj.NamePrefixColumnNo != 9999)
        //            {
        //                existingSubscriber.NamePrefix = data[obj.NamePrefixColumnNo];
        //            }
        //            else { existingSubscriber.NamePrefix = ""; }

        //            if (obj.AppendGroups == false) existingSubscriber.SubscribedGroups.Clear();

        //            if (existingSubscriber.SubscribedGroups != obj.Groups)
        //                existingSubscriber.SubscribedGroups.AddRange(obj.Groups);

        //            existingSubscriber.Save();

        //            log[0] = importProgress[0].ToString();
        //            if (obj.NameColumnNo != 9999) log[1] = data[obj.NameColumnNo];
        //            log[2] = data[obj.EmailColumnNo];
        //            log[3] = "Abonnee bestaat al. overschreven";
        //            log[4] = importProgress[1].ToString();
        //            log[5] = true.ToString();
        //        }
        //        else if (isValidDataLength(obj, data) == false)
        //        {
        //            string tooLong = "Het ";

        //            if (obj.EmailColumnNo != 9999)
        //            {
        //                if (data[obj.EmailColumnNo].ToString().Length > 50)
        //                {
        //                    tooLong += "email adres, ";
        //                }
        //            }
        //            if (obj.NameColumnNo != 9999)
        //            {
        //                if (data[obj.NameColumnNo].ToString().Length > 150)
        //                {
        //                    tooLong += "achternaam, ";
        //                }
        //            }
        //            if (obj.ForeNameColumnNo != 9999)
        //            {
        //                if (data[obj.ForeNameColumnNo].ToString().Length > 250)
        //                {
        //                    tooLong += "voornaam, ";
        //                }
        //            }
        //            if (obj.NamePrefixColumnNo != 9999)
        //            {
        //                if (data[obj.NamePrefixColumnNo].ToString().Length > 25)
        //                {
        //                    tooLong += "tussenvoegsel, ";
        //                }
        //            }
        //            log[0] = importProgress[0].ToString();
        //            if (obj.NameColumnNo != 9999) log[1] = data[obj.NameColumnNo];
        //            log[2] = data[obj.EmailColumnNo];
        //            log[3] = tooLong += "is te lang";
        //            log[4] = importProgress[1].ToString();
        //            log[5] = false.ToString();
        //        }
        //        else
        //        {
        //            log[0] = importProgress[0].ToString();
        //            if (obj.NameColumnNo != 9999) log[1] = data[obj.NameColumnNo];
        //            log[2] = data[obj.EmailColumnNo];
        //            log[3] = "Geen geldig email adres";
        //            log[4] = importProgress[1].ToString();
        //            log[5] = false.ToString();
        //        }
        //    }
        //    else
        //    {
        //        log[0] = importProgress[0].ToString();
        //        if (obj.NameColumnNo != 9999) log[1] = data[obj.NameColumnNo];
        //        log[2] = data[obj.EmailColumnNo];
        //        log[3] = "Dubbele abonnee is overgeslagen";
        //        log[4] = importProgress[1].ToString();
        //        log[5] = true.ToString();
        //    }
        //    return log;

        //}

        public void SendVerificationEmail(NewsletterSubscriber subscriber)
        {
            CmsSite site = SessionObject.CurrentSite;
            string content = ReplaceVerificationEmailTags(site.NewsletterOptInEmailContent, subscriber);
            //content = content.Replace("[OPTINURL]", site.DomainName + "/" + site.NewsletterOptInEmailPage.LastPublishedUrl + "?subsciber=" + subscriber.ID.ToString());
            EmailManager.SendMail(site.NewsletterSender, subscriber.Email, site.NewsletterOptInEmailSubject, content, true);
        }

        private string ReplaceVerificationEmailTags(string content, NewsletterSubscriber subscriber)
        {
            CmsSite site = SessionObject.CurrentSite;
            string newsgroups = "";
            foreach (NewsletterGroup group in subscriber.SubscribedGroups.Where(c => c.IsMandatoryGroup == false))
            {
                newsgroups += group.Name + ", ";
            }
            if (newsgroups != "") newsgroups = newsgroups.Substring(0, newsgroups.Length - 2);
            string optinUrl = site.DomainName + "/" + site.NewsletterOptInEmailPage.RelativeUrl + "?subscriber=" + subscriber.ID.ToString();
            content = content.Replace("[OPTINURL]", optinUrl)
                .Replace("[NAME]", subscriber.Name)
                .Replace("[FORENAME]", subscriber.ForeName)
                .Replace("[NAMEPREFIX]", subscriber.NamePrefix)
                .Replace("[USERCODE]", subscriber.ID.ToString())
                .Replace("[NEWSGROUPS]", newsgroups)
                .Replace("[OPTINLINK]", "<a href=\"" + optinUrl + "\">").Replace("[/OPTINLINK]", "</a>");
            return content;
        }

        private JsonResult DeleteSubscriberFromGroup(BaseCollection<NewsletterGroup> groups)
        {
            //int aantal;
            if (groups != null)
            {
                foreach (NewsletterGroup group in groups)
                {
                    //aantal = group.Subscribers.Count();
                    foreach (NewsletterSubscriber subscriber in group.Subscribers)
                    {
                        if (subscriber.RegistrationType == RegistrationTypeEnum.Import)
                        {
                            subscriber.Delete();
                        }
                    }
                    //group.Subscribers.DeleteAll();
                }
                return JsonResult.CreateResult(true);
            }
            else
            {
                return JsonResult.CreateResult(false);
            }
        }

        

        
    }
}
