using CommonLayer.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

using RepoLayer.Context;
using RepoLayer.Entity;
using RepoLayer.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;

namespace RepoLayer.Services
{
    public class UserRepo : IUserRepo
    {
        private readonly FunDoContext funDoContext;
        private readonly IConfiguration configuration;
        private readonly RabbitMQPublisher _rabbitMQPublisher;
        private readonly MessageServiceBus _messageBus;

        public UserRepo(FunDoContext funDoContext, IConfiguration configuration, RabbitMQPublisher rabbitMQPublisher,MessageServiceBus messageServiceBus)
        {
            this.funDoContext = funDoContext;
            this.configuration = configuration;
            this._rabbitMQPublisher = rabbitMQPublisher;   
            this._messageBus = messageServiceBus;   
        }

        //This is the UserReg method implementation using UserRegModel
        public UserEntity UserReg(UserRegModel model)
        {
            try
            {
                UserEntity userEntity = new UserEntity();
                userEntity.FirstName = model.FirstName;
                userEntity.LastName = model.LastName;
                userEntity.DateOfBirth = model.DateOfBirth;
                userEntity.Email = model.Email;
                userEntity.Password = model.Password;

                funDoContext.User.Add(userEntity);
                funDoContext.SaveChanges();
                if (userEntity != null)
                {
                    /*var message = new UserRegistrationMessage { Email = userEntity.Email };
                    var messageJson = JsonConvert.SerializeObject(message);
                    _rabbitMQPublisher.PublishMessage("User-Registration-Queue", messageJson);
                    // Example of sending a message to the RabbitMQ queue
                    // Print a message to the console to verify
                   // Console.WriteLine($"Message sent to queue: {messageJson}");*/

                    return userEntity;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //This is the UserLogin method implementation using UserLoginResultModel
        public string UserLogin(UserLoginModel model)
        {
            try
            {
                var userEntity = funDoContext.User.FirstOrDefault(u => u.Email == model.Email&&u.Password==model.Password);


                if (userEntity != null)

                {
                   // var result = userEntity.Password;
                    var JWTToken = GenerateJwtToken(userEntity.Email, userEntity.UserId);
                    return JWTToken;
                    
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;

        }
       



        ///Generate Token JWTMethod
        public string GenerateJwtToken(string Email, long UserId)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, Email),
                 new Claim("UserId", UserId.ToString()),
                // Add any other claims you want to include in the token
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(configuration["JwtSettings:Issuer"], configuration["JwtSettings:Audience"], claims, DateTime.Now, DateTime.Now.AddHours(1), creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //UserLoginResult class


        //This is the UserLogin method implementation using UserLoginResultModel

        public string forgetPassword(string email)
        {
            try
            {
                var emailValidity = funDoContext.User.FirstOrDefault(u => u.Email == email);
                if (emailValidity != null)
                {
                    var token = GenerateJwtToken(emailValidity.Email, emailValidity.UserId);

                    
                    _messageBus.SendMessageToQueueAsync(email, token);

/*                   MSMQ msmq = new MSMQ();
                    msmq.sendData2Queue(token);*/
                    //Use RabbitMQService to publish the token message
                  //  _rabbitMQPublisher.PublishMessage("password-reset-queue", token);
                    return token;

                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool ResetPassword(string Email, string NewPassword, string ConfirmPass)
        {
            try
            {
                if (NewPassword == ConfirmPass)
                {
                    var emailValid = funDoContext.User.FirstOrDefault(u => u.Email == Email);
                    if (emailValid != null)
                    {
                        emailValid.Password = ConfirmPass;
                        //emailValid.Password = EncryptService.EncryptConfirmPass);
                        funDoContext.User.Update(emailValid);
                        funDoContext.SaveChanges();

                        return true;
                    }
                }
                return false;

            }
            catch (Exception)
            {

                throw;
            }
        }


        public List<UserEntity> GetAllUsers()
        {
            try
            {
                List<UserEntity> userList = new List<UserEntity>();
                userList = funDoContext.User.ToList();
                return userList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UserEntity> GetAllUsersByID(int UserId)
        {
            try
            {

                var userEntity = funDoContext.User.FirstOrDefault(x => x.UserId == UserId);
                if (UserId == userEntity.UserId)
                {
                    List<UserEntity> userList = new List<UserEntity>();
                    userList.Add(userEntity);

                    return userList;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool DeleteUsers(long UserId)
        {
            try
            {
                var user = funDoContext.User.FirstOrDefault(u => u.UserId == UserId);
                if (user != null)
                {
                    funDoContext.User.Remove(user);
                    funDoContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


    }
}
