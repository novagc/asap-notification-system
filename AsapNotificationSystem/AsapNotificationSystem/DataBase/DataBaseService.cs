using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsapNotificationSystem.ConfigReaderService;
using AsapNotificationSystem.DataBase.Config;
using AsapNotificationSystem.DataBase.Context;
using AsapNotificationSystem.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace AsapNotificationSystem.DataBase
{
    public class DataBaseService
    {
        public event Action<User> NewUser;
        public string ConfigPath { get; }

        private DbConfig config;

        public DataBaseService(string configPath)
        {
            ConfigPath = configPath;
            config = new BasicConfigReader<DbConfig>(ConfigPath).ParseConfig();
        }

        public int SubscribeUser(string name, int serviceId, string profileId)
        {
            using var context = new PostgresDbContext(config);

            int? id = GetUserId(serviceId, profileId, context);

            User user;
            if (id == null)
            {
                user = new User
                {
                    Name = name,
                    ServiceId = serviceId,
                    ProfileId = profileId,
                    SendNotification = true,
                    Number = new BuildingNumber[] { }
                };

                context.Users.Add(user);
                context.SaveChanges();
                Task.Factory.StartNew(() => NewUser?.Invoke(user));
            }
            else
            {
                user = context.Users.Find(id);
                user.SendNotification = true;
                context.SaveChanges();
            }

            return user.Id;
        }

        public int UnsubscribeUser(string name, int serviceId, string profileId)
        {
            using var context = new PostgresDbContext(config);

            int? id = GetUserId(serviceId, profileId, context);

            User user;
            if (id == null)
            {
                user = new User
                {
                    Name = name,
                    ServiceId = serviceId,
                    ProfileId = profileId,
                    SendNotification = false,
                    Number = new BuildingNumber[]{}
                };

                context.Users.Add(user);
                context.SaveChanges();
                Task.Factory.StartNew(() => NewUser?.Invoke(user));
            }
            else
            {
                user = context.Users.Find(id);
                user.SendNotification = false;
                context.SaveChanges();
            }

            return user.Id;
        }

        public BuildingNumber[] GetUserBn(int serviceId, string profileId)
        {
            using var context = new PostgresDbContext(config);
            int? id = GetUserId(serviceId, profileId, context);

            if (id == null)
            {
                return new BuildingNumber[0];
            }

            return context.Users.Find(id).Number;
        }

        public int? ChangeBuildingNumber(int serviceId, string profileId, BuildingNumber bNumber)
        {
            using var context = new PostgresDbContext(config);
            int? id = GetUserId(serviceId, profileId, context);

            if (id != null)
            {
                var temp = context.Users.Find(id);
                if (temp.Number.Contains(bNumber))
                {
                    temp.Number = temp.Number.Where(x => x != bNumber).ToArray();
                }
                else
                {
                    temp.Number = temp.Number.Append(bNumber).ToArray();
                }

                context.SaveChanges();
            }
            else
            {
                var user = new User
                {
                    ProfileId = profileId,
                    ServiceId = serviceId,
                    SendNotification = true,
                    Number = new[] {bNumber}
                }; 

                context.Users.Add(user);
                context.SaveChanges();

                id = user.Id;
                Task.Factory.StartNew(() => NewUser?.Invoke(user));
            }

            return id;
        }

        public IEnumerable<string> SelectUsersByBuildingNumber(BuildingNumber buildingNumber, int? serviceId = null)
        {
            using var context = new PostgresDbContext(config);

            if (buildingNumber == BuildingNumber.All)
                return context.Users.Select(x => x.ProfileId).ToList();

            if (serviceId != null)
            {
                var temp = context.Users.AsNoTracking().Where(x => x.ServiceId == serviceId).ToList();
                return temp.Where(x => x.Number.Contains(buildingNumber)).Select(x => x.ProfileId);
            }

            var tmp = context.Users.AsNoTracking().ToList();
            return tmp.Where(x => x.Number.Contains(buildingNumber)).Select(x => x.ProfileId);
        }

        public int? GetUserId(int serviceId, string profileId)
        {
            using var context = new PostgresDbContext(config);

            var temp = 
                context
                    .Users
                    .AsNoTracking()
                    .Where(x => x.ServiceId == serviceId && x.ProfileId == profileId);

            if (temp.Any())
                return temp.First().Id;

            return null;
        }

        private int? GetUserId(int serviceId, string profileId, PostgresDbContext context)
        {
            var temp =
                context
                    .Users
                    .AsNoTracking()
                    .Where(x => x.ServiceId == serviceId && x.ProfileId == profileId);

            if (temp.Any())
                return temp.First().Id;

            return null;
        }
    }
}
