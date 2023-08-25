using CommonLayer.Model;
using RepoLayer.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using static RepoLayer.Services.UserRepo;

namespace BusinessLayer.Interface
{
    public interface IUserBusiness
    {
        public UserEntity UserReg(UserRegModel model);

        public string UserLogin(UserLoginModel model);

        public string forgetPassword(string email);
        public bool ResetPassword(string Email, string NewPassword, string ConfirmPass);
        public List<UserEntity> GetAllUsers();

        public List<UserEntity> GetAllUsersByID(int UserId);
        public bool DeleteUsers(long UserId);

    }
}
