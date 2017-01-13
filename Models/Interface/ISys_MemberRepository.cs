using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Interface
{
    public interface ISys_MemberRepository : IRepository<Sys_Member>
    {
        Sys_Member GetByID(int PID);
    }
}
