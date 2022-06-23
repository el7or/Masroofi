using FirebaseAdmin.Messaging;
using log4net;
using Newtonsoft.Json;
using Puzzle.Masroofi.SendNotification.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.SendNotification.Manager
{
    public class NotificationManager
    {
        private ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public NotificationManager()
        { }
        public void Send(DataForSend dataForSend)
        {
            var message = new Message()
            {
                Data = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataForSend.NotificationData),
                Token = dataForSend.RegisterId,
                //Token = "cezuGFY5Qk-3tlSsiTqvX2:APA91bHcDUhgkd0mJeAxVRn7t4GqKsMedY7-iXYqwIObN3zG59g4vM5kJgrXHYz1OHqy4-oIn3g06-MmVN7WJ0BXcDQDKKDSL4vhXuAtMtFz0K4NovGkwYYJMO8trULZww9lOXHahLYj"
                //Notification = new Notification
                //{
                //    Title = dataForSend.Title,
                //    Body = dataForSend.NotificationText
                //}
            };

            try
            {
                string response = FirebaseMessaging.DefaultInstance.SendAsync(message).GetAwaiter().GetResult();

                log.Info("Data Sent: Message: ( " + JsonConvert.SerializeObject(message) + " ), Result: ( " + response + " )");
            }
            catch (Exception ex)
            {
                log.Error("Data Failed: Message: ( " + JsonConvert.SerializeObject(message) + " )", ex);
            }
        }
    }
}
