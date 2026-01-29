using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.Account
{
    public class AccountNotFoundException(string? message) : NotFoundException(message)
    {
    }
}
