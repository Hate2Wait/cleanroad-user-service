﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gareon.WebService.Domain;
using Gareon.WebService.Domain.Account;

namespace Gareon.WebService.Logic.Abstractions.UsersSearchService
{
    public interface ITbUsersSearchService
    {
        Task<ICollection<TbUser>> FindAllAsync(Expression<Func<TbUser, bool>> predicate);
    }
}