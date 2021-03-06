﻿using PropertyRegister.REAU.Common.Models;
using CNSys.Data;
using System;
using System.Collections.Generic;

namespace PropertyRegister.REAU.Common.Persistence
{
    public class ServiceOperationSearchCriteria
    {
        public long? ServiceOperationID { get; set; }
        public Guid? OperationID { get; set; }
        public ServiceOperationTypes? ServiceOperationType { get; set; }
    }

    public interface IServiceOperationRepository : IRepository<ServiceOperation, long?, ServiceOperationSearchCriteria>
    {
    }

    public class ServiceOperationRepository : RepositoryBase<ServiceOperation, long?, ServiceOperationSearchCriteria, CommonDataContext>, IServiceOperationRepository
    {
        public ServiceOperationRepository() 
            : base(false)
        {
        }

        protected override void CreateInternal(CommonDataContext context, ServiceOperation item)
        {
            context.ServiceOperationCreate(item.OperationID.ToString(), (int)item.ServiceOperationType,
                out long serviceOperationID, out bool isCompleted, out string result);

            item.ServiceOperationID = serviceOperationID;
            item.IsCompleted = isCompleted;
            item.Result = result;
        }

        protected override void UpdateInternal(CommonDataContext context, ServiceOperation item) =>
            context.ServiceOperationUpdate(item.ServiceOperationID.Value, item.OperationID.ToString(), item.IsCompleted, item.Result.ToString());

        protected override IEnumerable<ServiceOperation> SearchInternal(CommonDataContext context, ServiceOperationSearchCriteria searchCriteria)
        {
            IEnumerable<ServiceOperation> res;

            using (var data = context.ServiceOperationSearch(searchCriteria.ServiceOperationID, searchCriteria.OperationID?.ToString(), (int?)searchCriteria.ServiceOperationType))
            {
                res = data.ReadToList<ServiceOperation>();
            }

            return res;
        }
    }
}
