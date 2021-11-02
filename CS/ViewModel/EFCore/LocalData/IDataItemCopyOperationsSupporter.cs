using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreIssues {
    public interface IDataItemCopyOperationsSupporter {
        object Clone(object item);
        void CopyTo(object source, object target);
    }
}
