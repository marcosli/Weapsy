﻿using System;
using AutoMapper;
using Weapsy.Domain.Users;
using UserDbEntity = Weapsy.Data.Entities.User;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Weapsy.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<UserDbEntity> _userManager;
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IMapper _mapper;

        public UserRepository(UserManager<UserDbEntity> userManager, IDbContextFactory dbContextFactory, IMapper mapper)
        {
            _userManager = userManager;
            _dbContextFactory = dbContextFactory;
            _mapper = mapper;
        }

        public User GetById(Guid id)
        {
            using (var context = _dbContextFactory.Create())
            {
                var dbEntity = context.Users.FirstOrDefault(x => x.Id == id && x.Status != UserStatus.Deleted);
                return dbEntity != null ? _mapper.Map<User>(dbEntity) : null;
            }
        }

        public User GetByUserName(string userName)
        {
            using (var context = _dbContextFactory.Create())
            {
                var dbEntity = context.Users.FirstOrDefault(x => x.UserName == userName && x.Status != UserStatus.Deleted);
                return dbEntity != null ? _mapper.Map<User>(dbEntity) : null;
            }
        }

        public User GetByEmail(string email)
        {
            using (var context = _dbContextFactory.Create())
            {
                var dbEntity = context.Users.FirstOrDefault(x => x.Email == email && x.Status != UserStatus.Deleted);
                return dbEntity != null ? _mapper.Map<User>(dbEntity) : null;
            }
        }

        public async Task CreateAsync(User user)
        {
            var entity = _mapper.Map<UserDbEntity>(user);

            var result = await _userManager.CreateAsync(entity);

            if (!result.Succeeded)
                throw new Exception(GetErrorMessage(result));
        }

        public async Task UpdateAsync(User user)
        {
            using (var context = _dbContextFactory.Create())
            {
                var dbEntity = await context.Users.FirstOrDefaultAsync(x => x.Id.Equals(user.Id));
                _mapper.Map(user, dbEntity);
                await context.SaveChangesAsync();
            }
        }

        public async Task AddToRoleAsync(Guid id, string roleName)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new Exception("User Not Found.");

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
                throw new Exception(GetErrorMessage(result));
        }

        public async Task RemoveFromRoleAsync(Guid id, string roleName)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new Exception("User Not Found.");

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (!result.Succeeded)
                throw new Exception(GetErrorMessage(result));
        }

        private string GetErrorMessage(IdentityResult result)
        {
            var builder = new StringBuilder();

            foreach (var error in result.Errors)
                builder.AppendLine(error.Description);

            return builder.ToString();
        }
    }
}
