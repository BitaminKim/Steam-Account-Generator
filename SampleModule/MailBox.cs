using MailKit.Net.Imap;
using MimeKit;
using SACModuleBase.Attributes;
using SACModuleBase.Models;
using SACModuleBase.Models.Mail;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SampleModule
{
    [SACModuleInfo("C89C87D9-D9FE-447B-85A6-92203F0C3397", "IMAP mail handler", "1.0.0.0")]
    public class MailBox : SACModuleBase.ISACHandlerMailBox
    {
        public bool ModuleEnabled { get; set; } = true;

        private ConfigManager<Models.MailConfig> Config;
        private SACModuleBase.ISACLogger Logger;
        public void ModuleInitialize(SACInitialize initialize)
        {
            Config = new ConfigManager<Models.MailConfig>(initialize.ConfigurationPath, "mail.json",
                new Models.MailConfig(), Misc.MailBoxConfigSync);
            if (!Config.Load())
                Config.Save();

            Logger = initialize.Logger;
        }

        public MailBoxResponse GetMailBox(MailBoxRequest request)
        {
            if (!Config.Load())
            {
                Config.Save();
                return null;
            }
            return new MailBoxResponse(Config.Running.Email);
        }

        public IReadOnlyCollection<MailBoxMailItem> GetMails(MailBoxResponse response)
        {
            Logger.Info($"Getting mails from {response.Email}...");
            var mails = new List<MailBoxMailItem>();
            if (Config.Load())
            {
                using (var client = new ImapClient())
                {
                    try
                    {
                        Logger.Info("Connecting to IMAP...");
                        client.Connect(Config.Running.Host, Config.Running.Port, Config.Running.UseSsh);

                        Logger.Info("Authenticating on IMAP...");
                        client.Authenticate(Config.Running.Email, Config.Running.Password);

                        var inbox = client.Inbox;
                        Logger.Info("Getting inbox...");
                        inbox.Open(MailKit.FolderAccess.ReadWrite);

                        var unreadSort = inbox.Sort(MailKit.Search.SearchQuery.NotSeen, new[] { MailKit.Search.OrderBy.Date, MailKit.Search.OrderBy.Subject });
                        var unread = inbox.Fetch(unreadSort, MailKit.MessageSummaryItems.All);
                        foreach (var inboxMail in unread)
                        {
                            Logger.Info("Unread message found...");
                            inbox.SetFlags(inboxMail.Index, MailKit.MessageFlags.Seen, true);
                            if (!(inboxMail?.NormalizedSubject?.ToLower()?.Contains("steam") ?? false))
                                continue;

                            var envelope = inboxMail?.Envelope;
                            var _body = inboxMail?.HtmlBody ?? inboxMail?.HtmlBody;
                            var textPart = inbox.GetBodyPart(inboxMail.Index, _body) as TextPart;

                            var mail = new MailBoxMailItem(envelope?.From?.Mailboxes?.FirstOrDefault()?.Address ?? "noreply@steampowered.com",
                                    envelope?.To?.Mailboxes?.FirstOrDefault()?.Address ?? response?.Email ?? "unknown",
                                    inboxMail?.NormalizedSubject ?? "none",
                                    textPart?.Text ?? "none");
                            mails.Add(mail);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Error getting mails...", ex);
                    }
                }
                Logger.Info($"Getting mails from {response.Email} success!");
            }
            return new ReadOnlyCollection<MailBoxMailItem>(mails);
        }
    }
}
