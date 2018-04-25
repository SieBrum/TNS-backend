﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using TNSApi.Mapping;
using TNSApi.Services;

namespace TNSApi.Controllers
{
    public class UsersController : ApiController
    {
        IDatabaseServiceProvider _database;

        public UsersController(IDatabaseServiceProvider database)
        {
            _database = database;
        }

        [HttpPost]
        public IHttpActionResult GetUsers([FromBody] User user)
        {
            int authorizedMessage = (int)AuthorizationService.CheckIfAuthorized(ref user, ref _database, Request.Headers, AccessLevel.Admin);

            if (authorizedMessage == 1 || authorizedMessage == 2)
            {
                return Content(HttpStatusCode.Forbidden, "User not logged in.");
            }
            if(authorizedMessage == 3)
            {
                return Content(HttpStatusCode.Unauthorized, "User has no permission.");
            }
            if (authorizedMessage == 4)
            {
                return Content(HttpStatusCode.Forbidden, "User account is disabled.");
            }


            List<User> users = _database.Users.ToList();
            List<ListPageUser> frontendUsers = new List<ListPageUser>();

            for (int i = 0; i < users.Count; i++)
            {
                frontendUsers.Add(new ListPageUser()
                {
                    Id = users[i].Id,
                    Username = users[i].Username,
                    AccessLevel = users[i].AccessLevel,
                    LastLogin = users[i].LastLogin,
                    Created = users[i].Created,
                    IsActive = users[i].IsActive,
                });
            }

            return Ok(frontendUsers);
        }
    }

    public class ListPageUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string AccessLevel { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime Created { get; set; }
        public Boolean IsActive { get; set; }
    }
}