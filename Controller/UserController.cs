﻿using System.Collections.Generic;
using ServerFramework.Servers;
using ServerFramework.Model;
using ServerFramework.DAO;
using Common;

namespace ServerFramework.Controller
{
    class UserController : BaseController
    {
        private UserDAO userDAO = new UserDAO();
        private ResultDAO resultDAO = new ResultDAO();
        private Dictionary<Client, User> userDict = new Dictionary<Client, User>();
        private Dictionary<Client, Result> resultDict = new Dictionary<Client, Result>();
        private const string UserResultStr = "{0},{1},{2},{3}";
        private const string LoginReturnStr = "{0},{1}";

        public string OnLogin(Client client, string data)
        {
            string[] strs = data.Split(Separator);
            User user = userDAO.GetUser(client.MySqlConn, strs[0], strs[1]);
            if (user == null)
            {
                return ((int)ReturnCode.Fail).ToString();
            }
            else
            {
                Result result = resultDAO.GetResultByUserId(client.MySqlConn, user.Id);
                userDict.Add(client, user);
                resultDict.Add(client, result);
                return string.Format(LoginReturnStr, ((int)ReturnCode.Success).ToString(), GetUserResult(client));
            }
        }

        public string OnRegister(Client client, string data)
        {
            string[] strs = data.Split(Separator);
            string username = strs[0];
            string password = strs[1];
            bool hasUser = userDAO.HasUser(client.MySqlConn, username);
            if (hasUser)
            {
                return ((int)ReturnCode.Fail).ToString();
            }
            userDAO.AddUser(client.MySqlConn, username, password);
            return ((int)ReturnCode.Success).ToString();
        }

        public int GetUserId(Client client)
        {
            if (userDict.ContainsKey(client))
            {
                return userDict[client].Id;
            }
            return -1;
        }

        public string GetUserResult(Client client)
        {
            if (userDict.ContainsKey(client) && resultDict.ContainsKey(client))
            {
                return string.Format(UserResultStr, userDict[client].Id, userDict[client].Username, resultDict[client].TotalCount, resultDict[client].WinCount);
            }
            return EmptyStr;
        }
    }
}
