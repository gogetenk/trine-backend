using System.Collections.Generic;
using Assistance.Operational.Dal.MongoImpl.Entities;

namespace Assistance.Operational.Dal.MongoImpl
{
    public class FrameContract : ContractBase
    {
        public List<SubContract> SubContracts { get; set; }
    }
}
