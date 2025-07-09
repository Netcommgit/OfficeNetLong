using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.UserService
{
    public interface ICurrentUserService
    {
        public string? GetUserId();
    }
}
