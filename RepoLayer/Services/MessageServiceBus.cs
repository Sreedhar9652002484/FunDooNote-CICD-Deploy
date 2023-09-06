using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Services
{
    public class MessageServiceBus
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ServiceBusSender _sender;
        private readonly IConfiguration _configuration;

        public MessageServiceBus(ServiceBusClient serviceBusClient, ServiceBusSender sender, IConfiguration configuration)
        {
            _serviceBusClient = serviceBusClient;
            _sender = sender;
            _configuration = configuration;
        }

        public async Task SendMessageToQueueAsync(string email, string token)
        {
            string messgeBody=$"Token:{token}";
            ServiceBusMessage message = new ServiceBusMessage();
            message.Subject = "Reset Password Token";
            message.To = email;
            message.Body=BinaryData.FromString(messgeBody);

            _sender.SendMessageAsync(message).Wait();
            ConsumeMessageFromQueue();
        }

        public void ConsumeMessageFromQueue()
        {
            string connectionString = _configuration["AzureServiceBus:AzureConnection"];
            string queueName = "password-reset-queue";

            ServiceBusProcessor processor=_serviceBusClient.CreateProcessor(queueName, new ServiceBusProcessorOptions());
            processor.ProcessMessageAsync += async args =>
            {
                string messageBody = Encoding.UTF8.GetString(args.Message.Body);
                string email = args.Message.Subject;

                sendEmail(email, messageBody);
                await args.CompleteMessageAsync(args.Message);
            };

            processor.ProcessErrorAsync += args =>
            {
                return Task.CompletedTask;
            };

            processor.StartProcessingAsync().Wait();

        }
        public void sendEmail(string email, string message)
        {
            var SMTP = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("nsridhary2k@gmail.com", "jtlicbixeacrsept"),
                EnableSsl = true
            };
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(email); 
            mailMessage.To.Add(email);
            mailMessage.Subject = "Subject of the Email";
            mailMessage.Body = message;

            try
            {
                SMTP.Send(mailMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
