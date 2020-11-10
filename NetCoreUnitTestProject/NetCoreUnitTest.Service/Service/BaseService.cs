using NetCoreUnitTestProject.Core.IService;
using NetCoreUnitTestProject.Data.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreUnitTest.Service.Service
{
    public class BaseService<T>:IBaseService<T> where T:class
    {
        public readonly UnitTestDbContext _dbContext;
        public BaseService(UnitTestDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
