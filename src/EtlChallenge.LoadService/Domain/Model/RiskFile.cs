using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtlChallenge.LoadService.Domain.Model
{
    public class RiskFile
    {
        /// <summary>
        /// Reference for locating the risks file in the storage.
        /// </summary>
        public required string StorageReference { get; set; }

        public ICollection<Risk> Risks { get; set; } = [];
    }
}
