using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using BitPlate.Domain.Utils;
using System.IO;

namespace BitSite._bitPlate._bitSystem
{
    public class SessionProvider : SessionStateStoreProviderBase
    {
        private string SessionStorePath = HttpContext.Current.Server.MapPath("") + "\\App_data\\Sessions\\";
        private string ApplicationName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;


        public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
        {
            return new SessionStateStoreData(new SessionStateItemCollection(), SessionStateUtility.GetSessionStaticObjects(context), timeout);
        }

        public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
        {
            //This method is not utilized
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override void EndRequest(HttpContext context)
        {
            throw new NotImplementedException();
        }

        public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            return GetSessionStoreItem(false, context, id, out locked, out lockAge, out lockId, out actions);
            //throw new NotImplementedException();
        }

        public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            return GetSessionStoreItem(true, context, id, out locked, out lockAge, out lockId, out actions);
            //throw new NotImplementedException();
        }

        public override void InitializeRequest(HttpContext context)
        {
    
        }

        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
        {
            string fileName = this.GetSessionFileName(id);
            SessionData sessionData = SessionData.ReadFromFile(fileName);
            if (sessionData != null && sessionData.SessionId == id && sessionData.ApplicationName == this.ApplicationName && sessionData.LockId == (int)lockId)
            {
                sessionData.Expires = DateTime.Now.AddMinutes(60); //TimeOut van een uur
                sessionData.Save();
            }
        }

        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
        {
            string fileName = this.GetSessionFileName(id);
            SessionData sessionData = SessionData.ReadFromFile(fileName);
            if (sessionData != null && sessionData.SessionId == id && sessionData.ApplicationName == this.ApplicationName && sessionData.LockId == (int)lockId)
            {
                sessionData.Delete();
            }
        }

        public override void ResetItemTimeout(HttpContext context, string id)
        {
            string fileName = this.GetSessionFileName(id);
            SessionData sessionData = SessionData.ReadFromFile(fileName);
            if (sessionData != null && sessionData.SessionId == id && sessionData.ApplicationName == this.ApplicationName)
            {
                sessionData.Expires = DateTime.Now.AddMinutes(60); //60minuten timeout
            }
        }

        public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
        {
            string fileName = this.GetSessionFileName(id);
            string sessionItemString = Serialize((SessionStateItemCollection)item.Items);
            SessionData sessionData = SessionData.ReadFromFile(fileName);
            if (newItem)
            {
                if (sessionData != null && sessionData.ApplicationName == this.ApplicationName && sessionData.Expires < DateTime.Now)
                {
                    sessionData.Delete();

                    sessionData = new SessionData();
                    sessionData.SessionId = id;
                    sessionData.ApplicationName = this.ApplicationName;
                    sessionData.Created = DateTime.Now;
                    sessionData.Expires = DateTime.Now.AddMinutes(item.Timeout);
                    sessionData.LockDate = DateTime.Now;
                    sessionData.Locked = true;
                    sessionData.LockId = 0;
                    sessionData.SessionItems = sessionItemString;
                    sessionData.Flags = 0;
                    sessionData.Timeout = item.Timeout;
                    sessionData.Save();
                }
            }
            else
            {
                if (sessionData != null && sessionData.SessionId == id && sessionData.ApplicationName == this.ApplicationName && sessionData.LockId == (int)lockId && sessionData.Expires > DateTime.Now)
                {
                    sessionData.Expires = DateTime.Now.AddMinutes(item.Timeout);
                    sessionData.SessionItems = sessionItemString;
                    sessionData.Locked = true;
                    sessionData.Save();
                }
            }
        }

        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
        {
            return false;
        }

