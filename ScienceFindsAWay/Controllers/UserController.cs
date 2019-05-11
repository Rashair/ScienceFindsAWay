﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ScienceFindsAWay.Models;

namespace ScienceFindsAWay.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private IConfiguration Configuration { get; }

        public UserController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IEnumerable<User> DbQuery(string sql)
        {
            var userList = new List<User>();
            using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("SFAWHackBase")))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var name = reader.GetString(reader.GetOrdinal("Name"));
                            var surname = reader.GetString(reader.GetOrdinal("Surname"));
                            var university = reader.GetString(reader.GetOrdinal("University"));
                            var faculty = reader.GetString(reader.GetOrdinal("Faculty"));
                            var mail = reader.GetString(reader.GetOrdinal("Mail"));
                            var id = reader.GetInt32(reader.GetOrdinal("UserID"));


                            userList.Add(new User(name, surname, university, faculty, mail, id, null, null));
                        }
                    }
                }
            }
            return userList;

        }

        [HttpGet("[action]")]
        public IEnumerable<User> GetAllUsers()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * ");
            sb.Append("FROM Users ");
            string sql = sb.ToString();

            return DbQuery(sql);
        }

        [HttpGet("[action]")]
        public User GetUsersWithID(int id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * ");
            sb.Append($"FROM Users where UserID={id} ");
            string sql = sb.ToString();


            return DbQuery(sql)?.First();

        }

        public IEnumerable<User> GetUsersByMeetingId(int id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.Name , u.Surname, u.Faculty, u.Mail, u.UserID ");
            sb.Append($"FROM Users u ");
            sb.Append($"JOIN MeetingUserMerge m on m.UserID = u.UserID and m.MeetingID = {id} ");
            string sql = sb.ToString();


            return DbQuery(sql);

        }
    }
}