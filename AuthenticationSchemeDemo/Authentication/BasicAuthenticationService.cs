using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationSchemeDemo.Authentication
{

    public class BasicAuthenticationService : IBasicAuthenticationService
    {
        //private readonly BasicAuthenticationOptions _authenticationOptions;

        //public BasicAuthenticationService(BasicAuthenticationOptions authenticationOptions)
        //{
        //    _authenticationOptions = authenticationOptions;
        //}

        private readonly IConfiguration _configuration;

        private string FileName { get; set; }
        private string RootPath { get; set; }
        private string FilePath {
            get
            {
                return RootPath + "/" + FileName;
            }
        }

        public BasicAuthenticationService(IConfiguration configuration, IHostingEnvironment env)
        {
            _configuration = configuration;
            FileName = configuration.GetSection("Basic").GetSection("FileName").Value;
            RootPath = env.ContentRootPath;
        }

        public Task<bool> IsValidUserAsync(string user, string password)
        {
            string base64 = ConvertUserPassword(user, password);
            try
            {
                string[] lines = File.ReadAllLines(FilePath);
                foreach (string line in lines)
                {
                    if (line.Equals(base64))
                    {
                        return Task.FromResult(true);
                    }

                }
                return Task.FromResult(false);
            }
            catch (Exception exception)
            {
                return Task.FromResult(false);
            }
        }

        public bool StoreBasicCredentials(string username, string password)
        {
            string usernamePassword = username + ":" + password;
            byte[] array = Encoding.UTF8.GetBytes(usernamePassword);

            string base64 = Convert.ToBase64String(array);
            return StoreBasicCredentials(base64);
        }

        public bool StoreBasicCredentials(string base64UserNameAndPassword)
        {
            try
            {
            File.AppendAllText(FilePath, base64UserNameAndPassword + "\n");
                return true;
            }
            catch(Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return false;
            }
        }

        private string ConvertUserPassword(string username, string password)
        {
            string usernamePassword = username + ":" + password;
            byte[] array = Encoding.UTF8.GetBytes(usernamePassword);

            string base64 = Convert.ToBase64String(array);
            return base64;
        }
    }
}
