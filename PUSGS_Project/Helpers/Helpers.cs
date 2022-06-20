using DataAcesss.Data;
using PUSGS_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PUSGS_Project.Helpers
{
    public class Helpers
    {
        public static void ProfileModelToUser(ProfileViewModel model, AppUser user)
        {
            user.UserName = model.Username;
            user.Email = model.Email;
            user.Fullname = model.Fullname;
            user.ImagePath = model.ImagePath;
            user.Address = model.Address;
            user.Birthdate = model.Birthdate;
            user.Password = model.Password;
        }

        public static void UserToProfileModel(AppUser user, ProfileViewModel model)
        {
            model.Username = user.UserName;
            model.Email = user.Email;
            model.Fullname = user.Fullname;
            model.ImagePath = user.ImagePath;
            model.Address = user.Address;
            model.Birthdate = user.Birthdate;
            model.Password = user.Password;
        }

        // Return true if user and model have same values for common fields
        public static bool ComapareUserAndProfileModel(AppUser user, ProfileViewModel model)
        {
            return 
            model.Username == user.UserName &&
            model.Email == user.Email &&
            model.Fullname == user.Fullname &&
            model.ImagePath == user.ImagePath &&
            model.Address == user.Address &&
            model.Birthdate == user.Birthdate &&
            model.Password == user.Password;
        }

        public static Dictionary<string, int> CountProducts(string products) {
            var d = new Dictionary<string, int>();
            if (products != null && products != "")
            {
                string[] prodArr = products.Split('|');
                foreach (var item in prodArr)
                {
                    if (item == string.Empty)
                    {
                        continue;
                    }
                    if (!d.ContainsKey(item))
                    {
                        d[item] = 1;
                    }
                    else
                    {
                        d[item] += 1;
                    }
                }
            }
            return d;
        }
    }
}
