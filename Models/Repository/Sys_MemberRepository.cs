using Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repository
{
    public class Sys_MemberRepository : GenericRepository<Sys_Member>, ISys_MemberRepository
    {
        public Sys_Member GetByID(int PID)
        {
            return this.Get(x => x.PID == PID);
        }
    }
}
