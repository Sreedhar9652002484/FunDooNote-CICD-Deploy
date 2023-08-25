using BusinessLayer.Interface;
using CommonLayer.Model;
using RepoLayer.Entity;
using RepoLayer.Interface;
using RepoLayer.Services;
using System;
using System.Collections.Generic;
using System.Text;
using static RepoLayer.Services.UserRepo;

namespace BusinessLayer.Services
{
    public class UserBusiness : IUserBusiness
    {
        private readonly IUserRepo _userRepo;
        //private readonly IUserRepo _loginRepo;

        public UserBusiness(IUserRepo userRepo)
        {
            _userRepo = userRepo;
            //_loginRepo = loginRepo;  
        }


        // //This is the UserReg method implementation using UserRegModel
        public UserEntity UserReg(UserRegModel model)
        {
            try
            {
                return _userRepo.UserReg(model);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //This is the UserLogin method implementation using UserRegModel
        public string UserLogin(UserLoginModel model)
        {
            try
            {
                return _userRepo.UserLogin(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string forgetPassword(string email)

        {
            try
            {
                return _userRepo.forgetPassword(email);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool ResetPassword(string Email, string NewPassword, string ConfirmPass)
        {
            try
            {
                return _userRepo.ResetPassword(Email, NewPassword, ConfirmPass);
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
                return _userRepo.GetAllUsers();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UserEntity> GetAllUsersByID(int UserId)
        {
            return _userRepo.GetAllUsersByID(UserId);
        }

        public bool DeleteUsers(long UserId)
        {
            try
            {
                return  _userRepo.DeleteUsers(UserId);

            }
            catch (Exception)
            {

                throw;
            }
        }
        
    }
}
