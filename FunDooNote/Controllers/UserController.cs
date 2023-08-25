using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interface;
using CommonLayer.Model;
using RepoLayer.Services;
using RepoLayer.Entity;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System;
using RepoLayer.Context;

namespace FunDooNoteApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {

        private readonly IUserBusiness userBusiness;


		public UserController(IUserBusiness userBusiness)
        {
            this.userBusiness = userBusiness;
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Registration(UserRegModel model)
        {


            // Encrypt the password before storing it in the database
            //model.Password = EncryptService.Encrypt(model.Password);
            var result = userBusiness.UserReg(model);
            if (result != null)
            {
                return this.Ok(new { success = true, Message = "User Registration Successful", Data = result });
            }
            else
            {
                return this.BadRequest(new { success = false, Message = "User Registration UnSuccessful", Data = result });

            }
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(UserLoginModel model)
        {
            var Result = userBusiness.UserLogin(model);

            if (Result != null)


            {
                
                // Passwords match, consider the login successful
                return Ok(new { message = "Login successful", data=Result });
            }
            else
            {
                // Passwords do not match, login failed
                return Unauthorized(new { message = "Invalid credentials" });
            }
        }
        [HttpPost]
        [Route("ForgetPassword")]
        public IActionResult ForgetPassword(string email)
        {
            var storedUser = userBusiness.forgetPassword(email);

            if (storedUser != null)
            {

                
                return Ok(new { message = "Token Sent Successful" });
            }
            else
            {
                // Passwords do not match, login failed
                return Unauthorized(new { message = "Invalid credentials, Unsuccessful" });
            }
        }
        [Authorize]
        [HttpPut]
        [Route("ResetPassword")]
        public IActionResult ResetPassword(string NewPassword, string ConfirmPass)
        {
            var emailClaim = User.FindFirst(ClaimTypes.Email).Value;
            if (emailClaim != null)
            {
                
                var storedUser = userBusiness.ResetPassword(emailClaim, NewPassword, ConfirmPass);

                if (storedUser != null)
                {

                    // Passwords match, consider the login successful
                    return Ok(new { success = "true", message = "Password Reset Successful" });
                }
                else
                {
                    // Passwords do not match, login failed
                    return Unauthorized(new { message = "Invalid credentials, Password Reset Unsuccessful" });
                }
            }
            return null;
        }

        [HttpGet]
        [Route("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var storedUser = userBusiness.GetAllUsers();

            return Ok(new { success = "True", message = "Get All Users Successfully", Data = userBusiness.GetAllUsers() });
            //return new JsonResult(userBusiness.GetAllUsers());
        }

		

		[HttpGet]
        [Route("GetUserById")]
        public IActionResult GetUserById(int UserId)
        {
            var storedUser = userBusiness.GetAllUsersByID(UserId);


            if (storedUser != null)
            {
                // Passwords match, consider the login successful
                return Ok(new { success = "True", message = "Get User By Id is Successful", Data = userBusiness.GetAllUsersByID(UserId) });
            }

            //return new JsonResult(userBusiness.GetAllUsersByID(UserId));
            else
            {
                // Passwords do not match, login failed
                return Unauthorized(new { message = "Invalid credentials" });
            }
        }
        [HttpDelete]
        [Route("DeleteUserById")]
        public IActionResult DeleteUserById(int UserId)
        {

            var storedUser=userBusiness.DeleteUsers(UserId);
            if (storedUser==true)
            {
                // Passwords match, consider the login successful
                return Ok(new { success = "True", message = "Deleted Successful", Data = userBusiness.GetAllUsersByID(UserId) });
            }

            //return new JsonResult(userBusiness.GetAllUsersByID(UserId));
            else
            {
                // Passwords do not match, login failed
                return Unauthorized(new { message = "Invalid credentials" });
            }
        }
    }
}
