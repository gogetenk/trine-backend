using System.Collections.Generic;

namespace Assistance.Operational.Model
{
    public class FrameContractModel : ContractBaseModel
    {
        public string Id { get; set; }
        public List<SubContractModel> SubContracts { get; set; }
    }
}
