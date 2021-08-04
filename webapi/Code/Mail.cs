using API.Models;
using MailKit.Net.Smtp;
using MimeKit;
using SPAVIPackage.SSO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.Code {
	public class Mail {

        protected AppSettings _AppSettings;

        public Mail(AppSettings appSettings) {
            this._AppSettings = appSettings;
		}

        public SmtpInfo GetSMTP(SmtpInfo _oSmtpInfo) {
            if (_oSmtpInfo != null) {
                return _oSmtpInfo;
			}
            return new SmtpInfo() {
                host = this._AppSettings.smtp_settings.host,
                port = this._AppSettings.smtp_settings.port,
                useSSL = this._AppSettings.smtp_settings.useSSL,
                username = this._AppSettings.smtp_settings.username,
                password = this._AppSettings.smtp_settings.password
            };
		}

        // https://docs.microsoft.com/it-it/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
        public static bool IsValidEmail(string email) {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match) {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            } catch (RegexMatchTimeoutException e) {
                return false;
            } catch (ArgumentException e) {
                return false;
            }

            try {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            } catch (RegexMatchTimeoutException) {
                return false;
            }
        }

        public string SendMail(SmtpInfo _oSmtpInfo, string _szFrom, string _szFromName, string _szTo,
            List<string> _aszCCs, string _szSubject, string _szMessage, string _szTag, List<Attachments> _aoAttachments) {

            var message = new MimeMessage();
            if (string.IsNullOrEmpty(_szFromName)) {
                message.From.Add(MailboxAddress.Parse(_szFrom));
            } else {
                message.From.Add(new MailboxAddress(_szFromName, _szFrom));
            }

            message.To.Add(MailboxAddress.Parse(_szTo));
            if (_aszCCs != null) {
                foreach (string cc in _aszCCs) {
                    message.Cc.Add(MailboxAddress.Parse(cc));
                }
            }

            message.Subject = _szSubject;
            BodyBuilder builder = new BodyBuilder();
            builder.HtmlBody = _szMessage;
            if (_aoAttachments != null && _aoAttachments.Count > 0) {
                foreach (Attachments item in _aoAttachments) {
                    if (string.IsNullOrEmpty(item.filecontentBase64)) {
                        builder.Attachments.Add(item.filename, item.filecontent);
                    } else {
                        builder.Attachments.Add(item.filename, Convert.FromBase64String(item.filecontentBase64));
                    }
                }
            }

            message.Body = builder.ToMessageBody();

            if (!string.IsNullOrEmpty(_szTag)) {
                message.Headers.Add("X-Mailin-Tag", _szTag);
            }

            using (var client = new SmtpClient()) {
                
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(_oSmtpInfo.host, _oSmtpInfo.port, _oSmtpInfo.useSSL);
                
                if (!string.IsNullOrEmpty(_oSmtpInfo.username)) {
                    client.Authenticate(_oSmtpInfo.username, _oSmtpInfo.password);
                }
                client.Send(message);
                client.Disconnect(true);
            }
            return message.MessageId;
        }

    }
}