        private SessionStateStoreData GetSessionStoreItem(bool lockRecord, HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actionFlags)
        {
            SessionStateStoreData item = null;
            lockAge = TimeSpan.Zero;
            lockId = null;
            locked = false;
            actionFlags = 0;

            string sessionFile = this.GetSessionFileName(id);
            string sessionLines = this.GetSessionLinesFromFile(sessionFile);
            if (sessionLines != "")
            {
                SessionData sessionData = JSONSerializer.Deserialize<SessionData>(sessionLines);

                lockAge = DateTime.Now.Subtract(sessionData.LockDate);
                lockId = sessionData.LockId;
                actionFlags = (SessionStateActions)sessionData.Flags;
                

                if (lockRecord)
                {
                    sessionData.Locked = true;
                    sessionData.LockDate = DateTime.Now;
                    sessionData.LockId = 0;
                    sessionData.SessionId = id;
                    sessionData.ApplicationName = this.ApplicationName;
                    sessionData.Save();
                    locked = true;
                }

                if (sessionData.Expires <= DateTime.Now)
                {
                    sessionData.Locked = false;
                    sessionData.Delete();
                }

                if (sessionData.Locked)
                {
                    sessionData.LockId++;
                    sessionData.SessionId = id;
                    sessionData.ApplicationName = ApplicationName;

                    if ((SessionStateActions)sessionData.Flags == SessionStateActions.InitializeItem)
                    {
                        item = CreateNewStoreData(context, 60);//timeout van een uur
                    }
                    else {
                        item = Deserialize(context, sessionData.SessionItems, 60);
                    }
                }
            }

            return item;
        }

        private string GetSessionFileName(string sessionId)
        {
            return this.SessionStorePath + "Session_" + this.ApplicationName + "_" + sessionId + ".session";
        }

        private string GetSessionLinesFromFile(string file)
        {
            string returnValue = "";
            if (File.Exists(file))
            {
                StreamReader sr = new StreamReader(file);
                returnValue = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
            }
            return returnValue;
        }

        private SessionStateStoreData Deserialize(HttpContext context, string serializedItems, int timeout)
        {

            MemoryStream ms = new MemoryStream(Convert.FromBase64String(serializedItems));

            SessionStateItemCollection sessionItems = new SessionStateItemCollection();

            if (ms.Length > 0)
            {
                BinaryReader reader = new BinaryReader(ms);
                sessionItems = SessionStateItemCollection.Deserialize(reader);
            }

            return new SessionStateStoreData(sessionItems, SessionStateUtility.GetSessionStaticObjects(context), timeout);
        }

        public string Serialize(SessionStateItemCollection items)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);

            if ((items != null))
                items.Serialize(writer);

            writer.Close();

            return Convert.ToBase64String(ms.ToArray());
        }
    }

  //  SessionId       Text(80)  NOT NULL,
  //ApplicationName Text(255) NOT NULL,
  //Created         DateTime  NOT NULL,
  //Expires         DateTime  NOT NULL,
  //LockDate        DateTime  NOT NULL,
  //LockId          Integer   NOT NULL,
  //Timeout         Integer   NOT NULL,
  //Locked          YesNo     NOT NULL,
  //SessionItems    Memo,
  //Flags           Integer   NOT NULL,

    public class SessionData
    {
        public string SessionFile { get; set; }

        public string SessionId { get; set; }
        public string ApplicationName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public DateTime LockDate { get; set; }
        public int LockId { get; set; }
        public int Timeout { get; set; }
        public bool Locked { get; set; }
        public string SessionItems { get; set; }
        public int Flags { get; set; }

        public void Save()
        {
            string sessionDataLines = JSONSerializer.Serialize(this);
            this.SaveLinesToFile(this.SessionFile, sessionDataLines);
        }

        private void SaveLinesToFile(string file, string lines)
        {
            StreamWriter sw = new StreamWriter(file);
            sw.Write(lines);
            sw.Close();
            sw.Dispose();
        }

        public void Delete()
        {
            if (!this.Locked)
            {
                File.Delete(this.SessionFile);
            }
        }

        public static SessionData ReadFromFile(string file)
        {
            string returnValue = "";
            if (File.Exists(file))
            {
                StreamReader sr = new StreamReader(file);
                returnValue = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
            }

            if (returnValue != "")
            {
                return JSONSerializer.Deserialize<SessionData>(returnValue);
            }
            else
            {
                return null;
            }
        }
    }
}