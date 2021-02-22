using System.Collections.Generic;

namespace Dto
{
    public class FrameContractDto : ContractBaseDto
    {
        public List<SubContractDto> SubContracts { get; set; }
    }
}